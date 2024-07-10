using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using BizPrintCommon;
using System.Windows.Forms;

namespace SilentPdfPrinter
{
    /// <summary>
    /// 起動元プロセスのプロセス名を取得する
    /// </summary>
    class ProcessNameChecker
    {
        /// <summary>
        /// プロセスIDを元に、起動元プロセス名を取得して返す
        /// </summary>
        /// <returns></returns>
        public static string GetParentModuleName()
        {
            return Process.GetProcessById((int)GetParentProcessId()).ProcessName;
        }

        /// <summary>
        /// 起動元プロセスIDを取得して返す
        /// </summary>
        /// <returns></returns>
        private static uint GetParentProcessId()
        {
            var myProcId = Process.GetCurrentProcess().Id;
            var query = string.Format("SELECT ParentProcessId FROM Win32_Process WHERE ProcessId = {0}", myProcId);

            using (var search = new System.Management.ManagementObjectSearcher(@"root\CIMV2", query))
            //クエリから結果を取得
            using (var results = search.Get().GetEnumerator())
            {

                if (!results.MoveNext()) throw new ApplicationException("Couldn't Get ParrentProcessId.");

                var queryResult = results.Current;
                //親プロセスのPIDを取得
                return (uint)queryResult["ParentProcessId"];
            }
        }
    }
}
