//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Sites.Repository;

namespace Kooboo.Module
{
    public class SiteModuleApiBase : ISiteModuleApi
    {
        public string ModelName
        {
            get
            {
                return "SiteModuleApiBase";
            }
        }
        public SiteDb SiteDb { get; set; }

        public RenderContext Context { get; set; }

        public T GetSetting<T>() where T : ISiteSetting
        {
            if (SiteDb != null)
            {
                return SiteDb.CoreSetting.GetSetting<T>();
            }
            return default(T);
        }

        public void SetSetting<T>(T setting) where T : ISiteSetting
        {
            if (SiteDb != null)
            {
                SiteDb.CoreSetting.AddOrUpdate(setting);
            }
        }
    }
}
