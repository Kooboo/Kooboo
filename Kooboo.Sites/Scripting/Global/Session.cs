//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Attributes;
using Kooboo.Data.Context;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace KScript
{
    public class Session : IDictionary<string, object>
    {
        private object _locker = new object(); 

        private RenderContext context { get; set; }

        [Description("All keys in current session storage")]
        public ICollection<string> Keys => SessionManager.Keys(getcookie());

        [Description("All values in current session storage")]
        public ICollection<object> Values => SessionManager.Values(getcookie());

        [Kooboo.Attributes.SummaryIgnore]
        [KIgnore]
        public int Count => SessionManager.Keys(getcookie()).Count();


        public int length { get { return this.Count();  } }

        [Kooboo.Attributes.SummaryIgnore]
        [KIgnore]
        public bool IsReadOnly => false;

        [Kooboo.Attributes.SummaryIgnore]
        [KIgnore]
        public object this[string key]
        {
            get
            {
                return SessionManager.Get(getcookie(), key);
            }
            set
            {
                SessionManager.Set(getcookie(), key, value);
            }
        }

        public Session(RenderContext context)
        {
            this.context = context;
        } 

        private Guid _cookie;

        private Guid getcookie()
        {
            if (_cookie != default(Guid))
            {
                return _cookie;
            }

            lock (_locker)
            {

                if (_cookie != default(Guid))
                {
                    return _cookie;
                }


                if (this.context.Request.Cookies.ContainsKey("_kb_session_id"))
                {
                    var value = this.context.Request.Cookies["_kb_session_id"];
                    if (!string.IsNullOrEmpty(value))
                    {
                        Guid.TryParse(value, out _cookie);
                    }
                }

                if (_cookie == default(Guid))
                {
                    _cookie = Guid.NewGuid();
                }

                this.context.Response.AddCookie(new System.Net.Cookie() { Name = "_kb_session_id", Value = _cookie.ToString(), Expires = DateTime.Now.AddDays(7) });

                return _cookie;
            }
        }

        [Description(@"Set a Key Value in current session.
  k.session.set(""key"", ""value""); 
k.session.key=""value"";")]
        public void set(string key, object value)
        {
            var id = getcookie();
            SessionManager.Set(id, key, value);
        }


        [Description(@"get a session value
var value = k.session.get(""key""); 
var value2 = k.session.key;")]
        public object get(string key)
        {
            var id = getcookie();
            return SessionManager.Get(id, key);
        }

        [Kooboo.Attributes.SummaryIgnore]
        [KIgnore]
        public bool ContainsKey(string key)
        {
            return Contains(key);
        }

        [Description(@"Check whether session has the key or not.
if (k.session.contains(""key"")){//}")]
        public bool Contains(string key)
        {
            var value = SessionManager.Get(getcookie(), key);
            return value != null;
        }


        [Kooboo.Attributes.SummaryIgnore]
        [KIgnore]
        public void Add(string key, object value)
        {
            set(key, value);
        }

        [Description(@"Remove item from session
k.session.remove(""key"");")]
        public bool Remove(string key)
        {
            SessionManager.Remove(getcookie(), key);
            return true;
        }

        [Kooboo.Attributes.SummaryIgnore]
        [KIgnore]
        public bool TryGetValue(string key, out object value)
        {
            throw new NotImplementedException();
        }

        [Kooboo.Attributes.SummaryIgnore]
        [KIgnore]
        public void Add(KeyValuePair<string, object> item)
        {
            throw new NotImplementedException();
        }


        public void Clear()
        {
            SessionManager.Clear(getcookie());
        }

        [Kooboo.Attributes.SummaryIgnore]
        [KIgnore]
        public bool Contains(KeyValuePair<string, object> item)
        {
            throw new NotImplementedException();
        }

        [Kooboo.Attributes.SummaryIgnore]
        [KIgnore]
        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        [Kooboo.Attributes.SummaryIgnore]
        [KIgnore]
        public bool Remove(KeyValuePair<string, object> item)
        {
            throw new NotImplementedException();
        }

        [Kooboo.Attributes.SummaryIgnore]
        [KIgnore]
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return SessionManager.All(getcookie()).GetEnumerator();
        }
        [Kooboo.Attributes.SummaryIgnore]
        [KIgnore]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return SessionManager.All(getcookie()).GetEnumerator();
        }
    }

    public static class SessionManager
    {
        //TODO: this session needs to check for removal every 30 minutes. 
        private static object _lock = new object();

        private static Dictionary<Guid, SessionData> data = new Dictionary<Guid, SessionData>();

        public static object Get(Guid cookieid, string key)
        {
            if (data.ContainsKey(cookieid))
            {
                var values = data[cookieid];
                values.LastModified = DateTime.Now;

                if (values.Values.ContainsKey(key))
                {
                    return values.Values[key];
                }
                return null;
            }
            return null;
        }


        public static void Remove(Guid cookieid, string key)
        {
            if (data.ContainsKey(cookieid))
            {
                var values = data[cookieid];
                values.LastModified = DateTime.Now;

                values.Values.Remove(key);
            }
        }

        public static List<object> Values(Guid cookieId)
        {
            if (data.ContainsKey(cookieId))
            {
                var datavalue = data[cookieId];

                datavalue.LastModified = DateTime.Now;

                var dict = datavalue.Values;

                return dict.Values.ToList();
            }
            return null;
        }

        public static List<string> Keys(Guid cookieId)
        {
            if (data.ContainsKey(cookieId))
            {
                var datavalue = data[cookieId];

                datavalue.LastModified = DateTime.Now;

                var dict = datavalue.Values;

                return dict.Keys.ToList();
            }
            return null;
        }

        public static Dictionary<string, object> All(Guid cookieId)
        {
            if (data.ContainsKey(cookieId))
            {
                var values = data[cookieId];
                values.LastModified = DateTime.Now;

                return values.Values;
            }
            return null;
        }

        public static void Clear(Guid cookieId)
        {
            if (data.ContainsKey(cookieId))
            {
                data.Remove(cookieId);
            }
        }

        public static void Set(Guid cookie, string key, object value)
        {
            CheckClean();

            if (!data.ContainsKey(cookie))
            {
                lock (_lock)
                {
                    if (!data.ContainsKey(cookie))
                    {
                        data[cookie] = new SessionData();
                    }
                }
            }

            var existing = data[cookie];

            existing.LastModified = DateTime.Now;
  

            if (existing.Values.Count() <= 50)
            {
                existing.Values[key] = value;
            }
        }

        internal static void CleanUp()
        {
            lock (_cleanlock)
            {
                HashSet<Guid> removeid = new HashSet<Guid>();
                foreach (var item in data)
                {
                    if (item.Value.LastModified < DateTime.Now.AddMinutes(-30))
                    {
                        removeid.Add(item.Key);
                    }
                }
                foreach (var item in removeid)
                {
                    data.Remove(item);
                }
            }
        }

        private static DateTime lastcheck { get; set; } = DateTime.Now;
        private static object _cleanlock = new object();
        internal static void CheckClean()
        {
            if (lastcheck < DateTime.Now.AddMinutes(-60))
            {
                lastcheck = DateTime.Now;
                Task.Factory.StartNew(CleanUp);
            }
        }
    }

    public class SessionData
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
            set { _values = value; }
        }

        public DateTime LastModified { get; set; }

    }
}
