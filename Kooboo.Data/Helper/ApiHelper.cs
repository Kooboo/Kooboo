//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Helper
{
   public static  class ApiHelper
    { 
        public static bool IsOnlineSever(string IP)
        {   
            string url = Data.Helper.AccountUrlHelper.System("VerifyServer");
            url = url += "?IP=" + IP; 
            return Lib.Helper.HttpHelper.Get<bool>(url);  
        }    
    }
}
