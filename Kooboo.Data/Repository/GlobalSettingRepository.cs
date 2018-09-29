//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.IndexedDB;

namespace Kooboo.Data.Repository
{
   public class GlobalSettingRepository : RepositoryBase<GlobalSetting>
    { 
        protected override ObjectStoreParameters StoreParameters
        {
            get
            {
                ObjectStoreParameters paras = new ObjectStoreParameters();
                paras.AddColumn<GlobalSetting>(o => o.Name);
                paras.AddColumn<GlobalSetting>(o => o.OrganizationId); 
                paras.SetPrimaryKeyField<GlobalSetting>(o => o.Id);
                return paras;
            }
        }

        public GlobalSetting GetByName(string name)
        {
            return this.Query.Where(o => o.Name == name).FirstOrDefault(); 
        }
    } 
}
