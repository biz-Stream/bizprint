using Microsoft.VisualStudio.TestTools.UnitTesting;
using BizPrintCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BizPrintCommon.PrintHistoryManager;

namespace BizPrintCommon.Tests
{
    [TestClass()]
    public class PrintHistoryManagerTests
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
            PrintHistoryManager.SettingMng = m_SetMng;
            

        }

        [TestMethod()]
        public void deletOverMaxHistoryTest()
        {
            int errCode = 0;
            int num = -1;
            num = GetHistoryListCount();
            Assert.AreEqual(num, 0);
            int max = m_SetMng.MaxHistoryNum;
            for (int i = 0; i < max-1; i++)
            {
                PrintHistoryInfo newInfo = new PrintHistoryInfo("TEST" + i.ToString(), 0);
                PrintHistoryManager.AddNewHistory(newInfo);
            }
            num = GetHistoryListCount();
            Assert.AreEqual(num, max - 1);

            //一個足してMAX
            PrintHistoryInfo newInfo2 = new PrintHistoryInfo("TEST9998", 0);
            PrintHistoryManager.AddNewHistory(newInfo2);
            num = GetHistoryListCount();
            Assert.AreEqual(num, max);

            PrintHistoryInfo newInfo4 = PrintHistoryManager.GetHistoryFromId("TEST0");
            Assert.AreNotEqual(newInfo4, null);

            //もう一個足してもMAX、先頭が消える
            PrintHistoryInfo newInfo3 = new PrintHistoryInfo("TEST9999", 0);
            PrintHistoryManager.AddNewHistory(newInfo3);
            num = GetHistoryListCount();
            Assert.AreEqual(num, max);

            newInfo4 = PrintHistoryManager.GetHistoryFromId("TEST0");
            Assert.AreEqual(newInfo4, null);

            Assert.AreEqual(errCode, 0);
        }

        [TestMethod()]
        public void addHistoryTest()
        {
            int errCode = 0;
            int num = -1;
            num = GetHistoryListCount();
            Assert.AreEqual(num, 0);
            for (int i = 0; i < 10; i++)
            {
                PrintHistoryInfo newInfo = new PrintHistoryInfo("TEST" + i.ToString(), 0);
                PrintHistoryManager.AddNewHistory(newInfo);
            }
            num = GetHistoryListCount();
            Assert.AreEqual(num, 10);
            Assert.AreEqual(errCode, 0);
        }

        [TestMethod()]
        public void cleanUpHistoryTest()
        {
            int errCode = 0;
            int num = -1;
            num = GetHistoryListCount();
            Assert.AreEqual(num, 0);
            for (int i = 0; i < 10; i++)
            {
                PrintHistoryInfo newInfo = new PrintHistoryInfo("TEST" + i.ToString(), 0);
                if (i == 2) {
                    TimeSpan ts = new TimeSpan(1, 12, 0, 0);
                    newInfo.createdTime -= ts;
                }
                PrintHistoryManager.AddNewHistory(newInfo);
            }
            num = GetHistoryListCount();
            Assert.AreEqual(num, 10);

            PrintHistoryInfo newInfo4 = PrintHistoryManager.GetHistoryFromId("TEST2");
            Assert.AreNotEqual(newInfo4, null);
            PrintHistoryManager.CleanUpTimeOverHistory();
            num = GetHistoryListCount();
            Assert.AreEqual(num, 9);
            newInfo4 = PrintHistoryManager.GetHistoryFromId("TEST2");
            Assert.AreEqual(newInfo4, null);
            Assert.AreEqual(errCode, 0);
        }



        [TestMethod()]
        public void getAllHistoryTest()
        {
            int errCode = 0;
            int num = -1;
            num = GetHistoryListCount();
            Assert.AreEqual(num, 0);
            PrintHistoryInfo[] allHist = PrintHistoryManager.GetAllHistory();
            Assert.AreEqual(allHist, null);

            for (int i = 0; i < 10; i++)
            {
                PrintHistoryInfo newInfo = new PrintHistoryInfo("TEST" + i.ToString(), 0);
                if (i == 2)
                {
                    TimeSpan ts = new TimeSpan(1, 12, 0, 0);
                    newInfo.createdTime -= ts;
                }
                PrintHistoryManager.AddNewHistory(newInfo);
            }
            num = GetHistoryListCount();
            Assert.AreEqual(num, 10);
            allHist = PrintHistoryManager.GetAllHistory();
            Assert.AreNotEqual(allHist, null);
            Assert.AreEqual(allHist.Length, 10);

            Assert.AreEqual(errCode, 0);
        }

        [TestMethod()]
        public void updatePrintInfoTest()
        {
            int errCode = 0;
            PrintHistoryInfo newInfo = new PrintHistoryInfo("TESTA"  , 0);
            PrintHistoryManager.AddNewHistory(newInfo);

            PrintHistoryInfo getInfo = PrintHistoryManager.GetHistoryFromId("TESTA");
            Assert.AreEqual(getInfo.printFileName, "");
            Assert.AreEqual(getInfo.printerName, "");
            Assert.AreEqual(getInfo.statusCode, 0);
            getInfo.printFileName = "AAAAA";
            getInfo.printerName = "BBBBB";
            getInfo.statusCode = 3;
            PrintHistoryManager.UpdatePrintInfo(getInfo);


            PrintHistoryInfo aftrInfo = PrintHistoryManager.GetHistoryFromId("TESTA");
            Assert.AreEqual(aftrInfo.printFileName, "AAAAA");
            Assert.AreEqual(aftrInfo.printerName, "BBBBB");
            Assert.AreEqual(aftrInfo.statusCode, 3);

            Assert.AreEqual(errCode, 0);
        }

        [TestMethod()]
        public void updatePrintStatusTest()
        {
            int errCode = 0;
            PrintHistoryInfo newInfo = new PrintHistoryInfo("TESTA", 0);
            PrintHistoryManager.AddNewHistory(newInfo);

            int cd = PrintHistoryManager.GetHistoryFromId("TESTA").statusCode;
            Assert.AreEqual(cd, 0);
            PrintHistoryManager.UpdatePrintStatusByID("TESTA", 1);
            //Finish以外で更新しても、印刷時間は更新されない
            DateTime tm = PrintHistoryManager.GetHistoryFromId("TESTA").lastEventTime;
            Assert.AreEqual(tm, DateTime.MinValue);
            cd = PrintHistoryManager.GetHistoryFromId("TESTA").statusCode;
            Assert.AreEqual(cd, 1);

            //Finishの場合は、印刷時間が更新される
            PrintHistoryManager.UpdatePrintStatusByID("TESTA", CommonConstants.JOB_STATUS_SUCCESS_FINISH);
            tm = PrintHistoryManager.GetHistoryFromId("TESTA").lastEventTime;
            Assert.AreNotEqual(tm, DateTime.MinValue);
            cd = PrintHistoryManager.GetHistoryFromId("TESTA").statusCode;
            Assert.AreEqual(cd, CommonConstants.JOB_STATUS_SUCCESS_FINISH);

            Assert.AreEqual(errCode, 0);
        }
    }
}