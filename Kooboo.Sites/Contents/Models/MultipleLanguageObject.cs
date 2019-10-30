//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;

namespace Kooboo.Sites.Contents.Models
{
    public abstract class MultipleLanguageObject : CoreObject, IDynamic
    {
        private Dictionary<string, object> _values;

        public Dictionary<string, object> Values
        {
            get { return _values ?? (_values = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)); }
            set
            {
                _values = value;
            }
        }

        public Object GetValue(string fieldName)
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

        public Object GetValue(string fieldName, RenderContext context)
        {
            return GetValue(fieldName);
        }
    }
}