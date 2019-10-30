//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Kooboo.Sites.Contents.Models;

namespace Kooboo.Sites.Scripting.Global.SiteItem
{
    public class MultilingualRepository
    {
        public MultilingualRepository(IRepository repo, RenderContext context)
        {
            this.repo = repo;
            this.context = context;
        }

        protected IRepository repo { get; set; }

        protected RenderContext context { get; set; }

        public virtual List<MultilingualObject> All()
        {
            List<MultilingualObject> result = new List<MultilingualObject>();
            foreach (var item in this.repo.All())
            {
                if (item is MultipleLanguageObject siteobjct)
                {
                    var model = new MultilingualObject(siteobjct, this.context);
                    result.Add(model);
                }
            }
            return result;
        }

        public virtual void Update(object siteObject)
        {
            if (siteObject is MultilingualObject multi)
            {
                var result = Activator.CreateInstance(this.repo.ModelType) as Kooboo.Sites.Contents.Models.MultipleLanguageObject;

                result.Name = multi.Name;
                result.Values = multi.Values;

                this.repo.AddOrUpdate(result);
            }
            else
            {
                var data = this.GetData(siteObject);

                if (data == null || !data.Any())
                {
                    return;
                }

                var result = Activator.CreateInstance(this.repo.ModelType) as Kooboo.Sites.Contents.Models.MultipleLanguageObject;

                foreach (var item in data)
                {
                    var lowerkey = item.Key.ToLower();
                    if (lowerkey == "name")
                    {
                        result.Name = item.Value.ToString();
                    }
                    else if (lowerkey == "value")
                    {
                        result.SetValue(this.context.Culture, item.Value);
                    }
                    else
                    {
                        result.SetValue(item.Key, item.Value);
                    }
                }
                this.repo.AddOrUpdate(result);
            }
        }

        public virtual MultilingualObject Get(object nameOrId)
        {
            var item = this.repo.GetByNameOrId(nameOrId.ToString());
            if (item != null && item is MultipleLanguageObject languageObject)
            {
                return new MultilingualObject(languageObject, this.context);
            }
            return null;
        }

        public virtual void Delete(object nameOrId)
        {
            var item = Get(nameOrId);
            if (item != null)
            {
                this.repo.Delete(item.Id);
            }
        }

        public virtual void Add(object siteObject)
        {
            var data = this.GetData(siteObject);

            var result = Activator.CreateInstance(this.repo.ModelType) as Kooboo.Sites.Contents.Models.MultipleLanguageObject;

            foreach (var item in data)
            {
                var lowerkey = item.Key.ToLower();
                if (lowerkey == "name")
                {
                    result.Name = item.Value.ToString();
                }
                else if (lowerkey == "value")
                {
                    result.SetValue(this.context.Culture, item.Value);
                }
                else
                {
                    result.SetValue(item.Key, item.Value);
                }
            }

            this.repo.AddOrUpdate(result);
        }

        internal Dictionary<string, object> GetData(object dataobj)
        {
            Dictionary<string, object> data = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            if (dataobj is IDictionary idict)
            {
                foreach (var item in idict.Keys)
                {
                    var value = idict[item];
                    if (value != null)
                    {
                        data.Add(item.ToString(), value.ToString());
                    }
                }
            }
            else
            {
                if (dataobj is IDictionary<string, object> dynamicobj)
                {
                    foreach (var item in dynamicobj.Keys)
                    {
                        var value = dynamicobj[item];
                        if (value != null)
                        {
                            data.Add(item.ToString(), value.ToString());
                        }
                    }
                }
            }

            return data;
        }
    }
}