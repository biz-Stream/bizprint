using Microsoft.VisualStudio.TestTools.UnitTesting;
using BizPrintCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BizPrintCommon.Tests
{
    [TestClass()]
    public class WebBrowserUtilTests
    {
        [TestInitialize()]
        public void ReadSetting()
        {
            SettingManeger m_SetMng = new SettingManeger(CommonConstants.MODE_DIRECT);
            if (!m_SetMng.LoadSetting())
            {
                //設定ファイル読み込みに失敗
                return;
            }
            SettingManeger.UpdateLatestEvent();
            WebBrowserUtil.SettingMng = m_SetMng;

        }
        [TestMethod()]
        public void createParamStringTest()
        {
            string chk = WebBrowserUtil.CreateParamString(ErrCodeAndmErrMsg.STATUS_OK);
            //string enced = HttpUtility.UrlEncode("RESULT=SUCCESS&ERROR_CODE=0000&ERROR_CAUSE=&ERROR_DETAILS=正常終了");
            string org = "RESULT=SUCCESS&ERROR_CODE=0000&ERROR_CAUSE=&ERROR_DETAILS=正常に印刷された。";
            Assert.AreEqual(chk, org);

            chk = WebBrowserUtil.CreateParamString(ErrCodeAndmErrMsg.ERR_CODE_0114);
            //enced = HttpUtility.UrlEncode("RESULT=FAIL&ERROR_CODE=0072&ERROR_CAUSE=DATA&ERROR_DETAILS=印刷キュー上限を超えたため、印刷要求を破棄した");
            org = "RESULT=FAIL&ERROR_CODE=0072&ERROR_CAUSE=DATA&ERROR_DETAILS=印刷キュー上限を超えたため、印刷要求を破棄した。";
            Assert.AreEqual(chk, org);


        }

        [TestMethod()]
        public void loadResponseHtmlAndReplaceTaglTest()
        {
            string rtn = WebBrowserUtil.LoadResponseHtmlAndReplaceTag("AAA", "BBB", "CCC", "DDD");



        }

        [TestMethod()]
        public void openByFireFoxTest()
        {
            int rtn = WebBrowserUtil.OpenByBrowser("http://yahoo.co.jp/", "iexplore",false);
            Assert.AreEqual(rtn, 0);
            rtn = WebBrowserUtil.OpenByBrowser("http://yahoo.co.jp/", "firefox", false);
            Assert.AreEqual(rtn, 0);
            rtn = WebBrowserUtil.OpenByBrowser("http://yahoo.co.jp/", "chrome", false);
            Assert.AreEqual(rtn, 0);
        }

        [TestMethod()]
        public void openByEdgeTest()
        {
            int rtn = WebBrowserUtil.OpenByEdge("http://yahoo.co.jp/");
            Assert.AreEqual(rtn, 0);

        }

        [TestMethod()]
        public void createOpenURLTest()
        {
            string str = WebBrowserUtil.CreateOpenURL("firefox","http://localhost:8080/test1020_002/WSS_Sample3_1", "", 404, "20161203_112233");
            Assert.AreNotEqual(str.Length, 0);




        }

        [TestMethod()]
        public void openRespNoIDTest()
        {
            int rtn = WebBrowserUtil.OpenResponceNoID("browser_broker", "http://localhost:8080/test1020_002/WSS_Sample3_1", "", 0202);
            Assert.AreEqual(rtn, 0);
        }
    }
}