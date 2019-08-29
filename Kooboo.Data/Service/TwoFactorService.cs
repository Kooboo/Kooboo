using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Data.Service
{

    // this is only for the online server...
    public static class TwoFactorService
    {
        static ITwoFactorProvder provider { get; set; }
        static TwoFactorService()
        {
            provider = Lib.IOC.Service.GetSingleTon<ITwoFactorProvder>(false); 
        }

        public static Dictionary<string, string> GetHeaders(Guid UserId)
        {
            if (provider != null)
            {
                return provider.GetHeaders(UserId);
            } 
            return null;
        }

        public static Kooboo.Data.Models.User Validate(HttpRequest request)
        {
            if (provider != null)
            {
                return provider.Validate(request);
            }
            return null;
        }

    }
}
