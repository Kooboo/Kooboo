//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.IO;

namespace Kooboo.Logging
{
    public class SimpleDateRollingLogWriter : IDisposable
    {
        private Func<DateTime, string> _getPath;

        private StreamWriter _writer;
        private DateTime _date;
        private object _createLock = new object();
        private object _writeLock = new object();

        public SimpleDateRollingLogWriter(Func<DateTime, string> getPath)
        {
            _getPath = getPath;
        }

        public void Log(string line)
        {
            Write($"{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff")}Z  {line}");
        }

        public void Write(string line)
        {
            if (_date < DateTime.UtcNow.Date)
            {
                if (_writer != null)
                {
                    // 保证最新的日志日期
                    _writer.Dispose();
                    _writer = null;
                }
            }

            if (_writer == null)
            {
                lock (_createLock)
                {
                    if (_writer == null)
                    {
                        _date = DateTime.UtcNow.Date;
                        CreateWriter();
                    }
                }
            }

            lock (_writeLock)
            {
                _writer.WriteLine(line);
                _writer.Flush();
            }
        }

        public void Dispose()
        {
            _writer.Dispose();
        }

        private void CreateWriter()
        {
            var path = _getPath(_date);
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            _writer = new StreamWriter(new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite));
        }
    }
}
