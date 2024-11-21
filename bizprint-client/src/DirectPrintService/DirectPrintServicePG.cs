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
using BizPrintCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DirectPrintService
{
    /// <summary>
    /// ダイレクト印刷アプリケーションメイン
    /// </summary>
    static class DirectPrintServicePG
    {
        private static Mutex DirectPrintMutex = null;

        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                //最終例外捕捉イベントの作成
                Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
                //多重起動の防止
                System.Security.AccessControl.MutexSecurity security = new MutexSecurity();
                MutexAccessRule rule = new MutexAccessRule(
                    new System.Security.Principal.SecurityIdentifier(WellKnownSidType.WorldSid, null),
                    MutexRights.Synchronize | MutexRights.Modify,
                    AccessControlType.Allow);
                security.AddAccessRule(rule);
                bool createdNew;

                DirectPrintMutex = new Mutex(false, @"Global\" + "DirectPrintService", out createdNew, security); // Mutex 生成 ; false = 所有権なし
                if (!DirectPrintMutex.WaitOne(0, false))
                {
                    MessageBox.Show("ダイレクト印刷はすでに起動しています");
                    DirectPrintMutex.ReleaseMutex();
                    Application.Exit();
                }
                else
                {

                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new DirectPrintServiceMain());
                }
            }
            catch (Exception)
            {
                //停止するので不要
                //MessageBox.Show("DirectPrintService Closed. " + ex.Message);
            }
            finally
            {
                if (DirectPrintMutex != null)
                {
                    DirectPrintMutex.ReleaseMutex();
                }
            }

        }
        /// <summary>
        /// 最終例外捕捉イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            //例外ログ
            LogUtility.OutputStaticLog("DP122", CommonConstants.LOGLEVEL_ERROR, DirectConstants.STATIC_LOG_DP_999 + e.Exception.ToString());
        }
    }
}
