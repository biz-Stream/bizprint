using Microsoft.VisualStudio.TestTools.UnitTesting;
using BizPrintCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BizPrintCommon.PrintHistoryManager;
using System.Xml;

namespace BizPrintCommon.Tests
{
    [TestClass()]
    public class StatusResponceCreaterTests
    {
        LogUtility m_log;
        public SettingManeger m_SetMng { set; get; }
        [TestInitialize()]
        public void ReadSetting()
        {
//            m_log = new LogUtility("..\\..\\..\\DirectPrintService\\Config\\DirectPrintService_logConfig.xml");
            m_log = new LogUtility("D:\\work\\v5_REP\\bizprint_client\\DirectPrintService\\Config\\DirectPrintService_logConfig.xml");
            m_SetMng = new SettingManeger(CommonConstants.MODE_DIRECT);
            if (!m_SetMng.LoadSetting())
            {
                //設定ファイル読み込みに失敗
                return;
            }
            SettingManeger.UpdateLatestEvent();
            PrintHistoryManager.SettingMng = m_SetMng;



        }
        [TestMethod()]
        public void makeStatusReqResponceXMLTest()
        {
            //ALL
            
            int num = -1;
            num = PrintHistoryManager.GetHistoryListCount();
            Assert.AreEqual(num, 0);
            for (int i = 0; i < 10; i++)
            {
                PrintHistoryInfo newInfo = new PrintHistoryInfo("TEST" + i.ToString(), 0);
                newInfo.statusCode = i;
                newInfo.printFileName = "TestJobName" + i;
                newInfo.statusCode = i;
                newInfo.statusString = "testStatus" + i;
                newInfo.printerName = "TestPtinter&AAA" + i;
                PrintHistoryManager.AddNewHistory(newInfo);
            }
            num = GetHistoryListCount();
            Assert.AreEqual(num, 10);

            StatusRequest sr = new StatusRequest();
            sr.ReadParam("");

            string str = StatusResponceCreater.MakeStatusReqResponceXML(sr);
            if (str.Length == 0)
            {
                Assert.Fail();
            }

        }

        [TestMethod()]
        public void makeStatusReqResponceXMLTest1()
        {
            //ALL
            int num = -1;
            num = PrintHistoryManager.GetHistoryListCount();
            Assert.AreEqual(num, 0);
            for (int i = 0; i < 10; i++)
            {
                PrintHistoryInfo newInfo = new PrintHistoryInfo("TEST" + i.ToString(), 0);
                newInfo.statusCode = i;
                newInfo.printFileName = "TestJobName" + i;
                newInfo.statusCode = i;
                newInfo.statusString = "testStatus" + i;
                newInfo.printerName = "TestPtinter" + i;
                PrintHistoryManager.AddNewHistory(newInfo);
            }
            num = GetHistoryListCount();
            Assert.AreEqual(num, 10);

            StatusRequest sr = new StatusRequest();
            //1個、成功
            sr.ReadParam("jobID=TEST3");
            string str = StatusResponceCreater.MakeStatusReqResponceXML(sr);
            XmlDocument document = new XmlDocument();
            
            document.LoadXml(str);

            //1個、FAIL
            sr.ReadParam("jobID=NONID");
            str = StatusResponceCreater.MakeStatusReqResponceXML(sr);

            //複数リクエスト、1個だけある
            sr.ReadParam("jobID=TESTJOBID1&jobID=TEST2");
            str = StatusResponceCreater.MakeStatusReqResponceXML(sr);

            //複数リクエスト、1個も無い
            sr.ReadParam("jobID=ERRID&jobID=NONID");
            str = StatusResponceCreater.MakeStatusReqResponceXML(sr);

            if (str.Length == 0)
            {
                Assert.Fail();
            }
        }
    }
}