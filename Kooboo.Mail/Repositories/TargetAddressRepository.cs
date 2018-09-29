//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using Kooboo.Extensions;
using Kooboo.IndexedDB;

namespace Kooboo.Mail.Repositories
{
  public  class TargetAddressRepository : RepositoryBase<TargetAddress>
    {
        public TargetAddressRepository(MailDb db) 
            : base(db.Db)
        { 
        }

        protected override ObjectStoreParameters StoreParameters
        {
            get
            {
                ObjectStoreParameters paras = new ObjectStoreParameters();
                paras.SetPrimaryKeyField<EmailAddress>(o => o.Id);
                return paras;
            }
        }

        public TargetAddress Get(string email)
        { 
            return Store.get(TargetAddress.ToId(email));
        }
    }
}
