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
using DirectPrintService;
using SilentPdfPrinter;

namespace BizPrintCommon.Tests
{
    [TestClass()]
    public class LogUtilityTests
    {
        [TestInitialize()]
        public void ReadSetting()
        {


        }
        [TestMethod()]
        public void outPutDebugLogTest()
        {
            string testid = System.Environment.UserName; ;
            log4net.GlobalContext.Properties["usrname"] = testid;

            //            string logConfPath = "..\\..\\Config\\DirectPrintService_logConfig.xml";
            string logConfPath = "D:\\work\\v5_REP\\bizprint_client\\BizPrintCommon\\conf\\DirectPrintService_logConfig.xml";
            LogUtility.InitLog4Net(logConfPath);
            LogUtility.InitLogUtility(ServicetConstants.DPlogIDBase, ServicetConstants.DPlogConfBaseDirect, ServicetConstants.DirectConfFolderName);
            LogUtility.OutputStaticLog("DP000", CommonConstants.LOGLEVEL_INFO, "DirectPrintService Start.");

            LogUtility.OutputDebugLog("E104");
            LogUtility.OutputDebugLog("E987", "ErrMsg");
            LogUtility.OutputDebugLog("E508", "ErrMsg");
        }
        [TestMethod()]
        public void allLogIDTest()
        {
            string logConfPath = "C:\\ProgramData\\brainsellers\\DirectPrint\\DirectPrintService_logConfig.xml";
            LogUtility.InitLog4Net(logConfPath);
            LogUtility.InitLogUtility(ServicetConstants.DPlogIDBase, ServicetConstants.DPlogConfBaseDirect, ServicetConstants.DirectConfFolderName);

            LogUtility.OutputLog("009", "AAA");
            LogUtility.OutputLog("010", "AAA");
            LogUtility.OutputLog("011", "AAA");
            LogUtility.OutputLog("012", "AAA", "BBB");
            LogUtility.OutputLog("013", "AAA");
            LogUtility.OutputLog("014");
            LogUtility.OutputLog("015", "AAA");
            LogUtility.OutputLog("016", "AAA");
            LogUtility.OutputLog("017", "AAA");
            LogUtility.OutputLog("018");
            LogUtility.OutputLog("019");
            LogUtility.OutputLog("020");
            LogUtility.OutputLog("021", "AAA");
            LogUtility.OutputLog("022");
            LogUtility.OutputLog("023");
            LogUtility.OutputLog("024");
            LogUtility.OutputLog("025");
            LogUtility.OutputLog("026", "AAA", "BBB");
            LogUtility.OutputLog("027", "AAA");
            LogUtility.OutputLog("028", "AAA", "BBB", "CCC");
            LogUtility.OutputLog("029");
            LogUtility.OutputLog("030");
            LogUtility.OutputLog("031");
            LogUtility.OutputLog("032");
            LogUtility.OutputLog("033", "AAA");
            LogUtility.OutputLog("034", "AAA", "BBB");
            LogUtility.OutputLog("035", "AAA");
            LogUtility.OutputLog("036", "AAA");
            LogUtility.OutputLog("037", "AAA");
            LogUtility.OutputLog("038", "AAA");
            LogUtility.OutputLog("039", "AAA");
            LogUtility.OutputLog("040", "AAA", "BBB", "CCC");
            LogUtility.OutputLog("041", "AAA");
            LogUtility.OutputLog("042", "AAA");
            LogUtility.OutputLog("043", "AAA");
            LogUtility.OutputLog("044");
            LogUtility.OutputLog("045");
            LogUtility.OutputLog("046", "AAA");
            LogUtility.OutputLog("047", "AAA");
            LogUtility.OutputLog("048");
            LogUtility.OutputLog("049");
            LogUtility.OutputLog("050");
            LogUtility.OutputLog("051");
            LogUtility.OutputLog("052", "AAA");
            LogUtility.OutputLog("053", "AAA");
            LogUtility.OutputLog("054", "AAA", "BBB");
            LogUtility.OutputLog("055", "AAA");
            LogUtility.OutputLog("056");
            LogUtility.OutputLog("057");
            LogUtility.OutputLog("058", "AAA");
            LogUtility.OutputLog("059", "AAA");
            LogUtility.OutputLog("060", "AAA");
            LogUtility.OutputLog("061", "AAA");
            LogUtility.OutputLog("062", "AAA");
            LogUtility.OutputLog("063", "AAA");
            LogUtility.OutputLog("064", "AAA");
            LogUtility.OutputLog("065", "AAA");
            LogUtility.OutputLog("070");
            LogUtility.OutputLog("071", "AAA");
            LogUtility.OutputLog("072", "AAA");
            LogUtility.OutputLog("073", "AAA");
            LogUtility.OutputLog("074");
            LogUtility.OutputLog("075", "AAA");
            LogUtility.OutputLog("076", "AAA");
            LogUtility.OutputLog("077", "AAA");
            LogUtility.OutputLog("078", "AAA", "BBB");
            LogUtility.OutputLog("079", "AAA", "BBB");
            LogUtility.OutputLog("080", "AAA", "BBB");
            LogUtility.OutputLog("081", "AAA");
            LogUtility.OutputLog("082", "AAA");
            LogUtility.OutputLog("083", "AAA");
            LogUtility.OutputLog("084", "AAA");
            LogUtility.OutputLog("085", "AAA");
            LogUtility.OutputLog("086", "AAA");
            LogUtility.OutputLog("087", "AAA");
            LogUtility.OutputLog("088", "AAA");
            LogUtility.OutputLog("089", "AAA");
            LogUtility.OutputLog("090", "AAA");
            LogUtility.OutputLog("091", "AAA");
            LogUtility.OutputLog("092");
            LogUtility.OutputLog("093", "AAA", "BBB");
            LogUtility.OutputLog("094", "AAA");
            LogUtility.OutputLog("095", "AAA");
            LogUtility.OutputLog("096", "AAA");
            LogUtility.OutputLog("097", "AAA", "BBB");
            LogUtility.OutputLog("099", "AAA", "BBB");
            LogUtility.OutputLog("100", "AAA", "BBB");
            LogUtility.OutputLog("101");
            LogUtility.OutputLog("102", "AAA");
            LogUtility.OutputLog("103", "AAA");
            LogUtility.OutputLog("104", "AAA");
            LogUtility.OutputLog("105", "AAA");
            LogUtility.OutputLog("106", "AAA");
            LogUtility.OutputLog("107", "AAA", "BBB");
            LogUtility.OutputLog("108", "AAA");
            LogUtility.OutputLog("109", "AAA");
            LogUtility.OutputLog("110", "AAA");
            LogUtility.OutputLog("111");
            LogUtility.OutputLog("112");
            LogUtility.OutputLog("113", "AAA");
            LogUtility.OutputLog("114", "AAA");
            LogUtility.OutputLog("115", "AAA");
            LogUtility.OutputLog("116");
            LogUtility.OutputLog("117");
            LogUtility.OutputLog("118", "AAA");
            LogUtility.OutputLog("119", "AAA");
            LogUtility.OutputLog("120");
            LogUtility.OutputLog("121");
            LogUtility.OutputLog("122", "AAA");
            LogUtility.OutputLog("123");
            LogUtility.OutputLog("124", "AAA");
            LogUtility.OutputLog("125");
            LogUtility.OutputLog("126");
            LogUtility.OutputLog("127", "AAA");
            LogUtility.OutputLog("128", "AAA");
            LogUtility.OutputLog("129", "AAA");
            LogUtility.OutputLog("130", "AAA");
            LogUtility.OutputLog("131", "AAA");
            LogUtility.OutputLog("132", "AAA");
            LogUtility.OutputLog("133", "AAA");
            LogUtility.OutputLog("134");
            LogUtility.OutputLog("135", "AAA");
            LogUtility.OutputLog("136");
            LogUtility.OutputLog("137");
            LogUtility.OutputLog("138", "AAA");
            LogUtility.OutputLog("139");
            LogUtility.OutputLog("140");
            LogUtility.OutputLog("141");
            LogUtility.OutputLog("142");
            LogUtility.OutputLog("143", "AAA");
            LogUtility.OutputLog("144");
            LogUtility.OutputLog("145");
            LogUtility.OutputLog("146", "AAA", "BBB");
            LogUtility.OutputLog("147", "AAA", "BBB", "CCC", "DDD");
            LogUtility.OutputLog("148", "AAA", "BBB");
            LogUtility.OutputLog("149", "AAA");
            LogUtility.OutputLog("150", "AAA");
            LogUtility.OutputLog("151", "AAA", "BBB");
            LogUtility.OutputLog("152", "AAA", "BBB");
            LogUtility.OutputLog("153", "AAA", "BBB");
            LogUtility.OutputLog("154", "AAA");
            LogUtility.OutputLog("155", "AAA", "BBB");
            LogUtility.OutputLog("156");
            LogUtility.OutputLog("157", "AAA", "BBB");
            LogUtility.OutputLog("158", "AAA", "BBB");
            LogUtility.OutputLog("166", "AAA", "BBB");
            LogUtility.OutputLog("167", "AAA", "BBB", "CCC", "DDD");
            LogUtility.OutputLog("168", "AAA", "BBB");
            LogUtility.OutputLog("169", "AAA");
            LogUtility.OutputLog("170", "AAA", "BBB");
            LogUtility.OutputLog("171", "AAA");
            LogUtility.OutputLog("172", "AAA");
            LogUtility.OutputLog("173", "AAA", "BBB");
            LogUtility.OutputLog("174", "AAA");
            LogUtility.OutputLog("175", "AAA");
            LogUtility.OutputLog("176", "AAA");
            LogUtility.OutputLog("200");
            LogUtility.OutputLog("201");
            LogUtility.OutputLog("202");
            LogUtility.OutputLog("203", "AAA");
            LogUtility.OutputLog("204", "AAA", "BBB");
            LogUtility.OutputLog("205", "AAA", "BBB", "CCC");
            LogUtility.OutputLog("206", "AAA", "BBB");
            LogUtility.OutputLog("212", "AAA");
            LogUtility.OutputLog("214");
            LogUtility.OutputLog("215");
            LogUtility.OutputLog("501", "AAA", "BBB");
            LogUtility.OutputLog("502", "AAA");
            LogUtility.OutputLog("503", "AAA");
        }
        [TestMethod()]
        public void allLogIDTestBP()
        {
            string logConfPath = "C:\\ProgramData\\brainsellers\\BatchPrint\\BatchPrintService_logConfig.xml";
            LogUtility.InitLog4Net(logConfPath);
            LogUtility.InitLogUtility(ServicetConstants.BPlogIDBaseBatch, ServicetConstants.BPlogConfBaseBatch, ServicetConstants.BatchConfFolderName);

            LogUtility.OutputLog("009", "AAA");
            LogUtility.OutputLog("010", "AAA");
            LogUtility.OutputLog("011", "AAA");
            LogUtility.OutputLog("012", "AAA", "BBB");
            LogUtility.OutputLog("013", "AAA");
            LogUtility.OutputLog("014");
            LogUtility.OutputLog("015", "AAA");
            LogUtility.OutputLog("016", "AAA");
            LogUtility.OutputLog("017", "AAA");
            LogUtility.OutputLog("018");
            LogUtility.OutputLog("019");
            LogUtility.OutputLog("020");
            LogUtility.OutputLog("021", "AAA");
            LogUtility.OutputLog("022");
            LogUtility.OutputLog("023");
            LogUtility.OutputLog("024");
            LogUtility.OutputLog("025");
            LogUtility.OutputLog("026", "AAA", "BBB");
            LogUtility.OutputLog("027", "AAA");
            LogUtility.OutputLog("028", "AAA", "BBB", "CCC");
            LogUtility.OutputLog("029");
            LogUtility.OutputLog("030");
            LogUtility.OutputLog("031");
            LogUtility.OutputLog("032");
            LogUtility.OutputLog("033", "AAA");
            LogUtility.OutputLog("034", "AAA", "BBB");
            LogUtility.OutputLog("035", "AAA");
            LogUtility.OutputLog("036", "AAA");
            LogUtility.OutputLog("037", "AAA");
            LogUtility.OutputLog("038", "AAA");
            LogUtility.OutputLog("039", "AAA");
            LogUtility.OutputLog("040", "AAA", "BBB", "CCC");
            LogUtility.OutputLog("041", "AAA");
            LogUtility.OutputLog("042", "AAA");
            LogUtility.OutputLog("043", "AAA");
            LogUtility.OutputLog("044");
            LogUtility.OutputLog("045");
            LogUtility.OutputLog("046", "AAA");
            LogUtility.OutputLog("047", "AAA");
            LogUtility.OutputLog("048");
            LogUtility.OutputLog("049");
            LogUtility.OutputLog("050");
            LogUtility.OutputLog("051");
            LogUtility.OutputLog("052", "AAA");
            LogUtility.OutputLog("053", "AAA");
            LogUtility.OutputLog("054", "AAA", "BBB");
            LogUtility.OutputLog("055", "AAA");
            LogUtility.OutputLog("056");
            LogUtility.OutputLog("057");
            LogUtility.OutputLog("058", "AAA");
            LogUtility.OutputLog("059", "AAA");
            LogUtility.OutputLog("060", "AAA");
            LogUtility.OutputLog("061", "AAA");
            LogUtility.OutputLog("062", "AAA");
            LogUtility.OutputLog("063", "AAA");
            LogUtility.OutputLog("064", "AAA");
            LogUtility.OutputLog("065", "AAA");
            LogUtility.OutputLog("070");
            LogUtility.OutputLog("071", "AAA");
            LogUtility.OutputLog("072", "AAA");
            LogUtility.OutputLog("073", "AAA");
            LogUtility.OutputLog("074");
            LogUtility.OutputLog("075", "AAA");
            LogUtility.OutputLog("076", "AAA");
            LogUtility.OutputLog("077", "AAA");
            LogUtility.OutputLog("078", "AAA", "BBB");
            LogUtility.OutputLog("079", "AAA", "BBB");
            LogUtility.OutputLog("080", "AAA", "BBB");
            LogUtility.OutputLog("081", "AAA");
            LogUtility.OutputLog("082", "AAA");
            LogUtility.OutputLog("083", "AAA");
            LogUtility.OutputLog("084", "AAA");
            LogUtility.OutputLog("085", "AAA");
            LogUtility.OutputLog("086", "AAA");
            LogUtility.OutputLog("087", "AAA");
            LogUtility.OutputLog("088", "AAA");
            LogUtility.OutputLog("089", "AAA");
            LogUtility.OutputLog("090", "AAA");
            LogUtility.OutputLog("091", "AAA");
            LogUtility.OutputLog("092");
            LogUtility.OutputLog("093", "AAA", "BBB");
            LogUtility.OutputLog("094", "AAA");
            LogUtility.OutputLog("095", "AAA");
            LogUtility.OutputLog("096", "AAA");
            LogUtility.OutputLog("097", "AAA", "BBB");
            LogUtility.OutputLog("099", "AAA", "BBB");
            LogUtility.OutputLog("100", "AAA", "BBB");
            LogUtility.OutputLog("101");
            LogUtility.OutputLog("102", "AAA");
            LogUtility.OutputLog("103", "AAA");
            LogUtility.OutputLog("104", "AAA");
            LogUtility.OutputLog("105", "AAA");
            LogUtility.OutputLog("106", "AAA");
            LogUtility.OutputLog("107", "AAA", "BBB");
            LogUtility.OutputLog("108", "AAA");
            LogUtility.OutputLog("109", "AAA");
            LogUtility.OutputLog("110", "AAA");
            LogUtility.OutputLog("111");
            LogUtility.OutputLog("112");
            LogUtility.OutputLog("113", "AAA");
            LogUtility.OutputLog("114", "AAA");
            LogUtility.OutputLog("115", "AAA");
            LogUtility.OutputLog("116");
            LogUtility.OutputLog("117");
            LogUtility.OutputLog("118", "AAA");
            LogUtility.OutputLog("119", "AAA");
            LogUtility.OutputLog("120");
            LogUtility.OutputLog("121");
            LogUtility.OutputLog("122", "AAA");
            LogUtility.OutputLog("123");
            LogUtility.OutputLog("124", "AAA");
            LogUtility.OutputLog("125");
            LogUtility.OutputLog("126");
            LogUtility.OutputLog("127", "AAA");
            LogUtility.OutputLog("128", "AAA");
            LogUtility.OutputLog("129", "AAA");
            LogUtility.OutputLog("130", "AAA");
            LogUtility.OutputLog("131", "AAA");
            LogUtility.OutputLog("132", "AAA");
            LogUtility.OutputLog("133", "AAA");
            LogUtility.OutputLog("134");
            LogUtility.OutputLog("135", "AAA");
            LogUtility.OutputLog("136");
            LogUtility.OutputLog("137");
            LogUtility.OutputLog("138", "AAA");
            LogUtility.OutputLog("139");
            LogUtility.OutputLog("140");
            LogUtility.OutputLog("141");
            LogUtility.OutputLog("142");
            LogUtility.OutputLog("143", "AAA");
            LogUtility.OutputLog("144");
            LogUtility.OutputLog("145");
            LogUtility.OutputLog("146", "AAA", "BBB");
            LogUtility.OutputLog("147", "AAA", "BBB", "CCC", "DDD");
            LogUtility.OutputLog("148", "AAA", "BBB");
            LogUtility.OutputLog("149", "AAA");
            LogUtility.OutputLog("150", "AAA");
            LogUtility.OutputLog("151", "AAA", "BBB");
            LogUtility.OutputLog("152", "AAA", "BBB");
            LogUtility.OutputLog("153", "AAA", "BBB");
            LogUtility.OutputLog("154", "AAA");
            LogUtility.OutputLog("155", "AAA", "BBB");
            LogUtility.OutputLog("156");
            LogUtility.OutputLog("157", "AAA", "BBB");
            LogUtility.OutputLog("158", "AAA", "BBB");
            LogUtility.OutputLog("166", "AAA", "BBB");
            LogUtility.OutputLog("167", "AAA", "BBB", "CCC", "DDD");
            LogUtility.OutputLog("168", "AAA", "BBB");
            LogUtility.OutputLog("169", "AAA");
            LogUtility.OutputLog("170", "AAA", "BBB");
            LogUtility.OutputLog("171", "AAA");
            LogUtility.OutputLog("172", "AAA");
            LogUtility.OutputLog("173", "AAA", "BBB");
            LogUtility.OutputLog("174", "AAA");
            LogUtility.OutputLog("175", "AAA");
            LogUtility.OutputLog("176", "AAA");
            LogUtility.OutputLog("200");
            LogUtility.OutputLog("201");
            LogUtility.OutputLog("202");
            LogUtility.OutputLog("203", "AAA");
            LogUtility.OutputLog("204", "AAA", "BBB");
            LogUtility.OutputLog("205", "AAA", "BBB", "CCC");
            LogUtility.OutputLog("206", "AAA", "BBB");
            LogUtility.OutputLog("212", "AAA");
            LogUtility.OutputLog("214");
            LogUtility.OutputLog("215");
            LogUtility.OutputLog("501", "AAA", "BBB");
            LogUtility.OutputLog("502", "AAA");
            LogUtility.OutputLog("503", "AAA");


        }
        [TestMethod()]
        public void allLogIDTestSI()
        {
            string logConfPath = "C:\\ProgramData\\brainsellers\\DirectPrint\\"+ SilentConstants.LogConfFileName;
            LogUtility.InitLog4Net(logConfPath);
            LogUtility.InitLogUtility(SilentConstants.LogIDBaseSilent, SilentConstants.LogConfBaseSilent, SilentConstants.ConfFolderName);


            LogUtility.OutputLog("009", "AAA");
            LogUtility.OutputLog("010");
            LogUtility.OutputLog("011");
            LogUtility.OutputLog("012", "AAA", "BBB");
            LogUtility.OutputLog("013");
            LogUtility.OutputLog("014", "AAA");
            LogUtility.OutputLog("016");
            LogUtility.OutputLog("017", "AAA");
            LogUtility.OutputLog("018", "AAA");
            LogUtility.OutputLog("019", "AAA");
            LogUtility.OutputLog("020", "AAA");
            LogUtility.OutputLog("021", "AAA");
            LogUtility.OutputLog("022", "AAA");
            LogUtility.OutputLog("023", "AAA", "BBB");
            LogUtility.OutputLog("024", "AAA");
            LogUtility.OutputLog("025", "AAA");
            LogUtility.OutputLog("026");
            LogUtility.OutputLog("027", "AAA");
            LogUtility.OutputLog("028", "AAA");
            LogUtility.OutputLog("029");
            LogUtility.OutputLog("030");
            LogUtility.OutputLog("031", "AAA");
            LogUtility.OutputLog("032");
            LogUtility.OutputLog("033");
            LogUtility.OutputLog("034");
            LogUtility.OutputLog("035");
            LogUtility.OutputLog("036", "AAA", "BBB", "CCC");
            LogUtility.OutputLog("037", "AAA");
            LogUtility.OutputLog("038", "AAA");
            LogUtility.OutputLog("039", "AAA");
            LogUtility.OutputLog("040", "AAA");

        }
    }
}