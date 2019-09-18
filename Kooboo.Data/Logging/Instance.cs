//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic; 

namespace Kooboo.Data.Log
{ 
    public static class Instance
    {

        static Instance()
        {
            Cache = new Dictionary<string, LogWriter>(StringComparer.OrdinalIgnoreCase);
        }

        private static object _locker = new object();

        private static LogWriter _http; 
        public static LogWriter Http
        {
            get
            {
                if (_http == null)
                {
                    lock(_locker)
                    {
                        if (_http == null)
                        {
                            _http = new LogWriter("http"); 
                        }
                    }
                }
                return _http; 
            }
        }


        private static LogWriter _exception; 

        public static LogWriter Exception
        {
            get
            {
                if (_exception== null)
                {
                    lock(_locker)
                    {
                        if (_exception== null)
                        {
                            _exception = new LogWriter("exception"); 
                        }
                    }
                }
                return _exception; 
            }
        }

        private static LogWriter _trace;
        public static LogWriter Trace
        {
            get
            {
                if (_trace == null)
                {
                    lock (_locker)
                    {
                        if (_trace == null)
                        {
                            _trace = new LogWriter("Trace");
                        }
                    }
                }
                return _trace;
            }
        }


        private static Dictionary<string, LogWriter> Cache { get; set; }

        public static void WriteLine(string LogFolderName, string message)
        {
           if (Cache.TryGetValue(LogFolderName, out LogWriter writer))
            {
                writer.Write(message); 
            }
           else
            {
                var newwriter = new LogWriter(LogFolderName);
                Cache[LogFolderName] = newwriter;
                newwriter.Write(message); 
            }
        }
    }
 
}
