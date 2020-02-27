using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.DataTraceAndModify.CustomTraces
{
    public class KConfigTrace : ITraceability
    {
        public string Source => "kconfig";

        readonly string _key;

        readonly string _attribute;

        public KConfigTrace(string key, string attribute)
        {
            _key = key;
            _attribute = attribute;
        }

        public IDictionary<string, string> GetTraceInfo()
        {
            var dic = new Dictionary<string, string>
            {
                {"key",_key }
            };

            if (_attribute != null) dic.Add("attribute", _attribute);
            return dic;
        }
    }
}
