//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.IndexedDB;
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;

namespace Kooboo.Sites.Repository
{
    public class kConfigRepository : SiteRepositoryBase<KConfig>
    {
        public override ObjectStoreParameters StoreParameters
        {
            get
            {
                ObjectStoreParameters para = new ObjectStoreParameters();
                para.SetPrimaryKeyField<KConfig>(o => o.Id);
                return para;
            }
        }

        public KConfig GetOrAdd(string key, string tagName, string tagHtml)
        {
            var old = GetByNameOrId(key);

            if (old != null)
            {
                return old;
            }
            else
            {
                KConfig config = new KConfig {Name = key, TagHtml = tagHtml, TagName = tagName};
                AddOrUpdate(config);
                return config;
            }
        }

        public KConfig GetOrAdd(string key, Kooboo.Dom.Element el)
        {
            var old = GetByNameOrId(key);

            if (old != null)
            {
                return old;
            }
            else
            {
                KConfig config = new KConfig
                {
                    Name = key, TagHtml = el.OuterHtml, TagName = el.tagName, Binding = GetBindings(el)
                };

                AddOrUpdate(config);
                return config;
            }
        }

        public Dictionary<string, string> GetBindings(Kooboo.Dom.Element el)
        {
            Dictionary<string, string> bindings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var item in el.attributes)
            {
                if (!IsIgnoreAttribute(item.name))
                {
                    bindings[item.name] = item.value;
                }
            }

            if (!Service.DomService.IsSelfCloseTag(el.tagName))
            {
                // if (!Bindings.ContainsKey("innerHtml"))
                // {
                bindings["innerHtml"] = el.InnerHtml;
                // }
                //else
                //{
                //    Bindings["innerKoobooText"] = El.InnerHtml;
                //}
            }

            return bindings;
        }

        public bool IsIgnoreAttribute(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return true;
            }

            string lower = name.ToLower();

            if (lower.StartsWith("on") || lower.Contains("-") || lower.Contains("_") || lower == "style")
            {
                return true;
            }
            return false;
        }
    }
}