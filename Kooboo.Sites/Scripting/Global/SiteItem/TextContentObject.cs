using Kooboo.Data.Context;
using Kooboo.Sites.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Sites.Scripting.Global.SiteItem
{

    public class TextContentObject : IDictionary<string, object>, System.Collections.IDictionary
    {
        public RenderContext context { get; set; }

        public Kooboo.Sites.Repository.SiteDb siteDb { get; set; }

        public string Culture { get; set; }

        public Kooboo.Sites.Contents.Models.TextContent TextContent { get; set; }

        public TextContentObject(Kooboo.Sites.Contents.Models.TextContent siteobject, RenderContext context)
        {
            this.context = context;
            this.Culture = context.Culture;
            this.TextContent = siteobject;
            this.siteDb = context.WebSite.SiteDb();
        }

        public void SetCulture(string culture)
        {
            this.Culture = culture;
        }

        public string get(string key)
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

                var obj = view.GetValue(key);
                if (obj != null)
                {
                    return obj.ToString();
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
                    this.TextContent.ContentTypeId = type.Id;
                    return;
                }
            }

            this.TextContent.SetValue(FieldName, Value, this.Culture);
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

        public bool IsReadOnly => false;

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

        public void Add(KeyValuePair<string, object> item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            this.TextContent.Contents.Clear();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(string key)
        {
            var item = this.get(key);
            return item != null;
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(string key)
        {

            var store = this.TextContent.GetContentStore(this.Culture);
            if (store != null)
            {
                return store.FieldValues.Remove(key);
            }
            return false;
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            throw new NotImplementedException();
        }

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


        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return data.GetEnumerator();
        }

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

                return result;

            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Contains(object key)
        {
            return this.ContainsKey(key.ToString());
        }

        public void Add(object key, object value)
        {
            this.Add(key.ToString(), value);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return data.GetEnumerator();
        }

        public void Remove(object key)
        {
            Remove(key.ToString());
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }


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


        public ICollection Values
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



        public bool IsFixedSize => false;

        public object SyncRoot => throw new NotImplementedException();

        public bool IsSynchronized => false;



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
