using Microsoft.VisualStudio.TestTools.UnitTesting;
using BizPrintCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DirectPrintService;

namespace BizPrintCommon.Tests
{
    [TestClass()]
    public class ErrCodeAndmErrMsgTests
    {
        [TestMethod()]
        public void chgCodeToCauseTest()
        {
            string rtn = ErrCodeAndmErrMsg.ChangeCodeToCause(0000);
            Assert.AreEqual(rtn, ErrCodeAndmErrMsg.ERR_CAUSE_NONE);
        }

        [TestMethod()]
        public void loadErrorDetailFileTest()
        {
            int rtn = 0;
            string str = ErrCodeAndmErrMsg.ChangeCodeToDetail(ErrCodeAndmErrMsg.STATUS_OK);
            str = ErrCodeAndmErrMsg.ChangeCodeToDetail(ErrCodeAndmErrMsg.ERR_CODE_0114);

            ErrCodeAndmErrMsg.SetExErrorMsg = "AAA";
            str = ErrCodeAndmErrMsg.ChangeCodeToDetail(ErrCodeAndmErrMsg.ERR_CODE_0501);
            ErrCodeAndmErrMsg.SetExErrorMsg = "BBB";
            str = ErrCodeAndmErrMsg.ChangeCodeToDetail(ErrCodeAndmErrMsg.ERR_CODE_0501);
            str = ErrCodeAndmErrMsg.ChangeCodeToDetail(ErrCodeAndmErrMsg.ERR_CODE_0409);


            rtn = ErrCodeAndmErrMsg.LoadErrorDetailFile(ServicetConstants.DirectConfFolderName);


            str = ErrCodeAndmErrMsg.ChangeCodeToDetail(ErrCodeAndmErrMsg.STATUS_OK);
            str = ErrCodeAndmErrMsg.ChangeCodeToDetail(ErrCodeAndmErrMsg.ERR_CODE_0114);

            ErrCodeAndmErrMsg.SetExErrorMsg = "AAA";
            str = ErrCodeAndmErrMsg.ChangeCodeToDetail(ErrCodeAndmErrMsg.ERR_CODE_0501);
            ErrCodeAndmErrMsg.SetExErrorMsg = "BBB";
            str = ErrCodeAndmErrMsg.ChangeCodeToDetail(ErrCodeAndmErrMsg.ERR_CODE_0501);
            str = ErrCodeAndmErrMsg.ChangeCodeToDetail(ErrCodeAndmErrMsg.ERR_CODE_0409);
            Assert.AreEqual(rtn,0);
        }
    }
}