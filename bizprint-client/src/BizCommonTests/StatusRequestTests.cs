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