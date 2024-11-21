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
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace BizPrintHealthChecker
{
    class SameProcessChecker
    {
        //kill処理を行ったかのフラグ
        public bool isKilled { set; get; } = false;

        public string killedID = "";
        public string killedName = "";
        public string killedArg = "";

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SameProcessChecker() { }
        /// <summary>
        /// 探索とkillの実行
        /// </summary>
        /// <param name="servicename"></param>
        /// <returns></returns>
        public bool doCheckAndKill(string[] args)
        {
            string commandline = "";
            for (int i = 0; i < args.Length; i++) {
                commandline += args[i];
                if (i < args.Length - 1) {
                    commandline += " ";
                }
            }

            //自分自身のプロセスを取得する
            System.Diagnostics.Process selfproc = System.Diagnostics.Process.GetCurrentProcess();
            //同名プロセスの一覧を取得する
            Process[] bphcAll = System.Diagnostics.Process.GetProcessesByName(BPHCConstants.PROCESSNAME_BPHC);
            if (bphcAll.Length > 0)
            {
                //取得結果に対してループ
                foreach (Process ps in bphcAll)
                {
                    //IDが違う & 引数まで一致する場合
                    if (ps.Id != selfproc.Id)
                    {
                        string psopt = getCommandLineOfProcess(ps.Id);
                        if (psopt.IndexOf(commandline) > -1)
                        {
                            //記録に追加
                            if (killedID.Length > 0) {
                                killedID += ",";
                                killedName += ",";
                                killedArg += ",";
                            }
                            killedID += ps.Id;
                            killedName += BPHCConstants.PROCESSNAME_BPHC;
                            killedArg += commandline;
                            //kill実行
                            SameProcessChecker.killProcById(ps.Id);
                            isKilled = true;
                        }


                    }
                }
            }
            else
            {
                //居ないので何もする必要なし
                return false;
            }
            return true;

        }

        /// <summary>
        /// 指定したプロセスIDのコマンドライン引数を取得する
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private string getCommandLineOfProcess(int id)
        {
            string rtnStr = "";
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT CommandLine FROM Win32_Process WHERE ProcessId = " + id.ToString()))
            using (ManagementObjectCollection objects = searcher.Get())
            {
                foreach (ManagementObject singleobj in objects)
                {
                    if (singleobj["CommandLine"] != null)
                    {
                        rtnStr += singleobj["CommandLine"].ToString();
                    }
                }
                    
            }
            int index = rtnStr.IndexOf(" ");
            if (rtnStr.Length > 0 && index > -1 && index < rtnStr.Length) {
                rtnStr = rtnStr.Substring(index+1);
            }
            return rtnStr;
        }

        public static void killProcById(int id) {
            string taskkill = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "taskkill.exe");
            using (var procKiller = new System.Diagnostics.Process())
            {
                procKiller.StartInfo.FileName = taskkill;
                procKiller.StartInfo.Arguments = string.Format("/PID {0} /T /F", id);
                procKiller.StartInfo.CreateNoWindow = true;
                procKiller.StartInfo.UseShellExecute = false;
                procKiller.Start();
                procKiller.WaitForExit();
            }
        }
    }

}
