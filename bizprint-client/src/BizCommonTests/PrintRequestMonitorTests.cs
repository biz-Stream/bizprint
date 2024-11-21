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
using System.Threading;

namespace BizPrintCommon.Tests
{
    [TestClass()]
    public class PrintRequestMonitorTests
    {
        LogUtility m_log;
        public SettingManeger m_SetMng { set; get; }


        [TestInitialize()]
        public void ReadSetting()
        {
            string logConfPath = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\" + ServicetConstants.DirectConfFolderName + "\\" + ServicetConstants.DPlogConfFileName;
            LogUtility.InitLog4Net(logConfPath);
            LogUtility.InitLogUtility(ServicetConstants.DPlogIDBase, ServicetConstants.DPlogConfBaseDirect, ServicetConstants.DirectConfFolderName);

        }

        [TestMethod()]
        public void PrintRequestMonitorTest()
        {
            int errCode = 0;
            PrintRequestMonitor PRM = new PrintRequestMonitor("FinePrint", m_SetMng);
            PRM.Start();
            while (true) {
                if (PRM.IsJobSetted) {
                    break;
                }
                Thread.Sleep(10000);
            }
            PRM.Stop();
            Assert.AreEqual(errCode, 0);
        }

        [TestMethod()]
        public void StartTest()
        {
            int errCode = 0;
            PrintRequestMonitor PRM = new PrintRequestMonitor("FinePrint", m_SetMng);


            LogUtility.OutputLog("096", PRM.getPrinterMsg(PRINTER_CHANGES.PRINTER_CHANGE_ADD_PRINTER));
            LogUtility.OutputLog("096", PRM.getPrinterMsg(PRINTER_CHANGES.PRINTER_CHANGE_SET_PRINTER));
            LogUtility.OutputLog("096", PRM.getPrinterMsg(PRINTER_CHANGES.PRINTER_CHANGE_DELETE_PRINTER));
            LogUtility.OutputLog("096", PRM.getPrinterMsg(PRINTER_CHANGES.PRINTER_CHANGE_FAILED_CONNECTION_PRINTER));
            LogUtility.OutputLog("096", PRM.getPrinterMsg(PRINTER_CHANGES.PRINTER_CHANGE_PRINTER));
            LogUtility.OutputLog("096", PRM.getPrinterMsg(PRINTER_CHANGES.PRINTER_CHANGE_ADD_JOB));
            LogUtility.OutputLog("096", PRM.getPrinterMsg(PRINTER_CHANGES.PRINTER_CHANGE_SET_JOB));
            LogUtility.OutputLog("096", PRM.getPrinterMsg(PRINTER_CHANGES.PRINTER_CHANGE_DELETE_JOB));
            LogUtility.OutputLog("096", PRM.getPrinterMsg(PRINTER_CHANGES.PRINTER_CHANGE_WRITE_JOB));
            LogUtility.OutputLog("096", PRM.getPrinterMsg(PRINTER_CHANGES.PRINTER_CHANGE_JOB));
            LogUtility.OutputLog("096", PRM.getPrinterMsg(PRINTER_CHANGES.PRINTER_CHANGE_ADD_FORM));
            LogUtility.OutputLog("096", PRM.getPrinterMsg(PRINTER_CHANGES.PRINTER_CHANGE_SET_FORM));
            LogUtility.OutputLog("096", PRM.getPrinterMsg(PRINTER_CHANGES.PRINTER_CHANGE_DELETE_FORM));
            LogUtility.OutputLog("096", PRM.getPrinterMsg(PRINTER_CHANGES.PRINTER_CHANGE_FORM));
            LogUtility.OutputLog("096", PRM.getPrinterMsg(PRINTER_CHANGES.PRINTER_CHANGE_ADD_PORT));
            LogUtility.OutputLog("096", PRM.getPrinterMsg(PRINTER_CHANGES.PRINTER_CHANGE_CONFIGURE_PORT));
            LogUtility.OutputLog("096", PRM.getPrinterMsg(PRINTER_CHANGES.PRINTER_CHANGE_DELETE_PORT));
            LogUtility.OutputLog("096", PRM.getPrinterMsg(PRINTER_CHANGES.PRINTER_CHANGE_PORT));
            LogUtility.OutputLog("096", PRM.getPrinterMsg(PRINTER_CHANGES.PRINTER_CHANGE_ADD_PRINT_PROCESSOR));
            LogUtility.OutputLog("096", PRM.getPrinterMsg(PRINTER_CHANGES.PRINTER_CHANGE_DELETE_PRINT_PROCESSOR));
            LogUtility.OutputLog("096", PRM.getPrinterMsg(PRINTER_CHANGES.PRINTER_CHANGE_PRINT_PROCESSOR));
            LogUtility.OutputLog("096", PRM.getPrinterMsg(PRINTER_CHANGES.PRINTER_CHANGE_ADD_PRINTER_DRIVER));
            LogUtility.OutputLog("096", PRM.getPrinterMsg(PRINTER_CHANGES.PRINTER_CHANGE_SET_PRINTER_DRIVER));
            LogUtility.OutputLog("096", PRM.getPrinterMsg(PRINTER_CHANGES.PRINTER_CHANGE_DELETE_PRINTER_DRIVER));
            LogUtility.OutputLog("096", PRM.getPrinterMsg(PRINTER_CHANGES.PRINTER_CHANGE_PRINTER_DRIVER));
            LogUtility.OutputLog("096", PRM.getPrinterMsg(PRINTER_CHANGES.PRINTER_CHANGE_TIMEOUT));
            LogUtility.OutputLog("096", PRM.getPrinterMsg(PRINTER_CHANGES.PRINTER_CHANGE_ALL));

            //UNKNOWN
            LogUtility.OutputLog("096", PRM.getPrinterMsg(0));
            Assert.AreEqual(errCode, 0);
        }

        [TestMethod()]
        public void PrinterNotifyWaitCallbackTest()
        {
            int errCode = 0;
            Assert.AreEqual(errCode, 0);
        }

        [TestMethod()]
        public void StopTest()
        {
            int errCode = 0;
            Assert.AreEqual(errCode, 0);
        }
    }
}