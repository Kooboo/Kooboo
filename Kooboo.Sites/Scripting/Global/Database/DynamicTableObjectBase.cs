using Kooboo.Data.Context;
using Kooboo.Sites.DataTraceAndModify;
using KScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kooboo.Sites.Scripting.Global.Database
{
    public abstract class DynamicTableObjectBase : IDynamicTableObject,
        ITraceability
    {
        public IDictionary<string, object> obj { get; set; }

        public Dictionary<string, object> Values
        {
            get
            {
                return this.obj.ToDictionary(o => o.Key, o => o.Value);
            }
        }

        public abstract string Source { get; }

        internal abstract object GetValueFromDict(string key);

        public object this[string key]
        {
            get
            {
                var value = GetValueFromDict(key);

                //[Kooboo.Lib\Jint\Native\Date\DateConstructor.cs FromDateTime] method will convent unspecified datetime to utc time,
                //but the data is saved using utc and then query out is unspecified 
                if (value is DateTime)
                {
                    var date = (DateTime)value;
                    if (date.Kind == DateTimeKind.Unspecified) value = date.ToLocalTime();
                }

                return value;
            }
            set
            {
                this.obj[key] = value;
            }
        }

        public object GetValue(string FieldName)
        {
            return GetValueFromDict(FieldName);
        }

        public object GetValue(string FieldName, RenderContext Context)
        {
            return GetValueFromDict(FieldName);
        }

        public void SetValue(string FieldName, object Value)
        {
            obj[FieldName] = Value;
        }

        public abstract IDictionary<string, string> GetTraceInfo();

    }
}
