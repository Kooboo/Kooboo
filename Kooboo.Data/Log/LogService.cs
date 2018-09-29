//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.IO;
using System.Text;

namespace Kooboo.Data.Log
{

    public static class SmtpLog
    {
        static SmtpLog()
        {
            _date = DateTime.UtcNow.Date;
            _writer = CreateWriter(_date);
            sb = new StringBuilder();
        }

        private static StreamWriter _writer;
        private static DateTime _date;
        private static object _createLock = new object();
        private static object _writeLock = new object();

        private static StringBuilder sb { get; set; }

        public static void Write(string line)
        {
            return; 

            if (!string.IsNullOrWhiteSpace(line))
            {
                Writer.WriteLine(line);
                Writer.Flush();
            }

            //sb.AppendLine(line);

            //if (sb.Length > 2000)
            //{
            //    string content = sb.ToString();
            //    sb.Clear();

            //    Writer.Write(content); 
            //    Writer.Flush();
            //}
        }

        public static StreamWriter Writer
        {
            get
            {
                if (_date < DateTime.UtcNow.Date)
                {
                    lock (_createLock)
                    {
                        if (_date < DateTime.UtcNow.Date)
                        {
                            _date = DateTime.UtcNow.Date;
                            _writer = CreateWriter(_date);
                        }
                    }
                }

                if (_writer == null)
                {
                    lock (_createLock)
                    {
                        if (_writer == null)
                        {
                            _date = DateTime.UtcNow.Date;
                            _writer = CreateWriter(_date);
                        }
                    }
                }

                return _writer;
            }

        }

        private static StreamWriter CreateWriter(DateTime utcdate)
        {
            string path = System.IO.Path.Combine(AppSettings.Global.LogPath, "smtp");

            string filename = System.IO.Path.Combine(path, utcdate.ToString("yy-MM-dd") + ".log");
            Lib.Helper.IOHelper.EnsureFileDirectoryExists(filename);
            return new StreamWriter(new FileStream(filename, FileMode.Append, FileAccess.Write, FileShare.ReadWrite));
        }

    }

    public static class HttpLog
    {
        static HttpLog()
        {
            _date = DateTime.UtcNow.Date;
            _writer = CreateWriter(_date);
            sb = new StringBuilder();
        }

        private static StreamWriter _writer;
        private static DateTime _date;
        private static object _createLock = new object();
        private static object _writeLock = new object();

        private static StringBuilder sb { get; set; }

        public static void Write(string line)
        {
            lock (_writeLock)
            {
                sb.AppendLine(line);
                if (sb.Length > 2000)
                {
                    string content = sb.ToString();
                    Writer.WriteAsync(content).Wait();
                    sb.Clear();
                }
            }
        }

        public static StreamWriter Writer
        {
            get
            {
                if (_date < DateTime.UtcNow.Date)
                {
                    lock (_createLock)
                    {
                        if (_date < DateTime.UtcNow.Date)
                        {
                            _date = DateTime.UtcNow.Date;
                            _writer = CreateWriter(_date);
                        }
                    }
                }

                if (_writer == null)
                {
                    lock (_createLock)
                    {
                        if (_writer == null)
                        {
                            _date = DateTime.UtcNow.Date;
                            _writer = CreateWriter(_date);
                        }
                    }
                }

                return _writer;
            }

        }

        private static StreamWriter CreateWriter(DateTime utcdate)
        {
            string path = System.IO.Path.Combine(AppSettings.Global.LogPath, "http");

            string filename = System.IO.Path.Combine(path, utcdate.ToString("yy-MM-dd") + ".log");
            Lib.Helper.IOHelper.EnsureFileDirectoryExists(filename);
            var wr = new StreamWriter(new FileStream(filename, FileMode.Append, FileAccess.Write, FileShare.ReadWrite));
            wr.AutoFlush = true;
            return wr;
        }

    }

    public static class ExceptionLog
    {
        static ExceptionLog()
        {
            _date = DateTime.UtcNow.Date;
            _writer = CreateWriter(_date);
            sb = new StringBuilder();
        }

        private static StreamWriter _writer;
        private static DateTime _date;
        private static object _createLock = new object();
        private static object _writeLock = new object();

        private static StringBuilder sb { get; set; }

        public static void Write(string line)
        {
            lock (_writeLock)
            {
                sb.AppendLine(line);
                if (sb.Length > 2000)
                {
                    string content = sb.ToString();
                    Writer.WriteAsync(content).Wait();
                    sb.Clear();
                }
            }
        }

        public static StreamWriter Writer
        {
            get
            {
                if (_date < DateTime.UtcNow.Date)
                {
                    lock (_createLock)
                    {
                        if (_date < DateTime.UtcNow.Date)
                        {
                            _date = DateTime.UtcNow.Date;
                            _writer = CreateWriter(_date);
                        }
                    }
                }

                if (_writer == null)
                {
                    lock (_createLock)
                    {
                        if (_writer == null)
                        {
                            _date = DateTime.UtcNow.Date;
                            _writer = CreateWriter(_date);
                        }
                    }
                }

                return _writer;
            }

        }

        private static StreamWriter CreateWriter(DateTime utcdate)
        {
            string path = System.IO.Path.Combine(AppSettings.Global.LogPath, "error");

            string filename = System.IO.Path.Combine(path, utcdate.ToString("yy-MM-dd") + ".log");
            Lib.Helper.IOHelper.EnsureFileDirectoryExists(filename);
            var wr = new StreamWriter(new FileStream(filename, FileMode.Append, FileAccess.Write, FileShare.ReadWrite));
            wr.AutoFlush = true;
            return wr;
        }

    }

}
