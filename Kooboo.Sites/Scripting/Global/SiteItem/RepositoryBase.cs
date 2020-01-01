//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Attributes;
using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Sites.Models;
using Kooboo.Sites.Scripting.Global;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KScript.Sites
{ 
    public class RepositoryBase
    {
        public RepositoryBase(IRepository repo, RenderContext context)
        {
            this.repo = repo;
            this.context = context;
        }

        [KIgnore]
        protected IRepository repo { get; set; }

        [KIgnore]
        internal RenderContext context { get; set; }

        [Description(@"Return an array of all the SiteObjects
var allStyles = k.site.styles.all(); ")]
        public virtual List<SiteObject> All()
        {
            List<SiteObject> result = new List<SiteObject>();
            foreach (var item in this.repo.All())
            {
                var siteobjct = item as SiteObject;
                if (siteobjct != null)
                {
                    result.Add(siteobjct);
                }
            }
            return result;
        }

        [Description(@"Update the SiteOject property values
    var style = k.site.styles.getByUrl(""/a.css""); 
    style.body = "".sample {}""; 
    k.site.styles.update(style);")]
        public virtual void Update(object SiteObject)
        {
            this.repo.AddOrUpdate(SiteObject);
        }

        [Description(@"Get an item based on Name or Id
   var view = k.site.views.get(""viewname"");")]
        public virtual ISiteObject Get(object nameOrId)
        {
            return this.repo.GetByNameOrId(nameOrId.ToString());
        }

        [Description(@"Delete an item
       var page = k.site.pages.getByUrl(""/pagename""); 
k.site.pages.delete(page.id);")]
        public virtual void Delete(object nameOrId)
        {
            var item = Get(nameOrId);
            if (item != null)
            {
                this.repo.Delete(item.Id);
            }
        } 

        [Description(@"Add a siteobject
       var view = { };
       view.name = ""viewname"";
  view.body = ""new  body""; 
  k.site.views.add(view);")]
        public virtual void Add(object SiteObject)
        {
            var rightobject = kHelper.PrepareData(SiteObject, this.repo.ModelType);
            this.repo.AddOrUpdate(rightobject);
        } 
    }


}
