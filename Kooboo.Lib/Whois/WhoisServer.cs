using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Kooboo.Lib.Helper;

namespace Kooboo.Lib.Whois
{
    public class WhoisServer
    {

        public static WhoisServer instance { get; set; } = new WhoisServer();


        private Dictionary<string, string> whoisServer = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public WhoisServer()
        {
            if (System.IO.File.Exists(this.ServerListFile))
            {
                using (StreamReader reader = new StreamReader(this.ServerListFile))
                {
                    var line = reader.ReadLine();

                    while (line != null)
                    {
                        var index = line.IndexOf(":");

                        if (index > -1)
                        {
                            var tld = line.Substring(0, index).Trim();
                            var whois = line.Substring(index + 1).Trim();

                            whoisServer[tld] = whois;
                        }

                        line = reader.ReadLine();
                    }
                }
            }
        }

        private string _fullFileName;
        public string ServerListFile
        {
            get
            {
                if (_fullFileName == null)
                {
                    var path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppData");
                    if (!Directory.Exists(path)) path = IOHelper.KoobooAppData;
                    string filename = System.IO.Path.Combine(path, "whois.txt");

                    Lib.Helper.IOHelper.EnsureFileDirectoryExists(filename);

                    _fullFileName = filename;
                }
                return _fullFileName;
            }
        }


        public DateTime GetExpires(string domain)
        {
            var res = GetRawRecord(domain);
            if (res == null)
            {
                return default(DateTime);
            }
            return Parser.instance.ReadExpires(res);
        }

        public WhoisRecord GetRecord(string domain)
        {
            var res = GetRawRecord(domain);
            return Parser.instance.ReadWhoisRecord(res, domain);
        }

        public async Task<WhoisRecord> GetRecordAsync(string domain)
        {
            var res = await GetRawRecordAsync(domain);
            return Parser.instance.ReadWhoisRecord(res, domain);
        }

        // must be a root domain. 
        public string GetRawRecord(string rootDomain)
        {
            if (string.IsNullOrEmpty(rootDomain))
            {
                return null;
            }
            var lastdot = rootDomain.LastIndexOf(".");
            if (lastdot > -1)
            {
                var tld = rootDomain.Substring(lastdot + 1);

                tld = tld.ToLower();

                string CMD = rootDomain;

                if (tld == "de")
                {
                    CMD = "-T dn,ace " + rootDomain;
                }
                else if (tld == "dk")
                {
                    CMD = "--show-handles " + rootDomain;
                }
                else if (tld == "jp")
                {
                    CMD = rootDomain + "/e";
                }

                var server = GetWhoisSerer(tld);

                TcpPipeReader reader = new TcpPipeReader();

                return reader.Read(server, CMD);

            }

            return null;

        }

        public async Task<string> GetRawRecordAsync(string rootDomain)
        {
            var lastdot = rootDomain.LastIndexOf(".");
            if (lastdot > -1)
            {
                var tld = rootDomain.Substring(lastdot + 1);

                tld = tld.ToLower();

                string CMD = rootDomain;

                if (tld == "de")
                {
                    CMD = "-T dn,ace " + rootDomain;
                }
                else if (tld == "dk")
                {
                    CMD = "--show-handles " + rootDomain;
                }
                else if (tld == "jp")
                {
                    CMD = rootDomain + "/e";
                }

                var server = await GetWhoisSererAsync(tld);

                TcpPipeReader reader = new TcpPipeReader();

                return await reader.ReadAsync(server, CMD);

            }

            return null;

        }


        public string GetWhoisSerer(string tld)
        {
            if (whoisServer.ContainsKey(tld))
            {
                return whoisServer[tld];
            }

            var whois = GetWhoisServerFromIana(tld);

            if (!string.IsNullOrWhiteSpace(whois))
            {
                var line = tld + ":" + whois;
                System.IO.File.AppendAllLines(this.ServerListFile, new string[] { line });

                this.whoisServer[tld] = whois;

                return whois;
            }

            return null;
        }

        public async Task<string> GetWhoisSererAsync(string tld)
        {
            if (whoisServer.ContainsKey(tld))
            {
                return whoisServer[tld];
            }

            var whois = await GetWhoisServerFromIanaAsync(tld);

            if (!string.IsNullOrWhiteSpace(whois))
            {
                var line = tld + ":" + whois;
                System.IO.File.AppendAllLines(this.ServerListFile, new string[] { line });

                this.whoisServer[tld] = whois;

                return whois;
            }

            return null;
        }

        public string GetWhoisServerFromIana(string tld)
        {
            TcpPipeReader reader = new TcpPipeReader();

            var text = reader.ReadIana(tld);

            if (text != null)
            {
                return Parser.instance.ReadWhoisServer(text);
            }

            return null;
        }

        public async Task<string> GetWhoisServerFromIanaAsync(string tld)
        {
            TcpPipeReader reader = new TcpPipeReader();

            var text = await reader.ReadIanaAsync(tld);

            if (text != null)
            {
                return Parser.instance.ReadWhoisServer(text);
            }

            return null;
        }

    }
}
