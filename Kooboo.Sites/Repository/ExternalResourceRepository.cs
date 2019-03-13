//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using Kooboo.IndexedDB;
using Kooboo.Sites.Models;

namespace Kooboo.Sites.Repository
{
    public class ExternalResourceRepository : SiteRepositoryBase<ExternalResource>
    {
        public override ObjectStoreParameters StoreParameters
        {
            get
            {
                ObjectStoreParameters paras = new ObjectStoreParameters();
                paras.AddColumn<ExternalResource>(o => o.DestinationObjectType);
                paras.AddColumn<ExternalResource>(o => o.UrlHash);
                paras.SetPrimaryKeyField<ExternalResource>(o => o.Id); 
                return paras;

            }
        }

        public void AddOrUpdate(string FullUrl, byte DestinationObjectType)
        {
            ExternalResource resource = new ExternalResource();
            resource.FullUrl = FullUrl;
            resource.DestinationObjectType = DestinationObjectType;
            AddOrUpdate(resource);
        }

        public override ExternalResource GetByUrl(string Url)
        {
            var id = Kooboo.Data.IDGenerator.Generate(Url, ConstObjectType.ExternalResource);
            return this.Get(id);
        }

        public void ChangeUrl(Guid Id, string NewUrl)
        {
            var siteobject = this.Get(Id);
            if (siteobject != null)
            {
                var usedby = this.GetUsedBy(siteobject.Id);
                foreach (var item in usedby)
                {
                    var type = item.ConstType;
                    var repo = this.SiteDb.GetRepository(type); 
                    Helper.ChangeHelper.ChangeUrl(this.SiteDb, repo, item.ObjectId, siteobject.FullUrl, NewUrl);   
                }

                this.Delete(Id); 
            }
        }

    }
}
