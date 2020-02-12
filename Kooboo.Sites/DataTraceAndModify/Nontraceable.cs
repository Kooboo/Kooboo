using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.DataTrace
{
    public class Nontraceable : ITraceability
    {

        public static Nontraceable Instance { get; } = new Nontraceable();

        public string Source => "none";

        public IDictionary<string, string> GetTraceInfo()
        {
            return new Dictionary<string, string>();
        }
    }
}
