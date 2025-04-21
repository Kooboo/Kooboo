using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Lib.Domain
{
    public static class KoobooDomain
    {
        public static bool UseDashForSubDomain(string rootOrFullDomain)
        {
            if (rootOrFullDomain == null)
            {
                return false;
            }
            rootOrFullDomain = rootOrFullDomain.ToLower();
            if (rootOrFullDomain.EndsWith("kooboo.cn") || rootOrFullDomain.EndsWith("kooboo.io") || rootOrFullDomain.EndsWith("kooboo.dev"))
            {
                return true;
            }
            return false;
        }


    }
}
