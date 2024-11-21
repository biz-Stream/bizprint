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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SppFileExtractTool
{
    /// <summary>
    /// SPP復号ツール固定値定義
    /// </summary>
    public static class SppToolConstants
    {
        /// <summary>正常終了メッセージ</summary>
        public const string SUCCESS_MSG_001 = "SPPファイルの復号を完了しました。";
        /// <summary>エラーメッセージ</summary>
        #region エラーメッセージ
        public const string ERR_MSG_01 = "入力ファイルが存在しません";
        public const string ERR_MSG_02 = "入力ファイルの形式が異常です";
        public const string ERR_MSG_03 = "入力ファイルの内容を取得できませんでした";
        public const string ERR_MSG_04 = "復号パスワードが間違っています";
        public const string ERR_MSG_05 = "入力ファイルの内容が異常です(ファイル数：";
        public const string ERR_MSG_06 = "入力ファイル内に想定外のファイルが存在します(：";
        public const string ERR_MSG_07 = "ファイルが保存できませんでした(";
        #endregion

        /// <summary>SPPファイル内の固定パラメータファイル名</summary>
        public const string PARAM_FILENAME = "param.txt";
    }
}
