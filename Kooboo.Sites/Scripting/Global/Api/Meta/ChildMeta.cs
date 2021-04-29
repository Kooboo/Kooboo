using System.Collections.Generic;

namespace Kooboo.Sites.Scripting.Global.Api.Meta
{
    public class ChildMeta : MetaBase
    {
        readonly object _obj;
        public ChildMeta(Jint.Engine engine, IDictionary<string, object> metas, object obj, string[] parents) : base(engine,metas, parents)
        {
            _obj = obj;
        }

        protected override object GetFrom() => _obj;
    }
}
