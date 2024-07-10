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
    public class AcrobatRegistryUtilTests
    {
        LogUtility m_log;

        [TestInitialize()]
        public void ReadSetting()
        {
            m_log = new LogUtility("..\\..\\..\\DirectPrintService\\Config\\DirectPrintService_logConfig.xml");



        }


        [TestMethod()]
        public void getAcrobatReaderVersionStringTest()
        {
            string chk = AcrobatRegistryUtil.GetAcrobatReaderVersionString();
            Assert.AreEqual(chk, "DC");
        }

        [TestMethod()]
        public void getChkBoxRegdataTest()
        {
            int chk = AcrobatRegistryUtil.SetAcrobatCheckRegistory();
            Assert.AreEqual(chk, 0);
        }

        [TestMethod()]
        public void getAcrobatVersionStringTest()
        {
            string chk = AcrobatRegistryUtil.GetAcrobatVersionString();
            Assert.AreEqual(chk, "DC");
        }
        [TestMethod()]
        public void isUsingAcrobatTest()
        {

            //bool chk = AcrobatRegistryUtil.IsUsingAcrobat32bit();

            //Assert.AreEqual(chk, false);
        }
    }
}