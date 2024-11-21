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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BizPrintCommon
{
    class DefaultPrinterGetter
    {
        //所得関数が返すプリンタ名
        public string PrinterName { set; get; } = "";

        //取得成功フラグ
        bool isSuccess = false;

        //取得時タイムアウトフラグ
        bool isTimeOut = false;

        //チェックループのウェイト時間
        int defprinterCheckInterval = 33;
        //作成失敗タイムアウト時間
        int defPrinterTimeoutMsec = 500;
#if DEBUG
        private static int debugcount = 0;
#endif

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DefaultPrinterGetter()
        {
            defPrinterTimeoutMsec = SettingManeger.DefaultPrinterTimeout;
            defprinterCheckInterval = SettingManeger.DefaultPrinterCheckInterval;

        }
        /// <summary>
        /// 取得成功か失敗かを返す関数本体
        /// </summary>
        /// <returns></returns>
        public bool getDefaultPrinter()
        {
            int tid = System.Threading.Thread.CurrentThread.ManagedThreadId;
            try
            {
                LogUtility.OutputLog("480");//level Debug

#if DEBUG
                //for log 483
                debugcount++;
                if (debugcount == 7)
                {
                    throw new Exception("Debug Exception");
                }
#endif
                isSuccess = false;
                isTimeOut = false;


                //取得チャレンジスレッド起動
                Thread trygetThraed = new Thread(new ThreadStart(PrinterNameGetWorker));
                trygetThraed.IsBackground = true;
                trygetThraed.SetApartmentState(ApartmentState.MTA);
                trygetThraed.Start();

                //監視スレッド起動
                Thread timeoutchkThraed = new Thread(new ThreadStart(TimeoutCheckWorker));
                timeoutchkThraed.IsBackground = true;
                timeoutchkThraed.SetApartmentState(ApartmentState.MTA);
                timeoutchkThraed.Start();

                while (true)
                {
                    //タイムアウト or 取得成功でブレイク
                    if (isTimeOut || isSuccess)
                    {
                        break;
                    }
                    Thread.Sleep(defprinterCheckInterval);
                }


                //この時点で監視スレッドと取得スレッドは殺していい
                if (trygetThraed.IsAlive)
                {
                    trygetThraed.Abort();
                }
                if (timeoutchkThraed.IsAlive)
                {
                    timeoutchkThraed.Abort();
                }


                //成功なら返す
                if (isSuccess)
                {
                    LogUtility.OutputLog("481", PrinterName);//level Debug
                    return true;
                }
                //タイムアウト or 失敗
                else
                {
                    LogUtility.OutputLog("482");//level Warn
                }
            }
            catch (Exception ex)
            {
                LogUtility.OutputLog("483", ex.Message);//level Warn
            }
            return false;
        }

        private void PrinterNameGetWorker()
        {
            //デフォルトプリンタ―の取得
            try
            {
                System.Drawing.Printing.PrintDocument pd = new System.Drawing.Printing.PrintDocument();
                PrinterName = pd.PrinterSettings.PrinterName;
                this.isSuccess = true;
            }
            catch (Exception)
            {
                //スレッド強制終了時or取得処理で例外発生時。取得には失敗。なにもしない。
            }

        }
        /// <summary>
        /// タイムアウトを監視するワーカスレッド
        /// </summary>
        private void TimeoutCheckWorker()
        {
            //開始時刻取得
            int id = System.Threading.Thread.CurrentThread.ManagedThreadId;
            DateTime starttime = DateTime.Now;
            try
            {
                while (true)
                {
                    //既に作成成功してれば自身を終了
                    if (this.isSuccess)
                    {
                        break;
                    }

                    //タイムアウト時間を超えてるならばタイムアウトにする
                    TimeSpan timeDiffer = DateTime.Now - starttime;
                    if (timeDiffer.TotalMilliseconds > defPrinterTimeoutMsec)
                    {
                        //タイムアウトフラグ立てる
                        this.isTimeOut = true;
                        break;
                    }
                    Thread.Sleep(defprinterCheckInterval);
                }


            }
            catch (Exception)
            {
                //スレッド強制終了時。なにもしない。
            }

        }

    }
}
