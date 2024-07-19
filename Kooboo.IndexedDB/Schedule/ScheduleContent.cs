//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.IO;
using Kooboo.IndexedDB.ByteConverter;

namespace Kooboo.IndexedDB.Schedule
{
    public class ScheduleContent<TValue>
    {
        private IByteConverter<TValue> ValueConverter;

        private FileStream _filestream;

        private object _object = new object();

        private ISchedule _schedule;

        internal string FileName { get; set; }

        internal string FullFileName { get; set; }

        private int DayInt { get; set; }

        public ScheduleContent(int DayInt, ISchedule schedule)
        {
            this._schedule = schedule;

            this.FileName = FileNameGenerator.GetContentFileName(DayInt);
            this.DayInt = DayInt;
            this.FullFileName = FileNameGenerator.GetContentFullFileName(DayInt, schedule);

            if (!File.Exists(this.FullFileName))
            {
                File.WriteAllText(this.FullFileName, "schedule content file, do not modify\r\n");
            }

            this.ValueConverter = ObjectContainer.GetConverter<TValue>();

        }


        /// <summary>
        /// Add a new record and return the block position. 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public long Add(TValue input)
        {
            byte[] contentbytes = this.ValueConverter.ToByte(input);
            int contentbytelen = contentbytes.Length;

            lock (_object)
            {
                Int64 currentposition = Stream.Length;

                Stream.Position = currentposition;
                Stream.Write(BitConverter.GetBytes(contentbytelen), 0, 4);

                Stream.Write(contentbytes, 0, contentbytelen);
                return currentposition;
            }
        }


        public TValue Get(long blockposition)
        {
            Stream.Position = blockposition;
            byte[] counter = new byte[4];
            Stream.Read(counter, 0, 4);

            int len = BitConverter.ToInt32(counter, 0);

            byte[] content = new byte[len];

            Stream.Read(content, 0, len);

            return this.ValueConverter.FromByte(content);

        }


        public void Close()
        {
            if (_filestream != null)
            {
                lock (_object)
                {
                    if (_filestream != null)
                    {
                        _filestream.Close();
                        _filestream = null;
                    }
                }
            }
        }

        protected FileStream Stream
        {
            get
            {
                if (_filestream == null || !_filestream.CanRead)
                {
                    lock (_object)
                    {
                        if (_filestream == null || !_filestream.CanRead)
                        {
                            _filestream = StreamManager.GetFileStream(this.FullFileName);
                        }
                    }
                }
                return _filestream;
            }
        }



    }
}
