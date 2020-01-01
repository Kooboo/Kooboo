//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Attributes;
using Kooboo.Data.Context;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KScript.Sites
{
    public class MultilingualObject : Kooboo.Sites.Contents.Models.MultipleLanguageObject, IDictionary<string, object>
    {
        public MultilingualObject()
        {
            // should not use. 
        }

        public RenderContext context { get; set; }

        public MultilingualObject(Kooboo.Sites.Contents.Models.MultipleLanguageObject siteobject, RenderContext context)
        {
            this.context = context;
            this.Values = siteobject.Values;
            this.Name = siteobject.Name;
        }

        public object this[string key]
        {

            get
            {

                if (this.Values.ContainsKey(key))
                {
                    return this.Values[key];
                }

                if (key.ToLower() == "value")
                {
                    var culture = this.context.Culture;
                    if (this.Values.ContainsKey(culture))
                    {
                        return this.Values[culture];
                    }
                    else  
                    {
                        return this.Values.First();
                    }
                }

                return null;

            }
            set
            {
                if (key.ToLower() == "value")
                {
                    var culture = this.context.Culture;
                    this.SetValue(culture, value);
                }
                else
                {
                    this.SetValue(key, value);
                }
            }
        }

        public ICollection<string> Keys => this.Values.Keys;

        public int Count => this.Values.Count;

        public bool IsReadOnly => false;

        ICollection<object> IDictionary<string, object>.Values => this.Values.Values;

        public void Add(string key, object value)
        {
            this.SetValue(key, value);
        }

        public void Add(KeyValuePair<string, object> item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            this.Values.Clear();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(string key)
        {
            if (this.Values.ContainsKey(key))
            {
                return true;
            }
            if (key.ToLower()== "value")
            {
                return true; 
            }
            return false; 
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(string key)
        {
            return this.Values.Remove(key);
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(string key, out object value)
        {
            if (key == null)
            {
                value = null;
                return false;
            }

            if (key.ToLower() == "value")
            {
                var culture = this.context.Culture;
                if (this.Values.ContainsKey(culture))
                {
                    value =  this.Values[culture];
                    return true; 
                }
                else
                {  
                    if (this.Values.Any())
                    {
                        value = this.Values.First();
                        return true;
                    }
                    else
                    {
                        value = null; 
                        return false; 
                    }
                   
                }
            }

            if (this.Values.ContainsKey(key))
            {
                value = this.Values[key];
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }


        [Kooboo.Attributes.SummaryIgnore]
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return this.Values.GetEnumerator();
        }

        [Kooboo.Attributes.SummaryIgnore]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }















    }
}
