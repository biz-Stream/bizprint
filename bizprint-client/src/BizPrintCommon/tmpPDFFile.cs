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

namespace BizPrintCommon
{
    /// <summary>
    /// 印刷一時ファイルクラス(PDFファイル実体作成、削除)
    /// </summary>
    public class TmpPDFFile
    {

        /// <summary>
        /// ファイルフルパス。取得可能、設定不可
        /// </summary>
        public string PrintFilePath { private set; get; } = String.Empty;
        /// <summary>
        /// 拡張子付きのファイル名
        /// </summary>
        public string PrintFileNameWithExt { private set; get; } = String.Empty;
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TmpPDFFile()
        {
        }
        public bool CreateTmpFile(string path, string jobID, byte[] data)
        {

            //フォルダ存在確認。無ければ作る
            if (!System.IO.Directory.Exists(path))
            {
                try
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                catch (Exception ex)
                {
                    //作成に失敗したのでエラーリターン
                    LogUtility.OutputLog("118", path, ex.Message);
                    return false;
                }
            }
            //ファイル名決定
            string fullPath = path + "\\" + jobID + ".pdf";
            PrintFileNameWithExt = jobID + ".pdf";
            //かぶってたら(n)をつけていく
            if (System.IO.File.Exists(fullPath))
            {
                int plus = 1;
                while (System.IO.File.Exists(fullPath))
                {
                    fullPath = path + "\\" + jobID + "(" + plus + ")" + ".pdf";
                    PrintFileNameWithExt = jobID + "(" + plus + ")" + ".pdf";
                    plus++;
                }
            }
            PrintFilePath = fullPath;
            try
            {
                FileStream newFile = new FileStream(fullPath, FileMode.Create);
                BinaryWriter bw = new BinaryWriter(newFile);
                bw.Write(data);
                bw.Close();
                newFile.Close();
            }
            catch (Exception ex)
            {
                //失敗時(ログID:BP102)
                LogUtility.OutputLog("102", PrintFilePath, ex.Message);
                return false;
            }

            //ファイル作成、書き込み
            //作成成功(ログID:BP081)
            LogUtility.OutputLog("081", PrintFilePath);
            return true;
        }
        /// <summary>
        /// 一時ファイルの能動的削除
        /// </summary>
        public bool DeleteTmpFile()
        {
            if (!SettingManeger.CleanupTmpFolder)
            {
                LogUtility.OutputLog("484");
                return true;
            }

            //ファイル存在確認
            if (System.IO.File.Exists(PrintFilePath))
            {
                //削除実行
                try
                {
                    System.IO.File.Delete(PrintFilePath);
                }
                catch (Exception e)
                {
                    //削除失敗
                    LogUtility.OutputLog("086", PrintFilePath, e.ToString());
                    return false;
                }
                //削除成功(ログID:BP082)
                LogUtility.OutputLog("082", PrintFilePath);
            }
            return true;
        }
        /// <summary>
        /// デストラクタ
        /// </summary>
        ~TmpPDFFile()
        {
            if (!SettingManeger.CleanupTmpFolder)
            {
                LogUtility.OutputLog("485");
                return;
            }

            //念のためファイルが存在していたら削除
            if (System.IO.File.Exists(PrintFilePath))
            {
                FileAttributes attr = System.IO.File.GetAttributes(PrintFilePath);
                //読み取り専用チェック
                if ((attr & System.IO.FileAttributes.ReadOnly) ==
                    System.IO.FileAttributes.ReadOnly)
                {
                    LogUtility.OutputLog("084", PrintFilePath);
                    try
                    {
                        //読み取り専用解除
                        System.IO.File.SetAttributes(PrintFilePath, attr & (~System.IO.FileAttributes.ReadOnly));
                    }
                    catch (Exception e)
                    {
                        //解除失敗
                        LogUtility.OutputLog("085", e.Message);
                        return;
                    }
                }
                //削除実行
                try
                {
                    System.IO.File.Delete(PrintFilePath);
                }
                catch (Exception e)
                {
                    //削除失敗
                    LogUtility.OutputLog("086", PrintFilePath, e.Message);
                    return;
                }
                LogUtility.OutputLog("083", PrintFilePath);
            }
        }
    }
}
