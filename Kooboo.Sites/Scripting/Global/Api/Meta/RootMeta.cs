using Kooboo.Data.Attributes;
using Kooboo.Data.Context;
using Kooboo.Sites.Scripting.Global.Api.Meta;
using System;
using System.Collections.Generic;

namespace KScript.Api
{
    public class RootMeta : MetaBase
    {
        [KIgnore]
        public RenderContext Context { get; }

        public RootMeta(Jint.Engine engine, IDictionary<string, object> metas, RenderContext context) : base(engine, metas, new string[0])
        {
            Context = context;
        }

        public virtual ValueFrom? From
        {
            get
            {
                if (Metas.TryGetValue("from", out var from))
                {
                    return (ValueFrom)Enum.Parse(typeof(ValueFrom), from.ToString());
                }
                else return null;
            }
        }

        protected override object GetFrom()
        {
            switch (From)
            {
                case ValueFrom.Header:
                    return Context.Request.Headers.Get(Name);
                case ValueFrom.Cookie:
                    Context.Request.Cookies.TryGetValue(Name, out var cookie);
                    return cookie;
                case ValueFrom.Form:
                    return Context.Request.Forms.Get(Name);
                case ValueFrom.Body:
                    return Context.Request.Body;
                case ValueFrom.QueryString:
                default:
                    return Context.Request.QueryString.Get(Name);
            }
        }

        protected override object ToObject(object value)
        {
            value = base.ToObject(value);

            if (value is string && From == ValueFrom.Body)
            {
                value = Helpers.FormToObject(Context);
            }

            return value;
        }
    }
}
