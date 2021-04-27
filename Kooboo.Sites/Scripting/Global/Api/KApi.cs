using Jint.Native;
using Jint.Native.Function;
using Kooboo.Data.Context;
using Kooboo.Lib.Helper;
using System;
using System.Collections.Generic;

namespace Kooboo.Sites.Scripting.Global.Api
{
    public class KApi
    {
        private readonly RenderContext _renderContext;

        public KApi(RenderContext renderContext)
        {
            _renderContext = renderContext;
        }

        public void Get(MulticastDelegate action, IDictionary<string, object> options) => On(action, "GET", options);
        public void Get(MulticastDelegate action) => Get(action, null);

        public void Post(MulticastDelegate action, IDictionary<string, object> options) => On(action, "POST", options);
        public void Post(MulticastDelegate action) => Post(action, null);

        public void Put(MulticastDelegate action, IDictionary<string, object> options) => On(action, "PUT", options);
        public void Put(MulticastDelegate action) => Put(action, null);

        public void Delete(MulticastDelegate action, IDictionary<string, object> options) => On(action, "DELETE", options);
        public void Delete(MulticastDelegate action) => Delete(action, null);

        private void On(MulticastDelegate action, string method, IDictionary<string, object> options)
        {
            if (_renderContext.Request.Method != method) return;
            var func = action.Target as ScriptFunctionInstance;

            var result = func.Call(func, new JsValue[] { }).ToObject();

            if (result != null)
            {
                var json = JsonHelper.SerializeCaseSensitive(result);
                _renderContext.Response.AppendString(json);
            }
        }
    }
}
