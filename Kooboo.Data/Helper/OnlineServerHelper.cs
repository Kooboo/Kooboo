//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Data.Models;
using System.Configuration;

namespace Kooboo.Data.Helper
{
    public class OnlineServerHelper
    {
        private static string RootUrl = ConfigurationManager.AppSettings.Get("RootUrl");
        private static List<OnlineServer> cacheServers { get; set; }
        private static DateTime NextCheck { get; set; }

        public static List<OnlineServer> AllServers()
        {
            if (NextCheck < DateTime.Now)
            {
                cacheServers = GetAllServers();
                NextCheck = DateTime.Now.AddHours(1);
            }
            return cacheServers;
        }

        public static List<OnlineServer> GetAllServers()
        {
            try
            {
                string serverurl = RootUrl + "Server/all";
                return Lib.Helper.HttpHelper.Get<List<OnlineServer>>(serverurl);
            }
            catch (Exception ex)
            {

            }
            List<OnlineServer> result = new List<OnlineServer>();
            return result;
        }
    }
}
