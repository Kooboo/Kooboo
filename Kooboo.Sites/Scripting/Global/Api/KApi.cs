using Jint.Native;
using Jint.Native.Function;
using Jint.Native.Json;
using Kooboo.Data.Attributes;
using Kooboo.Data.Context;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Scripting.Global;
using Kooboo.Sites.Scripting.Global.Api.Meta;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace KScript.Api
{
    public class KApi
    {
        private readonly RenderContext _renderContext;

        public KApi(RenderContext renderContext)
        {
            _renderContext = renderContext;
        }

        #region Get
        [Description(@"
//GET /test?id=23

k.api.get(function(id){
    return id;
})

//result '23'
")]
        public void Get(MulticastDelegate action) => Get(action, null, null);

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
        public void Get(MulticastDelegate action, IDictionary<string, object>[] metas) => Get(action, metas, null);

        [Description(@"
//GET /test?id=23
k.api.get(function (id) {
    return id;
},function (code, data) {
    var success = code < 400

    return {
        success: success,
        data: success ? data : null,
        error: success ? null : data,
        code: code
    }
})

//result { 'success': true, 'data': 23, 'error': null, 'code': 200 }
")]
        [KDefineType(Params = new[] { typeof(object), typeof(object) })]
        public void Get(MulticastDelegate action, MulticastDelegate resultHander) => Get(action, null, resultHander);

        [Description(@"
//GET /test?id=23
k.api.get(function (id) {
    return id;
}, [
    {name: 'id',type: 'Number'}
],function (code, data) {
    var success = code < 400

    return {
        success: success,
        data: success ? data : null,
        error: success ? null : data,
        code: code
    }
})

//result { 'success': true, 'data': 23, 'error': null, 'code': 200 }
")]
        [KDefineType(Params = new[] { typeof(object), typeof(RootMeta[]) })]
        public void Get(MulticastDelegate action, IDictionary<string, object>[] metas, MulticastDelegate resultHander) => CallAction(action, "GET", metas, resultHander);
        #endregion

        #region Post
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

//result {'id': '23','body': {'age': 40}}
")]
        public void Post(MulticastDelegate action) => Post(action, null, null);

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

//result {'id': '23','body': {'age': 40}}
")]
        [KDefineType(Params = new[] { typeof(object), typeof(RootMeta[]) })]
        public void Post(MulticastDelegate action, IDictionary<string, object>[] metas) => Post(action, metas, null);

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
},function (code, data) {
    var success = code < 400

    return {
        success: success,
        data: success ? data : null,
        error: success ? null : data,
        code: code
    }
})

//result { 'success': true, 'data': {'id': 23,'body': {'age': 40}}, 'error': null, 'code': 200 }
")]
        [KDefineType(Params = new[] { typeof(object), typeof(object) })]
        public void Post(MulticastDelegate action, MulticastDelegate resultHander) => Post(action, null, resultHander);

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
],function (code, data) {
    var success = code < 400

    return {
        success: success,
        data: success ? data : null,
        error: success ? null : data,
        code: code
    }
})

//result { 'success': true, 'data': {'id': 23,'body': {'age': 40}}, 'error': null, 'code': 200 }
")]
        [KDefineType(Params = new[] { typeof(object), typeof(RootMeta[]) })]
        public void Post(MulticastDelegate action, IDictionary<string, object>[] metas, MulticastDelegate resultHander) => CallAction(action, "POST", metas, resultHander);
        #endregion

        #region Put
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

//result {'id': '23','body': {'age': 40}}
")]
        public void Put(MulticastDelegate action) => Put(action, null, null);

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

//result {'id': '23','body': {'age': 40}}
")]
        [KDefineType(Params = new[] { typeof(object), typeof(RootMeta[]) })]
        public void Put(MulticastDelegate action, IDictionary<string, object>[] metas) => Put(action, metas, null);

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
},function (code, data) {
    var success = code < 400

    return {
        success: success,
        data: success ? data : null,
        error: success ? null : data,
        code: code
    }
})

//result { 'success': true, 'data': {'id': 23,'body': {'age': 40}}, 'error': null, 'code': 200 }
")]
        [KDefineType(Params = new[] { typeof(object), typeof(object) })]
        public void Put(MulticastDelegate action, MulticastDelegate resultHander) => Put(action, null, resultHander);

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
],function (code, data) {
    var success = code < 400

    return {
        success: success,
        data: success ? data : null,
        error: success ? null : data,
        code: code
    }
})

