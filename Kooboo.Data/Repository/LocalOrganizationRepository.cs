using Kooboo.Data.Models;
using Kooboo.IndexedDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Data.Repository
{
  
    public class LocalOrganizationRepository : RepositoryBase<Organization>
    {
        protected override ObjectStoreParameters StoreParameters
        {
            get
            {
                ObjectStoreParameters paras = new ObjectStoreParameters(); 
                paras.SetPrimaryKeyField<User>(o => o.Id);
                return paras;
            }
        }

        public Organization Get(string nameOrGuid)
        {
            Guid key = Kooboo.Lib.Helper.IDHelper.ParseKey(nameOrGuid);
            return this.Get(key); 
        }
    }  
}
