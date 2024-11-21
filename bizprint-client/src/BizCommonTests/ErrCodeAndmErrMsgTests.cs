// Copyright 2024 BrainSellers.com Corporation
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
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