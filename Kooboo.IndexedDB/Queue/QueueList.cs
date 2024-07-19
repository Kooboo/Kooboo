//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.IO;

namespace Kooboo.IndexedDB.Queue
{

    /// <summary>
    /// The list of queue items. 
    /// 10 bytes per records. one byte for sanity, one for deleted/availalbe, 8 bytes for location. 
    /// </summary>
    public class QueueList
    {
        private object _object = new object();

        public string FullFileName;
        private FileStream _stream;

        private byte sanitybyte = 10;

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

                // the first 10 bytes for header.  contains record counter. 
                byte[] headerbyte = new byte[10];

                headerbyte[0] = this.sanitybyte;
                headerbyte[1] = 1;

                int dequeuecount = 0;

                System.Buffer.BlockCopy(BitConverter.GetBytes(dequeuecount), 0, headerbyte, 2, 4);

                openstream.Write(headerbyte, 0, 10);
                openstream.Close();
            }

        }

        public QueueList(string fullfilename)
        {
            this.FullFileName = fullfilename;
            _initialize();
        }

        public bool Exists()
        {
            return File.Exists(FullFileName);
        }

        public void Add(Int64 blockposition)
        {
            byte[] recordbyte = new byte[10];
            recordbyte[0] = this.sanitybyte;
            recordbyte[1] = 1;    /// 1 = record ok, 0 = record deleted.

            lock (_object)
            {
                Int64 startwriteposition = Stream.Length;

                long leftbyte = startwriteposition % 10;
                if (leftbyte != 0)
                {
                    /// ok, we fuck, previous reocrds was not write corrected. 
                    startwriteposition = startwriteposition - leftbyte;
                }

                Stream.Position = startwriteposition;

                System.Buffer.BlockCopy(BitConverter.GetBytes(blockposition), 0, recordbyte, 2, 8);

                Stream.Write(recordbyte, 0, 10);
            }
        }

        /// <summary>
        /// set the dequeue counter.
        /// </summary>
        /// <param name="newcounter"></param>
        public void SetCounter(int newcounter)
        {
            byte[] counterbyte = BitConverter.GetBytes(newcounter);

            lock (_object)
            {
                Stream.Position = 2;
                Stream.Write(counterbyte, 0, 4);
            }
        }

        /// <summary>
        /// get the dequeue counter. 
        /// </summary>
        /// <returns></returns>
        public int GetCounter()
        {
            byte[] counterbyte = new byte[4];

            lock (_object)
            {
                Stream.Position = 2;
                Stream.Read(counterbyte, 0, 4);
            }

            return BitConverter.ToInt32(counterbyte, 0);
        }

        /// <summary>
        /// total number of record in this list.
        /// </summary>
        /// <returns></returns>
        public int TotalCount()
        {
            long length = Stream.Length - 10;   // minus the 10 header bytes.
            return Convert.ToInt32((length / 10));
        }

        /// <summary>
        /// get the record index  block position. 
        /// it must be checked already that recordindex  smaller than total count.
        /// </summary>
        /// <param name="PreviousCount"></param>
        /// <returns></returns>
        public long GetBlockPosition(int recordindex)
        {
            Int64 startposition = recordindex * 10 + 2;
            byte[] positionbyte = new byte[8];

            lock (_object)
            {
                Stream.Position = startposition;
                Stream.Read(positionbyte, 0, 8);
            }
            return BitConverter.ToInt64(positionbyte, 0);
        }

        /// <summary>
        /// Check whether all queue items has been dequeued out or not. 
        /// </summary>
        /// <returns></returns>
        public bool isDequeueFinished()
        {
            return (GetCounter() >= TotalCount());
        }


        public void close()
        {
            if (_stream != null)
            {
                _stream.Close();
            }
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
