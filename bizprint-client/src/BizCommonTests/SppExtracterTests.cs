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

namespace BizPrintCommon.Tests
{
    [TestClass()]
    public class SppExtracterTests
    {
        LogUtility m_log;
        public SettingManeger m_SetMng { set; get; }
        private PrintParameter m_param { set; get; } = null;
        private PrintParameter m_param_withDlg { set; get; } = null;
        //private static string ORG_FILE = @"D:\tmp\test\org.pdf";
        private static string ERR_FILE = @"D:\tmp\test\aaa.txt";
        private static string OK_FILE = @"D:\tmp\20170112_154246.spp";
        [TestInitialize()]
        public void ReadSetting()
        {
            m_log = new LogUtility("..\\..\\..\\DirectPrintService\\Config\\DirectPrintService_logConfig.xml");
            


        }
        [TestMethod()]
        public void doExtractTest()
        {
            int errCode = 0;
            byte[] orgData = File.ReadAllBytes(ERR_FILE);
            SppExtracter se = new SppExtracter();
            errCode = se.DoExtract(orgData);

            Assert.AreNotEqual(errCode, 0);


        }

        [TestMethod()]
        public void SppExtracterTest()
        {
            int errCode = 0;
            byte[] orgData = File.ReadAllBytes(OK_FILE);
            SppExtracter se = new SppExtracter();
            se.InitPass("dGVzdHBhc3M=");
            errCode = se.DoExtract(orgData);

            Assert.AreEqual(errCode, 0);
        }

        [TestMethod()]
        public void InitPassTest()
        {
            int errCode = 0;
            Assert.AreEqual(errCode, 0);
        }


    }
}