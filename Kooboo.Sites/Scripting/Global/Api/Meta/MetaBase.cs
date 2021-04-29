using Jint.Native;
using Jint.Native.Function;
using Jint.Native.Json;
using Kooboo.Data.Attributes;
using Kooboo.Lib.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Kooboo.Sites.Scripting.Global.Api.Meta
{
    public abstract class MetaBase
    {
        readonly string[] _parents;
        readonly Jint.Engine _engine;

        protected IDictionary<string, object> Metas { get; }

        public MetaBase(Jint.Engine engine, IDictionary<string, object> metas, string[] parents)
        {
            _engine = engine;
            Metas = metas;
            _parents = parents.Union(new string[] { Name }).ToArray();
        }

        public string Name
        {
            get
            {
                Metas.TryGetValue("name", out var name);
                var value = name as string;
                return value;
            }
        }

        [Description(@"
//GET /test?id=23
k.api.get(function (id) {
    return id;
}, [
    {
        name: 'id',
        required: true
    }
])
")]
        public bool? Required
        {
            get
            {
                if (Metas.TryGetValue("required", out var required))
                {
                    return true.Equals(required);
                }
                else return null;
            }
        }

        [Description(@"
//GET /test?id=23
k.api.get(function (id) {
    return id;
}, [
    {
        name: 'id',
        type: 'Number'
    }
])
")]
        public Types? Type
        {
            get
            {
                if (Metas.TryGetValue("type", out var type))
                {
                    return (Types)Enum.Parse(typeof(Types), type.ToString());
                }
                else return null;
            }
        }

        [Description(@"
//GET /test?id=23
k.api.get(function (id) {
    return id;
}, [
    {
        name: 'id',
        type: 'Number', // if type == 'String' match string length
        min: 30
    }
])
")]
        public double? Min
        {
            get
            {
                Metas.TryGetValue("min", out var min);
                return min as double?;
            }
        }

        [Description(@"
//GET /test?id=23
k.api.get(function (id) {
    return id;
}, [
    {
        name: 'id',
        type: 'Number', // if type == 'String' match string length
        max: 30
    }
])
")]
        public double? Max
        {
            get
            {
                Metas.TryGetValue("max", out var max);
                return max as double?;
            }
        }

        [Description(@"
//GET /test?id=bee
k.api.get(function (id) {
    return id;
}, [
    {
        name: 'id',
        pattern: 'be+'
    }
])
")]
        public string Pattern
        {
            get
            {
                Metas.TryGetValue("pattern", out var pattern);
                return pattern as string;
            }
        }

        [Description(@"
//GET /test?id=bee
k.api.get(function (id) {
    return id;
}, [
    {
        name: 'id',
        validator: function (value) {
            return value == 'bee'
        }
    }
])
")]
        public MulticastDelegate Validator
        {
            get
            {
                Metas.TryGetValue("validator", out var action);
                return action as MulticastDelegate;
            }
        }

        [Description(@"
//POST /test
// {
//     'age':23
// }

k.api.post(function (body) {
    return body;
}, [
    {
        name: 'body',
        from: 'Body',
        children: [
            {
                name: 'age',
                min: 10
            }
        ]
    }
])
")]
        public ChildMeta[] Children { get; }

        [KIgnore]
        IDictionary<string, object>[] _Children
        {
            get
            {
                object children = null;
                Metas?.TryGetValue("children", out children);
                if (children == null) return null;
                return (children as object[])?.Select(s => s as IDictionary<string, object>)?.ToArray();
            }
        }

        [KIgnore]
        public object Value
        {
            get
            {
                object value = GetFrom();

                if (value == null)
                {
                    if (Required.HasValue && Required.Value) throw new RequiredException(_parents);
                }
                else
                {
                    value = TypeMatch(value);
                }

                CustomValidate(value);
                return value;
            }
        }

        private object TypeMatch(object value)
        {
            var type = Type;

            if (type == Types.Boolean) value = GetBoolean(value);
            else if (type == Types.Number || value is double) value = GetNumber(value);
            else if (type == Types.Object || value is IDictionary<string, object>) value = GetObject(value);
            else if (type == Types.Array || (value is IEnumerable && !(value is string))) value = GetArray(value);
            else if (type == Types.String || value is string) value = GetString(value);

            return value;
        }

        private object GetArray(object value)
        {
            object obj = ToObject(value);
            if (!(obj is IEnumerable)) throw new TypeException(_parents, Types.Array);
            var arr = obj as IEnumerable;
            var metas = _Children;
            var result = new List<object>();
            var i = 0;

            foreach (var item in arr)
            {
                var itemMetas = item is IDictionary<string, object> ? Metas : metas.FirstOrDefault();
                itemMetas["name"] = $"[{i}]";
                var itemResult = new ChildMeta(_engine, itemMetas, item, _parents).Value;
                result.Add(itemResult);
                i++;
            }

            value = result.ToArray();
            return value;
        }

        private object GetObject(object value)
        {
            object obj = ToObject(value);
            if (!(obj is IDictionary<string, object>)) throw new TypeException(_parents, Types.Object);
            var dic = obj as IDictionary<string, object>;
            var metas = Helpers.NamedMetas(_Children);
            if (metas != null) Helpers.CheckRequired(dic.Keys.ToArray(), metas);
            var result = new Dictionary<string, object>();

            foreach (var item in dic)
            {
                var meta = metas?.FirstOrDefault(f => item.Key.Equals(f["name"]));
                result[item.Key] = meta == null ? item.Value : new ChildMeta(_engine, meta, item.Value, _parents).Value;
            }

            value = result;
            return value;
        }

        private void CustomValidate(object value)
        {
            var validator = Validator;

            if (validator != null)
            {
                var func = validator.Target as ScriptFunctionInstance;

                try
                {
                    var validValue = value is JsValue ? value : JsValue.FromObject(func.Engine, value);
                    var @params = new[] { JsValue.FromObject(func.Engine, validValue) };
                    var result = func.Call(func, @params);
                    if (!true.Equals(result.ToObject())) throw new InvalidException(_parents);
                }
                catch (Exception)
                {
                    throw new InvalidException(_parents);
                }
            }
        }

        private object GetString(object value)
        {
            if (!(value is string)) throw new TypeException(_parents, Types.String);

            var min = Min;
            var max = Max;
            var pattern = Pattern;
            var result = value as string;
            int number = result.Length;

            if (min != null && number < min) throw new MinLengthException(_parents, min);
            if (max != null && number > max) throw new MaxLengthException(_parents, max);

            if (pattern != null && !System.Text.RegularExpressions.Regex.IsMatch(result, pattern))
            {
                throw new NotMatchException(_parents);
            }

            return result;
        }

        private object GetNumber(object value)
        {
            var min = Min;
            var max = Max;

            if (value is string && double.TryParse(value as string, out var result))
            {
                value = result;
            }
            else if (!(value is double))
            {
                throw new TypeException(_parents, Types.Number);
            }

            if (min != null && (double)value < min) throw new MinException(_parents, min);
            if (max != null && (double)value > max) throw new MaxException(_parents, max);

            return value;
        }

        private object GetBoolean(object value)
        {
            if (value is string && bool.TryParse(value as string, out var result))
            {
                value = result;
            }
            else if (!(value is bool))
            {
                throw new TypeException(_parents, Types.Boolean);
            }

            return value;
        }

        protected abstract object GetFrom();

        protected virtual object ToObject(object value)
        {
            if ((value is string) && JsonHelper.IsJson(value as string))
            {
                value = new JsonParser(_engine).Parse(value as string).ToObject();
            }

            return value;
        }
    }
}
