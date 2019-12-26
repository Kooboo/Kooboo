//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Scripting
{
    public class KScriptExtension : System.Collections.Generic.IDictionary<string, object>
    {

        private RenderContext context { get; set; }

        public KScriptExtension(RenderContext context)
        {
            this.context = context;
        }

        public object this[string key] { get { return ExtensionContainer.Get(key, context); } set { ExtensionContainer.Set(value); } }

        public ICollection<string> Keys { get { return ExtensionContainer.List.Keys; } }

        public ICollection<object> Values { get { throw new NotImplementedException(); } }


        public int Count => ExtensionContainer.List.Count();

        public bool IsReadOnly => false;

        public void Add(string key, object value)
        {
            throw new NotImplementedException();
        }

        [Attributes.SummaryIgnore]
        public void Add(KeyValuePair<string, object> item)
        {
            throw new NotImplementedException();
        }

        [Attributes.SummaryIgnore]
        public void Clear()
        {
            throw new NotImplementedException();
        }

        [Attributes.SummaryIgnore]
        public bool Contains(KeyValuePair<string, object> item)
        {
            throw new NotImplementedException();
        }

        [Attributes.SummaryIgnore]
        public bool ContainsKey(string key)
        {
            return ExtensionContainer.List.ContainsKey(key);
        }

        [Attributes.SummaryIgnore]
        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        [Attributes.SummaryIgnore]
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        [Attributes.SummaryIgnore]
        public bool Remove(string key)
        {
            return ExtensionContainer.List.Remove(key);
        }

        [Attributes.SummaryIgnore]
        public bool Remove(KeyValuePair<string, object> item)
        {
            throw new NotImplementedException();
        }

        [Attributes.SummaryIgnore]
        public bool TryGetValue(string key, out object value)
        {
            var instance = ExtensionContainer.Get(key, this.context);
            if (instance == null)
            {
                value = null;
                return false;
            }
            else
            {
                value = instance;
                return true;
            }
        }

        [Attributes.SummaryIgnore]
        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
