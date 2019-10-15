using Kooboo.Data.Interface;

namespace Kooboo.Data.Service
{
    public static class UserLoginProtection
    {
        static UserLoginProtection()
        {
            BlockingService = Lib.IOC.Service.GetSingleTon<ILoginBlock>();
        }

        private static ILoginBlock BlockingService { get; set; }

        public static bool CanTryLogin(string userName, string ip)
        {
            if (BlockingService != null)
            {
                if (BlockingService.IsIpBlocked(ip))
                {
                    return false;
                }
                if (BlockingService.IsUserNameBlocked(userName))
                {
                    return false;
                }
            }
            return true;
        }

        public static void AddLoginFail(string userName, string ip)
        {
            if (BlockingService != null)
            {
                BlockingService.AddIpFailure(ip);
                BlockingService.AddUserNameFailure(userName);
            }
        }

        public static void AddLoginOk(string userName, string ip)
        {
            BlockingService?.AddLoginOk(userName, ip);
        }
    }
}