using Kooboo.Sites.DataTrace;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.DataTraceAndModify
{
    public class RepeatItemTrace : ITraceability
    {
        public string Source => "repeatitem";

        public IDictionary<string, string> GetTraceInfo()
        {
            return new Dictionary<string, string>();
        }
    }
}
