//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Context
{ 
    public class HttpStringCollection : IEnumerable<KeyValuePair<string, string[]>>
    {
        private readonly Dictionary<string, string[]> _dic;

        public HttpStringCollection() 
        {
            this._dic = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase); 
        }

        public HttpStringCollection(IEnumerable<KeyValuePair<string, string[]>> values)
        {
            _dic = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
            if (values != null)
            {
                foreach (var each in values)
                {
                    Set(each.Key, each.Value);
                }
            }
        }
         
        public bool Contains(string key)
        {
            return _dic.ContainsKey(key);
        }

        public int Count
        {
            get { return _dic.Count; }
        }

        public ICollection<string> Keys
        {
            get { return _dic.Keys; }
        }

        public string this[string key]
        {
            get { return Get(key); }
            set { Set(key, value); }
        }

        public string Get(string key)
        {
            var values = GetValues(key);
            if (values.Count == 0)
            {
                return null;
            }

            return String.Join(",", values);
        }

        public IList<string> GetValues(string key)
        {
            string[] values;
            if (_dic.TryGetValue(key, out values))
            {
                return values;
            }

            return new List<string>();
        }

        public bool Remove(string key)
        {
            return _dic.Remove(key);
        }

        public void Clear()
        {
            _dic.Clear();
        }

        public void Add(string key, string value)
        {
            Set(key, value);
        }

        public void AddRange(IEnumerable<KeyValuePair<string, string>> values)
        {
            foreach (var kv in values)
            {
                Set(kv.Key, kv.Value);
            }
        }

        public void AddRange(IEnumerable<KeyValuePair<string, object>> values)
        {
            foreach (var kv in values)
            {
                if (kv.Value != null)
                {
                    Set(kv.Key, kv.Value.ToString());
                }
            }
        }

        public void AddRange(IEnumerable<KeyValuePair<string, IEnumerable<string>>> values)
        {
            foreach (var kv in values)
            {
                Set(kv.Key, kv.Value.ToArray());
            }
        }

        public void Set(string key, string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                Remove(key);
            }
            else
            {
                Set(key, new[] { value });
            }
        }

        public void Set(string key, params string[] values)
        {
            if (values == null)
            {
                Remove(key);
            }
            else
            {
                if (_dic.ContainsKey(key))
                {
                    _dic[key] = values;
                }
                else
                {
                    _dic.Add(key, values);
                }
            }
        }

        public Dictionary<string, string> ToJoinedDictionary()
        {
            return _dic.ToDictionary(x => x.Key, x => Get(x.Key));
        }

        public IEnumerator<KeyValuePair<string, string[]>> GetEnumerator()
        {
            return _dic.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
