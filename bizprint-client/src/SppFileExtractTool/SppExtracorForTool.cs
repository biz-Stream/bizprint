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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ionic.Zip;
using System.Windows.Forms;

namespace SppFileExtractTool
{

    /// <summary>
    /// ログ出力・設定読み込みなどをオミットした、ツール用Sppファイル解凍クラス
    /// </summary>
    class SppExtracorForTool
    {
        /// <summary>
        /// パラメータファイルバイト配列
        /// </summary>
        public byte[] ParamByte { private set; get; } = null;
        /// <summary>
        /// PDFファイルバイト配列
        /// </summary>
        public byte[] PdfByte { private set; get; } = null;
        /// <summary>
        /// PDFファイル名
        /// </summary>
        public string PdfFileName { private set; get; } = "";

        private const string SPPPATH_BEFORE = "___RANDOM_STRINGS1___";
        private const string SPPPATH_AFTER = "___RANDOM_STRINGS2___";


        public const int SPP_INDATA_ERROR = -4;
        public const int SPP_DATA_NUM_ERROR = -3;
        public const int SPP_DATA_ERROR = -2;
        public const int SPP_PASSWDERROR = -1;


        /// <summary>
        /// SPPファイル解凍パスワード文字列
        /// </summary>
        private string SppPassword { set; get; } = "";

        public int InitPass(string settedPass)
        {
            SppPassword = SPPPATH_BEFORE + settedPass + SPPPATH_AFTER;
            return 0;
        }
        /// <summary>
        /// 解凍実行
        /// </summary>
        /// <param name="input">sppファイルのバイト配列</param>
        /// <returns></returns>
        public int DoExtract(byte[] input)
        {
            int rtn = 0;
            MemoryStream spp_Mstp = new MemoryStream(input);
            //zipライブラリで解凍
            try
            {
                ZipFile zip = ZipFile.Read(spp_Mstp);
                if (zip.Entries.Count != 2)
                {
                    //Error
                    return zip.Entries.Count;
                }
                foreach (Ionic.Zip.ZipEntry entry in zip)
                {
                    bool passFlg = false;
                    string orgName = entry.FileName;
                    System.IO.MemoryStream ms = new System.IO.MemoryStream();
                    try
                    {
                        //パスワード無しor2個目以降は設定されたパスワードで解凍try
                        entry.Extract(ms);
                    }
                    catch (Ionic.Zip.BadPasswordException err)
                    {
                        //パスワードがあるので1個目はここは必ずException
                        string errlog = err.Message;
                        passFlg = true;
                    }
                    if (passFlg)
                    {
                        //パスワード設定
                        zip.Password = SppPassword;


                        try
                        {
                            entry.Extract(ms);//パスワード違うとException
                        }
                        catch (Ionic.Zip.BadPasswordException err)
                        {
                            string errlog = err.Message;
                            //パスワード違うとException
                            //パスワード付けても開けないエラー
                            //log 108
                            return SPP_PASSWDERROR;

                        }
                    }
                    byte[] file = ms.ToArray();
                    if (orgName.Equals("param.txt"))
                    {
                        ParamByte = file;
                    }
                    else if (orgName.EndsWith(".pdf"))
                    {
                        PdfByte = file;
                        PdfFileName = orgName;
                    }
                    else
                    {
                        MessageBox.Show(SppToolConstants.ERR_MSG_06 + orgName + ")", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return SPP_INDATA_ERROR;
                    }

                }
                if (PdfByte == null || PdfByte.Length == 0)
                {
                    return SPP_DATA_ERROR;
                }
                if (ParamByte == null)
                {
                    return SPP_DATA_ERROR;
                }
                //PDFファイル名称異常
                if (PdfFileName.Length == 0 || !PdfFileName.EndsWith(".pdf"))
                {
                    return SPP_DATA_ERROR;
                }

            }
            catch (Exception)
            {
                return SPP_DATA_ERROR;
            }

            return rtn;

        }

    }
}
