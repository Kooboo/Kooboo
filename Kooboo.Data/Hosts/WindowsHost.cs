//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Kooboo.Data.Hosts
{
    /// <summary>
    /// Change directly into host file for DNS records.
    /// The host file can have some special block that mark with like #<kooboo> recordss.#</kooboo>
    /// </summary>
    public static class WindowsHost
    {
        private static HostChange _change;

        public static HostChange Change
        {
            get =>
                _change ?? (_change = System.IO.File.Exists(HostFile)
                    ? new HostChange { NoChange = false }
                    : new HostChange { NoChange = true });
            set => _change = value;
        }

        private static object _object = new object();

        private static string kooboostart = "#<kooboo>";
        private static string koobooend = "#</kooboo>";

        // split lines var result = Regex.Split(text, "\r\n|\r|\n");
        private static string _hostfile;

        public static string HostFile
        {
            get
            {
                if (string.IsNullOrEmpty(_hostfile))
                {
                    string systemfolder = Environment.GetEnvironmentVariable("SystemRoot");
                    if (!string.IsNullOrWhiteSpace(systemfolder))
                    {
                        _hostfile = System.IO.Path.Combine(systemfolder, "system32", "drivers", "etc", "hosts");
                    }
                }
                return _hostfile;
            }
        }

        private static bool IsIp(string input)
        {
            int count = input.Length;
            for (int i = 0; i < count; i++)
            {
                var onechar = input[i];
                if (!Lib.Helper.CharHelper.isAsciiDigit(onechar))
                {
                    if (onechar == '.' || onechar == ':')
                    {
                        continue;
                    }

                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// add a new record and insert into host file.
        /// </summary>
        /// <param name="fullDomain"></param>
        /// <param name="ip"></param>
        public static void AddOrUpdate(string fullDomain, string ip)
        {
            if (Change.NoChange)
            {
                return;
            }

            if (string.IsNullOrEmpty(fullDomain) || IsIp(fullDomain))
            {
                return;
            }

            fullDomain = fullDomain.ToLower();

            var list = GetList();

            foreach (var item in list.Where(item => item.Domain == fullDomain))
            {
                if (item.IpAddress == ip)
                {
                    // already there.
                    return;
                }

                // already there, but different ip... need to update it.
                string newline = item.LineString.Replace(item.IpAddress, ip);
                Replacelines(item.LineString, newline);
                return;
            }

            // if there is not a reocrd. we append to the end of the #</kooboo>
            string recordline = ip + " " + fullDomain + "\r\n" + koobooend;
            Replacelines(koobooend, recordline);
        }

        /// <summary>
        /// Del a record from hostfile.
        /// </summary>
        /// <param name="fullDomain"></param>
        public static void Delete(string fullDomain)
        {
            if (Change.NoChange)
            {
                return;
            }

            if (string.IsNullOrEmpty(fullDomain))
            {
                return;
            }

            fullDomain = fullDomain.ToLower();
            var list = GetList();
            foreach (var item in list)
            {
                if (item.Domain == fullDomain)
                {
                    ///based on different hardwared, there are 3 possibilities.
                    string fulllinestring = item.LineString + "\r\n";
                    string fulllinestring2 = item.LineString + "\r";
                    string fulllinestring3 = item.LineString + "\n";

                    Replacelines(fulllinestring, "");
                    Replacelines(fulllinestring2, "");
                    Replacelines(fulllinestring3, "");
                }
            }
        }

        public static void RemoveAll()
        {
            if (Change.NoChange)
            {
                return;
            }

            var list = GetList();
            foreach (var item in list)
            {
                Delete(item.Domain);
            }
        }

        public static List<HostRecord> GetList()
        {
            if (Change.NoChange)
            {
                return new List<HostRecord>();
            }

            lock (_object)
            {
                List<HostRecord> list = new List<HostRecord>();

                string[] hostfilelines = Regex.Split(System.IO.File.ReadAllText(HostFile, Encoding.UTF8), "\r\n|\r|\n");

                bool hasStartTag = false;

                foreach (var line in hostfilelines)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        if (hasStartTag)
                        {
                            HostRecord record = HostRecord.Parse(line);
                            if (record != null)
                            {
                                list.Add(record);
                            }
                        }

                        if (line.Contains(kooboostart))
                        {
                            hasStartTag = true;
                        }

                        if (line.Contains(koobooend))
                        {
                            return list;
                        }
                    }
                }

                if (!hasStartTag)
                {
                    EnsureKoobooTag();
                }
                return list;
            }
        }

        /// <summary>
        /// replace a line in the host file.
        /// </summary>
        /// <param name="oldline"></param>
        /// <param name="newline"></param>
        private static void Replacelines(string oldline, string newline)
        {
            lock (_object)
            {
                string alltext = System.IO.File.ReadAllText(HostFile, Encoding.UTF8);
                alltext = alltext.Replace(oldline, newline);
                System.IO.File.WriteAllText(HostFile, alltext, Encoding.UTF8);
            }
        }

        /// <summary>
        /// append a line to the host file.
        /// </summary>
        /// <param name="lines"></param>
        private static void Appendline(string lines)
        {
            lock (_object)
            {
                System.IO.File.AppendAllText(HostFile, lines, Encoding.UTF8);
            }
        }

        private static void EnsureKoobooTag()
        {
            lock (_object)
            {
                string alltext = System.IO.File.ReadAllText(HostFile, Encoding.UTF8);
                if (alltext.IndexOf(kooboostart) < 0)
                {
                    Appendline("\r\n\r\n" + kooboostart + "\r\n" + koobooend);
                }
            }
        }
    }

    public class HostChange

    {
        public bool NoChange { get; set; }
    }
}