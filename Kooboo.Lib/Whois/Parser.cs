using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Lib.Whois
{
    public class Parser
    {

        public static Parser instance { get; set; } = new Parser();

        public string ReadWhoisServer(string responseText)
        {
            string[] lines = responseText.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            foreach (var item in lines)
            {
                string text = item.ToLower().Trim();

                if (text.StartsWith("whois"))
                {
                    var index = text.IndexOf(":");

                    if (index > -1)
                    {
                        return text.Substring(index + 1).Trim();
                    }
                }
            }

            return null;
        }


        public WhoisRecord ReadWhoisRecord(string RawResponse, string queryHost)
        {
            if (RawResponse == null)
            {
                return null;
            }

            // To Dictionary list in our case easier/ 
            RawResponse = RawResponse.ToLower();

            List<NameValue> values = ToValue(RawResponse);

            WhoisRecord record = new WhoisRecord();

            record.NameServers = readNameServer(values, RawResponse, queryHost);

            record.Expiration = readExpires(values, RawResponse);

            return record;
        }

        public DateTime ReadExpires(string rawResponse)
        {
            if (rawResponse == null)
            {
                return default(DateTime);
            }

            // To Dictionary list in our case easier/ 
            rawResponse = rawResponse.ToLower();

            List<NameValue> values = ToValue(rawResponse);
            return readExpires(values, rawResponse);
        }

        private static List<NameValue> ToValue(string RawResponse)
        {
            List<NameValue> values = new List<NameValue>();

            string[] lines = RawResponse.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            foreach (var item in lines)
            {
                if (item.Length > 300)
                {
                    continue;
                }

                int index = item.IndexOf(":");

                if (index > -1)
                {
                    var name = item.Substring(0, index);
                    var value = item.Substring(index + 1);

                    if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(value))
                    {

                        values.Add(new NameValue() { name = name.Trim(), value = value.Trim() });
                    }
                }
            }

            return values;
        }

        private DateTime readExpires(List<NameValue> values, string orginalText)
        {
            List<string> keys = new List<string>();
            keys.Add("Registry Expiry Date".ToLower());
            keys.Add("expiry");
            keys.Add("expires");
            keys.Add("expiration");
            keys.Add("expire");

            foreach (var key in keys)
            {
                var items = values.FindAll(o => o.name == key);

                if (items != null && items.Any())
                {
                    foreach (var item in items)
                    {
                        var date = ParseDate(item.value);
                        if (date != default(DateTime))
                        {
                            return date;
                        }

                    }
                }

                items = values.FindAll(o => o.name.Contains(key));

                if (items != null && items.Any())
                {
                    foreach (var item in items)
                    {
                        var date = ParseDate(item.value);
                        if (date != default(DateTime))
                        {
                            return date;
                        }
                    }
                }

            }

            //not found, can be like: 
            //    Record expires on 2023-11-06 01:29:50 (UTC+8)
            // Record created on 2005 - 11 - 06 01:29:50(UTC + 8)

            string[] lines = orginalText.Split('\n', StringSplitOptions.RemoveEmptyEntries);


            foreach (var item in lines)
            {

                int index = item.IndexOf(" on ");

                if (index > -1)
                {
                    var name = item.Substring(0, index);

                    bool NameMatch = false;

                    foreach (var key in keys)
                    {
                        if (name.Contains(key))
                        {
                            NameMatch = true;
                            break;
                        }
                    }
                    if (NameMatch)
                    {
                        var value = item.Substring(index + 4).Trim();

                        var date = ParseDate(value);
                        if (date != default(DateTime))
                        {
                            return date;
                        }
                    }
                }
            }


            return default(DateTime);
        }

        private DateTime ParseDate(string dateString)
        {
            if (string.IsNullOrEmpty(dateString))
            {
                return default(DateTime);
            }
            dateString = dateString.Trim();
            int index = dateString.IndexOf("(");
            if (index > -1)
            {
                dateString = dateString.Substring(0, index);
            }

            if (DateTime.TryParse(dateString, out var date))
            {
                return date;
            }

            return default(DateTime);

        }


        private List<string> readNameServer(List<NameValue> values, string orginalText, string queryHost)
        {
            if (queryHost != null)
            {
                queryHost = queryHost.ToLower();
            }

            // rule one. contains "name server"; 
            var items = values.FindAll(o => o.name == "name server");
            if (items != null && items.Any())
            {
                var names = items.Select(o => o.value).ToList();
                if (IsDomain(names, queryHost))
                {
                    return names;
                }
            }

            // rule two nserver. 
            items = values.FindAll(o => o.name == "nserver");
            if (items != null && items.Any())
            {
                var names = items.Select(o => o.value).ToList();
                if (IsDomain(names, queryHost))
                {
                    return names;
                }
            }


            items = values.FindAll(o => o.name == "name servers");
            if (items != null && items.Any())
            {
                var names = items.Select(o => o.value).ToList();
                if (IsDomain(names, queryHost))
                {
                    return names;
                }
            }

            items = values.FindAll(o => o.name == "nservers");
            if (items != null && items.Any())
            {
                var names = items.Select(o => o.value).ToList();
                if (IsDomain(names, queryHost))
                {
                    return names;
                }
            }


            items = values.FindAll(o => o.name == "host name");
            if (items != null && items.Any())
            {
                var names = items.Select(o => o.value).ToList();
                if (IsDomain(names, queryHost))
                {
                    return names;
                }
            }

            items = values.FindAll(o => o.name == "hostname");
            if (items != null && items.Any())
            {
                var names = items.Select(o => o.value).ToList();
                if (IsDomain(names, queryHost))
                {
                    return names;
                }
            }

            items = values.FindAll(o => o.name.Contains("name") && o.name.Contains("server") && o.name.Length < 15);
            if (items != null && items.Any())
            {
                var names = items.Select(o => o.value).ToList();
                if (IsDomain(names, queryHost))
                {
                    return names;
                }
            }

            items = values.FindAll(o => o.name.Contains("ns") && o.name.Length < 5);
            if (items != null && items.Any())
            {
                var names = items.Select(o => o.value).ToList();
                if (IsDomain(names, queryHost))
                {
                    return names;
                }
            }

            items = values.FindAll(o => o.name.Contains("host") && o.name.Contains("name") && o.name.Length < 15);
            if (items != null && items.Any())
            {
                var names = items.Select(o => o.value).ToList();
                if (IsDomain(names, queryHost))
                {
                    return names;
                }
            }


            items = values.FindAll(o => o.name.Contains("domain") && o.name.Contains("server") && o.name.Length < 35);
            if (items != null && items.Any())
            {
                var names = items.Select(o => o.value).ToList();
                if (IsDomain(names, queryHost))
                {
                    return names;
                }
            }


            //  jprs database provides information on network administration.its use is    ]
            //[restricted to network administration purposes. for further information,     ]
            //[use 'whois -h whois.jprs.jp help'.to suppress japanese output, add'/e']
            //[at the end of command, e.g. 'whois -h whois.jprs.jp xxx/e'.                 ]

            //domain information:
            //a. [domain name]                google.co.jp
            //g. [organization]               google japan g.k.
            //l. [organization type]          gk
            //m. [administrative contact]     yn47525jp
            //n. [technical contact]          sh36113jp
            //p. [name server]                ns1.google.com
            //p. [name server]                ns2.google.com
            //p. [name server]                ns3.google.com


            string[] lines = orginalText.Split('\n', StringSplitOptions.RemoveEmptyEntries);


            List<string> dnsServers = new List<string>();

            var sep = " ,][;    ".ToCharArray();

            foreach (var item in lines)
            {
                string[] parts = item.Split(sep, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Any())
                {
                    var list = parts.ToList();

                    for (int i = 0; i < list.Count(); i++)
                    {
                        var parti = list[i].Trim();
                        if (parti != null && parti != queryHost && Kooboo.Lib.Domain.DomainService.IsValidDomain(parti))
                        {
                            for (int j = 0; j < i; j++)
                            {
                                var partj = list[j];

                                if (partj != null && partj.Contains("name") || partj.Contains("server") || partj.Contains("host"))
                                {
                                    dnsServers.Add(parti);
                                    break;
                                }
                            }
                        }
                    }
                }

            }

            if (dnsServers.Count() >= 2)
            {
                return dnsServers;
            }


            List<string> servers = new List<string>();


            bool InValueState = false;

            foreach (var item in lines)
            {

                if (string.IsNullOrWhiteSpace(item) || item.Length > 300)
                {
                    continue;
                }

                if (InValueState)
                {
                    var value = item.Trim();
                    if (Kooboo.Lib.Domain.DomainService.IsValidDomain(value))
                    {
                        servers.Add(value);
                        continue;
                    }
                    else
                    {
                        return servers;
                    }
                }

                int index = item.IndexOf(":");

                if (index > -1)
                {
                    var name = item.Substring(0, index);

                    name = name.Replace(" ", "");
                    if (name == "nameserver" || name == "nameservers" || name == "nserver" || name == "nservers")
                    {
                        InValueState = true;
                    }
                    //Domain servers in listed order 
                    //Domain nameservers:
                    // usc2.akam.net

                    else if (name.Contains("domain") && name.Contains("server") && name.Contains("listedorder"))
                    {
                        InValueState = true;
                    }

                    else if (name.Contains("domain") && name.Contains("server"))
                    {
                        InValueState = true;
                    }
                }
            }

            return servers;
        }

        private bool IsDomain(List<string> list, string queryHost)
        {
            foreach (var item in list)
            {
                if (!Lib.Domain.DomainService.IsValidDomain(item))
                {
                    return false;
                }

                if (queryHost == item)
                {
                    return false;
                }
            }
            return true;
        }

    }

    public struct NameValue
    {
        public string name { get; set; }

        public string value { get; set; }
    }
}
