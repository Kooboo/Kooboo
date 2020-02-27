using Kooboo.Sites.DataTraceAndModify;
using Kooboo.Sites.Scripting.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Scripting.Global.Database
{
    [Newtonsoft.Json.JsonConverter(typeof(JsonConverterKeyValueObject))]
    public class KeyValueObject : ITraceability
    {
        public string Source => "keyvalue";

        readonly string _key;
        readonly string _value;

        public KeyValueObject(string key, string value)
        {
            _key = key;
            _value = value;
        }

        public IDictionary<string, string> GetTraceInfo()
        {
            return new Dictionary<string, string> {
               {"key",_key }
            };
        }

        public override string ToString()
        {
            return _value;
        }
    }
}
