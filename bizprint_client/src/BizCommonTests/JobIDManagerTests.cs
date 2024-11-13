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
    public class JobIDManagerTests
    {
        [TestMethod()]
        public void CreateJobIDTest()
        {

            
            string tmp = "org_JobName";
            string newID1 = JobIDManager.CreateJobID(tmp);
            string newID2 = JobIDManager.CreateJobID(tmp);
            string newID3 = JobIDManager.CreateJobID(tmp);
            string newID4 = JobIDManager.CreateJobID(tmp);
            string newID5 = JobIDManager.CreateJobID("Test\\::<&<>==>/A??A_*_A");
            string newID6 = JobIDManager.CreateJobID(":*?<>=AAA");
            Assert.AreNotEqual(newID1, newID2);
            Assert.AreNotEqual(newID1, newID3);
            Assert.AreNotEqual(newID1, newID4);
            Assert.AreNotEqual(newID2, newID3);
            Assert.AreNotEqual(newID2, newID4);
            Assert.AreNotEqual(newID3, newID4);
            Assert.AreNotEqual(newID3, newID5);
            Assert.AreNotEqual(newID6, newID5);
        }
    }
}