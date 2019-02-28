//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Helper
{
    public static class ServerHelper
    {
        public static List<ReverseDns> PraseReverseDns(string PTRText)
        {
            List<ReverseDns> result = new List<ReverseDns>();
            if (!string.IsNullOrWhiteSpace(PTRText))
            {
                var sep = ";,".ToCharArray();
                var splited = PTRText.Split(sep, StringSplitOptions.RemoveEmptyEntries);

                foreach (var item in splited)
                {
                    if (item.IndexOf(":") != -1)
                    {
                        var index = item.IndexOf(":");
                        var ip = item.Substring(0, index);
                        var domain = item.Substring(index + 1);
                        ReverseDns record = new ReverseDns() { IP = ip.Trim(), HostName = domain.Trim() };
                        result.Add(record);
                    }
                }
            }

            return result;

        }

        public static HashSet<string> ParseIps(string IPListText)
        {
            HashSet<String> result = new HashSet<string>();

            if (!string.IsNullOrEmpty(IPListText))
            {
                var sep = ";,".ToCharArray();
                var splited = IPListText.Split(sep, StringSplitOptions.RemoveEmptyEntries);

                foreach (var item in splited)
                {
                    System.Net.IPAddress tempip;
                    if (System.Net.IPAddress.TryParse(item.Trim(), out tempip))
                    {
                        result.Add(item.Trim());
                    }
                }
            }
            return result;
        }
                                       

    }
}
