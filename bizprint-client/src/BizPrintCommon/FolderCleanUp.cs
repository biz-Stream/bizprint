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
    /// 一時フォルダクリーンアップクラス
    /// </summary>
    public class FolderCleanUp
    {

        /// <summary>
        /// コンストラクタは触らせない
        /// </summary>
        private FolderCleanUp()
        {
        }
        /// <summary>
        /// クリーンアップの実行
        /// </summary>
        /// <param name="cleanupPath">対象パス</param>
        /// <returns>true:成功 false：失敗</returns>
        public static bool CleanUpFolder(string cleanupPath)
        {
            if (!SettingManeger.CleanupTmpFolder) {
                LogUtility.OutputLog("486", cleanupPath);
                return true;
            }
            LogUtility.OutputLog("103", cleanupPath);
            bool rtn = true;
            //pathがあるか、フォルダか確認
            if (!System.IO.Directory.Exists(cleanupPath))
            {
                //存在しない場合はそのままtrueでreturn(一時ファイルを作成する際にフォルダも作成されるため)
                return rtn;
            }

            //フォルダ内のファイルインフォをすべて取得
            string[] fileNames = System.IO.Directory.GetFiles(cleanupPath);

            int NumberOfDelete = 0;
            //全て削除
            for (int i = 0; i < fileNames.Length; i++)
            {

                bool DeleteSuccessed = true;
                DeleteSuccessed = DeleteSingleFile(fileNames[i]);
                //一個削除に失敗した場合でも、全体としてクリーンアップに失敗したことにはしない。
                if (!DeleteSuccessed)
                {
                    //rtn = false;
                }
                else
                {
                    NumberOfDelete++;
                }
            }
            LogUtility.OutputLog("016", NumberOfDelete.ToString());
            return rtn;

        }
        /// <summary>
        /// 対象ファイルの削除
        /// </summary>
        /// <param name="filePath">削除対象パス</param>
        /// <returns>true:成功 false：失敗</returns>
        public static bool DeleteSingleFile(string filePath)
        {
            FileAttributes filesAttribute = System.IO.File.GetAttributes(filePath);
            //読み取り専用チェック
            if ((filesAttribute & System.IO.FileAttributes.ReadOnly) ==
                System.IO.FileAttributes.ReadOnly)
            {
                LogUtility.OutputLog("084", filePath);
                try
                {
                    //読み取り専用解除
                    System.IO.File.SetAttributes(filePath, filesAttribute & (~System.IO.FileAttributes.ReadOnly));
                }
                catch (Exception ex)
                {
                    //解除失敗
                    LogUtility.OutputLog("104", filePath, ex.Message);
                    ErrCodeAndmErrMsg.SetExErrorMsg = ex.Message;
                    return false;
                }
            }
            //削除実行
            try
            {
                System.IO.File.Delete(filePath);
            }
            catch (Exception ex)
            {
                //削除失敗
                LogUtility.OutputLog("106", filePath, ex.Message);
                ErrCodeAndmErrMsg.SetExErrorMsg = ex.Message;
                return false;
            }
            LogUtility.OutputLog("105", filePath);
            return true;
        }
    }

}
