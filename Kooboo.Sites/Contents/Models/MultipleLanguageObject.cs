using System.Collections.Generic;
using Kooboo.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Data.Interface;
using Kooboo.Data.Context;
using System;

namespace Kooboo.Sites.Contents.Models
{
    public abstract class MultipleLanguageObject : CoreObject, IDynamic
    {         

        private Dictionary<string, object> _values;

        public Dictionary<string, object> Values
        {
            get
            {
                if (_values == null)
                {
                    _values = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase); 
                }
                return _values; 
            }
            set
            {
                _values = value;
            }
        }

        public Object  GetValue(string fieldName)
        {  
            if (Values.ContainsKey(fieldName))
            {
                return Values[fieldName]; 
            }

            if (Values.ContainsKey(""))
            {
                return Values[""]; 
            }

            string lower = fieldName.ToLower(); 
            foreach (var item in Values)
            {
                if (item.Key.ToLower().StartsWith(lower))
                {
                    return item.Value; 
                }
            } 
            
            return string.Empty;
        }

        public virtual void SetValue(string culture, string value)
        {
            this.Values[culture] = value;
        }

        public virtual void SetValue(string field, object value)
        {
            this.Values[field] = value;
        }

        public override int GetHashCode()
        {
            string unique = Name;
            foreach (var item in Values)
            {
                unique += item.Key + item.Value;
            }
            return Lib.Security.Hash.ComputeIntCaseSensitive(unique);
        }

        public Object GetValue(string FieldName, RenderContext Context)
        {
            return GetValue(FieldName); 
        }
    }
}
