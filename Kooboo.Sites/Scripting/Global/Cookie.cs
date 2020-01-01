//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Attributes;
using Kooboo.Data.Context;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo; 

namespace KScript
{ 
    public class Cookie : IDictionary<string, string>
    {
        private RenderContext context { get; set; }

        [Description("All keys in current cookie collection")]
        public ICollection<string> Keys => context.Request.Cookies.Keys;
        [Description("All values in current cookie collection")]
        public ICollection<string> Values => context.Request.Cookies.Values;

        [Kooboo.Attributes.SummaryIgnore]
        [KIgnore]
        public int Count => context.Request.Cookies.Count();

        public int length => this.Count;

        [Kooboo.Attributes.SummaryIgnore]
        [KIgnore]
        public bool IsReadOnly => true;

        [KIgnore]
        public string this[string key] { get => this.get(key); set => this.set(key, value); }

        public Cookie(RenderContext context)
        {
            this.context = context;
        }

        [Description("set a cookie expire in days\r\n k.cookie.set(\"cookiename\", \"cookievalue\", 30);")]
        public void set(string name, string value, int days)
        {
            this.context.Response.AppendCookie(name, value, days);
        }

        [Description("set a cookie with an expiration time in minutes\r\nk.cookie.setByMinutes(\"cookiename\", \"value\", 240);")]
        public void setByMinutes(string name, string value, int mins)
        {
            this.context.Response.AppendCookie(name, value, DateTime.Now.AddMinutes(mins));
        }

        [Description("set a cookie that default expires in 1 day. \r\nk.cookie.set(\"cookiename\", \"cookie value\")")]
        public void set(string name, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                this.context.Response.DeleteCookie(name);
            }
            else
            {
                this.context.Response.AppendCookie(name, value, 1);
            }
        }

        [Description(@"Get the cookie value by name.
var cookievalue =  k.cookie.get(""cookiename"");
var cookievalue2 =  k.cookie.cookiename;")]
        public string get(string Name)
        {
            if (this.context.Request.Cookies.ContainsKey(Name))
            {
                return this.context.Request.Cookies[Name];
            }
            return null;
        }

        [Description("check whether cookie has the key or not\r\nif (k.cookie.containsKey(\"key\"))\r\n{\r\n//has value\r\n}")]
        public bool ContainsKey(string key)
        {
            return this.context.Request.Cookies.ContainsKey(key);
        }

        [Kooboo.Attributes.SummaryIgnore]
        [KIgnore]
        public void Add(string key, string value)
        {
            this.set(key, value);
        }

        [Description("Remove cookie by key \r\nk.cookie.remove(\"key\");")]
        public bool Remove(string key)
        {
            this.context.Response.DeleteCookie(key);
            return true;
        }

        [Kooboo.Attributes.SummaryIgnore]
        [KIgnore]
        public bool TryGetValue(string key, out string value)
        {
            value = this.get(key);
            return true;
        }

        [Kooboo.Attributes.SummaryIgnore]
        [KIgnore]
        public void Add(KeyValuePair<string, string> item)
        {
            throw new NotImplementedException();
        }

        [Description("delete all cookies")]
        public void Clear()
        {
            foreach (var item in this.Keys)
            {
                this.context.Response.DeleteCookie(item);
            }
        }

        [Kooboo.Attributes.SummaryIgnore]
        [KIgnore]
        public bool Contains(KeyValuePair<string, string> item)
        {
            throw new NotImplementedException();
        }

        [Kooboo.Attributes.SummaryIgnore]
        [KIgnore]
        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        [Kooboo.Attributes.SummaryIgnore]
        [KIgnore]
        public bool Remove(KeyValuePair<string, string> item)
        {
            throw new NotImplementedException();
        }

        [KIgnore]
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return this.context.Request.Cookies.GetEnumerator();
        }

        [KIgnore]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.context.Request.Cookies.GetEnumerator();
        }
    }
}
