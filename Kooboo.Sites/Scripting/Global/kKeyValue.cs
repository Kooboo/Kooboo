//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data;
using Kooboo.Data.Attributes;
using Kooboo.Data.Context;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace KScript
{
    public class kKeyValue : System.Collections.Generic.IDictionary<string, string>
    {
        private RenderContext context { get; set; }

        public ICollection<string> Keys
        {
            get
            {
                var all = this.table.All();
                List<string> keys = new List<string>();
                foreach (var item in all)
                {
                    var value = item["key"];
                    keys.Add(value.ToString());
                }
                return keys;
            }
        }

        public ICollection<string> Values
        {
            get
            {
                List<string> result = new List<string>();

                var all = this.table.All();
                List<string> keys = new List<string>();
                foreach (var item in all)
                {
                    var value = item["value"];
                    if (value != null)
                    {
                        result.Add(value.ToString());
                    }
                    else
                    {
                        result.Add(null);
                    }
                }

                return result;
            }
        }

        private int MaxValueLen { get; set; } = 4096;

        private Kooboo.IndexedDB.Dynamic.Table _table;

        private Kooboo.IndexedDB.Dynamic.Table table
        {
            get
            {
                if (_table == null)
                {
                    _table = Kooboo.Data.DB.GetOrCreateTable(this.context.WebSite, "_sys_keyvalues");
                }
                return _table;
            }
        }


        [Kooboo.Attributes.SummaryIgnore]
        [KIgnore]
        public int Count
        {
            get
            {
                var index = this.table.Indexs.Find(o => o.IsSystem);
                return index.Count(true);
            }
        }


        public int length
        {
            get { return this.Count; }
        }

        [Kooboo.Attributes.SummaryIgnore]
        [KIgnore]
        public bool IsReadOnly
        {
            get { return true; }
        }

        [Kooboo.Attributes.SummaryIgnore]
        [KIgnore]
        public string this[string key]
        {
            get
            {
                return this.get(key);
            }
            set
            {
                set(key, value);
            }
        }

        public kKeyValue(RenderContext context)
        {
            this.context = context;
        }

        [Description(@"k.keyValue.set(""key"", ""value"");
    var value = k.keyValue.get(""key"");
    // or
    var value = k.keyValue.key;")]
        public void set(string key, string value)
        {
            if (value != null)
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(value);
                if (bytes.Length > this.MaxValueLen)
                {
                    throw new Exception(Kooboo.Data.Language.Hardcoded.GetValue("Maximun value length reached", this.context));
                }
            }

            if (!string.IsNullOrEmpty(key))
            {
                var hash = Kooboo.IndexedDB.Helper.KeyHelper.ComputeGuid(key);

                var item = this.table.Get(hash);

                if (item == null)
                {
                    IDictionary dict = new Dictionary<string, object>();
                    dict.Add("_id", hash);
                    dict.Add("key", key);
                    dict.Add("value", value);
                    this.table.Add(dict);
                }
                else
                {
                    item["value"] = value;
                    item["key"] = key;
                    this.table.Update(hash, item);
                }
            }
        }

        [Description(@"k.keyValue.set(""key"", ""value"");
    var value = k.keyValue.get(""key"");
    // or
    var value = k.keyValue.key;")]
        public string get(string key)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                var hash = Kooboo.IndexedDB.Helper.KeyHelper.ComputeGuid(key);
                var value = this.table.Get(hash);
                if (value != null && value.ContainsKey("value"))
                {
                    var result = value["value"];
                    if (result != null)
                    {
                        return result.ToString();
                    }
                }
            }
            return null;
        }

        [Kooboo.Attributes.SummaryIgnore]
        [KIgnore]
        public bool ContainsKey(string key)
        {
            var hash = Kooboo.IndexedDB.Helper.KeyHelper.ComputeGuid(key);
            var value = this.table.Get(hash);
            return value != null;
        }

        public bool Contains(string key)
        {
            var hash = Kooboo.IndexedDB.Helper.KeyHelper.ComputeGuid(key);
            var value = this.table.Get(hash);
            return value != null;
        }


        [Kooboo.Attributes.SummaryIgnore]
        [KIgnore]
        public void Add(string key, string value)
        {
            set(key, value);
        }

        public bool Remove(string key)
        {
            var hash = Kooboo.IndexedDB.Helper.KeyHelper.ComputeGuid(key);
            var value = this.table.Get(hash);
            if (value != null)
            {
                this.table.Delete(hash);
                return true;
            }
            return false;
        }

        [Kooboo.Attributes.SummaryIgnore]
        [KIgnore]
        public bool TryGetValue(string key, out string value)
        {
            var dbvalue = this.get(key);
            if (dbvalue != null)
            {
                value = dbvalue;
                return true;
            }
            value = null;
            return false;
        }

        [Kooboo.Attributes.SummaryIgnore]
        [KIgnore]
        public void Add(KeyValuePair<string, string> item)
        {
            set(item.Key, item.Value);
        }

        [Kooboo.Attributes.SummaryIgnore]
        [KIgnore]
        public void Clear()
        {
            this.table.DelSelf();
        }

        [Kooboo.Attributes.SummaryIgnore]
        [KIgnore]
        public bool Contains(KeyValuePair<string, string> item)
        {
            var value = this.get(item.Key);
            return value != null && value == item.Value;
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
            return Remove(item.Key);
        }

        [Kooboo.Attributes.SummaryIgnore]
        [KIgnore]
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            Dictionary<string, string> allvalues = new Dictionary<string, string>();

            foreach (var item in this.Keys)
            {
                var value = this.get(item);
                allvalues.Add(item, value);
            }
            return allvalues.GetEnumerator();
        }

        [Kooboo.Attributes.SummaryIgnore]
        [KIgnore]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
