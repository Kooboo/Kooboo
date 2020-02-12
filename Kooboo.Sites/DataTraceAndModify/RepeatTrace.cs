using Kooboo.Sites.DataTrace;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.DataTraceAndModify
{
    public class RepeatTrace : ITraceability
    {
        public string Source => "repeat";

        public IDictionary<string, string> GetTraceInfo()
        {
            return new Dictionary<string, string>();
        }
    }
}
