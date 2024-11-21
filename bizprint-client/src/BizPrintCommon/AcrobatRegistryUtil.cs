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
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BizPrintCommon
{
    /// <summary>
    /// Acrobat/AcrobatReaderのレジストリ操作
    /// </summary>
    public class AcrobatRegistryUtil
    {
        /// <summary>
        /// Acrobatの実行ファイルパスが格納されているレジストリ
        /// </summary>
        const string PATH_REG_ACROBAT = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\Acrobat.exe";
        /// <summary>
        /// AcrobatReaderの実行パスが格納されているレジストリ
        /// </summary>
        const string PATH_REG_ACROREADER = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\AcroRd32.exe";

        /// <summary>
        /// チェックボックスダイアログ情報が書き込まれているレジストリ情報。バージョンが間に入る
        /// </summary>
        const string REG_PATH_ACRO_FROM_HKEY = @"HKEY_CURRENT_USER\SOFTWARE\Adobe\Adobe Acrobat\";
        const string REG_PATH_ACRO_FROM_SOFT = @"SOFTWARE\Adobe\Adobe Acrobat\";
        const string REG_PATH_RD_FROM_HKEY = @"HKEY_CURRENT_USER\SOFTWARE\Adobe\Acrobat Reader\";
        const string REG_PATH_RD_FROM_SOFT = @"SOFTWARE\Adobe\Acrobat Reader\";

        const string REG_ACROBAT_EXEC = @"HKEY_LOCAL_MACHINE\Software\Classes\SOFTWARE\Adobe\Acrobat\Exe";

        const string REG_ACROBAT_TYPE = "HKEY_LOCAL_MACHINE\\Software\\Adobe\\Adobe Acrobat\\DC\\Installer";
        /// <summary>書き込み先レジストリ固定値</summary>
        const string REG_PATH_CHK_TO = @"\AVAlert\cCheckbox\cAcrobat";
        const string REGNAME_CHK = "iWarnScriptPrintAll";
        const string ACROBAT_READER_FOLDER_CHK = "Acrobat Reader ";
        const string ACROBAT_FOLDER_CHK = "Acrobat ";

        /// <summary>取得されたバージョン文字列</summary>
        private static string VersionStringAcrobat = "";
        private static string VersionStringReader = "";
        /// <summary>整数（REG_DWORD）で1が設定されていた場合に、チェックダイアログが表示されなくなる</summary>
        const int REG_CHK_ON = 1;
        /// <summary>
        /// Readerのバージョン文字列を取得
        /// </summary>
        /// <returns></returns>
        public static string GetAcrobatReaderVersionString()
        {
            string rtn = "";
            string regValue = (string)Microsoft.Win32.Registry.GetValue(PATH_REG_ACROREADER, "Path", "");
            if (regValue != null && regValue.Length > 0)
            {


                string[] splitted = regValue.Split(new char[] { '\\' });

                for (int i = 0; i < splitted.Length; i++)
                {
                    if (splitted[i].StartsWith(ACROBAT_READER_FOLDER_CHK))
                    {
                        rtn = splitted[i].Substring(ACROBAT_READER_FOLDER_CHK.Length);
                    }

                }
            }

            return rtn;
        }
        /// <summary>
        /// Readerだけではなく、Acrobatがインストールされてバージョン違いの可能性があるのでチェック
        /// </summary>
        /// <returns></returns>
        public static string GetAcrobatVersionString()
        {
            //インストールされていなければ空文字列が帰る
            string rtn = "";
            string regValue = (string)Microsoft.Win32.Registry.GetValue(PATH_REG_ACROBAT, "Path", "");
            if (regValue != null && regValue.Length > 0)
            {


                string[] splitted = regValue.Split(new char[] { '\\' });

                for (int i = 0; i < splitted.Length; i++)
                {
                    if (splitted[i].StartsWith(ACROBAT_FOLDER_CHK))
                    {
                        rtn = splitted[i].Substring(ACROBAT_FOLDER_CHK.Length);
                    }

                }
            }

            return rtn;
        }


        /// <summary>
        /// レジストリ情報の取得をし、無ければ書き込む
        /// </summary>
        public static int SetAcrobatCheckRegistory()
        {
            int rtn = -1;
            //Reader側
            //現在のバージョンを示す文字列を取得
            try
            {
                VersionStringReader = GetAcrobatReaderVersionString();
                if (VersionStringReader != null && VersionStringReader.Length > 0)
                {
                    string chkReg = REG_PATH_RD_FROM_HKEY + VersionStringReader + REG_PATH_CHK_TO;

                    int nowReg = (int)Microsoft.Win32.Registry.GetValue(chkReg, REGNAME_CHK, -1);
                    if (nowReg != REG_CHK_ON)
                    {
                        //取得した結果が1ではない、または取得できなかった場合、作成して1を書き込む
                        rtn = WriteCheckBoxReg(REG_PATH_RD_FROM_SOFT, VersionStringReader);
                    }
                    else
                    {
                        //なにもしなくていい
                        rtn = ErrCodeAndmErrMsg.STATUS_OK;
                    }
                }
            }
            catch (Exception)
            {
                //インストール済みだがキーそのものが存在してない場合はException
                try
                {
                    rtn = WriteCheckBoxReg(REG_PATH_RD_FROM_SOFT, VersionStringReader);
                }
                catch (Exception) { }
            }
            //Acrobat側
            try
            {
                VersionStringAcrobat = GetAcrobatVersionString();
                if (VersionStringAcrobat != null && VersionStringAcrobat.Length > 0)
                {
                    string chkReg = REG_PATH_ACRO_FROM_HKEY + VersionStringAcrobat + REG_PATH_CHK_TO;

                    int nowReg = (int)Microsoft.Win32.Registry.GetValue(chkReg, REGNAME_CHK, -1);
                    if (nowReg != REG_CHK_ON)
                    {
                        //取得した結果が1はない、または取得できなかった場合、作成して1を書き込む
                        rtn = WriteCheckBoxReg(REG_PATH_ACRO_FROM_SOFT, VersionStringAcrobat);
                    }
                    else
                    {
                        //なにもしなくていい
                        rtn = ErrCodeAndmErrMsg.STATUS_OK;
                    }
                }
            }
            catch (Exception)
            {
                //インストール済みだがキーそのものが存在してない場合はException
                try
                {
                    rtn = WriteCheckBoxReg(REG_PATH_ACRO_FROM_SOFT, VersionStringAcrobat);
                }
                catch (Exception)
                {
                }
            }

            return rtn;
        }

        /// <summary>
        /// レジストリへの書き込み実行
        /// </summary>
        /// <returns></returns>
        public static int WriteCheckBoxReg(string FromStr, string VerStr)
        {
            string regStr = FromStr + VerStr + REG_PATH_CHK_TO;
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(regStr);
            if (regKey != null)
            {
                regKey.SetValue(REGNAME_CHK, REG_CHK_ON);
                LogUtility.OutputLog("158", regStr + "\\" + REGNAME_CHK, REG_CHK_ON.ToString());

                regKey.Close();
            }
            else
            {
                return -1;
            }

            return ErrCodeAndmErrMsg.STATUS_OK;
        }
        /// <summary>
        /// 印刷に使用するのがAcrobat 32bit版かを判定する
        /// </summary>
        /// <returns>true:Acrobat 32bit false:それ以外のアプリケーション</returns>

        //public static bool IsUsingAcrobat32bit()
        //{
        //    //インストールされていなければtrue、Acrobat32ならばtrue、それ以外はfalse
        //    string chkReader = "AcroRd32.exe";

        //    string regValue = (string)Microsoft.Win32.Registry.GetValue(REG_ACROBAT_EXEC, "", "");
        //    if (regValue == null || regValue.Length == 0) {
        //        LogUtility.OutputLog("215");
        //        return true;
        //    }
        //    LogUtility.OutputLog("217", regValue);
        //    //OSが64bitかつIs64BitProduct情報が取れたらfalse
        //    if (Environment.Is64BitOperatingSystem) {
        //        //REG_ACROBAT64_TYPE
        //        try {
        //            int reg64BitValue = (int)Microsoft.Win32.Registry.GetValue(REG_ACROBAT_TYPE, "Is64BitProduct", -1);
        //            if (reg64BitValue >= 0)
        //            {
        //                return false;
        //            }

        //        }
        //        catch (Exception ex) {
        //            //キー値が無いので32bit
        //            //LogUtility.OutputLog("997-64bitOs reg64BitValue Catch=" + ex.Message);
        //        }
        //    }
        //    //Acrobat Reader ならばfalse
        //    if (regValue.Length > 0 && regValue.IndexOf(chkReader) > 0)
        //    {
        //        return false;
        //    }
        //    //Acrobat 32biなのでtrue
        //    return true;
        //}

    }
}
