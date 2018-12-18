//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Logging
{
    public class LogProvider
    {
        private static object _lock = new object();
        private static Dictionary<string, ILogger> _loggers = new Dictionary<string, ILogger>();

        static LogProvider()
        {
            var str = System.Configuration.ConfigurationManager.AppSettings["LogLevel"];
            if (!String.IsNullOrEmpty(str))
            {
                LogLevel level;
                if (Enum.TryParse<LogLevel>(str, out level))
                {
                    Level = level;
                }
            }
        }

        public static LogLevel Level { get; set; } = LogLevel.Information;

        public static ILogger GetLogger(string group, string name)
        {
            var key = group + "_" + name;
            
            ILogger result;
            if (_loggers.TryGetValue(key, out result))
                return result;

            lock (_lock)
            {
                if (_loggers.TryGetValue(key, out result))
                    return result;

                result = new DefaultLogger(group, name);
                _loggers.Add(key, result);
                return result;
            }
        }
    }
}
