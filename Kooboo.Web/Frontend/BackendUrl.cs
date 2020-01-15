using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Web.Frontend
{
   public static class BackendUrl
    {

        public static bool IsBackendUrl(string relativeUrl)
        {  
            //string relativeUrl = Relativeurl.ToLower(); 
            //if (relativeUrl.StartsWith("/_api/") ||
            //    relativeUrl.StartsWith("/_admin/") ||
            //    relativeUrl.StartsWith("/_spa/") ||
            //     relativeUrl.StartsWith("/_thumbnail/") ||
            //     relativeUrl.StartsWith("/.well-known/acme-challenge")
            //    )
            //{
            //    return true;
            //}
            return false; 
        }
    }
}
