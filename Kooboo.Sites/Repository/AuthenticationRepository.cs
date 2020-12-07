using Kooboo.IndexedDB;
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Repository
{
    public class AuthenticationRepository : SiteRepositoryBase<Authentication>
    {
        public override ObjectStoreParameters StoreParameters
        {
            get
            {
                var paras = new ObjectStoreParameters();
                paras.AddColumn<Authentication>(it => it.Id);
                paras.AddColumn<Authentication>(it => it.Name);
                paras.AddColumn<Authentication>(it => it.LastModified);
                paras.SetPrimaryKeyField<Authentication>(o => o.Id);
                return paras;
            }
        }
    }
}
