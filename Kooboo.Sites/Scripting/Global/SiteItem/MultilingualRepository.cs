//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Attributes;
using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Sites.Models;
using KScript.Sites;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KScript.Sites
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
                var siteobjct = item as Kooboo.Sites.Contents.Models.MultipleLanguageObject;
                if (siteobjct != null)
                {
                    var model = new MultilingualObject(siteobjct, this.context);
                    result.Add(model);
                }
            }
            return result;
        }

        public virtual MultilingualObject Get(object nameOrId)
        {
            var item = this.repo.GetByNameOrId(nameOrId.ToString());
            if (item != null && item is Kooboo.Sites.Contents.Models.MultipleLanguageObject)
            {
                return new MultilingualObject(item as Kooboo.Sites.Contents.Models.MultipleLanguageObject, this.context);
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

        [KIgnore]
        public virtual void Add(object SiteObject)
        {
            var data = this.GetData(SiteObject);

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

        [Description("Add an item with default culture")]
        public void Add(string name, string value)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            var old = this.repo.GetByNameOrId(name);
            if (old == null)
            {
                var result = Activator.CreateInstance(this.repo.ModelType) as Kooboo.Sites.Contents.Models.MultipleLanguageObject;
                result.Name = name;
                result.SetValue(this.context.Culture, value);
                this.repo.AddOrUpdate(result);
            }
            else
            {
                var oldmul = old as Kooboo.Sites.Contents.Models.MultipleLanguageObject; 
                if (oldmul!=null)
                {
                    oldmul.SetValue(this.context.Culture, value);
                    this.repo.AddOrUpdate(oldmul);
                } 
            }
        }

        public void Add(string name, string value, string culture)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }
            var old = this.repo.GetByNameOrId(name);
            if (old == null)
            { 
                var result = Activator.CreateInstance(this.repo.ModelType) as Kooboo.Sites.Contents.Models.MultipleLanguageObject;
                result.Name = name;
                result.SetValue(culture, value);
                this.repo.AddOrUpdate(result);
            }
            else
            {
                var oldmul = old as Kooboo.Sites.Contents.Models.MultipleLanguageObject;
                if (oldmul != null)
                {
                    oldmul.SetValue(culture, value);
                    this.repo.AddOrUpdate(oldmul);
                }  
            }
        }

        public void Update(string name, string value)
        {
            this.Add(name, value);
        }

        public void Update(string name, string value, string culture)
        {
            this.Add(name, value, culture);
        }

        public virtual void Update(object SiteObject)
        {
            if (SiteObject is MultilingualObject)
            {
                var result = Activator.CreateInstance(this.repo.ModelType) as Kooboo.Sites.Contents.Models.MultipleLanguageObject;

                var multi = SiteObject as MultilingualObject;

                result.Name = multi.Name;
                result.Values = multi.Values;

                this.repo.AddOrUpdate(result); 
            }
            else
            {

                var data = this.GetData(SiteObject);

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

        internal Dictionary<string, object> GetData(object dataobj)
        {
            Dictionary<string, object> data = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            System.Collections.IDictionary idict = dataobj as System.Collections.IDictionary;

            if (idict != null)
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
                var dynamicobj = dataobj as IDictionary<string, object>;
                if (dynamicobj != null)
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
