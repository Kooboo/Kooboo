using Jint.Native;
using Jint.Native.Function;
using Jint.Native.Json;
using Kooboo.Data.Attributes;
using Kooboo.Data.Context;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Scripting.Global.Api.Meta;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        [Description(@"
//GET /test?id=23
k.api.get(function (id) {
    return id;
}, [
    {name: 'id',type: 'Number'}
])

//result 23.0
")]
        [KDefineType(Params = new[] { typeof(object), typeof(RootMeta[]) })]
        public void Get(MulticastDelegate action, IDictionary<string, object>[] metas) => On(action, "GET", metas);

        [Description(@"
//GET /test?id=23

k.api.get(function(id){
    return id;
})

//result '23'
")]
        public void Get(MulticastDelegate action) => Get(action, null);

        [Description(@"
//POST /test?id=23
// {
//     'age':40
// }

k.api.post(function (id, body) {
    return {
        id: id,
        body: body
    }
}, [
    { name: 'id', type: 'Number' },
    { name: 'body', from: 'Body', type: 'Object' },
])

//result
// {
//   'id': 23.0,
//   'body': {
//     'age': 40.0
//   }
// }
")]
        [KDefineType(Params = new[] { typeof(object), typeof(RootMeta[]) })]
        public void Post(MulticastDelegate action, IDictionary<string, object>[] metas) => On(action, "POST", metas);

        [Description(@"
//POST /test?id=23
// {
//     'age':40
// }

k.api.post(function (id, body) {
    return {
        id: id,
        body: body
    }
})

//result
// {
//   'id': '23',
//   'body': {
//     'age': 40.0
//   }
// }
")]
        public void Post(MulticastDelegate action) => Post(action, null);

        [Description(@"
//PUT /test?id=23
// {
//     'age':40
// }

k.api.put(function (id, body) {
    return {
        id: id,
        body: body
    }
}, [
    { name: 'id', type: 'Number' },
    { name: 'body', from: 'Body', type: 'Object' },
])

//result
// {
//   'id': 23.0,
//   'body': {
//     'age': 40.0
//   }
// }
")]
        [KDefineType(Params = new[] { typeof(object), typeof(RootMeta[]) })]
        public void Put(MulticastDelegate action, IDictionary<string, object>[] metas) => On(action, "PUT", metas);

        [Description(@"
//PUT /test?id=23
// {
//     'age':40
// }

k.api.put(function (id, body) {
    return {
        id: id,
        body: body
    }
})

//result
// {
//   'id': '23',
//   'body': {
//     'age': 40.0
//   }
// }
")]
        public void Put(MulticastDelegate action) => Put(action, null);

        [Description(@"
//DELETE /test?id=23
k.api.delete(function (id) {
    return id;
},[
    {name:'id',type:'Number'}
])

//result 23.0
")]
        [KDefineType(Params = new[] { typeof(object), typeof(RootMeta[]) })]
        public void Delete(MulticastDelegate action, IDictionary<string, object>[] metas) => On(action, "DELETE", metas);

        [Description(@"
//DELETE /test?id=23
k.api.delete(function (id) {
    return id;
})

//result '23'
")]
        public void Delete(MulticastDelegate action) => Delete(action, null);

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
            if (metas != null) Helpers.CheckRequired(func.FormalParameters, metas);
            var result = new List<JsValue>();

            foreach (var item in func.FormalParameters)
            {
                object value;
                var meta = metas?.FirstOrDefault(f => item.Equals(f["name"]));

                if (meta == null)
                {
                    value = GetDefaultValue(func, item);
                }
                else
                {
                    value = new RootMeta(func.Engine, meta, _renderContext).Value;
                }

                var jsValue = JsValue.FromObject(func.Engine, value);
                result.Add(jsValue);
            }

            return result.ToArray();
        }

        private object GetDefaultValue(ScriptFunctionInstance func, string item)
        {
            object value;

            if (item.ToLower() == "body")
            {
                if (JsonHelper.IsJson(_renderContext.Request.Body))
                {
                    value = new JsonParser(func.Engine).Parse(_renderContext.Request.Body).ToObject();
                }
                else
                {
                    value = Helpers.FormToObject(_renderContext);
                }
            }
            else
            {
                value = _renderContext.Request.QueryString.Get(item);
            }

            return value;
        }
    }
}
