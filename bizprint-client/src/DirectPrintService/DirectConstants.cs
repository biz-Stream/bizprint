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
using BizPrintCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectPrintService
{
    /// <summary>
    /// Directのみで使用する固定値定義クラス
    /// </summary>
    public static class DirectConstants
    {

        //ログ設定読み込み前に出力する固定ログ内容
        public const string Direct_Static_logString_001 = "DirectPrintService Start.";

        public const string STATIC_LOG_DP_000 = "DirectPrintService Start.";

        public const string STATIC_LOG_DP_888 = "多重起動エラーを検知しました。終了します。";
        public const string STATIC_LOG_DP_999 = "最終例外補足でExceptionを検知しました:";

    }
}
