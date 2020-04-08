using System;
using System.Diagnostics;
using System.IO;
namespace Kooboo.Data.Log
{
    public class LogWriter
    {
        private string Folder { get; set; }

        public LogWriter(string FolderName)
        {
            this.Folder = FolderName;
            _date = DateTime.UtcNow.Date;
            _writer = CreateWriter(_date);
        }

        private StreamWriter _writer;
        private DateTime _date;
        private object _createLock = new object();
        private object _writeLock = new object();

        public void Write(string line)
        {
            lock (_writeLock)
            {
                Writer.WriteLine(line);
            }
        }

        public void WriteObj(object JsonObject)
        {
            var text = Lib.Helper.JsonHelper.Serialize(JsonObject);
            Write(text);
        }

        public void WriteException(Exception ex)
        {
            // Output UTC event time
            var builder = new System.Text.StringBuilder()
                .Append(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff")).Append("Z");

            // Output exception
            builder.Append("  ").Append(ex.ToString());
             
            var st = new StackTrace(ex, true);
            // Get the top stack frame
            var frame = st.GetFrame(0);
            // Get the line number from the stack frame
            var line = frame.GetFileLineNumber();

            // Output line number
            builder.Append(" line number: ").Append(line.ToString());

            // Exception all has a big stack trace, add a line for easier human reading
            builder.AppendLine();
             
            Write(builder.ToString());
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




