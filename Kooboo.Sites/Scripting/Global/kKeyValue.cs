using Kooboo.Data;
using Kooboo.Data.Context;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Kooboo.Sites.Scripting.Global
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

        private IndexedDB.Dynamic.Table _table;

        private IndexedDB.Dynamic.Table table
        {
            get
            {
                if (_table == null)
                {
                    var db =  DB.GetKDatabase(this.context.WebSite);
                    IndexedDB.Dynamic.Setting setting = new IndexedDB.Dynamic.Setting();
                    setting.AppendColumn("key", typeof(string), 256);
                    setting.AppendColumn("value", typeof(string), MaxValueLen);
                    _table = db.GetOrCreateTable("_sys_keyvalues", setting);
                }
                return _table;
            }
        }


        [Attributes.SummaryIgnore]
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

        [Attributes.SummaryIgnore]
        public bool IsReadOnly
        {
            get { return true; }
        }

        [Attributes.SummaryIgnore]
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

        public void set(string key, string value)
        {
            if (value != null)
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(value);
                if (bytes.Length > this.MaxValueLen)
                {
                    throw new Exception(Data.Language.Hardcoded.GetValue("Maximun value length reached", this.context));
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

        [Attributes.SummaryIgnore]
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


        [Attributes.SummaryIgnore]
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

        [Attributes.SummaryIgnore]
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

        [Attributes.SummaryIgnore]
        public void Add(KeyValuePair<string, string> item)
        {
            set(item.Key, item.Value);
        }


        [Attributes.SummaryIgnore]
        public void Clear()
        {
            this.table.DelSelf();
        }

        [Attributes.SummaryIgnore]
        public bool Contains(KeyValuePair<string, string> item)
        {
            var value = this.get(item.Key);
            return value != null && value == item.Value;
        }

        [Attributes.SummaryIgnore]
        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        [Attributes.SummaryIgnore]
        public bool Remove(KeyValuePair<string, string> item)
        {
            return Remove(item.Key);
        }

        [Attributes.SummaryIgnore]
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

        [Attributes.SummaryIgnore]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
