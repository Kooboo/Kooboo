using Kooboo.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Module
{
   public static class ModuleApiHelper
    {

        public static ApiMethod GetApiMethod(ApiCall call)
        {
            return null;
        }

        public static System.Reflection.ConstructorInfo GetConstrutor(Type type)
        {
            var consturcs = type.GetConstructors();
            if (!consturcs.Any())
            {
                return null;
            }
            foreach (var item in consturcs)
            {
                var para = item.GetParameters(); 
                if (para.Any())
                {
                    return item; 
                }
            }
            return null; 
        }

    }
}
