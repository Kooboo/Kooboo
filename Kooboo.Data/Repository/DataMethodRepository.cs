//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Models;
using Kooboo.IndexedDB;

namespace Kooboo.Data.Repository
{
    public class DataMethodRepository : RepositoryBase<DataMethodSetting>
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