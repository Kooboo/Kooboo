using System;
using System.IO;

namespace Kooboo.Data.Log
{
    public class LogWriter
    {
        private string Folder { get; set; }

        public LogWriter(string folderName)
        {
            this.Folder = folderName;
            _date = DateTime.UtcNow.Date;
            _writer = CreateWriter(_date);
        }

        private StreamWriter _writer;
        private DateTime _date;
        private static object _createLock = new object();

        public void Write(string line)
        {
            Writer.WriteLine(line);
        }

        private StreamWriter Writer
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

        private StreamWriter CreateWriter(DateTime utcdate)
        {
            string path = System.IO.Path.Combine(AppSettings.Global.LogPath, this.Folder);

            string filename = System.IO.Path.Combine(path, utcdate.ToString("yy-MM-dd") + ".log");
            Lib.Helper.IOHelper.EnsureFileDirectoryExists(filename);
            var wr = new StreamWriter(new FileStream(filename, FileMode.Append, FileAccess.Write, FileShare.ReadWrite));
            wr.AutoFlush = true;
            return wr;
        }
    }
}