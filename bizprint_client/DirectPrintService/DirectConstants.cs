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