//result { 'success': true, 'data': {'id': 23,'body': {'age': 40}}, 'error': null, 'code': 200 }
")]
        [KDefineType(Params = new[] { typeof(object), typeof(RootMeta[]) })]
        public void Put(MulticastDelegate action, IDictionary<string, object>[] metas, MulticastDelegate resultHander) => CallAction(action, "PUT", metas, resultHander);
        #endregion

        #region Delete
        [Description(@"
//DELETE /test?id=23
k.api.delete(function (id) {
    return id;
})

//result '23'
")]
        public void Delete(MulticastDelegate action) => Delete(action, null, null);

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
        public void Delete(MulticastDelegate action, IDictionary<string, object>[] metas) => Delete(action, metas, null);

        [Description(@"
//DELETE /test?id=23
k.api.delete(function (id) {
    return id;
},[
    {name:'id',type:'Number'}
],function (code, data) {
    var success = code < 400

    return {
        success: success,
        data: success ? data : null,
        error: success ? null : data,
        code: code
    }
})

//result { 'success': true, 'data': 23, 'error': null, 'code': 200 }
")]
        [KDefineType(Params = new[] { typeof(object), typeof(object) })]
        public void Delete(MulticastDelegate action, MulticastDelegate resultHander) => Delete(action, null, resultHander);

        [Description(@"
//DELETE /test?id=23
k.api.delete(function (id) {
    return id;
},[
    {name:'id',type:'Number'}
],function (code, data) {
    var success = code < 400

    return {
        success: success,
        data: success ? data : null,
        error: success ? null : data,
        code: code
    }
})

//result { 'success': true, 'data': 23, 'error': null, 'code': 200 }
")]
        [KDefineType(Params = new[] { typeof(object), typeof(RootMeta[]) })]
        public void Delete(MulticastDelegate action, IDictionary<string, object>[] metas, MulticastDelegate resultHander) => CallAction(action, "DELETE", metas, resultHander);
        #endregion

        #region Http result
        [Description(@"
//GET /test?id=23
k.api.get(function (id) {
    return k.api.ok();
})

//result  httcode 200
")]
        public KApiResponse Ok() => new KApiResponse(200, null);

        [Description(@"
//GET /test?id=23
k.api.get(function (id) {
    return k.api.ok(id);
})

//result '23'
")]
        public KApiResponse Ok(object data) => new KApiResponse(200, Helpers.ToJson(data));

        [Description(@"
//GET /test?id=23
k.api.get(function (id) {
    return k.api.httpCode(400);
})

//result httpcode 400
")]
        public KApiResponse HttpCode(int code) => new KApiResponse(code);

        [Description(@"
//GET /test?id=23
k.api.get(function (id) {
    return k.api.httpCode(400,{error:'id is Required'});
})

//result httpcode:400 body {'error':'id is Required'}
")]
        public KApiResponse HttpCode(int code, object data) => new KApiResponse(code, Helpers.ToJson(data));

        [Description(@"
//GET /test?id=23
k.api.get(function (id) {
    return k.api.redirect('http://www.kooboo.cn');
})
")]
        public KApiResponse Redirect(string url) => new KApiResponse(302, url);

        [Description(@"
//GET /test?id=23
k.api.get(function (id) {
    return k.api.badRequest();
})
 d
//result  httcode 400
")]
        public KApiResponse BadRequest() => new KApiResponse(400, null);

        [Description(@"
//GET /test?id=23
k.api.get(function (id) {
    return k.api.unauthorized();
})
 d
//result  httcode 401
")]
        public KApiResponse Unauthorized() => new KApiResponse(401, null);

        [Description(@"
//GET /test?id=23
k.api.get(function (id) {
    return k.api.forbidden();
})
 d
//result  httcode 403
")]
        public KApiResponse Forbidden() => new KApiResponse(403, null);

        [Description(@"
//GET /test?id=23
k.api.get(function (id) {
    return k.api.notFound();
})
 d
//result  httcode 404
")]
        public KApiResponse NotFound() => new KApiResponse(404, null);
        #endregion

        private void CallAction(MulticastDelegate action, string method, IDictionary<string, object>[] metas, MulticastDelegate resultHander)
        {
            if (_renderContext.Request.Method != method) return;
            KApiResponse response;

            try
            {
                metas = Helpers.NamedMetas(metas);
                var func = action.Target as ScriptFunctionInstance;
                var parameters = GetParameters(func, metas);
                var result = func.Call(func, parameters).ToObject();

                if (result is KApiResponse)
                {
                    response = result as KApiResponse;
                }
                else
                {
                    var data = result == null ? null : Helpers.ToJson(result);
                    response = new KApiResponse(200, data);
                }
            }
            catch (KApiException e)
            {
                response = new KApiResponse(400, e.Message);
            }
            catch (Exception e)
            {
                response = new KApiResponse(500, e.ToString());
            }

            if (resultHander == null)
            {
                DefaultResultHandler(response);
            }
            else
            {
                CustomResultHandler(resultHander, response);
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

        void DefaultResultHandler(KApiResponse response)
        {
            _renderContext.Response.StatusCode = response.Code;
            _renderContext.SetItem(new CustomStatusCode() { IsCustomSet = true, Code = response.Code });
            if (response.Code > 300 && response.Code <= 302) _renderContext.Response.Redirect(302, response.Data);
            else if (response.Data != null) _renderContext.Response.AppendString(response.Data);
        }

        private void CustomResultHandler(MulticastDelegate resultHander, KApiResponse response)
        {
            var func = resultHander.Target as ScriptFunctionInstance;
            var result = func.Call(func, new[] { JsValue.FromObject(func.Engine, response.Code), JsValue.FromObject(func.Engine, response.Data) }).ToObject();
            if (result != null) _renderContext.Response.AppendString(Helpers.ToJson(result));
        }
    }
}
