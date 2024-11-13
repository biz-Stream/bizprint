using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SppFileExtractTool
{
    /// <summary>
    /// SPPファイル復号ツールメインフォーム
    /// </summary>
    public partial class SppExtractMainForm : Form
    {

        string savePath = String.Empty;
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SppExtractMainForm()
        {
            InitializeComponent();
        }
        /// <summary>
        /// ファイル選択ボタンクリック時処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnInput_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDlg = new OpenFileDialog();
            openFileDlg.Filter = "sppファイル(*.spp;*.SPP)|*.spp;*.SPP|すべてのファイル(*.*)|*.*";
            openFileDlg.FilterIndex = 1;
            //ダイアログを表示する
            if (openFileDlg.ShowDialog() == DialogResult.OK)
            {
                //OKボタンがクリックされたとき、選択されたファイル名を表示する
                txtInput.Text = openFileDlg.FileName;
            }
        }

        /// <summary>
        /// 復号ボタンクリック時処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExtract_Click(object sender, EventArgs e)
        {
            string spppath = txtInput.Text;
            //ファイル存在確認
            if (!System.IO.File.Exists(spppath))
            {
                ShowWarnMsgBox(SppToolConstants.ERR_MSG_01);
                this.btnInput.Focus();
                return;
            }
            byte[] orgData = null;
            //ファイル読み込み
            try
            {

                orgData = File.ReadAllBytes(spppath);

            }
            catch (Exception ex)
            {
                ShowWarnMsgBox(SppToolConstants.ERR_MSG_02 + ex.Message);
                this.btnInput.Focus();
                return;

            }
            if (orgData == null || orgData.Length == 0)
            {
                ShowWarnMsgBox(SppToolConstants.ERR_MSG_05);
                this.btnInput.Focus();
                return;
            }
            //解凍クラスで解凍
            SppExtracorForTool ext = new SppExtracorForTool();
            ext.InitPass(txtPassword.Text);
            int chk = ext.DoExtract(orgData);
            //解凍失敗パターンでワーニング内容分岐
            if (chk > 0)
            {
                ShowWarnMsgBox(SppToolConstants.ERR_MSG_05 + chk + ")");
                this.btnInput.Focus();
                return;
            }
            else if (chk == SppExtracorForTool.SPP_INDATA_ERROR)
            {
                this.btnInput.Focus();
                return;
            }
            else if (chk == SppExtracorForTool.SPP_DATA_ERROR)
            {
                ShowWarnMsgBox(SppToolConstants.ERR_MSG_03);
                this.btnInput.Focus();
                return;
            }
            else if (chk == SppExtracorForTool.SPP_PASSWDERROR)
            {
                ShowWarnMsgBox(SppToolConstants.ERR_MSG_04);
                this.txtPassword.Focus();
                return;
            }
            else
            {
                //解凍成功＞保存先選択
                if (GetSaveFolderName() == 0)
                {

                    chk = DoSaveFile(ext.PdfByte, ext.PdfFileName);
                    if (chk != 0)
                    {

                        return;
                    }
                    chk = DoSaveFile(ext.ParamByte, SppToolConstants.PARAM_FILENAME);
                    if (chk != 0)
                    {
                        return;
                    }
                    MessageBox.Show(SppToolConstants.SUCCESS_MSG_001, "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
            }


        }
        /// <summary>
        /// ワーニングメッセージボックスを表示する
        /// </summary>
        /// <param name="Message"></param>
        private void ShowWarnMsgBox(String Message)
        {
            MessageBox.Show(Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        /// <summary>
        /// 保存フォルダ名を取得する。キャンセルされたらそこで終わり
        /// </summary>
        /// <returns></returns>
        private int GetSaveFolderName()
        {
            FolderBrowserDialog folderBrowseDlg = new FolderBrowserDialog();
            folderBrowseDlg.Description = "保存先フォルダを指定してください。";
            string present_dir = System.IO.Directory.GetCurrentDirectory();
            folderBrowseDlg.SelectedPath = present_dir;
            //ダイアログを表示する
            if (folderBrowseDlg.ShowDialog(this) == DialogResult.OK)
            {
                //選択されたフォルダを表示する
                savePath = folderBrowseDlg.SelectedPath;
                return 0;
            }
            return -1;
        }
        /// <summary>
        /// ファイルをセーブする。同名のがあった場合、(n)をカウントアップ
        /// </summary>
        /// <param name="data"></param>
        /// <param name="savefilenema"></param>
        /// <returns></returns>
        private int DoSaveFile(byte[] data, string savefilenema)
        {

            try
            {
                //ファイル名、拡張子、パス名取得(Windowsのパスとして正しい事はパラメタ読み込み時にチェック済み)
                string saveDir = savePath;
                string fileName = Path.GetFileNameWithoutExtension(savefilenema);
                string fileExt = Path.GetExtension(savefilenema);
                //ファイル保存
                //既に同名ファイルがあるかチェック
                string fname = saveDir + "\\" + fileName + fileExt;
                if (File.Exists(fname))
                {
                    //ある場合、拡張子の前に(n)をカウントアップしていき、独自にする
                    int plus = 1;
                    while (File.Exists(fname))
                    {
                        fname = saveDir + "\\" + fileName + "(" + plus + ")" + fileExt;
                        plus++;
                    }
                }
                //保存実行
                FileStream newFile = new FileStream(fname, FileMode.Create);
                BinaryWriter bw = new BinaryWriter(newFile);
                bw.Write(data);
                bw.Close();
                newFile.Close();

            }
            catch (Exception ex)
            {
                ShowWarnMsgBox(savefilenema + SppToolConstants.ERR_MSG_07 + ex.Message + ")");
                return -1;
            }
            System.Diagnostics.Process.Start(savePath);
            return 0;

        }
        /// <summary>
        /// ファイル名入力テキストボックスへのドラッグ＆ドロップ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtInput_DragDrop(object sender, DragEventArgs e)
        {
            //ドロップされたファイルの一覧を取得
            string[] fileName = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            if (fileName.Length <= 0)
            {
                return;
            }

            // ドロップ先がTextBoxであるかチェック
            TextBox txtTarget = sender as TextBox;
            if (txtTarget == null)
            {
                return;
            }

            //TextBoxの内容をファイル名に変更
            txtTarget.Text = fileName[0];
        }
        /// <summary>
        /// ファイル名入力テキストボックスへのドラッグ中動作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtInput_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }
    }
}
