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

namespace BizPrintCommon.Tests
{
    [TestClass()]
    public class PrinterSettingTests
    {
        LogUtility m_log;
        public SettingManeger m_SetMng { set; get; }
        [TestInitialize()]
        public void ReadSetting()
        {
            m_log = new LogUtility("..\\..\\..\\DirectPrintService\\Config\\DirectPrintService_logConfig.xml");
            m_SetMng = new SettingManeger(CommonConstants.MODE_DIRECT);
            if (!m_SetMng.LoadSetting())
            {
                //設定ファイル読み込みに失敗
                return;
            }
            SettingManeger.UpdateLatestEvent();

        }
        [TestMethod()]
        public void ChangePrinterSettingTest()
        {
            int errCode = 0;
            PrinterSetting PS = new PrinterSetting();
            //PS.ChangePrinterSetting("FinePrint", 1);
            //--PS.setDefaultTrayByNo("FinePrint", 256);
            //PS.ChangePrinterSetting("RICOH imagio MP C4002 RPCS", 1);

            Assert.AreEqual(errCode, 0);

        }

        [TestMethod()]
        public void changeDefaultTrayTest()
        {
            int errCode = 0;
            PrinterSetting PS = new PrinterSetting();
            errCode = PS.ChangeDefaultTray("FinePrint", 1,"UPPER");
            errCode = PS.ChangeDefaultTray("FinePrint", 256,"MANUAL");
            Assert.AreEqual(errCode, 0);
        }

        [TestMethod()]
        public void changePrintNumTest()
        {
            int errCode = 0;
            PrinterSetting PS = new PrinterSetting();

            Assert.AreEqual(errCode, 0);
        }
    }
}