//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.IndexedDB;
using Kooboo.Sites.Contents;
using Kooboo.Sites.Contents.Models;
using Kooboo.Sites.ViewModel;
using System;
using System.Linq;

namespace Kooboo.Sites.Repository
{
    public class TextContentRepository : SiteRepositoryBase<TextContent>
    {
        public override ObjectStoreParameters StoreParameters
        {
            get
            {
                ObjectStoreParameters paras = new ObjectStoreParameters();
                paras.AddColumn<TextContent>(it => it.Id);
                paras.AddColumn<TextContent>(it => it.ParentId);
                paras.AddColumn<TextContent>(it => it.FolderId);
                paras.AddColumn<TextContent>(it => it.ContentTypeId);
                paras.AddColumn<TextContent>(it => it.Online);
                paras.AddColumn<TextContent>(it => it.LastModified);
                paras.SetPrimaryKeyField<TextContent>(o => o.Id);
                return paras;
            }
        }

        public override TextContent GetByNameOrId(string NameOrGuid)
        {
            Guid key;
            bool parseok = Guid.TryParse(NameOrGuid, out key);

            if (!parseok)
            {
                key = Kooboo.Lib.Security.Hash.ComputeGuidIgnoreCase(NameOrGuid);
            }
            return Get(key);
        }

        public void EnsureUserKey(TextContent content)
        {
            if (string.IsNullOrWhiteSpace(content.UserKey))
            {
                content.UserKey = this.GenerateUserKey(content);
            }
        }

        public override bool AddOrUpdate(TextContent textContent, Guid UserId = default(Guid))
        {
            EnsureUserKey(textContent);  

            return base.AddOrUpdate(textContent, UserId);
        }

        public TextContentViewModel GetView(Guid id, string lang)
        {
            return GetView(this.Get(id), lang);
        }

        public TextContentViewModel GetView(TextContent content, string lang)
        {
            if (content != null)
            {
                var prop = this.SiteDb.ContentTypes.GetColumns(content.ContentTypeId);
                return Helper.ContentHelper.ToView(content, lang, prop);
            }
            return null;
        }

        // get the default content item...search for all possible text repositories..
        public TextContentViewModel GetDefaultContentFromFolder(Guid FolderId, string CurrentCulture = null)
        {
            if (string.IsNullOrWhiteSpace(CurrentCulture))
            {
                CurrentCulture = this.WebSite.DefaultCulture;
            }

            var list = this.Query.Where(o => o.FolderId == FolderId).Take(10);

            foreach (var item in list.Where(o => o.Online))
            {
                var view = GetView(item, CurrentCulture);
                if (view != null && view.TextValues.Count() > 0)
                {
                    return view;
                }
            }

            foreach (var item in list.Where(o => !o.Online))
            {
                var view = GetView(item, CurrentCulture);
                if (view != null && view.TextValues.Count() > 0)
                {
                    return view;
                }
            }

            return null;
        }

        public string GenerateUserKey(TextContent content)
        {
            string lang = this.SiteDb.WebSite.DefaultCulture;
            var contenttype = this.SiteDb.ContentTypes.Get(content.ContentTypeId);
            if (contenttype != null)
            {
                object value = null;
                foreach (var item in contenttype.Properties.Where(o => o.IsSummaryField))
                {
                    value = content.GetValue(item.Name, lang);
                    if (value != null && !string.IsNullOrEmpty(value.ToString()))
                    {
                        return GetUserKey(value.ToString());
                    }
                }

                foreach (var item in contenttype.Properties.Where(o => o.DataType == Data.Definition.DataTypes.String))
                {
                    value = content.GetValue(item.Name, lang);
                    if (value != null && !string.IsNullOrEmpty(value.ToString()))
                    {
                        return GetUserKey(value.ToString());
                    }
                }

                foreach (var item in contenttype.Properties.Where(o => !o.IsSummaryField && !o.IsSystemField))
                {
                    value = content.GetValue(item.Name, lang);
                    if (value != null && !string.IsNullOrEmpty(value.ToString()))
                    {
                        return GetUserKey(value.ToString());
                    }
                }
            }

            foreach (var item in content.Contents)
            {
                foreach (var field in item.FieldValues)
                {
                    if (!string.IsNullOrEmpty(field.Value))
                    {
                        return GetUserKey(field.Value);
                    }
                }
            }

            return Guid.NewGuid().ToString();
        }

        private string GetUserKey(string key)
        {
            key = UserKeyHelper.ToSafeUserKey(key);
            if (!IsUserKeyExists(key))
            {
                return key;
            }
            string newkey = string.Empty;
            for (int i = 0; i < 99; i++)
            {
                newkey = key + i.ToString();
                if (!IsUserKeyExists(newkey))
                {
                    return newkey;
                }
            }
            Random rnd = new Random();
            for (int i = 0; i < 9999; i++)
            {
                newkey = key + rnd.Next(i, int.MaxValue).ToString();
                if (!IsUserKeyExists(newkey))
                {
                    return newkey;
                }
            }
            return null;
        }

        internal bool IsUserKeyExists(string userKey)
        {
            if (string.IsNullOrEmpty(userKey))
            { return true; }
            Guid id = Lib.Security.Hash.ComputeGuidIgnoreCase(userKey);
            return this.Get(id) != null;
        }

        public void EusureNonLangContent(TextContent content, ContentType contenttype = null)
        {
            if (contenttype == null)
            {
                contenttype = this.SiteDb.ContentTypes.Get(content.ContentTypeId);
            }


            string defaultculture = this.SiteDb.WebSite.DefaultCulture;

            var NoSysNoMul = contenttype.Properties.Where(o => o.IsSystemField == false && o.MultipleLanguage == false).ToList();

            foreach (var item in NoSysNoMul)
            {
                string value = null;
                var langstore = content.GetContentStore(defaultculture);
                if (langstore != null && langstore.FieldValues.ContainsKey(item.Name))
                {
                    value = langstore.FieldValues[item.Name];
                }

                if (string.IsNullOrEmpty(value))
                {
                    foreach (var lang in content.Contents)
                    {
                        if (lang.FieldValues.ContainsKey(item.Name))
                        {
                            value = lang.FieldValues[item.Name];
                            break;
                        }
                    }
                }

                bool valueset = false;
                // remove the key...  
                foreach (var citem in content.Contents)
                {
                    if (citem.Lang != defaultculture)
                    {
                        citem.FieldValues.Remove(item.Name);
                    }
                    else
                    {
                        citem.FieldValues[item.Name] = value;
                        valueset = true;
                    }
                }

                if (!valueset)
                {
                    content.SetValue(item.Name, value, defaultculture);
                }

            }
        }
    }
}
