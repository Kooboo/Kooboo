//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Kooboo.Data.Context
{
    public class RenderContext
    {
        private object _locker = new object();

        private HttpRequest _request;
        public HttpRequest Request
        {
            get
            {
                if (_request == null)
                {
                    _request = new HttpRequest();
                }
                return _request;
            }
            set { _request = value; }
        }

        private HttpResponse _response;

        public bool EnableTextGZip { get; set; } = true;

        public HttpResponse Response
        {
            get
            {
                if (_response == null)
                {
                    _response = new HttpResponse();
                }
                return _response;
            }
            set
            {
                _response = value;
            }
        }

        private DataContext _DataContext;
        public DataContext DataContext
        {
            get
            {
                if (_DataContext == null)
                {
                    _DataContext = new DataContext(this);
                    _DataContext.OnDataPush = AssignHeaderValue;
                }
                return _DataContext;
            }
            set
            {
                _DataContext = value;
                _DataContext.OnDataPush = AssignHeaderValue;
            }
        }

        // TODO: this should be init and append the object we would like to make available
        public Jint.Engine JsEngine
        {
            get; set;
        }

        private bool HasWebSiteCheck { get; set; } = false;
        private WebSite _website;
        public WebSite WebSite
        {
            get
            {
                if (_website == null && !HasWebSiteCheck)
                {
                    HasWebSiteCheck = true;
                    _website = WebServerContext.GetWebSite(this);
                }
                return _website;
            }
            set { _website = value; }
        }

        private bool HasUserCheck { get; set; } = false;

        private User _user;
        public User User
        {
            get
            {
                if (_user == null && !HasUserCheck)
                {
                    HasUserCheck = true;
                    _user = WebServerContext.GetUser(this.Request, this);
                }
                return _user;
            }
            set
            {
                _user = value;
                HasUserCheck = true;
            }
        }

        private string _culture = null;
        public string Culture
        {
            get
            {
                if (_culture == null)
                {
                    _culture = RequestManager.GetSetCulture(this.WebSite, this);
                    if (_culture == null)
                    {
                        if (this.WebSite != null)
                        {
                            _culture = this.WebSite.DefaultCulture;
                        }
                        else
                        {
                            _culture = AppSettings.CmsLang; // default
                        }
                    }
                }
                return _culture;
            }
            set { _culture = value; }

        }

        private Dictionary<string, string> _placeholdercontents;
        public Dictionary<string, string> PlaceholderContents
        {
            get
            {
                if (_placeholdercontents == null)
                {
                    _placeholdercontents = new Dictionary<string, string>();
                }
                return _placeholdercontents;
            }
            set
            {
                _placeholdercontents = value;
            }
        }

        public void AddPlaceHolderContent(string Key, string value)
        {
            if (_placeholdercontents == null || !this.PlaceholderContents.ContainsKey(Key))
            {
                this.PlaceholderContents.Add(Key, value);
            }
            else
            {
                var currentvalue = this.PlaceholderContents[Key];
                currentvalue += value;
                this.PlaceholderContents[Key] = currentvalue;
            }
        }

        private Dictionary<string, object> _items;

        [JsonIgnore]
        public Dictionary<string, object> Items
        {
            get
            {
                if (_items == null)
                {
                    _items = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                }
                return _items;
            }
            set { _items = value; }
        }

        public void SetItem<T>(T value, string KeyName = null)
        {
            if (string.IsNullOrEmpty(KeyName))
            {
                KeyName = typeof(T).Name;
            }
            Items[KeyName] = value;
        }

        public T GetItem<T>(string KeyName = null)
        {
            if (this._items == null)
            { return default(T); }
            if (string.IsNullOrEmpty(KeyName))
            {
                KeyName = typeof(T).Name;
            }
            if (this.Items.ContainsKey(KeyName))
            {
                return (T)Items[KeyName];
            }
            return default(T);
        }

        public T GetItem<T>(string keyname, Func<RenderContext, T> Setter)
        {
            if (!this.Items.ContainsKey(keyname))
            {
                lock (_locker)
                {
                    if (!this.Items.ContainsKey(keyname))
                    {
                        var obj = Setter(this);
                        this.Items[keyname] = obj;
                    }
                }
            }

            var result = this.Items[keyname];
            if (result == null)
            {
                return default(T);
            }
            else
            {
                return (T)this.Items[keyname];
            }
        }
        public bool HasItem<T>(string KeyName = null)
        {
            if (this._items == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(KeyName))
            {
                KeyName = typeof(T).Name;
            }
            return this.Items.ContainsKey(KeyName);
        }

        private List<HeaderBindings> _headerBinding;

        [JsonIgnore]
        public List<HeaderBindings> HeaderBindings
        {
            get
            {
                if (_headerBinding == null)
                {
                    _headerBinding = new List<HeaderBindings>();
                }
                return _headerBinding;
            }
            set
            {
                _headerBinding = value;
                if (_headerBinding != null)
                {
                    InitHeaderValue();
                }
            }
        }

        private void AssignHeaderValue(IDictionary data)
        {
            if (this._headerBinding != null)
            {
                foreach (var item in this.HeaderBindings)
                {
                    if (item.RequireBinding)
                    {
                        item.ValueQuery.TryAssignValue(data, this);
                    }
                }
            }
        }

        private void InitHeaderValue()
        {
            if (_headerBinding != null)
            {
                foreach (var item in this.HeaderBindings)
                {
                    if (item.RequireBinding)
                    {
                        item.ValueQuery.InitValue(this);
                    }
                }
            }
        }

        public bool IsSiteBinding { get; set; }

        public bool IsApp { get; set; }

        public string AppParentUrl { get; set; }
    }
}
