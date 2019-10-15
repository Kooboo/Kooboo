//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Models;
using System;
using System.Collections.Generic;

namespace Kooboo.Data.Helper
{
    public static class ServerHelper
    {
        public static List<ReverseDns> PraseReverseDns(string ptrText)
        {
            List<ReverseDns> result = new List<ReverseDns>();
            if (!string.IsNullOrWhiteSpace(ptrText))
            {
                var sep = ";,".ToCharArray();
                var splited = ptrText.Split(sep, StringSplitOptions.RemoveEmptyEntries);

                foreach (var item in splited)
                {
                    if (item.IndexOf(":") != -1)
                    {
                        var index = item.IndexOf(":");
                        var ip = item.Substring(0, index);
                        var domain = item.Substring(index + 1);
                        ReverseDns record = new ReverseDns { IP = ip.Trim(), HostName = domain.Trim() };
                        result.Add(record);
                    }
                }
            }

            return result;
        }

        public static HashSet<string> ParseIps(string ipListText)
        {
            var result = new HashSet<string>();

            if (!string.IsNullOrEmpty(ipListText))
            {
                var sep = ";,".ToCharArray();
                var splited = ipListText.Split(sep, StringSplitOptions.RemoveEmptyEntries);

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