using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Kooboo.Lib.Logging
{ 
     
    public class SimpleLogWriter
    {
        private string Folder { get; set; }

        public SimpleLogWriter(string FolderName)
        {
            this.Folder = FolderName;
            _date = DateTime.UtcNow.Date;
            _writer = CreateWriter(_date);
        }

        private StreamWriter _writer;
        private DateTime _date;
        private object _createLock = new object();
        
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
            string basefolder = AppDomain.CurrentDomain.BaseDirectory;
            basefolder = System.IO.Path.Combine(basefolder, "LibLog"); 
             
            string path = System.IO.Path.Combine(basefolder, this.Folder);

            string filename = System.IO.Path.Combine(path, utcdate.ToString("yy-MM-dd") + ".log");
            Lib.Helper.IOHelper.EnsureFileDirectoryExists(filename);
            var wr = new StreamWriter(new FileStream(filename, FileMode.Append, FileAccess.Write, FileShare.ReadWrite));
            wr.AutoFlush = true;
            return wr;
        }

    }







}
