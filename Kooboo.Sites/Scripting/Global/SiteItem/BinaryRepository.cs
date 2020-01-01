//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Sites.Extensions;
using System;

namespace KScript.Sites
{           
    public class BinaryRepository : RepositoryBase
    {
        public BinaryRepository(IRepository repo, RenderContext context) : base(repo, context)
        {
        }

        public void UpdateBinary(object NameOrId, byte[] Binary)
        {
            var item = this.repo.GetByNameOrId(NameOrId.ToString());
            var binaryitem = item as IBinaryFile;

            binaryitem.ContentBytes = Binary;
            this.repo.AddOrUpdate(item);
        }
                   

        public ISiteObject GetByUrl(string url)
        {
            var route = this.context.WebSite.SiteDb().Routes.GetByUrl(url);
            if (route != null && route.objectId != default(Guid))
            {
                return this.repo.Get(route.objectId);
            }
            return null;
        }

    }   
}
