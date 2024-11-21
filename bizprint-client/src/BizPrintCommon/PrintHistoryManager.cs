// Copyright 2024 BrainSellers.com Corporation
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizPrintCommon
{
    /// <summary>
    /// 印刷履歴管理クラス。静的関数で保持、編集を行う。
    /// </summary>
    public class PrintHistoryManager
    {
        /// <summary>
        /// 印刷履歴を保持するリスト。JobID順にソートされる(yyyymmdd_hhmmssでソートされる)
        /// </summary>
        public static SortedList PrnStatusJobList { private set; get; } = SortedList.Synchronized(new SortedList());

        #region 印刷履歴状態構造体
        public class PrintHistoryInfo
        {
            public PrintHistoryInfo() { }
            public PrintHistoryInfo(string Id, int Code)
            {
                jobId = Id;
                statusCode = Code;
            }
            //割り振られたJobID(キー)
            public string jobId { set; get; } = String.Empty;
            //Acrobatに渡したファイル名
            public string printFileName { set; get; } = String.Empty;
            //現在のステータスコード
            public int statusCode { set; get; } = CommonConstants.JOB_STATUS_RECIEVED;
            //異常時のステータス文字列
            public string statusString { set; get; } = String.Empty;
            //エラーコード
            public int errCode { set; get; } = ErrCodeAndmErrMsg.STATUS_OK;
            //印刷に使用したプリンタ名
            public string printerName { set; get; } = String.Empty;
            //履歴の作成時間(要求受信時刻)
            public DateTime createdTime { set; get; } = DateTime.Now;
            //ステータス更新時刻
            public DateTime lastEventTime { set; get; } = DateTime.Now;
            //レスポンス送信済みフラグ(ダイレクトでのみ使用)
            public bool isResponsed { set; get; } = false;
        }

        #endregion
        public static SettingManeger SettingMng { set; get; }

        /// <summary>
        /// コンストラクタは使用不可
        /// </summary>
        private PrintHistoryManager() { }


        /// <summary>
        /// 最大数をオーバーしていた場合、最初に追加された履歴を1個削除する。
        /// </summary>
        public static void DeleteOverMaxHistory()
        {
            lock (PrnStatusJobList.SyncRoot)
            {
                if (PrnStatusJobList.Count >= SettingMng.MaxHistoryNum)
                {
                    PrintHistoryInfo delInfo = (PrintHistoryInfo)PrnStatusJobList.GetByIndex(0);
                    LogUtility.OutputLog("064", delInfo.jobId);
                    PrnStatusJobList.RemoveAt(0);
                    PrnStatusJobList.TrimToSize();
                }
            }
        }
        /// <summary>
        /// 履歴保持数を返す
        /// </summary>
        /// <returns></returns>
        public static int GetHistoryListCount()
        {
            return PrnStatusJobList.Count;
        }
        /// <summary>
        /// 履歴の追加
        /// </summary>
        /// <param name="newInfo"></param>
        public static void AddNewHistory(PrintHistoryInfo newInfo)
        {
            //追加前に時間切れのを削除
            CleanUpTimeOverHistory();

            //既に存在するIDの場合、上書き(基本的にはありえない)
            if (PrnStatusJobList.ContainsKey(newInfo.jobId))
            {
                UpdatePrintInfo(newInfo);
                LogUtility.OutputLog("063", newInfo.jobId);
                LogUtility.OutputDebugLog("E072");
            }
            else
            {
                DeleteOverMaxHistory();
                PrnStatusJobList.Add(newInfo.jobId, newInfo);
                LogUtility.OutputLog("062", newInfo.jobId);
            }
            return;
        }
        /// <summary>
        ///時間切れになった履歴のクリーンアップ 
        /// </summary>
        public static void CleanUpTimeOverHistory()
        {

            LogUtility.OutputLog("070");
            for (int i = 0; i < PrnStatusJobList.Count; i++)
            {
                PrintHistoryInfo pi = (PrintHistoryInfo)PrnStatusJobList.GetByIndex(i);
                TimeSpan timeDiffer = DateTime.Now - pi.createdTime;
                if ((timeDiffer.TotalSeconds > SettingMng.HistoryHoldTime))
                {
                    LogUtility.OutputLog("071", pi.jobId);
                    PrnStatusJobList.RemoveAt(i);
                }
            }

        }
        /// <summary>
        /// JOBID指定された履歴を返す なければNULLを返す
        /// </summary>
        public static PrintHistoryInfo GetHistoryFromId(string jobID)
        {
            CleanUpTimeOverHistory();
            LogUtility.OutputLog("043", jobID);
            //存在確認
            if (PrnStatusJobList.ContainsKey(jobID))
            {
                LogUtility.OutputLog("046", jobID);
                return (PrintHistoryInfo)PrnStatusJobList[jobID];
            }
            else
            {
                LogUtility.OutputLog("047", jobID);
                return null;
            }
        }
        /// <summary>
        /// 現在保持している全ての履歴を返す
        /// </summary>
        public static PrintHistoryInfo[] GetAllHistory()
        {
            CleanUpTimeOverHistory();
            LogUtility.OutputLog("044");
            if (PrnStatusJobList.Count == 0)
            {
                //存在しない
                LogUtility.OutputLog("045");

                return null;
            }
            else
            {
                //データがあるなら全部返す
                PrintHistoryInfo[] rtn = new PrintHistoryInfo[PrnStatusJobList.Count];
                for (int i = 0; i < PrnStatusJobList.Count; i++)
                {
                    rtn[i] = (PrintHistoryInfo)PrnStatusJobList.GetByIndex(i);
                }
                LogUtility.OutputLog("124", PrnStatusJobList.Count.ToString());
                return rtn;
            }
        }

        /// <summary>
        /// Infoを引数にしての更新
        /// </summary>
        /// <param name="Info"></param>
        /// <returns></returns>
        public static int UpdatePrintInfo(PrintHistoryInfo Info)
        {

            lock (PrnStatusJobList.SyncRoot)
            {
                //存在確認
                if (PrnStatusJobList.ContainsKey(Info.jobId))
                {
                    LogUtility.OutputLog("107", Info.jobId, Info.statusCode.ToString());
                    int Idx = PrnStatusJobList.IndexOfKey(Info.jobId);
                    Info.lastEventTime = DateTime.Now;
                    PrnStatusJobList.SetByIndex(Idx, Info);
                }
                else
                {
                    DeleteOverMaxHistory();
                    Info.lastEventTime = DateTime.Now;
                    PrnStatusJobList.Add(Info.jobId, Info);
                }
            }
            return 0;

        }
        /// <summary>
        /// JobIDを指定してステータスのアップデート
        /// </summary>
        /// <param name="jobID"></param>
        /// <param name="StatusCD"></param>
        /// <returns></returns>
        public static int UpdatePrintStatusByID(string jobID, int StatusCD)
        {
            int rtn = 0;
            lock (PrnStatusJobList.SyncRoot)
            {
                LogUtility.OutputLog("107", StatusCD.ToString(), jobID);
                //存在確認
                if (PrnStatusJobList.ContainsKey(jobID))
                {
                    PrintHistoryInfo wkInfo = (PrintHistoryInfo)PrnStatusJobList[jobID];
                    wkInfo.statusCode = StatusCD;
                    wkInfo.lastEventTime = DateTime.Now;
                    int Idx = PrnStatusJobList.IndexOfKey(jobID);
                    PrnStatusJobList.SetByIndex(Idx, wkInfo);

                }
                else
                {
                    LogUtility.OutputLog("129", jobID);
                    rtn = -1;
                }
            }
            return rtn;
        }
        /// <summary>
        /// JobIDを指定してステータスコードとエラーコードのアップデート
        /// </summary>
        /// <param name="jobID"></param>
        /// <param name="StatusCD"></param>
        /// <param name="ErrCD"></param>
        /// <returns></returns>
        public static int UpdatePrintStatusAndErrCodeByID(string jobID, int StatusCD, int ErrCD)
        {
            int rtn = 0;
            lock (PrnStatusJobList.SyncRoot)
            {
                LogUtility.OutputLog("157", jobID, StatusCD.ToString());
                //存在確認
                if (PrnStatusJobList.ContainsKey(jobID))
                {
                    PrintHistoryInfo wkInfo = (PrintHistoryInfo)PrnStatusJobList[jobID];
                    wkInfo.statusCode = StatusCD;
                    wkInfo.errCode = ErrCD;
                    wkInfo.lastEventTime = DateTime.Now;
                    int Idx = PrnStatusJobList.IndexOfKey(jobID);
                    PrnStatusJobList.SetByIndex(Idx, wkInfo);

                }
                else
                {
                    LogUtility.OutputLog("130", jobID);
                    rtn = -1;
                }
            }
            return rtn;
        }
        /// <summary>
        /// JobIDを指定してプリンタ名のアップデート
        /// </summary>
        /// <param name="jobID"></param>
        /// <param name="StatusCD"></param>
        /// <returns></returns>
        public static int UpdatePrinterNameById(string jobID, string printerName)
        {
            int rtn = 0;
            lock (PrnStatusJobList.SyncRoot)
            {
                //存在確認
                if (PrnStatusJobList.ContainsKey(jobID))
                {
                    PrintHistoryInfo wkInfo = (PrintHistoryInfo)PrnStatusJobList[jobID];
                    wkInfo.printerName = printerName;
                    wkInfo.lastEventTime = DateTime.Now;
                    int Idx = PrnStatusJobList.IndexOfKey(jobID);
                    PrnStatusJobList.SetByIndex(Idx, wkInfo);

                }
                else
                {
                    LogUtility.OutputLog("131", jobID);
                    rtn = -1;
                }
            }
            return rtn;
        }
        /// <summary>
        /// JobIDを指定して印刷ファイル名のアップデート
        /// </summary>
        /// <param name="jobID"></param>
        /// <param name="StatusCD"></param>
        /// <returns></returns>
        public static int UpdatePrintFileNameById(string jobID, string fileName)
        {
            int rtn = 0;
            lock (PrnStatusJobList.SyncRoot)
            {
                //存在確認
                if (PrnStatusJobList.ContainsKey(jobID))
                {
                    PrintHistoryInfo wkInfo = (PrintHistoryInfo)PrnStatusJobList[jobID];
                    wkInfo.printFileName = fileName;
                    wkInfo.lastEventTime = DateTime.Now;
                    int Idx = PrnStatusJobList.IndexOfKey(jobID);
                    PrnStatusJobList.SetByIndex(Idx, wkInfo);

                }
                else
                {
                    LogUtility.OutputLog("132", jobID);
                    rtn = -1;
                }
            }
            return rtn;
        }
        /// <summary>
        /// JobIDを指定してレスポンス終了のアップデート
        /// </summary>
        /// <param name="jobID"></param>
        /// <param name="StatusCD"></param>
        /// <returns></returns>
        public static int UpdateResponcedStatusById(string jobID, bool responsed)
        {
            int rtn = 0;
            lock (PrnStatusJobList.SyncRoot)
            {
                //存在確認
                if (PrnStatusJobList.ContainsKey(jobID))
                {
                    PrintHistoryInfo wkInfo = (PrintHistoryInfo)PrnStatusJobList[jobID];
                    wkInfo.isResponsed = responsed;
                    wkInfo.lastEventTime = DateTime.Now;
                    int Idx = PrnStatusJobList.IndexOfKey(jobID);
                    PrnStatusJobList.SetByIndex(Idx, wkInfo);

                }
                else
                {
                    LogUtility.OutputLog("133", jobID);
                    rtn = -1;
                }
            }
            return rtn;
        }


    }
}
