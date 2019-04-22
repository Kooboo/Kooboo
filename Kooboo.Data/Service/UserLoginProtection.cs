using Kooboo.Data.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Data.Service
{
    public static class UserLoginProtection
    {
        static UserLoginProtection()
        {
            BlockingService = Lib.IOC.Service.GetSingleTon<ILoginBlock>(); 
        }
         
        private static ILoginBlock  BlockingService {get;set;}

        public static bool CanTryLogin(string UserName, string IP)
        {
             if (BlockingService !=null)
            {
                if (BlockingService.IsIpBlocked(IP))
                {
                    return false; 
                }
                if (BlockingService.IsUserNameBlocked(UserName))
                {
                    return false; 
                }
            }
            return true; 
        }

         
        public static void AddLoginFail(string UserName, string IP)
        {
            if (BlockingService !=null)
            {
                BlockingService.AddIpFailure(IP);
                BlockingService.AddUserNameFailure(UserName); 
            }
        }

        public static void AddLoginOK(string UserName, string IP)
        {
            if (BlockingService != null)
            {
                BlockingService.AddLoginOK(UserName, IP);  
            }
        } 
    }
}
