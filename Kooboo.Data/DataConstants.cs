//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.IO;
using System.Text;
using Kooboo.Lib;
using Kooboo.Extensions;

namespace Kooboo
{
    public static class DataConstants
    {
  
        public const string KoobooGlobalDb = "Global";
           
        public const int DefaultPort = 80;

        public static Encoding DefaultEncoding = Encoding.UTF8;
        
        public const string SiteId = "SiteId";
  
        public const string UserApiSessionKey = "_kooboo_api_user";
        public const string UserServerIp = "KoobooServerIp"; 
         
        public const string DefaultDiagnosticsGroupName = "BuiltIn";
          
        public const string DefaultUserName = "___KoobooUserDefaultUser";
         
        public const string RootPathName = "root"; 

        public const string Default403Page = "/_Admin/Error/403";
        public const string Default404Page = "/_Admin/Error/404";
        public const string Default500Page = "/_Admin/Error/500";
        public const string Default407Page = "/_Admin/Error/407";
        public const string Default402Page = "/_Admin/Error/402"; //continue download limitation..
        public const string DefaultError = "/_admin/error/default";


    }
}
