//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;

namespace Kooboo.Mail.Smtp
{
    public static class Log
    {
        #region Info

        private static bool EnableDebugInfo = System.Configuration.ConfigurationManager.AppSettings["smtpLog"] == "true";
        private static TimeSpan TimerRecycleInterval = TimeSpan.FromDays(1);
        private static ConcurrentDictionary<string, StreamWriter> _writers = new ConcurrentDictionary<string, StreamWriter>();
        private static Timer _timer = new Timer(TimerCallback, null, TimerRecycleInterval, TimerRecycleInterval);
        private static object _infoWriterCreationLock = new object();

        public static void LogInfo(string message)
        {
            try
            {
                if (!EnableDebugInfo)
                    return;

                var writer = GetWriter();
                writer.WriteLine(DateTime.UtcNow.ToString("MM-dd HH:mm:ss.fff") + " " + message);
            }
            catch
            {
                // ignored
            }
        }

        private static void TimerCallback(object s)
        {
            foreach (var key in _writers.Keys)
            {
                var writerDate = DateTime.Parse(key.Substring(key.IndexOf('-') + 1));
                if (DateTime.UtcNow.Subtract(writerDate).TotalDays > 2)
                {
                    _writers.TryRemove(key, out var writer);
                    writer.BaseStream.Dispose();
                    writer.Dispose();
                }
            }
        }

        private static StreamWriter GetWriter()
        {
            // eg. Receive-2014-08-15.log
            var fileName = "Info-" + DateTime.UtcNow.ToString("yyyy-MM-dd");
            if (_writers.TryGetValue(fileName, out var writer))
                return writer;

            lock (_infoWriterCreationLock)
            {
                if (_writers.TryGetValue(fileName, out writer))
                    return writer;

                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "smtpserver", fileName + ".log");
                var fileStream = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Read);
                writer = new StreamWriter(fileStream);
                writer.AutoFlush = true;

                _writers.TryAdd(fileName, writer);
                return writer;
            }
        }

        #endregion Info

        #region Exception

        private static object _exceptionWriteLock = new object();

        /// <summary>
        /// append the log record to the error.txt file.
        /// 
        /// </summary>
        /// <param name="ex"></param>
        public static void LogError(Exception ex)
        {
            lock (_exceptionWriteLock)
            {
                string logfolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "smtpserver");
                string logFile = System.IO.Path.Combine(logfolder, System.DateTime.Now.Year.ToString() + System.DateTime.Now.Month.ToString() + System.DateTime.Now.Day.ToString() + "r.txt");

                string directory = System.IO.Path.GetDirectoryName(logFile);
                if (!System.IO.Directory.Exists(directory))
                {
                    System.IO.Directory.CreateDirectory(directory);
                }

                if (!System.IO.File.Exists(logFile))
                {
                    using (StreamWriter w = new StreamWriter(logFile))
                    {
                        OutputException(w, ex);
                    }
                }
                else
                {
                    using (StreamWriter w = new StreamWriter(logFile, true))
                    {
                        OutputException(w, ex);
                        // Close the writer and underlying file.
                    }
                }
            }
            //TODO: implements Email notification.
        }

        private static void OutputException(StreamWriter w, Exception ex)
        {
            var output = new System.Text.StringBuilder();
            output
                .Append(DateTime.UtcNow.ToString() + ": ")
                .AppendLine(ex.Message)
                .AppendLine(ex.StackTrace)
                .ToString();
            Console.WriteLine(output);
            w.WriteLine(output);
        }

        #endregion Exception
    }
}