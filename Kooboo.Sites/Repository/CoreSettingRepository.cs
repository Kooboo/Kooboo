//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Interface;
using Kooboo.IndexedDB;
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;


namespace Kooboo.Sites.Repository
{
    public class CoreSettingRepository : SiteRepositoryBase<CoreSetting>
    {
        public override ObjectStoreParameters StoreParameters
        {
            get
            {
                ObjectStoreParameters para = new ObjectStoreParameters();
                para.SetPrimaryKeyField<CoreSetting>(o => o.Id);
                return para;
            }
        }

        public override bool AddOrUpdate(CoreSetting value)
        {
            cache.Remove(value.Name);
            return base.AddOrUpdate(value);
        }

        public override bool AddOrUpdate(CoreSetting value, Guid UserId)
        {
            cache.Remove(value.Name);
            return base.AddOrUpdate(value, UserId);
        }

        public override void Delete(Guid id)
        {
            var obj = this.Get(id);
            if (obj != null)
            {
                cache.Remove(obj.Name);
            }
            base.Delete(id);
        }

        public override void Delete(Guid id, Guid UserId)
        {
            var obj = this.Get(id);
            if (obj != null)
            {
                cache.Remove(obj.Name);
            }
            base.Delete(id, UserId);
        }

        public Dictionary<string, object> cache = new Dictionary<string, object>();

        public T GetSetting<T>() where T: ISiteSetting
        {
            var type = typeof(T);

            var result = GetSiteSetting(type) as ISiteSetting; 
            if (result !=null)
            {
                return (T)result; 
            } 
            return default(T);  
        }

        public object GetSiteSetting(Type siteSettingType)
        {
            var name = Sites.Service.CoreSettingService.GetName(siteSettingType); 
            if (string.IsNullOrEmpty(name))
            {
                return null; 
            }

            if (cache.ContainsKey(name))
            {
               return cache[name]; 
            }
            var obj = this.Get(name);
            if (obj != null)
            {
                var result = Kooboo.Sites.Service.CoreSettingService.GetSetting(obj, siteSettingType);
                cache[name] = result;
                return result;
            }
            return null; 
        }


        public void AddOrUpdate(ISiteSetting setting)
        { 
            if (setting == null)
            {
                return; 
            }

            var coresetting = Kooboo.Sites.Service.CoreSettingService.GetCoreSetting(setting); 
            this.AddOrUpdate(coresetting);    
        }
         
         

    }
     

}
