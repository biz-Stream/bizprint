using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BizPrintCommon
{
    public class BatchRestarter
    {
        /// <summary>再起動要求イベント</summary>
        public static ManualResetEvent MrRestartRequest { set; get; } = new ManualResetEvent(false);


    }
}
