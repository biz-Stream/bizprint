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
using System.Text;
using Ionic.Zip;
using System.IO;
using Ionic.Zlib;

namespace com.brainsellers.bizstream.bizprint
{
    /// <summary>
    /// 暗号化圧縮SPPファイルを作成するクラス
    /// </summary>
    public class CreateEncryptSpp
    {
        /// <summary>sppパスワード固定部分前半</summary>
        const String constPassBefore = "___RANDOM_STRINGS1___";

        /// <summary>sppパスワード固定部分後半</summary>
        const String constPassAfter = "___RANDOM_STRINGS2___";

        /// <summary>sppパスワード全体</summary>
        private String sppPassword = constPassBefore + constPassAfter;

        /// <summary>送信パラメータファイル名</summary>
        protected String bsParamName = "param.txt";


        /// <summary>
        /// パスワードのユーザ指定部分をセットし、圧縮パスワードをセットします
        /// </summary>
        /// <param name="enterdPass"></param>
        public void setPassword(string enterdPass)
        {
            sppPassword = enterdPass;
        }

        /// <summary>
        /// sppファイルの圧縮処理本体です。
        /// </summary>
        /// <param name="paramStr">印刷指定パラメータ文字列</param>
        /// <param name="jobName">ジョブ名</param>
        /// <param name="pdfData">印刷対象PDFファイル</param>
        /// <returns></returns>
        public byte[] createCompressedSpp(string paramStr, string jobName, byte[] pdfData)
        {
            if (paramStr == null || paramStr.Length == 0)
            {
                throw new ArgumentException("parameter error");
            }
            if (jobName == null || jobName.Length == 0)
            {
                throw new ArgumentException("jobname error");
            }
            if (pdfData == null || pdfData.Length == 0)
            {
                throw new ArgumentException("pdfdata error");
            }

            MemoryStream outputMemStream = new MemoryStream();
            try
            {
                ZipFile sppFile = new ZipFile(Encoding.GetEncoding("UTF-8"));
                // 圧縮率指定
                sppFile.CompressionLevel = CompressionLevel.Level7;
                // 圧縮パスワード指定
                sppFile.Password = constPassBefore + sppPassword + constPassAfter;
                // 暗号化種別と強度指定
                sppFile.Encryption = Ionic.Zip.EncryptionAlgorithm.WinZipAes256;
                // パラメータファイル書き込み
                sppFile.AddEntry(bsParamName, Encoding.UTF8.GetBytes(paramStr));

                // PDF実体ファイル定義。zipファイル内に拡張子付きで保存する
                if (!jobName.EndsWith(".pdf"))
                {
                    jobName = jobName + ".pdf";
                }
                sppFile.AddEntry(jobName, pdfData);

                sppFile.Save(outputMemStream);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return outputMemStream.ToArray();
        }

    }
}
