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
    public class PrintQueueTests
    {
        [TestMethod()]
        public void PrintQueueTest()
        {
            //初期化直後は空
            Assert.AreEqual(PrintReqQueue.IsReqQueHaveData(), false);
            //Assert.AreEqual(PrintQueue.isResponceeQueHaveData(), false);
        }

        [TestMethod()]
        public void AddReqestTest()
        {
            bool chk = true;
            PrintParameter param = new PrintParameter();
            try
            {
                PrintReqQueue.AddReqest(param);
            }
            catch (Exception ex)
            {
                ErrCodeAndmErrMsg.SetExErrorMsg = ex.Message;
                chk = false;
            }
            Assert.AreEqual(PrintReqQueue.IsReqQueHaveData(), true);
            Assert.AreEqual(chk, true);
        }

        [TestMethod()]
        public void AddResponceTest()
        {
            bool chk = true;
            //PrintParameter param = new PrintParameter();
            //try
            //{
            //    PrintQueue.AddResponce(param);
            //}
            //catch (Exception ex)
            //{
            //    chk = false;
            //}
            //Assert.AreEqual(PrintQueue.isResponceeQueHaveData(), true);
            Assert.AreEqual(chk, true);
        }

        [TestMethod()]
        public void GetNextReqestTest()
        {
            PrintReqQueue.ClearAllQue();
            bool chk = true;
            PrintParameter param = new PrintParameter();
            param.JobID = "test01";
            try
            {
                PrintReqQueue.AddReqest(param);
            }
            catch (Exception )
            {
                chk = false;
            }
            Assert.AreEqual(PrintReqQueue.IsReqQueHaveData(), true);
            Assert.AreEqual(chk, true);
            PrintParameter param2 = (PrintParameter)PrintReqQueue.GetNextReqest();
            Assert.AreEqual(param2.JobID, "test01");
            Assert.AreEqual(PrintReqQueue.IsReqQueHaveData(), false);

        }

        [TestMethod()]
        public void GetNextResponseTest()
        {
            PrintReqQueue.ClearAllQue();
            bool chk = true;
            //PrintParameter param = new PrintParameter();
            //param.m_jobID = "test02";
            //try
            //{
            //    PrintQueue.AddResponce(param);
            //}
            //catch (Exception ex)
            //{
            //    chk = false;
            //}
            //Assert.AreEqual(PrintQueue.isResponceeQueHaveData(), true);
            Assert.AreEqual(chk, true);
            //PrintParameter param2 = (PrintParameter)PrintQueue.GetNextResponse();
            //Assert.AreEqual(param2.m_jobID, "test02");
            //Assert.AreEqual(PrintQueue.isResponceeQueHaveData(), false);
        }

        [TestMethod()]
        public void ReadNextReqestTest()
        {
            //Readでは消えない事を確認
            PrintReqQueue.ClearAllQue();
            bool chk = true;
            PrintParameter param = new PrintParameter();
            param.JobID = "test03";
            try
            {
                PrintReqQueue.AddReqest(param);
            }
            catch (Exception )
            {
                chk = false;
            }
            Assert.AreEqual(PrintReqQueue.IsReqQueHaveData(), true);
            Assert.AreEqual(chk, true);
           //--テストのみで使用 PrintParameter param2 = (PrintParameter)PrintReqQueue.ReadNextReqest();
           //-- Assert.AreEqual(param2.JobID, "test03");
            Assert.AreEqual(PrintReqQueue.IsReqQueHaveData(), true);
            PrintParameter param3 = (PrintParameter)PrintReqQueue.GetNextReqest();
            Assert.AreEqual(param3.JobID, "test03");
            Assert.AreEqual(PrintReqQueue.IsReqQueHaveData(), false);

        }

        [TestMethod()]
        public void ReadNextResponseTest()
        {
            PrintReqQueue.ClearAllQue();
            bool chk = true;
            //PrintParameter param = new PrintParameter();
            //param.m_jobID = "test04";
            //try
            //{
            //    PrintQueue.AddResponce(param);
            //}
            //catch (Exception ex)
            //{
            //    chk = false;
            //}
            //Assert.AreEqual(PrintQueue.isResponceeQueHaveData(), true);
            Assert.AreEqual(chk, true);
            //PrintParameter param2 = (PrintParameter)PrintQueue.ReadNextResponse();
            //Assert.AreEqual(param2.m_jobID, "test04");
            //Assert.AreEqual(PrintQueue.isResponceeQueHaveData(), true);
            //PrintParameter param3 = (PrintParameter)PrintQueue.GetNextResponse();
            //Assert.AreEqual(param3.m_jobID, "test04");
            //Assert.AreEqual(PrintQueue.isResponceeQueHaveData(), false);

        }
    }
}