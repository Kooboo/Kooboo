//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Sites.Extensions;
using System;
using System.ComponentModel;

namespace KScript.Sites
{           
    public class BinaryRepository : RepositoryBase
    {
        public BinaryRepository(IRepository repo, RenderContext context) : base(repo, context)
        {
        }

        [Description(@"Update the binary content
 if (k.request.files.length>0)
   {
         var image = k.site.images.getByUrl(""/kooboo.png"");
         k.site.images.updateBinary(image.id, k.request.files[0].bytes); 
   }")]
        public void UpdateBinary(object NameOrId, byte[] Binary)
        {
            var item = this.repo.GetByNameOrId(NameOrId.ToString());
            var binaryitem = item as IBinaryFile;

            binaryitem.ContentBytes = Binary;
            this.repo.AddOrUpdate(item);
        }

        [Description(@"get the Binary Object by Url
       var image = k.site.images.getByUrl(""/kooboo.png"");")]
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
