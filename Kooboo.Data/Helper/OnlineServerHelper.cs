//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using Kooboo.Data.Models;
using System.Configuration;

namespace Kooboo.Data.Helper
{
    public class OnlineServerHelper
    {
        private static string RootUrl = ConfigurationManager.AppSettings.Get("RootUrl");
        private static List<OnlineServer> CacheServers { get; set; }
        private static DateTime NextCheck { get; set; }

        public static List<OnlineServer> AllServers()
        {
            if (NextCheck < DateTime.Now)
            {
                CacheServers = GetAllServers();
                NextCheck = DateTime.Now.AddHours(1);
            }
            return CacheServers;
        }

        public static List<OnlineServer> GetAllServers()
        {
            try
            {
                string serverurl = RootUrl + "Server/all";
                return Lib.Helper.HttpHelper.Get<List<OnlineServer>>(serverurl);
            }
            catch (Exception)
            {
                // ignored
            }

            List<OnlineServer> result = new List<OnlineServer>();
            return result;
        }
    }
}
