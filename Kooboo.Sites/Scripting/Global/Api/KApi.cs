using Jint.Native;
using Jint.Native.Function;
using Kooboo.Data.Attributes;
using Kooboo.Data.Context;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Scripting.Global.Api.Meta;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Sites.Scripting.Global.Api
{
    public class KApi
    {
        private readonly RenderContext _renderContext;

        public KApi(RenderContext renderContext)
        {
            _renderContext = renderContext;
        }

        [KDefineType(Params = new[] { typeof(object), typeof(RootMeta[]) })]
        public void Get(MulticastDelegate action, IDictionary<string, object>[] metas) => On(action, "GET", metas);
        public void Get(MulticastDelegate action) => Get(action, new IDictionary<string, object>[0]);

        [KDefineType(Params = new[] { typeof(object), typeof(RootMeta[]) })]
        public void Post(MulticastDelegate action, IDictionary<string, object>[] metas) => On(action, "POST", metas);
        public void Post(MulticastDelegate action) => Post(action, new IDictionary<string, object>[0]);

        public void Put(MulticastDelegate action, IDictionary<string, object>[] metas) => On(action, "PUT", metas);
        public void Put(MulticastDelegate action) => Put(action, new IDictionary<string, object>[0]);

        public void Delete(MulticastDelegate action, IDictionary<string, object>[] metas) => On(action, "DELETE", metas);
        public void Delete(MulticastDelegate action) => Delete(action, new IDictionary<string, object>[0]);

        private void On(MulticastDelegate action, string method, IDictionary<string, object>[] metas)
        {
            if (_renderContext.Request.Method != method) return;
            metas = Helpers.NamedMetas(metas);
            var func = action.Target as ScriptFunctionInstance;
            var parameters = GetParameters(func, metas);
            var result = func.Call(func, parameters).ToObject();

            if (result != null)
            {
                var json = JsonHelper.SerializeCaseSensitive(result);
                _renderContext.Response.AppendString(json);
            }
        }

        private JsValue[] GetParameters(ScriptFunctionInstance func, IDictionary<string, object>[] metas)
        {
            Helpers.CheckRequired(func.FormalParameters, metas);
            var result = new List<JsValue>();

            foreach (var item in func.FormalParameters)
            {
                object value;
                var meta = metas.FirstOrDefault(f => item.Equals(f["name"]));
                if (meta == null) value = _renderContext.Request.QueryString.Get(item);
                else value = new RootMeta(func.Engine, meta, _renderContext).Value;
                var jsValue = JsValue.FromObject(func.Engine, value);
                result.Add(jsValue);
            }

            return result.ToArray();
        }
    }
}
