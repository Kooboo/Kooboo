//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.IndexedDB;
using Kooboo.Data.Models;

namespace Kooboo.Data.Repository
{
  public  class DataMethodRepository : RepositoryBase<DataMethodSetting>
    {
        protected override ObjectStoreParameters StoreParameters
        {
            get
            {
                ObjectStoreParameters paras = new ObjectStoreParameters();
                paras.AddColumn<DataMethodSetting>(o => o.MethodSignatureHash);
                paras.AddColumn<DataMethodSetting>(o => o.DeclareTypeHash);
                paras.AddColumn<DataMethodSetting>(o => o.IsThirdPartyType); 
                paras.SetPrimaryKeyField<DataMethodSetting>(o => o.Id);
                return paras; 
            }
        }
    }
}
