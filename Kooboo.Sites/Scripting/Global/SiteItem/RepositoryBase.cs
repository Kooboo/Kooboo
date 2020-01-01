//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Sites.Models;
using Kooboo.Sites.Scripting.Global;
using System;
using System.Collections.Generic;
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
         
         
        protected IRepository repo { get; set; }

        internal RenderContext context { get; set; }

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

        public virtual void Update(object SiteObject)
        {
            this.repo.AddOrUpdate(SiteObject);
        }

        public virtual ISiteObject Get(object nameOrId)
        {
            return this.repo.GetByNameOrId(nameOrId.ToString());
        }

        public virtual void Delete(object nameOrId)
        {
            var item = Get(nameOrId);
            if (item != null)
            {
                this.repo.Delete(item.Id);
            }
        }

        public virtual void Add(object SiteObject)
        {
            var rightobject = kHelper.PrepareData(SiteObject, this.repo.ModelType);
            this.repo.AddOrUpdate(rightobject);
        }

         

    }


}
