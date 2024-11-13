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
    public class PrintParameterTests
    {
        [TestInitialize()]
        public void ReadSetting()
        {
            LogUtility m_log;
            m_log = new LogUtility("..\\..\\..\\DirectPrintService\\Config\\DirectPrintService_logConfig.xml");

        }
        [TestMethod()]
        public void PrintParameterTest()
        {
            PrintParameter pp = new PrintParameter();
            if (pp.RequestedTime > DateTime.Now)
            {
                Assert.Fail();

            }

        }

        [TestMethod()]
        public void readParamFileTest()
        {
            PrintParameter pp = new PrintParameter();
            string rtn = "";
            rtn += "printerName=FinePrint"; rtn += "\n";
            rtn += "numberOfCopy=33"; rtn += "\n";
            rtn += "selectedTray=AUTO"; rtn += "\n";
            rtn += "jobName=TestJOB"; rtn += "\n";
            rtn += "doFit=true"; rtn += "\n";
            rtn += "responseURL=http://yahoo.co.jp/"; rtn += "\n";
            rtn += "saveFileName=D:\\tmp\\savetest.pdf"; rtn += "\n";
            rtn += "target=TestTarget"; rtn += "\n";
            rtn += "printDialog=true"; rtn += "\n";
            rtn += "fromPage=2"; rtn += "\n";
            rtn += "toPage=3"; rtn += "\n";
            byte[] data = Encoding.ASCII.GetBytes(rtn);

            //デフォルト値
            Assert.AreEqual(pp.PrinterName, "");
            Assert.AreEqual(pp.NumberOfCopy, 1);
            Assert.AreEqual(pp.SelectedTray, "");
            Assert.AreEqual(pp.JobName, "");
            Assert.AreEqual(pp.DoFit, false);
            Assert.AreEqual(pp.ResponseURL, "");
            Assert.AreEqual(pp.SaveFileName, "");
            Assert.AreEqual(pp.TargetFrame, "");
            Assert.AreEqual(pp.IsPrintDialog, false);
            Assert.AreEqual(pp.FromPage, 0);
            Assert.AreEqual(pp.ToPage, -1);
            Assert.AreEqual(pp.BrowserProcessname, "");

            //セット
            int chk = pp.ReadParamFile(data);
            pp.BrowserProcessname = "browser_broker";

            Assert.AreEqual(pp.PrinterName, "FinePrint");
            Assert.AreEqual(pp.NumberOfCopy, 33);
            Assert.AreEqual(pp.SelectedTray, "AUTO");
            Assert.AreEqual(pp.JobName, "TestJOB");
            Assert.AreEqual(pp.DoFit, true);
            Assert.AreEqual(pp.ResponseURL, "http://yahoo.co.jp/");
            Assert.AreEqual(pp.SaveFileName, "D:\\tmp\\savetest.pdf");
            Assert.AreEqual(pp.TargetFrame, "TestTarget");
            Assert.AreEqual(pp.IsPrintDialog, true);
            Assert.AreEqual(pp.FromPage, 2);
            Assert.AreEqual(pp.ToPage, 3);
            Assert.AreEqual(pp.BrowserProcessname, "browser_broker");
            Assert.AreEqual(chk, 0);

        }

        [TestMethod()]
        public void setParamTest()
        {
            PrintParameter pp = new PrintParameter();
            if (pp == null)
            {
                Assert.Fail();
            }
            int chk = 0;
            //マシンにないプリンタはセットされないでエラー
            Assert.AreEqual(pp.PrinterName, "");
            chk = pp.SetParamLine("printerName=AAAAA");
            Assert.AreEqual(pp.PrinterName, "");
            Assert.AreNotEqual(chk, 0);
            //あればセットされる
            chk = pp.SetParamLine("printerName=FinePrint");
            Assert.AreEqual(pp.PrinterName, "FinePrint");
            Assert.AreEqual(chk, 0);
            //空文字はそれでOK
            chk = pp.SetParamLine("printerName=");
            Assert.AreEqual(chk, 0);
            Assert.AreEqual(pp.PrinterName, "");

        }

        [TestMethod()]
        public void IsSafeURLTest()
        {
            Assert.AreEqual(PrintParameter.IsSafeURLString(@"http://localhost:3000/doprint"), true);
            Assert.AreEqual(PrintParameter.IsSafeURLString("AAAAA"), false);
            Assert.AreEqual(PrintParameter.IsSafeURLString("あいうえお"), false);
            Assert.AreEqual(PrintParameter.IsSafeURLString("localhost:3000/doprint"), false);
            Assert.AreEqual(PrintParameter.IsSafeURLString(@"C:\tmp\test.pdf"), false);
            Assert.AreEqual(PrintParameter.IsSafeURLString(""), false);
        }

        [TestMethod()]
        public void IsSafeWindowsPathTest()
        {
            bool rtn = PrintParameter.IsSafeWindowsPath(@"C:\\tmp\\test.pdf");
            rtn = PrintParameter.IsSafeWindowsPath(@"C://tmp//test.pdf");

            Assert.AreEqual(PrintParameter.IsSafeWindowsPath(@"C:\\tmp\\test.pdf"), true);
            Assert.AreEqual(PrintParameter.IsSafeWindowsPath(@"C:/tmp/test.pdf"), false);
            Assert.AreEqual(PrintParameter.IsSafeWindowsPath(@"http://localhost:3000/doprint"), false);
            Assert.AreEqual(PrintParameter.IsSafeWindowsPath("AAAAA"), false);
            Assert.AreEqual(PrintParameter.IsSafeWindowsPath("あいうえお"), false);
            Assert.AreEqual(PrintParameter.IsSafeWindowsPath("localhost:3000/doprint"), false);
            Assert.AreEqual(PrintParameter.IsSafeWindowsPath(""), false);
        }

        [TestMethod()]
        public void TrayNameToNumTest()
        {
            int rtn = 0;
            rtn = PrintParameter.ChangeTrayNameToNum("ErrorTray");
            Assert.AreEqual(rtn, -1);

            rtn = PrintParameter.ChangeTrayNameToNum("");
            Assert.AreEqual(rtn, CommonConstants.DMBIN_AUTO);

            rtn = PrintParameter.ChangeTrayNameToNum("FIRST");
            Assert.AreEqual(rtn, CommonConstants.DMBIN_FIRST);

            rtn = PrintParameter.ChangeTrayNameToNum("UPPER");
            Assert.AreEqual(rtn, CommonConstants.DMBIN_UPPER);

            rtn = PrintParameter.ChangeTrayNameToNum("ONLYONE");
            Assert.AreEqual(rtn, CommonConstants.DMBIN_ONLYONE);

            rtn = PrintParameter.ChangeTrayNameToNum("LOWER");
            Assert.AreEqual(rtn, CommonConstants.DMBIN_LOWER);

            rtn = PrintParameter.ChangeTrayNameToNum("MIDDLE");
            Assert.AreEqual(rtn, CommonConstants.DMBIN_MIDDLE);

            rtn = PrintParameter.ChangeTrayNameToNum("MANUAL");
            Assert.AreEqual(rtn, CommonConstants.DMBIN_MANUAL);

            rtn = PrintParameter.ChangeTrayNameToNum("ENVELOPE");
            Assert.AreEqual(rtn, CommonConstants.DMBIN_ENVELOPE);

            rtn = PrintParameter.ChangeTrayNameToNum("ENVMANUAL");
            Assert.AreEqual(rtn, CommonConstants.DMBIN_ENVMANUAL);

            rtn = PrintParameter.ChangeTrayNameToNum("AUTO");
            Assert.AreEqual(rtn, CommonConstants.DMBIN_AUTO);

            rtn = PrintParameter.ChangeTrayNameToNum("TRACTOR");
            Assert.AreEqual(rtn, CommonConstants.DMBIN_TRACTOR);

            rtn = PrintParameter.ChangeTrayNameToNum("SMALLFMT");
            Assert.AreEqual(rtn, CommonConstants.DMBIN_SMALLFMT);

            rtn = PrintParameter.ChangeTrayNameToNum("LARGEFMT");
            Assert.AreEqual(rtn, CommonConstants.DMBIN_LARGEFMT);

            rtn = PrintParameter.ChangeTrayNameToNum("LARGECAPACITY");
            Assert.AreEqual(rtn, CommonConstants.DMBIN_LARGECAPACITY);

            rtn = PrintParameter.ChangeTrayNameToNum("CASSETTE");
            Assert.AreEqual(rtn, CommonConstants.DMBIN_CASETTE);

            rtn = PrintParameter.ChangeTrayNameToNum("FORMSOURCE");
            Assert.AreEqual(rtn, CommonConstants.DMBIN_FORMSOURCE);

            rtn = PrintParameter.ChangeTrayNameToNum("LAST");
            Assert.AreEqual(rtn, CommonConstants.DMBIN_LAST);



        }

        [TestMethod()]
        public void isSafeTrayNameTest()
        {
            int errCode = PrintParameter.IsPrinterHasTrayName("", "");
            Assert.AreEqual(errCode, 0);

            errCode = PrintParameter.IsPrinterHasTrayName("FinePrint", "");
            Assert.AreEqual(errCode, 0);
            errCode = PrintParameter.IsPrinterHasTrayName("FinePrint", "CUSTOM");
            Assert.AreEqual(errCode, -1);
            errCode = PrintParameter.IsPrinterHasTrayName("FinePrint", "Upper");
            Assert.AreEqual(errCode, 0);
            errCode = PrintParameter.IsPrinterHasTrayName("FinePrint", "MANUAL");
            Assert.AreEqual(errCode, 0);
            errCode = PrintParameter.IsPrinterHasTrayName("ErrName", "CUSTUM");
            Assert.AreEqual(errCode, -1);
            errCode = PrintParameter.IsPrinterHasTrayName("Fax", "CUSTOM");
            Assert.AreEqual(errCode, -1);
            errCode = PrintParameter.IsPrinterHasTrayName("Fax", "Upper");
            Assert.AreEqual(errCode, 0);
            errCode = PrintParameter.IsPrinterHasTrayName("Adobe PDF", "Auto");
            Assert.AreEqual(errCode, 0);
            errCode = PrintParameter.IsPrinterHasTrayName("RICOH imagio MP C4002 RPCS", "AUTO");
            Assert.AreEqual(errCode, 0);
            errCode = PrintParameter.IsPrinterHasTrayName("RICOH imagio MP C4002 RPCS", "TEST");
            Assert.AreEqual(errCode, -1);
        }

        [TestMethod()]
        public void isDefaultPrinterSettedTest()
        {
            bool chk = PrintParameter.IsDefaultPrinterSetted();
            if (!chk) {
                Assert.AreEqual(chk, false);
            }
        }
    }
}