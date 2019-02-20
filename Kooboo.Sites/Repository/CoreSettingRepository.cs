using Kooboo.Data.Interface;
using Kooboo.IndexedDB;
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;


namespace Kooboo.Sites.Repository
{
    public class CoreSettingRepository : SiteRepositoryBase<CoreSetting>
    {
        internal override ObjectStoreParameters StoreParameters
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

        public Dictionary<string, ISiteSetting> cache = new Dictionary<string, ISiteSetting>();

        public T GetSetting<T>() where T: ISiteSetting
        {
            var type = typeof(T);

            var result = GetSetting(type); 
            if (result !=null)
            {
                return (T)result; 
            } 
            return default(T);  
        }


        public ISiteSetting GetSetting(Type siteSettingType)
        {
            if (cache.ContainsKey(siteSettingType.Name))
            {
               return cache[siteSettingType.Name]; 
            }
            var obj = this.Get(siteSettingType.Name);
            if (obj != null)
            {
                var result = Kooboo.Sites.Service.CoreSettingService.GetSiteSetting(obj, siteSettingType);
                cache[siteSettingType.Name] = result;
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
