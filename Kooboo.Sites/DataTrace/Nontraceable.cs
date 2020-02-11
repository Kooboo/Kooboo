using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.DataTrace
{
    public class Nontraceable : ITraceability
    {

        public static Nontraceable Instance { get; } = new Nontraceable();

        public PersistenceMode Source => PersistenceMode.none;

        public IDictionary<string, string> GetTraceInfo()
        {
            return new Dictionary<string, string>();
        }
    }
}
