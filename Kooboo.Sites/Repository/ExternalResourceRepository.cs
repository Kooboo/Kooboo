//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.IndexedDB;
using Kooboo.Sites.Models;
using System;

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

        public void AddOrUpdate(string fullUrl, byte destinationObjectType)
        {
            ExternalResource resource = new ExternalResource
            {
                FullUrl = fullUrl, DestinationObjectType = destinationObjectType
            };
            AddOrUpdate(resource);
        }

        public override ExternalResource GetByUrl(string url)
        {
            var id = Kooboo.Data.IDGenerator.Generate(url, ConstObjectType.ExternalResource);
            return this.Get(id);
        }

        public void ChangeUrl(Guid id, string newUrl)
        {
            var siteobject = this.Get(id);
            if (siteobject != null)
            {
                var usedby = this.GetUsedBy(siteobject.Id);
                foreach (var item in usedby)
                {
                    var type = item.ConstType;
                    var repo = this.SiteDb.GetRepository(type);
                    Helper.ChangeHelper.ChangeUrl(this.SiteDb, repo, item.ObjectId, siteobject.FullUrl, newUrl);
                }

                this.Delete(id);
            }
        }
    }
}