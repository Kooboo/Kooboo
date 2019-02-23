using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Module;
using Kooboo.Sites.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public T GetSetting<T>() where T:ISiteSetting
        {
            if (SiteDb !=null)
             {
                return SiteDb.CoreSetting.GetSetting<T>(); 
             }
            return default(T); 
        }
    }
}
