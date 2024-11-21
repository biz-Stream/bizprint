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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizPrintCommon
{
    public class ProcKiller
    {
        public ProcKiller()
        {
        }

        /// <summary>
        /// Acrobat関連のプロセスを、自身の子プロセスで合った場合にkillする
        /// 引数はカンマ区切りの対象プロセス名
        /// </summary>
        public void killAcrobatProocesses(string killProcNames)
        {
            LogUtility.OutputLog("440");
            if (killProcNames == null || killProcNames.Length == 0) {
                return;
            }
            //kill対象になる各プロセス名
            string[] killName = killProcNames.Split(',');

            foreach (string name in killName)
            {
#if DEBUG
                LogUtility.OutputLog("997-debug kill-procname="+name);
#endif
                ChkChildProcessAndKill(name);
            }
        }
        /// <summary>
        /// 指定されたプロセス名を探し、自分の子プロセスだったら殺す
        /// </summary>
        /// <param name="pName"></param>
        private void ChkChildProcessAndKill(string pName)
        {
            //このプロセスのID
            int thisID = System.Diagnostics.Process.GetCurrentProcess().Id;

            //プロセス名から検索する
            System.Diagnostics.Process[] ps = System.Diagnostics.Process.GetProcessesByName(pName);
            //配列から1つずつ取り出す
            foreach (System.Diagnostics.Process killp in ps)
            {

                uint parentId = 0;
                var queryString = string.Format("SELECT ParentProcessId FROM Win32_Process WHERE ProcessId = {0}", killp.Id);
                using (var searcher = new System.Management.ManagementObjectSearcher(@"root\CIMV2", queryString))
                    try
                    {
                        //クエリから結果を取得
                        using (var results = searcher.Get().GetEnumerator())
                        {

                            if (!results.MoveNext()) throw new ApplicationException("Couldn't Get ParrentProcessId.");

                            var QueryResult = results.Current;
                            //親プロセスのPIDを取得
                            parentId = (uint)QueryResult["ParentProcessId"];
                        }
                        //親プロセスのIDが自身と一致してたらkill
                        if (parentId == thisID)
                        {
                            LogUtility.OutputLog("441", pName, killp.Id.ToString());
                            KillProcessTree(killp.Id);
                        }
                    }
                    catch (Exception)
                    {
                        //例外が発生しても無視。既にプロセス自体が存在しないなら、殺さなくてもいい。
                        return;
                    }
            }

        }
        /// <summary>
        /// 指定されたプロセスIDから、子プロセス含めて全部殺す
        /// </summary>
        /// <param name="ID">プロセスID</param>
        public static void KillProcessTree(int ID)
        {
            string taskkill = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "taskkill.exe");
            using (var procKiller = new System.Diagnostics.Process())
            {
                procKiller.StartInfo.FileName = taskkill;
                procKiller.StartInfo.Arguments = string.Format("/PID {0} /T /F", ID);
                procKiller.StartInfo.CreateNoWindow = true;
                procKiller.StartInfo.UseShellExecute = false;
                procKiller.Start();
                procKiller.WaitForExit();
            }
        }
    }
}
