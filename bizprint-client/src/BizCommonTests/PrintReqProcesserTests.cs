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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BizPrintCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DirectPrintService;

namespace BizPrintCommon.Tests
{
    [TestClass()]
    public class PrintReqProcesserTests
    {
        LogUtility m_log;

        public SettingManeger m_SetMng { set; get; }
        private PrintParameter m_param { set; get; } = null;
        private PrintParameter m_param_withDlg { set; get; } = null;
        private static string ORG_FILE = @"D:\tmp\test\org.pdf";

        [TestInitialize()]
        public void ReadSetting()
        {
            m_log = new LogUtility(@"D:\work\v5_REP\V5.0.X\BizStreamWindowsSoftwares\BizPrintCommonTests\Config\DirectPrintService_logConfig.xml");
            LogUtility.InitLogUtility(ServicetConstants.DPlogIDBase, ServicetConstants.DPlogConfBaseDirect, ServicetConstants.DirectConfFolderName);

            m_param = new PrintParameter();
            string rtn = "";
            rtn += "printerName="; rtn += "\n";
            rtn += "numberOfCopy=5"; rtn += "\n";
            rtn += "selectedTray="; rtn += "\n";
            rtn += "jobName=TestJOB"; rtn += "\n";
            rtn += "doFit=true"; rtn += "\n";
            rtn += "responseURL=http://yahoo.co.jp/"; rtn += "\n";
            rtn += "saveFileName="; rtn += "\n";
            rtn += "target="; rtn += "\n";
            rtn += "printDialog="; rtn += "\n";
            rtn += "fromPage=0"; rtn += "\n";
            rtn += "toPage=-1"; rtn += "\n";
            byte[] data = Encoding.ASCII.GetBytes(rtn);
            int chk = m_param.ReadParamFile(data);

            byte[] orgData = File.ReadAllBytes(ORG_FILE);
            m_param.JobID = "20161123_121314";
            m_param.PdfDocumentByte = orgData;
            m_param.PdfFileName = "20161123_121314.pdf";
            m_param.BrowserProcessname = "firefox";

            m_SetMng = new SettingManeger(CommonConstants.MODE_DIRECT);
            if (!m_SetMng.LoadSetting())
            {
                //設定ファイル読み込みに失敗
                return;
            }
            SettingManeger.UpdateLatestEvent();
            WebBrowserUtil.SettingMng = m_SetMng;

        }
        [TestMethod()]
        public void CallPrintFormTest()
        {
            PrintReqQueue.AddReqest(m_param);
            PrintReqProcesser PRP = new PrintReqProcesser(CommonConstants.MODE_DIRECT, m_SetMng);

            PRP.CallPrintForm();

            Assert.AreEqual(0, 0);
        }
    }
}