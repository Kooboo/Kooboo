//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Data.Context
{
    public class RenderContext
    {
        private HttpRequest _request;
        public HttpRequest Request
        {
            get => _request ?? (_request = new HttpRequest());
            set => _request = value;
        }

        private HttpResponse _response;

        public HttpResponse Response
        {
            get { return _response ?? (_response = new HttpResponse()); }
            set => _response = value;
        }

        private DataContext _dataContext;
        public DataContext DataContext
        {
            get => _dataContext ?? (_dataContext = new DataContext(this) {OnDataPush = AssignHeaderValue});
            set
            {
                _dataContext = value;
                _dataContext.OnDataPush = AssignHeaderValue;
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
            set => _website = value;
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
            get =>
                _culture ?? (_culture = RequestManager.GetSetCulture(this.WebSite, this) ??
                                        (this.WebSite != null
                                            ? this.WebSite.DefaultCulture
                                            : AppSettings.CmsLang));
            set => _culture = value;
        }

        private Dictionary<string, string> _placeholdercontents;
        public Dictionary<string, string> PlaceholderContents
        {
            get => _placeholdercontents ?? (_placeholdercontents = new Dictionary<string, string>());
            set => _placeholdercontents = value;
        }

        public void AddPlaceHolderContent(string key, string value)
        {
            if (_placeholdercontents == null || !this.PlaceholderContents.ContainsKey(key))
            {
                this.PlaceholderContents.Add(key, value);
            }
            else
            {
                var currentvalue = this.PlaceholderContents[key];
                currentvalue += value;
                this.PlaceholderContents[key] = currentvalue;
            }
        }

        private Dictionary<string, object> _items;

        [JsonIgnore]
        public Dictionary<string, object> Items
        {
            get => _items ?? (_items = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase));
            set => _items = value;
        }

        public void SetItem<T>(T value, string keyName = null)
        {
            if (string.IsNullOrEmpty(keyName))
            {
                keyName = typeof(T).Name;
            }
            Items[keyName] = value;
        }

        public T GetItem<T>(string keyName = null)
        {
            if (this._items == null)
            { return default; }
            if (string.IsNullOrEmpty(keyName))
            {
                keyName = typeof(T).Name;
            }
            if (this.Items.ContainsKey(keyName))
            {
                return (T)Items[keyName];
            }
            return default(T);
        }

        public bool HasItem<T>(string keyName = null)
        {
            if (this._items == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(keyName))
            {
                keyName = typeof(T).Name;
            }
            return this.Items.ContainsKey(keyName);
        }

        private List<HeaderBindings> _headerBinding;

        [JsonIgnore]
        public List<HeaderBindings> HeaderBindings
        {
            get => _headerBinding ?? (_headerBinding = new List<HeaderBindings>());
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
                foreach (var item in this.HeaderBindings.Where(item => item.RequireBinding))
                {
                    item.ValueQuery.TryAssignValue(data, this);
                }
            }
        }

        private void InitHeaderValue()
        {
            if (_headerBinding != null)
            {
                foreach (var item in this.HeaderBindings.Where(item => item.RequireBinding))
                {
                    item.ValueQuery.InitValue(this);
                }
            }
        }

        public bool IsSiteBinding { get; set; }

        public bool IsApp { get; set; }

        public string AppParentUrl { get; set; }    
    }
}
