using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizPrintCommon
{
    /// <summary>
    /// 印刷要求キュークラス。静的に管理する。
    /// </summary>
    public class PrintReqQueue
    {
        #region キュー本体宣言

        /// <summary>要求データキュー</summary>
        public static Queue PrintReqestQue = Queue.Synchronized(new Queue());
        public static int NumberOfQueMax { set; private get; } = ServicetConstants.DEFAULT_QUEUESIZE;
        #endregion
        /// <summary>
        /// コンストラクタは触らせない
        /// </summary>
        private PrintReqQueue() { }
        /// <summary>
        /// 要求データキューに追加
        /// </summary>
        /// <param name="command"></param>
        public static void AddReqest(object command)
        {

            LogUtility.OutputLog("134");
            PrintReqestQue.Enqueue(command);

        }
        /// <summary>
        /// 要求データキューから取り出し　データが無い場合はNULLを返却
        /// </summary>
        public static object GetNextReqest()
        {
            object ret = null;

            if (0 < PrintReqestQue.Count)
            {
                try
                {
                    ret = PrintReqestQue.Dequeue();
                }
                catch (System.InvalidOperationException)
                {
                    // 他スレッドが先行取得
                    Console.WriteLine("");
                }
                finally
                {

                }
            }
            LogUtility.OutputLog("135", PrintReqestQue.Count.ToString());
            return ret;

        }
        /// <summary>
        /// 次に印刷する印刷要求のプリンタ名を取得する
        /// </summary>
        /// <returns>プリンタ名(デフォルトプリンタの場合は空文字)</returns>
        public static String getNextPrinter() {
            String rtn = "";
            if (0 < PrintReqestQue.Count)
            {
                try
                {
                    PrintParameter nextQ = null;
                    nextQ = (PrintParameter) PrintReqestQue.Peek();
                    if (nextQ != null) {
                        rtn = nextQ.PrinterName;
                    }
                }
                catch (System.InvalidOperationException)
                {
                    // 他スレッドが先行取得
                    Console.WriteLine("");
                }
                finally
                {

                }
            }
            return rtn;
        }

        /// <summary>
        /// 要求データキューにデータがあるかチェック
        /// </summary>
        /// <returns>true:ある false：ない</returns>
        public static bool IsReqQueHaveData()
        {
            return (0 < PrintReqestQue.Count);

        }

        /// <summary>
        /// 全てのキューを削除
        /// </summary>
        public static void ClearAllQue()
        {
            //破棄時にはすべてクリア
            PrintReqestQue.Clear();
            LogUtility.OutputLog("137");
        }
        /// <summary>
        /// キュー最大値に達しているかをチェック
        /// </summary>
        /// <returns></returns>
        public static bool IsQueSizeOverMax()
        {
            return (PrintReqestQue.Count >= NumberOfQueMax);
        }

    }
}