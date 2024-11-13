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
    public class StatusRequestTests
    {
        [TestMethod()]
        public void ReadParamTest()
        {
            StatusRequest st = new StatusRequest();
            st.ReadParam("jobID=TESTJOBID1&jobID=TEST2");
            Assert.AreEqual(st.ReqJobIDList.Count, 2);
            Assert.AreEqual(st.IsAllRequest, false);

            StatusRequest st2 = new StatusRequest();
            st2.ReadParam("jobID=");
            Assert.AreEqual(st2.ReqJobIDList.Count, 1);
            Assert.AreEqual(st2.IsAllRequest, true);
        }
    }
}