using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using System;
using System.Collections.Generic;

namespace Kooboo.Data.Service
{
    // this is only for the online server...
    public static class TwoFactorService
    {
        private static ITwoFactorProvder provider { get; set; }

        static TwoFactorService()
        {
            provider = Lib.IOC.Service.GetSingleTon<ITwoFactorProvder>(false);
        }

        public static Dictionary<string, string> GetHeaders(Guid userId)
        {
            return provider?.GetHeaders(userId);
        }

        public static Kooboo.Data.Models.User Validate(HttpRequest request)
        {
            return provider?.Validate(request);
        }
    }
}