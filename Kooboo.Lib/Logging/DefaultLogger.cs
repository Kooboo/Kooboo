//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;

namespace Kooboo.Logging
{
    public class DefaultLogger : ILogger
    {
        private SimpleDateRollingLogWriter _writer;

        public DefaultLogger(string group, string name)
        {
            Level = LogProvider.Level;

            _writer = new SimpleDateRollingLogWriter(d =>
                System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", group, $"{name}-{d.ToString("yyyy-MM-dd")}.txt")
            );
        }

        public LogLevel Level { get; set; }

        public void LogDebug(string message)
        {
            if (Level > LogLevel.Debug)
                return;

            DoLog(LogLevel.Debug, message);
        }

        public void LogInformation(string message)
        {
            if (Level > LogLevel.Information)
                return;

            DoLog(LogLevel.Information, message);
        }

        public void LogWarning(string message)
        {
            if (Level > LogLevel.Warning)
                return;

            DoLog(LogLevel.Warning, message);
        }

        public void LogError(Exception exception, string message)
        {
            if (Level > LogLevel.Error)
                return;

            DoLog(LogLevel.Error, exception, message);
        }

        public void LogCritical(Exception exception, string message)
        {
            DoLog(LogLevel.Critical, exception, message);
        }

        private void DoLog(LogLevel level, Exception exception, string message)
        {
            if (exception == null)
            {
                DoLog(level, message);
            }
            else
            {
                _writer.Log($"[{level}] {message}\r\n{exception.Message}\r\n{exception.StackTrace}");
            }
        }

        private void DoLog(LogLevel level, string message)
        {
            _writer.Log($"[{level}] {message}");
        }
    }
}
