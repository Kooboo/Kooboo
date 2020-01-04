//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Attributes;
using Kooboo.Data.Context;
using Kooboo.Sites.Contents.Models;
using Kooboo.Sites.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KScript.Sites
{
    public class TextContentObject : IDictionary<string, object>, System.Collections.IDictionary
    {
        [KIgnore]
        public RenderContext context { get; set; }
        [KIgnore]
        public Kooboo.Sites.Repository.SiteDb siteDb { get; set; }

        [KIgnore]
        public string Culture { get; set; }

        [KIgnore]
        public Kooboo.Sites.Contents.Models.TextContent TextContent { get; set; }

        public TextContentObject(Kooboo.Sites.Contents.Models.TextContent siteobject, RenderContext context)
        {
            this.context = context;
            this.Culture = context.Culture;
            this.TextContent = siteobject;
            this.siteDb = context.WebSite.SiteDb();
        }

        [KIgnore]
        public void SetCulture(string culture)
        {
            this.Culture = culture;
        }

        public object get(string key)
        {

            string lower = key.ToLower();

            if (lower == "userkey")
            {
                return this.TextContent.UserKey;
            }
            else if (lower == "folderid")
            {
                return this.TextContent.FolderId.ToString();
            }

            else if (lower == "id")
            {
                return this.TextContent.Id.ToString();
            }
            else if (lower == "contenttypeid")
            {
                return this.TextContent.ContentTypeId.ToString();
            }

            else if (lower == "online")
            {
                return this.TextContent.Online.ToString();
            }
            else if (lower == "lastmodified")
            {
                return this.TextContent.LastModified.ToString();
            }

            var store = this.TextContent.GetContentStore(this.Culture);

            if (store != null)
            {
                if (store.FieldValues.ContainsKey(key))
                {
                    return store.FieldValues[key];
                }
            }

            if (lower == "folder")
            {
                var folder = this.siteDb.ContentFolders.Get(this.TextContent.FolderId);
                return folder.Name;
            }

            else if (lower == "contenttype")
            {
                var type = this.siteDb.ContentTypes.Get(this.TextContent.ContentTypeId);
                if (type != null)
                {
                    return type.Name;
                }
            }
            else if (lower == "order" || lower == "sequence")
            {
                return this.TextContent.Order.ToString();
            }

            var contenttype = this.context.WebSite.SiteDb().ContentTypes.Get(this.TextContent.ContentTypeId);

            if (contenttype != null)
            {
                var view = Kooboo.Sites.Helper.ContentHelper.ToView(this.TextContent, this.Culture, contenttype.Properties);

                var obj = view.GetValue(key, context);
                if (obj != null)
                {
                    return obj;
                }
            }

            return null;
        }

        public object this[string key]
        {

            get
            {
                return get(key);

            }
            set
            {
                this.SetObjectValue(key, value.ToString());
            }
        }

        public object GetValue(string fieldName)
        {
            return this.get(fieldName); 
        }

        public void SetValue(string fieldName, string value)
        {
            this.SetObjectValue(fieldName, value); 
        }
     
        public void SetObjectValue(string FieldName, string Value)
        {
            string lower = FieldName.ToLower();

            if (lower == "folder" || lower == "contentfolder")
            {
                var folder = this.siteDb.ContentFolders.Get(Value);
                if (folder != null)
                {
                    this.TextContent.FolderId = folder.Id;
                    return;
                }
            }

            else if (lower == "contenttype")
            {
                var type = this.siteDb.ContentTypes.Get(Value);
                if (type != null)
                {
                    contentType = type;
                    this.TextContent.ContentTypeId = type.Id;
                    return;
                }
            }

            if (contentType == null)
            {
                if (this.TextContent.ContentTypeId != default(Guid))
                {
                    contentType = siteDb.ContentTypes.Get(this.TextContent.ContentTypeId);
                }
            }

            if (contentType == null || contentType.Properties.Find(o => Kooboo.Lib.Helper.StringHelper.IsSameValue(o.Name, FieldName)) != null)
            {
                this.TextContent.SetValue(FieldName, Value, this.Culture);
            }
            else
            {
                AdditionalValues[FieldName] = Value;
            } 
        }
         
        public ContentType contentType { get; set; }

        private Dictionary<string, string> _AdditionalValues;

        [KIgnore]
        public Dictionary<string, string> AdditionalValues
        {
            get
            {
                if (_AdditionalValues == null)
                {
                    _AdditionalValues = new Dictionary<string, string>();
                }
                return _AdditionalValues;
            }
            set
            {
                _AdditionalValues = value;
            }
        }

        public ICollection<string> Keys
        {
            get
            {
                List<string> mykey = new List<string>();
                mykey.Add("id");
                mykey.Add("userKey");
                mykey.Add("lastModifled");

                var store = this.TextContent.GetContentStore(this.Culture);
                if (store != null)
                {
                    foreach (var item in store.FieldValues.Keys)
                    {
                        mykey.Add(item);
                    }
                }

                return mykey;
            }
        }

        [KIgnore]
        public int Count
        {
            get
            {
                var store = this.TextContent.GetContentStore(this.Culture);
                if (store != null)
                {
                    return store.FieldValues.Count();
                }
                return 0;
            }
        }

        [KIgnore]
        public bool IsReadOnly => false;

        [KIgnore]
        ICollection<object> IDictionary<string, object>.Values
        {
            get
            {
                var store = this.TextContent.GetContentStore(this.Culture);
                if (store != null)
                {
                    return store.FieldValues.Values.ToList<object>();
                }
                return new List<object>();
            }
        }
         
        public void Add(string key, object value)
        {
            this.TextContent.SetValue(key, value.ToString(), Culture);
        }

        [KIgnore]
        public void Add(KeyValuePair<string, object> item)
        {
            throw new NotImplementedException();
        }

        [KIgnore]
        public void Clear()
        {
            this.TextContent.Contents.Clear();
        }

        [KIgnore]
        public bool Contains(KeyValuePair<string, object> item)
        {
            throw new NotImplementedException();
        }

        [KIgnore]
        public bool ContainsKey(string key)
        {
            var item = this.get(key);
            return item != null;
        }
        [KIgnore]
        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }
        [KIgnore]
        public bool Remove(string key)
        { 
            var store = this.TextContent.GetContentStore(this.Culture);
            if (store != null)
            {
                return store.FieldValues.Remove(key);
            }
            return false;
        }
        [KIgnore]
        public bool Remove(KeyValuePair<string, object> item)
        {
            throw new NotImplementedException();
        }
        [KIgnore]
        public bool TryGetValue(string key, out object value)
        {
            var result = get(key);
            if (result != null)
            {
                value = result;
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        [KIgnore]
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return data.GetEnumerator();
        }
        [KIgnore]
        private Dictionary<string, object> data
        {
            get
            {
                Dictionary<string, object> result = new Dictionary<string, object>();
                var store = this.TextContent.GetContentStore(this.Culture);
                if (store != null)
                {
                    foreach (var item in store.FieldValues)
                    {
                        result.Add(item.Key, item.Value);
                    }
                }

                result["id"] = this.TextContent.Id;
                result["userKey"] = this.TextContent.UserKey;
                result["lastModified"] = this.TextContent.LastModified;
                result["ContentTypeId"] = this.TextContent.ContentTypeId.ToString();
                result["ParentId"] = this.TextContent.ParentId.ToString();

                return result;

            }
        }
        [KIgnore]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        [KIgnore]
        public bool Contains(object key)
        {
            return this.ContainsKey(key.ToString());
        }
        [KIgnore]
        public void Add(object key, object value)
        {
            this.Add(key.ToString(), value);
        }
        [KIgnore]
        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return data.GetEnumerator();
        }
        [KIgnore]
        public void Remove(object key)
        {
            Remove(key.ToString());
        }
        [KIgnore]
        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        [KIgnore]
        ICollection IDictionary.Keys
        {
            get
            {
                //var store = this.TextContent.GetContentStore(this.Culture);
                //if (store != null)
                //{
                //    return store.FieldValues.Keys;
                //}
                //return new List<string>();

                List<string> mykey = new List<string>();
                mykey.Add("id");
                mykey.Add("userKey");
                mykey.Add("lastModifled");

                var store = this.TextContent.GetContentStore(this.Culture);
                if (store != null)
                {
                    foreach (var item in store.FieldValues.Keys)
                    {
                        mykey.Add(item);
                    }
                }

                return mykey;


            }
        }

        [KIgnore]
        public ICollection Values
        {
            get
            {
                return this.data;
            }
        }


        [KIgnore]
        public bool IsFixedSize => false;
        [KIgnore]
        public object SyncRoot => throw new NotImplementedException();
        [KIgnore]
        public bool IsSynchronized => false;

        [KIgnore]
        public object this[object key]
        {

            get
            {
                return get(key.ToString());

            }
            set
            {
                this.SetObjectValue(key.ToString(), value.ToString());
            }
        } 

    }
}
