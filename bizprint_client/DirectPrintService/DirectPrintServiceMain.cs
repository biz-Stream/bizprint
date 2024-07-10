
using BizPrintCommon;
using DirectPrintService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DirectPrintService
{
    /// <summary>
    /// ダイレクト印刷メインフォーム(最小化、タスクバー表示なし、トレイアイコンのみのフォーム)
    /// </summary>
    public partial class DirectPrintServiceMain : Form
    {
        private static SettingManeger SetingMng;

        private HttpReciever RecieveTH = null;
        private PrintReqProcesser PrintTH = null;
        /// <summary>
        /// ソケット接続待ちスレッドの終了結果を監視するスレッド。
        /// </summary>
        private Thread RecieveTHWatchTH = null;

        /// <summary>
        /// 印刷キュー監視スレッドの終了結果を監視するスレッド。
        /// </summary>
        private Thread PrintTHWatchTH = null;
        /// <summary>右クリックメニューからの強制終了フラグ</summary>
        private bool ForceCloseFlg = false;
        /// <summary>
        /// 自身の処理から閉じる際にtrueにする
        /// </summary>
        private Boolean closeOwnFlg = false;

        /// <summary>
        /// メインクラスコンストラクタ
        /// </summary>
        public DirectPrintServiceMain()
        {
            InitializeComponent();

            //ログ設定初期化
            //ログ出力初期化と開始ログ出力
            string logConfPath = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\" + ServicetConstants.DirectConfFolderName + "\\" + ServicetConstants.DPlogConfFileName;
            // DirectConstants.m_logger = new LogUtility(logConfPath);
            LogUtility.InitLog4Net(logConfPath);
            LogUtility.InitLogUtility(ServicetConstants.DPlogIDBase, ServicetConstants.DPlogConfBaseDirect, ServicetConstants.DirectConfFolderName);
            LogUtility.OutputStaticLog("DP000", CommonConstants.LOGLEVEL_INFO, "DirectPrintService Start.");

            if (!Init())
            {
                closeOwnFlg = true;
                this.Close();
                return;
            }

        }
        /// <summary>
        /// 初期化処理
        /// </summary>
        /// <returns>true:成功 false:失敗</returns>
        private bool Init()
        {
            bool rtn = true;
            Version OSver = Environment.OSVersion.Version;
            //初期化ログ
            LogUtility.OutputStaticLog("DP001", CommonConstants.LOGLEVEL_INFO, CommonConstants.STATIC_LOG_001 + System.Runtime.InteropServices.RuntimeEnvironment.GetSystemVersion() + " [" + Environment.Version.ToString() + "] " + LogUtility.GetWindowsVersion());
            LogUtility.OutputStaticLog("DP002", CommonConstants.LOGLEVEL_INFO, CommonConstants.STATIC_LOG_002 + System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory() + " Excecute File Version=" + LogUtility.GetSelfFileVersion());
            LogUtility.OutputStaticLog("DP003", CommonConstants.LOGLEVEL_INFO, CommonConstants.STATIC_LOG_003 + System.Runtime.InteropServices.RuntimeEnvironment.SystemConfigurationFile);

            //同名プロセス起動済みチェックは済んでいる


            //設定ファイル読み込み
            SetingMng = new SettingManeger(CommonConstants.MODE_DIRECT);
            if (!SetingMng.LoadSetting())
            {
                //設定ファイル読み込みに失敗
                LogUtility.OutputLog("014", "Can not Load Setting File");
                MessageBox.Show(LogUtility.GetLogStr("014", "Can not Load Setting File"), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            SettingManeger.UpdateLatestEvent();
            ErrCodeAndmErrMsg.LoadErrorDetailFile(ServicetConstants.DirectConfFolderName);


            //Staticなクラスの初期化
            WebBrowserUtil.SettingMng = SetingMng;
            PrintReqQueue.NumberOfQueMax = SetingMng.QueueMaxSize;
            PrintHistoryManager.SettingMng = SetingMng;

            //最小化＆タスクトレイに格納は、フォームの設定から。

            //一時フォルダクリーンアップ処理を行う
            FolderCleanUp.CleanUpFolder(SetingMng.TmpFolderPath);

            try
            {

                //キュー監視スレッド起動
                PrintTH = new PrintReqProcesser(CommonConstants.MODE_DIRECT, SetingMng);
                PrintTH.StartPrintProcessThread();
                //HTTP受信スレッド起動
                RecieveTH = new HttpReciever(CommonConstants.MODE_DIRECT, SetingMng);
                RecieveTH.StartListhen();

            }
            catch (Exception ex)
            {
                ErrCodeAndmErrMsg.SetExErrorMsg = ex.Message;
                //スレッド起動に失敗したのでエラー終了
                LogUtility.OutputLog("015", ex.Message);
                MessageBox.Show(LogUtility.GetLogStr("015", ex.Message), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            //終了イベントタイマー開始

            //HTTP受信スレッド終了結果を監視するスレッド起動
            RecieveTHWatchTH = new Thread(new ThreadStart(recieveTHEndWatch));
            RecieveTHWatchTH.IsBackground = true;
            RecieveTHWatchTH.Start();

            //印刷スレッド終了結果を監視するスレッド起動
            PrintTHWatchTH = new Thread(new ThreadStart(printTHEndWatch));
            PrintTHWatchTH.IsBackground = true;
            PrintTHWatchTH.Start();

            return rtn;
        }
        /// <summary>
        /// 右クリックメニュー
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void directPrintServiceCloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!ForceCloseFlg)
            {
                closeOwnFlg = true;
                LogUtility.OutputLog("201");
                this.ChkRecieveTimer.Enabled = false;
                //終了処理実行
                if (RecieveTH != null && PrintTH != null)
                {
                    StopHttpReciever();
                }
                ForceCloseFlg = true;
            }
            else
            {
                closeOwnFlg = true;
                //2回目以降は、強制終了として扱う
                try
                {
                    //アプリケーションを強制終了する
                    LogUtility.OutputLog("202");
                    this.Close();
                    Application.Exit();
                }
                catch (Exception ex)
                {
                    LogUtility.OutputLog("203", ex.Message);
                }
            }
        }
        /// <summary>
        /// 停止処理 1：Http受信スレッド停止
        /// </summary>
        private void StopHttpReciever()
        {
            if (RecieveTH != null)
            {
                HttpReciever.IsThreadStarted = false;
                HttpReciever.MreDiscontinuanceRequest.Set();
                HttpReciever.MreAllDone.Set();
            }
        }
        /// <summary>
        /// 停止処理 2：受信スレッドの終了を待ち受け、印刷キュー監視スレッド停止要求を出す
        /// </summary>
        private void recieveTHEndWatch()
        {
            try
            {
                // ソケット接続待ちスレッドの終了結果シグナルがONになるまで待機する。
                HttpReciever.MreDiscontinuanceResult.WaitOne();

                // 印刷キュー監視監視終了セット
                StopPrintProcess();
            }
            catch (ThreadAbortException tex)
            {
                ErrCodeAndmErrMsg.SetExErrorMsg = tex.Message;
            }
            catch (Exception ex)
            {
                ErrCodeAndmErrMsg.SetExErrorMsg = ex.Message;
            }
            finally
            {
                LogUtility.OutputLog("116");
            }
        }
        /// <summary>
        /// 停止処理 3：印刷キュー監視スレッドに停止要求を出す
        /// </summary>
        private void StopPrintProcess()
        {
            if (PrintTH != null)
            {
                PrintTH.IsPrintProcessThreadStarted = false;
                PrintReqProcesser.MrePrintEndRequest.Set();
                HttpReciever.MreAllDone.Set();
            }
        }
        /// <summary>
        /// 停止処理 4：印刷キュー監視スレッドの停止を待ち、アプリケーションそのものを終了させる
        /// </summary>
        private void printTHEndWatch()
        {
            try
            {


                // 印刷キュー監視スレッドの終了結果シグナルがONになるまで待機する。
                PrintReqProcesser.MrePrintEndResult.WaitOne();


            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception ex)
            {
                ErrCodeAndmErrMsg.SetExErrorMsg = ex.Message;
            }
            finally
            {
                //終了時の一時フォルダクリーンアップ
                FolderCleanUp.CleanUpFolder(SetingMng.TmpFolderPath);

                LogUtility.OutputLog("117");
                //アプリケーションを終了する
                LogUtility.OutputLog("200");
                closeOwnFlg = true;
                //System.InvalidOperationException回避のため
                try
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        this.Close();
                        Application.Exit();
                    });
                }
                catch (Exception ex)
                {
                    ErrCodeAndmErrMsg.SetExErrorMsg = ex.Message;
                }
            }
        }
        /// <summary>
        /// タイマーイベント ダイレクトのみ、最後に要求を受け付けてTimeOut時刻、かつ、解析＆印刷
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChkRecieveTimer_Tick(object sender, EventArgs e)
        {

            //最終イベント時間と現在時刻の差分を取得
            TimeSpan timeDiffer = DateTime.Now - SettingManeger.GetLatestEventTime();
            double diff = timeDiffer.TotalSeconds;

            //タイムアウト有効かつタイムアウト秒数を超えていた場合
            if (SetingMng.ExitByTimerEnabled && SetingMng.ExitTimerLimit < diff)
            {
                //スレッド内の作業状況と、キューに未処理のデータがあるかをチェックして、まだ作業中なら時間を更新
                if (RecieveTH.IsAnalyzing() || PrintTH.isPrinting() || PrintReqQueue.IsReqQueHaveData())
                {
                    SettingManeger.UpdateLatestEvent();
                }
                //受信・解析の停止＞受信を先に止めないと、Silentから来た要求がQueにあるのに印刷されない状態になる
                //印刷の停止
                else if (RecieveTH != null && PrintTH != null)
                {
                    StopHttpReciever();
                    this.ChkRecieveTimer.Enabled = false;//test
                }


            }
        }

        private void DirectPrintServiceMain_Activated(object sender, EventArgs e)
        {
            Rectangle r = PrintForm.GetTotalBound();
            this.Location = new Point(r.X - this.Size.Width - 1, r.Y - this.Size.Height - 1);
#if DEBUG   
            LogUtility.OutputLog("902", "Direct X0,Y0=" + r.X.ToString() + "," + r.Y.ToString() + " this.Location=" + this.Location.X + "," + this.Location.Y);
#endif
        }

        private void DirectPrintServiceMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!closeOwnFlg)
            {
                e.Cancel = true;
            }

        }

        private void contextMenuStripDirect_Opening(object sender, CancelEventArgs e)
        {
            if (PrintTH != null) {
                PrintTH.setNowEndDlg(true);
            }
            //フォーカスを自身に渡す
            this.Activate();
            //メッセージボックスを表示する
            DialogResult result = MessageBox.Show("biz-Stream ダイレクト印刷を終了してもよろしいですか？",
                "biz-Stream ダイレクト印刷の終了",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button2);
            if (PrintTH != null)
            {
                PrintTH.setNowEndDlg(false);
            }
            if (result == DialogResult.OK)
            {
                closeOwnFlg = true;
                if (!ForceCloseFlg)
                {
                    LogUtility.OutputLog("201");
                    //終了処理実行
                    if (RecieveTH != null && PrintTH != null)
                    {
                        StopHttpReciever();
                    }
                    ForceCloseFlg = true;

                }
                else
                {
                    //2回目以降は、強制終了として扱う
                    try
                    {
                        //アプリケーションを強制終了する 
                        LogUtility.OutputLog("202");
                        this.Close();
                        Application.Exit();
                    }
                    catch (Exception ex)
                    {
                        LogUtility.OutputLog("203", ex.Message);
                    }

                }
            }
            else if (result == DialogResult.Cancel)
            {
                //「キャンセル」が選択された時はとくになにもしない
            }

        }
    }
}
