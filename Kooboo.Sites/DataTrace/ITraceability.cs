using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.DataTrace
{
    public interface ITraceability
    {
        PersistenceMode Source { get; }
        IDictionary<string, string> GetTraceInfo();
    }
}
