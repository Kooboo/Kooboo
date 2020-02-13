using Kooboo.Sites.DataTrace;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.DataTraceAndModify
{
    public class RepeatItemTrace : ITraceability
    {
        public RepeatItemTrace(string path)
        {
            Path = path;
        }

        public string Source => "repeatitem";

        public string Path { get; }

        public IDictionary<string, string> GetTraceInfo()
        {
            return new Dictionary<string, string>{
                {"path",Path}
            };
        }
    }
}
