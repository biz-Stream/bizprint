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
    public class tmpPDFFileTests
    {
        private static string PATH_TMP = @"D:\tmp\test";
        private static string JobID = "20160102_030405_001";
        private static string JobID2 = "20160102_030405_002";
        private static string ORG_FILE = @"D:\tmp\test\org.pdf";
        [TestMethod()]
        public void tmpPDFFileTest()
        {
            //LogUtility log = new LogUtility("..\\..\\..\\DirectPrintService\\Config\\DirectPrintService_logConfig.xml");

            LogUtility.InitLog4Net("..\\..\\..\\DirectPrintService\\Config\\DirectPrintService_logConfig.xml");
            LogUtility.OutputStaticLog("TEST1",1,"TEST2");

            TmpPDFFile tp = new TmpPDFFile();
            Assert.AreEqual(tp.PrintFilePath, "");

        }

        [TestMethod()]
        public void createFileTest()
        {
            byte[] orgData = File.ReadAllBytes(ORG_FILE);
            LogUtility log = new LogUtility("..\\..\\..\\DirectPrintService\\Config\\DirectPrintService_logConfig.xml");
            TmpPDFFile tp = new TmpPDFFile();
            tp.CreateTmpFile(PATH_TMP,JobID, orgData);
            TmpPDFFile tp2 = new TmpPDFFile();
            tp2.CreateTmpFile(PATH_TMP, JobID, orgData);
            Assert.AreEqual(File.Exists(@"D:\tmp\test\\20160102_030405_001.pdf"), true);

            //デストラクタで削除
        }

        [TestMethod()]
        public void deteleFileTest()
        {
            byte[] orgData = File.ReadAllBytes(ORG_FILE);
            LogUtility log = new LogUtility("..\\..\\..\\DirectPrintService\\Config\\DirectPrintService_logConfig.xml");
            TmpPDFFile tp = new TmpPDFFile();
            tp.CreateTmpFile(PATH_TMP, JobID2, orgData);
            Assert.AreEqual(File.Exists(@"D:\tmp\test\\20160102_030405_002.pdf"), true);
            //明示的に削除
            tp.DeleteTmpFile();
            Assert.AreEqual(File.Exists(@"D:\tmp\test\\20160102_030405_002.pdf"), false);
        }
    }
}