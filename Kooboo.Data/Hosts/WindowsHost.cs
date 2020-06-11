//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
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
        public static HostChange change {
            get
            {
                if (_change == null)
                {
                   if (System.IO.File.Exists(HostFile))
                    {
                        _change = new HostChange() { NoChange = false }; 
                    }
                   else
                    {
                        _change = new HostChange() { NoChange = true }; 
                    }
                }
                return _change; 
            }
            set { _change = value;  }
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
                    else
                    {
                        return false; 
                    }
                }
            }

            return true; 
        }

        /// <summary>
        /// add a new record and insert into host file.
        /// </summary>
        /// <param name="FullDomain"></param>
        /// <param name="IP"></param>
        public static void AddOrUpdate(string FullDomain, string IP)
        {
            if (change.NoChange)
            {
                return; 
            }

            if (string.IsNullOrEmpty(FullDomain) || IsIp(FullDomain))
            {
                return; 
            }

            FullDomain = FullDomain.ToLower();

            var list = GetList();

            foreach (var item in list)
            {
                if (item.Domain == FullDomain)
                {
                    if (item.IpAddress == IP)
                    {
                        // already there. 
                        return;
                    }
                    else
                    {
                        // already there, but different ip... need to update it. 
                        string newline = item.LineString.Replace(item.IpAddress, IP);
                        replacelines(item.LineString, newline);
                        return; 
                    }
                }
            }

            // if there is not a reocrd. we append to the end of the #</kooboo>
            string recordline = IP + " " + FullDomain + "\r\n" + koobooend;
            replacelines(koobooend, recordline);
        
            return;
        }

        /// <summary>
        /// Del a record from hostfile. 
        /// </summary>
        /// <param name="FullDomain"></param>
        public static void Delete(string FullDomain)
        {
            if (change.NoChange)
            {
                return; 
            }

            if (string.IsNullOrEmpty(FullDomain))
            {
                return; 
            }

            FullDomain = FullDomain.ToLower();
            var list = GetList();
            foreach (var item in list)
            {
                if (item.Domain == FullDomain)
                {
                    ///based on different hardwared, there are 3 possibilities. 
                    string fulllinestring = item.LineString + "\r\n";
                    string fulllinestring2 = item.LineString + "\r";
                    string fulllinestring3 = item.LineString + "\n";

                    replacelines(fulllinestring, "");
                    replacelines(fulllinestring2, "");
                    replacelines(fulllinestring3, "");

                }
            }
        }

        public static void RemoveAll()
        {
            if (change.NoChange)
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
            if (change.NoChange)
            {
                return new List<HostRecord>(); 
            }

            lock (_object)
            {
                List<HostRecord> list = new List<HostRecord>();

                string[] hostfilelines = Regex.Split(System.IO.File.ReadAllText(HostFile, Encoding.UTF8), "\r\n|\r|\n");

                bool HasStartTag = false;

                foreach (var line in hostfilelines)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {

                        if (HasStartTag)
                        {
                            HostRecord record = HostRecord.Parse(line);
                            if (record != null)
                            {
                                list.Add(record);
                            }
                        }

                        if (line.Contains(kooboostart))
                        {
                            HasStartTag = true;
                        }


                        if (line.Contains(koobooend))
                        {
                            return list;
                        }
                    }
                }

                if (!HasStartTag)
                {
                    ensureKoobooTag();
                }
                return list;
            }
        }


        /// <summary>
        /// replace a line in the host file. 
        /// </summary>
        /// <param name="oldline"></param>
        /// <param name="newline"></param>
        private static void replacelines(string oldline, string newline)
        {
            lock (_object)
            {
                string alltext = System.IO.File.ReadAllText(HostFile, Encoding.UTF8);
                alltext = alltext.Replace(oldline, newline);
                try
                {
                    System.IO.File.WriteAllText(HostFile, alltext, Encoding.UTF8);
                }
                catch (Exception ex)
                { 

                } 
            }
        }

        /// <summary>
        /// append a line to the host file. 
        /// </summary>
        /// <param name="lines"></param>
        private static void appendline(string lines)
        {
            lock (_object)
            {
                System.IO.File.AppendAllText(HostFile, lines, Encoding.UTF8);
            }
        }

        private static void ensureKoobooTag()
        {
            lock (_object)
            {
                string alltext = System.IO.File.ReadAllText(HostFile, Encoding.UTF8);
                if (alltext.IndexOf(kooboostart) < 0)
                {
                    appendline("\r\n\r\n" + kooboostart + "\r\n" + koobooend);
                }
            }
        }

    }

    public class HostChange

    {
        public bool NoChange { get; set;  }
    }
}
