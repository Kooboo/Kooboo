//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Models;
using Kooboo.IndexedDB;
using System;

namespace Kooboo.Data.Repository
{
    public class NotificationRepository : RepositoryBase<Notificationnew>
    {
        protected override ObjectStoreParameters StoreParameters
        {
            get
            {
                ObjectStoreParameters paras = new ObjectStoreParameters();
                paras.AddIndex<Notificationnew>(o => o.UtcLastModified);
                paras.AddColumn<Notificationnew>(o => o.OrganizationId);
                paras.SetPrimaryKeyField<Notificationnew>(o => o.Id);
                return paras;
            }
        }

        public void Add(string name, string message, NotifyType notifytype, Guid webSiteId, Guid organizationId, Guid userId = default(Guid))
        {
            Notificationnew notify = new Notificationnew
            {
                Message = message,
                Name = name,
                NotifyType = notifytype,
                WebSiteId = webSiteId,
                OrganizationId = organizationId,
                UserId = userId
            };

            this.AddOrUpdate(notify);
        }
    }
}