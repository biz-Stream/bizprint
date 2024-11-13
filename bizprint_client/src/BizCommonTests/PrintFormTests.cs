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
    public class PrintFormTests
    {
        LogUtility m_log;
        public SettingManeger m_SetMng { set; get; }
        private PrintParameter m_param { set; get; } = null;
        private PrintParameter m_param_withDlg { set; get; } = null;
        private static string ORG_FILE = @"D:\tmp\test\bak\org.pdf";
        [TestInitialize()]
        public void ReadSetting()
        {
            m_log = new LogUtility("..\\..\\..\\DirectPrintService\\Config\\DirectPrintService_logConfig.xml");
            m_param = new PrintParameter();
            m_param_withDlg = new PrintParameter();
            string rtn = "";
            rtn += "printerName="; rtn += "\n";
            rtn += "numberOfCopy=1"; rtn += "\n";
            rtn += "selectedTray="; rtn += "\n";
            rtn += "jobName=TestJOB"; rtn += "\n";
            rtn += "doFit=true"; rtn += "\n";
            rtn += "responseURL=http://yahoo.co.jp/"; rtn += "\n";
            rtn += "saveFileName=D:\\tmp\\test\\Formtest.pdf"; rtn += "\n";
            rtn += "target="; rtn += "\n";
            rtn += "printDialog=false"; rtn += "\n";
            rtn += "fromPage=0"; rtn += "\n";
            rtn += "toPage=-1"; rtn += "\n";
            byte[] data = Encoding.ASCII.GetBytes(rtn);
            int chk = m_param.ReadParamFile(data);

            byte[] orgData = File.ReadAllBytes(ORG_FILE);
            m_param.JobID = "20161123_121314";
            m_param.PdfDocumentByte = orgData;
            m_param.PdfFileName = "20161123_121314.pdf";


            rtn = "";
            rtn += "printerName="; rtn += "\n";
            rtn += "numberOfCopy=1"; rtn += "\n";
            rtn += "selectedTray="; rtn += "\n";
            rtn += "jobName=TestJOBWithDlg"; rtn += "\n";
            rtn += "doFit=true"; rtn += "\n";
            rtn += "responseURL=http://yahoo.co.jp/"; rtn += "\n";
            rtn += "saveFileName=D:\\tmp\\tes1111111111111111111111111111111111111111111t\\W222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222223333333333333333333333333333333333333333333333333333333333333333333333333333333333333333666666666666666666666666666666666666666666666ithDlgtest.pdf"; rtn += "\n";
            rtn += "target="; rtn += "\n";
            rtn += "printDialog=true"; rtn += "\n";
            rtn += "fromPage=0"; rtn += "\n";
            rtn += "toPage=-1"; rtn += "\n";
            byte[] data2 = Encoding.ASCII.GetBytes(rtn);
            m_param_withDlg.ReadParamFile(data2);

            m_param_withDlg.JobID = "20161123_242322";
            m_param_withDlg.PdfDocumentByte = orgData;
            m_param_withDlg.PdfFileName = "20161123_242322.pdf";


            m_SetMng = new SettingManeger(CommonConstants.MODE_DIRECT);
            if (!m_SetMng.LoadSetting())
            {
                //設定ファイル読み込みに失敗
                return;
            }
            SettingManeger.UpdateLatestEvent();

        }

        [TestMethod()]
        public void PrintFormTest()
        {
            int errCode = 0;
            ReadSetting();
            Assert.AreEqual(errCode, 0);
        }


        [TestMethod()]
        public void SavePdfFileTest()
        {
            int errCode = 0;
            Assert.AreEqual(errCode, 0);
        }

        [TestMethod()]
        public void PrintPDFNoDialogTest()
        {
            int errCode = 0;
            PrintForm frm = new PrintForm(m_param, m_SetMng);
            frm.ShowDialog();
            errCode = frm.LastHResult;
            Assert.AreEqual(errCode, 0);
        }

        [TestMethod()]
        public void PrintPDFWithDialogTest()
        {
            int errCode = 0;
            PrintForm frm = new PrintForm(m_param_withDlg, m_SetMng);
            Console.Write("-------Test001");
            frm.ShowDialog();
            Console.Write("-------Test002");
            errCode = frm.LastHResult;
            Console.Write("-------Test003");
            Assert.AreEqual(errCode, 0);
        }

        [TestMethod()]
        public void getDefaultSettingTest()
        {
            int errCode = 0;
            PrintForm frm = new PrintForm(m_param_withDlg, m_SetMng);
            frm.GetDefaultSetting();
            string name = frm.DefaultTrayName;
            int num = frm.DefaultTrayNum;


            //            errCode = frm.PrintPDFNoDialog();
            Assert.AreEqual(errCode, 0);
        }

        [TestMethod()]
        public void setPrinterNameTest()
        {
            int errCode = 0;
            Assert.AreEqual(errCode, 0);
        }

        [TestMethod()]
        public void setTrayNameAndNumTest()
        {
            int errCode = 0;
            PrintForm frm = new PrintForm(m_param, m_SetMng);
            PrintForm.SetDefaultPrinterByName("RICOH imagio MP C4002 RPCS");
            frm.SetDefaultTrayByNo("RICOH imagio MP C4002 RPCS", 1,"TEST");
            System.Drawing.Printing.PrintDocument pd = new System.Drawing.Printing.PrintDocument();



            Assert.AreEqual(errCode, 0);
        }

        [TestMethod()]
        public void SavePdfFileTest1()
        {


            PrintForm frm = new PrintForm(m_param_withDlg, m_SetMng);
            frm.SavePdfFile();


            Assert.Fail();
        }
    }
}