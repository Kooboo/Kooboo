using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.DataTraceAndModify.CustomTraces
{
    public class ComponentTrace : ITraceability
    {
        public string Source { get; private set; }


        readonly string _id;

        public ComponentTrace(string id, string componentName)
        {
            _id = id;
            Source = componentName;
        }

        public IDictionary<string, string> GetTraceInfo()
        {
            return new Dictionary<string, string>
            {
                {"id",_id }
            };
        }
    }
}
