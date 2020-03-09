using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.DataTraceAndModify
{
    public interface ITraceability
    {
        string Source { get; }
        IDictionary<string, string> GetTraceInfo();
    }
}
