using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kooboo.Dom;
using Kooboo.Data.Context;

namespace Kooboo.Model.Render
{
    public class TagParseContext
    {
        private Dictionary<string, object> _data;

        public TagParseContext(ViewParseContext viewContext, ViewParseOptions options)
        {
            ViewContext = viewContext;
            Js = viewContext.Js;
            Options = options;
        }

        public ViewParseContext ViewContext { get; }

        public ViewParseOptions Options { get; }

        public IJsBuilder Js { get; }

        public bool TryGet<T>(string key, out T value)
        {
            value = default(T);

            if (_data == null)
                return false;

            if (!_data.TryGetValue(key, out object obj))
                return false;

            value = (T)obj;
            return true;
        }

        public void Set(string key, object value)
        {
            if (_data == null)
            {
                _data = new Dictionary<string, object>();
            }

            _data[key] = value;
        }
    }
}
