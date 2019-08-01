//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Scripting.Global
{
  
    public class Cookie: IDictionary<string, string>
    {
        private RenderContext context { get; set; }

        public ICollection<string> Keys => context.Request.Cookies.Keys;

        public ICollection<string> Values => context.Request.Cookies.Values;

        [Attributes.SummaryIgnore]
        public int Count => context.Request.Cookies.Count();

        public int length => this.Count; 

        [Attributes.SummaryIgnore]
        public bool IsReadOnly => true;

        public string this[string key] { get => this.get(key); set => this.set(key, value); }

        public Cookie(RenderContext context)
        {
            this.context = context;
        }
         
        public void set(string name, string value, int days)
        {
            this.context.Response.AppendCookie(name, value, days);
        }

        public void setByMinutes(string name, string value, int mins)
        {
            this.context.Response.AppendCookie(name, value, DateTime.Now.AddMinutes(mins));
        }
         
        public void set(string name, string value)
        {
            if(string.IsNullOrWhiteSpace(value))
            {
                this.context.Response.DeleteCookie(name); 
            }
            else
            {
                this.context.Response.AppendCookie(name, value, 1);
            }
            
        }

        public string get(string Name)
        {
            if (this.context.Request.Cookies.ContainsKey(Name))
            {
                return this.context.Request.Cookies[Name]; 
            }
            return null; 
        }

        public bool ContainsKey(string key)
        {
            return this.context.Request.Cookies.ContainsKey(key); 
        }

        [Attributes.SummaryIgnore]
        public void Add(string key, string value)
        {
            this.set(key, value); 
        }
         
        public bool Remove(string key)
        {
            this.context.Response.DeleteCookie(key);
            return true; 
        }

        [Attributes.SummaryIgnore]
        public bool TryGetValue(string key, out string value)
        {
            value = this.get(key);
            return true; 
        }

        [Attributes.SummaryIgnore]
        public void Add(KeyValuePair<string, string> item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            foreach (var item in this.Keys)
            {
                this.context.Response.DeleteCookie(item); 
            }
        }

        [Attributes.SummaryIgnore]
        public bool Contains(KeyValuePair<string, string> item)
        {
            throw new NotImplementedException();
        }

        [Attributes.SummaryIgnore]
        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        [Attributes.SummaryIgnore]
        public bool Remove(KeyValuePair<string, string> item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
           return this.context.Request.Cookies.GetEnumerator(); 
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.context.Request.Cookies.GetEnumerator(); 
        }
    }
}
