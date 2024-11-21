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
using System.Threading.Tasks;

namespace BizPrintCommon
{
    /// <summary>
    /// JobIDを作成する。オリジナルのJobNameを基に、日時と数値を付加することでユニークとする
    /// </summary>
    public class JobIDManager
    {
        //初期値
        private static int Counter = 0;
        //最大カウンター値
        private const int JOBID_MAX = 9999;
        //前回発行したJOBID
        private static string lastTimeID = "";

        /// <summary>
        /// JOBIDの作成
        /// </summary>
        /// <param name="jobName"></param>
        /// <returns></returns>
        public static string CreateJobID(string jobName)
        {
            string rtn = "";
            if (Counter >= JOBID_MAX)
            {
                Counter = 0;
            }
            //ファイルに使用できない文字、パラメータ指定に仕えない文字を削除
            char[] invChr = System.IO.Path.GetInvalidFileNameChars();
            string noInvaridString = jobName;
            for (int i = 0; i < invChr.Length; i++)
            {
                noInvaridString = noInvaridString.Replace(invChr[i].ToString(), "");
            }
            noInvaridString = noInvaridString.Replace("=", "");
            noInvaridString = noInvaridString.Replace("&", "");

            int startCount = Counter;
            DateTime DT = new DateTime();
            DT = DateTime.Now;
            rtn = noInvaridString + DT.ToString("_yyMMdd_HHmmss_");
            rtn += Counter.ToString("D4");

            //秒まで同じのが来たらカウンター進める事で重複を防ぐ
            while (lastTimeID.Equals(rtn))
            {
                Counter++;
                rtn = jobName + DT.ToString("_yyMMdd_HHmmss_");
                rtn += Counter.ToString("D4");
            }
            lastTimeID = rtn;

            //同じ秒内に要求が来なかった場合にもカウンターは進める
            if (startCount == Counter)
            {
                Counter++;
            }
            LogUtility.OutputLog("077", rtn);

            return rtn;
        }
    }
}
