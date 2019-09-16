//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Helper
{
 public static   class AccountUrlHelper
    {
        public static string AccountBase = AppSettings.AccountApiUrl; 
         
        public static string User(string relativeUrl)
        { 
            return AccountBase + "/account/user" + ensureRelative(relativeUrl);
        }

        public static string UserPublish(string RelativeUrl)
        {
            return AccountBase + "/account/userpublish" + ensureRelative(RelativeUrl); 
        }

        public static string Org(string relativeUrl)
        {   
            return AccountBase + "/account/organization" + ensureRelative(relativeUrl);
        }

        public static string Cluster(string relativeUrl)
        {
            return AccountBase + "/account/cluster" + ensureRelative(relativeUrl);
        }

        public static string Domain(string relativeUrl)
        {  
            return AccountBase + "/account/domain" + ensureRelative(relativeUrl);
        } 

        public static string System(string relativeUrl)
        {
            return AccountBase + "/account/system" + ensureRelative(relativeUrl);
        }

        public static string Template(string relativeUrl)
        {
            return AccountBase + "/account/template" + ensureRelative(relativeUrl);
        }

        public static string Certificate(string relativeUrl)
        {
            return AccountBase + "/account/certificate" + ensureRelative(relativeUrl); 
        }

        public static string Ssl(string relativeUrl)
        {
            return AccountBase + "/account/ssl" + ensureRelative(relativeUrl); 
        } 

        public static string OnlineDataCenter(string relativeUrl)
        {
            return AccountBase + "/account/OnlineDataCenter" + ensureRelative(relativeUrl); 
        }


        private static string ensureRelative(string relativeurl)
        {
            if (string.IsNullOrEmpty(relativeurl))
            {
                return null; 
            }
            if (!relativeurl.StartsWith("/"))
            { relativeurl = "/" + relativeurl; }

            return relativeurl; 
        }
    }
}
