//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.IndexedDB.ByteConverter;
using System;
using System.IO;

namespace Kooboo.IndexedDB.Queue
{
    /// <summary>
    /// The content body of TValue class.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class QueueContent<TValue>
    {
        private object _object = new object();

        public string FullFileName;
        private FileStream _stream;

        private IByteConverter<TValue> _valueConverter;

        private void _initialize()
        {
            if (!File.Exists(FullFileName))
            {
                FileInfo fileinfo = new FileInfo(FullFileName);

                if (!fileinfo.Directory.Exists)
                {
                    fileinfo.Directory.Create();
                }

                FileStream openstream = File.Create(FullFileName);

                byte[] bytes = System.Text.Encoding.ASCII.GetBytes(" Kooboo queue content file, do not modify\r\n");

                openstream.Write(bytes, 0, bytes.Length);
                openstream.Close();
            }
        }

        public QueueContent(string fullfilename)
        {
            this.FullFileName = fullfilename;
            _initialize();
            this._valueConverter = ObjectContainer.GetConverter<TValue>();
        }

        public bool Exists()
        {
            return File.Exists(FullFileName);
        }

        /// <summary>
        /// Write the byte value and return the disk position pointer.
        /// </summary>
        /// <returns></returns>
        public Int64 Add(TValue T)
        {
            byte[] valueytes = this._valueConverter.ToByte(T);
            int count = valueytes.Length;

            lock (_object)
            {
                Int64 startwriteposition = Stream.Length;

                Int64 returnPosition = startwriteposition;  // to be return for outside.
                Stream.Position = returnPosition;
                Stream.Write(BitConverter.GetBytes(count), 0, 4);   // the value length counter.

                Stream.Position = returnPosition + 4;
                Stream.Write(valueytes, 0, valueytes.Length);

                return returnPosition;
            }
        }

        public TValue Get(Int64 position)
        {
            lock (_object)
            {
                Stream.Position = position;

                byte[] counterbyte = new byte[4];

                Stream.Read(counterbyte, 0, 4);

                int counter = BitConverter.ToInt32(counterbyte, 0);

                byte[] contentbytes = new byte[counter];

                Stream.Read(contentbytes, 0, counter);

                return this._valueConverter.FromByte(contentbytes);
            }
        }

        public void close()
        {
            _stream?.Close();
        }

        public FileStream Stream
        {
            get
            {
                if (_stream == null || _stream.CanRead == false)
                {
                    lock (_object)
                    {
                        if (_stream == null || _stream.CanRead == false)
                        {
                            _stream = StreamManager.GetFileStream(this.FullFileName);
                        }
                    }
                }
                return _stream;
            }
        }
    }
}