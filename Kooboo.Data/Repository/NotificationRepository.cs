//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Models;
using Kooboo.IndexedDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Repository
{
  public  class NotificationRepository: RepositoryBase<Notificationnew>
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

        public void Add(string Name, string Message, NotifyType notifytype, Guid WebSiteId, Guid OrganizationId, Guid UserId=default(Guid))
        {
            Notificationnew notify = new Notificationnew();
            notify.Message = Message;
            notify.Name = Name;
            notify.NotifyType = notifytype;
            notify.WebSiteId = WebSiteId;
            notify.OrganizationId = OrganizationId;
            notify.UserId = UserId;

            this.AddOrUpdate(notify); 

        }
        
    }
}
