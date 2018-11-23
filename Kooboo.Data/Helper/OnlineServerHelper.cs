using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Data.Models;

namespace Kooboo.Data.Helper
{
    public class OnlineServerHelper
    {
        private static string RootUrl = "https://162.211.126.186:50005/_api/";
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
                string serverurl = RootUrl + "Server/all?AccessToken=6db401e9-3a9c-9ec0-04af-910b55d8a85b";
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
