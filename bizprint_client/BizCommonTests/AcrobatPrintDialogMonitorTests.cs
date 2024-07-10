using BizPrintCommon;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizPrintCommon.Tests
{
    [TestClass()]
    public class AcrobatPrintDialogMonitorTests
    {
        LogUtility m_log;

        [TestInitialize()]
        public void ReadSetting()
        {
            m_log = new LogUtility("..\\..\\..\\DirectPrintService\\Config\\DirectPrintService_logConfig.xml");



        }

        [TestMethod()]
        public void isPrintDialogNowTest()
        {
            bool chkFlg = true;
            AcrobatPrintDialogMonitor APD = new AcrobatPrintDialogMonitor("印刷");
            chkFlg = APD.IsPrintDialogNow();
            Assert.AreEqual(chkFlg,false);
        }
    }
}