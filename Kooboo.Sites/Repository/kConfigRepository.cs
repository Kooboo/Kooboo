//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.IndexedDB;
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public KConfig GetOrAdd(string key, string TagName, string TagHtml)
        {
            var old = GetByNameOrId(key);

            if (old != null)
            {
                return old;
            }
            else
            {
                KConfig config = new KConfig();
                config.Name = key;
                config.TagHtml = TagHtml;
                config.TagName = TagName;
                AddOrUpdate(config);
                return config;
            }
        }


        public KConfig GetOrAdd(string key, Kooboo.Dom.Element El)
        {
            var old = GetByNameOrId(key);

            if (old != null)
            {
                return old;
            }
            else
            {
                KConfig config = new KConfig();
                config.Name = key;
                config.TagHtml = El.OuterHtml;
                config.TagName = El.tagName;

                config.Binding = GetBindings(El);
                AddOrUpdate(config);
                return config;
            }
        }

        public Dictionary<string, string> GetBindings(Kooboo.Dom.Element El)
        {
            Dictionary<string, string> Bindings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var item in El.attributes)
            {
                if (!IsIgnoreAttribute(item.name))
                {
                    Bindings[item.name] = item.value;
                }
            }

            if (!Service.DomService.IsSelfCloseTag(El.tagName))
            {
                // if (!Bindings.ContainsKey("innerHtml"))
                // {
                Bindings["innerHtml"] = El.InnerHtml;
                // }
                //else
                //{
                //    Bindings["innerKoobooText"] = El.InnerHtml;
                //}
            }

            return Bindings;
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
