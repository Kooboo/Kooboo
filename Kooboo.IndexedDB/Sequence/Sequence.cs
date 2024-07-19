//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.IO;
using Kooboo.IndexedDB.ByteConverter;
using Kooboo.IndexedDB.Sequence;

namespace Kooboo.IndexedDB
{
    /// <summary>
    /// Used for read only data storage, sequence write and read. 
    /// To delete records,  del the entire sequence file. 
    /// </summary>
    public class Sequence<TValue> : ISequence
    {
        private object _object = new object();

        public string FullFileName;
        private FileStream _stream;

        private long start = 10;   // ignore the first 10 bytes. 
        private byte sanitybyteone = 10;
        private byte sanitybytetwo = 13;

        private IByteConverter<TValue> ValueConverter;

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

                byte[] bytes = System.Text.Encoding.ASCII.GetBytes("sequence");

                byte[] header = new byte[10];

                System.Buffer.BlockCopy(bytes, 0, header, 0, bytes.Length);

                openstream.Write(header, 0, 10);
                openstream.Close();
            }

        }

        /// <summary>
        /// the global sequence. 
        /// </summary>
        /// <param name="sequencename"></param>
        public Sequence(string SequenceNameOrFullFileName)
        {
            if (System.IO.Path.IsPathRooted(SequenceNameOrFullFileName))
            {
                this.FullFileName = SequenceNameOrFullFileName;
            }
            else
            {
                string globalSequencefolder = System.IO.Path.Combine(GlobalSettings.RootPath, GlobalSettings.SequencePath);

                if (!System.IO.Directory.Exists(globalSequencefolder))
                {
                    System.IO.Directory.CreateDirectory(globalSequencefolder);
                }

                this.FullFileName = System.IO.Path.Combine(globalSequencefolder, SequenceNameOrFullFileName + ".seq");
            }

            _initialize();
            this.ValueConverter = ObjectContainer.GetConverter<TValue>();
        }

        public bool Exists()
        {
            return File.Exists(FullFileName);
        }

        /// <summary>
        /// Write the byte value and return the disk position pointer.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Int64 Add(TValue T)
        {
            byte[] valueytes = this.ValueConverter.ToByte(T);
            int count = valueytes.Length;
            byte[] countbytes = BitConverter.GetBytes(count);

            int totalcount = count + 12;

            byte[] totalreocrd = new byte[totalcount];
            totalreocrd[0] = this.sanitybyteone;
            totalreocrd[1] = this.sanitybytetwo;

            totalreocrd[totalcount - 2] = this.sanitybyteone;
            totalreocrd[totalcount - 1] = this.sanitybytetwo;

            System.Buffer.BlockCopy(countbytes, 0, totalreocrd, 2, 4);
            System.Buffer.BlockCopy(valueytes, 0, totalreocrd, 6, count);
            System.Buffer.BlockCopy(countbytes, 0, totalreocrd, 6 + count, 4);

            totalreocrd[totalcount - 2] = this.sanitybyteone;
            totalreocrd[totalcount - 1] = this.sanitybytetwo;


            long returnposition = 0;

            lock (_object)
            {
                returnposition = Stream.Length;

                Stream.Position = returnposition;
                Stream.Write(totalreocrd, 0, totalcount);
            }

            return returnposition;

        }

        /// <summary>
        /// get the valud bytes count of current record. 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="ascending">check forward or backward.</param>
        /// <returns></returns>
        public int GetValueBytesCount(long position, bool ascending)
        {
            long counterposition = 0;
            byte[] counterbyte = new byte[4];

            if (ascending)
            {
                counterposition = position + 2;
            }
            else
            {
                counterposition = position - 6;
            }

            lock (_object)
            {
                Stream.Position = counterposition;
                Stream.Read(counterbyte, 0, 4);
            }

            return BitConverter.ToInt32(counterbyte, 0);

        }

        public TValue Get(Int64 position, int bytescount)
        {
            if (position <= 0 || bytescount < 0)
            {
                return default(TValue);
            }

            // the first 6 byte are the sanity checker and bytecounter. 
            byte[] contentbytes = new byte[bytescount];

            lock (_object)
            {
                Stream.Position = position + 6;
                Stream.Read(contentbytes, 0, bytescount);
            }
            return this.ValueConverter.FromByte(contentbytes);
        }

        public TValue Get(long position)
        {
            int count = GetValueBytesCount(position, true);

            if (count < DBSetting.MaxSequenceObjectSize)
            {
                return Get(position, count);
            }
            else
            {
                return default(TValue);
            }

        }

        public Stream GetBlockStream(long position)
        {
            int count = GetValueBytesCount(position, true);

            return new BlockStream(this.FullFileName, position + 6, count);
        }

        public SequenceCollection<TValue> GetCollection(bool ascending = false)
        {
            long start = this.start;
            long end = getEndRecord();

            SequenceCollection<TValue> collection = new SequenceCollection<TValue>(this, ascending, start, end);

            return collection;
        }

        public List<TValue> AllItemList(bool ascending = false)
        {
            List<TValue> list = new List<TValue>();

            foreach (var item in this.GetCollection(ascending))
            {
                list.Add(item);
            }
            return list;
        }

        public List<TValue> Take(bool ascending, int skip = 0, int count = 100)
        {
            List<TValue> col = new List<TValue>();

            int skipped = 0;
            int taken = 0;

            foreach (var item in GetCollection(ascending))
            {
                if (skipped < skip)
                {
                    skipped += 1;
                    continue;
                }

                if (taken >= count)
                {
                    return col;
                }

                col.Add(item);

                taken += 1;
            }
            return col;
        }


        public SequenceQuery<TValue> QueryAscending(Predicate<TValue> Query)
        {
            SequenceQuery<TValue> query = new SequenceQuery<TValue>(this);
            query.Ascending = true;
            query.Predicate = Query;
            return query;
        }

        public SequenceQuery<TValue> QueryDescending(Predicate<TValue> Query)
        {
            SequenceQuery<TValue> query = new SequenceQuery<TValue>(this);
            query.Ascending = false;
            query.Predicate = Query;
            return query;
        }

        /// <summary>
        /// return the last end position.
        /// </summary>
        /// <returns></returns>
        private long getEndRecord()
        {
            long endposition = Stream.Length;

            long lastend = endposition;

            byte[] sanitybyte = new byte[2];

            Stream.Position = endposition - 2;
            Stream.Read(sanitybyte, 0, 2);

            if (sanitybyte[0] == this.sanitybyteone && sanitybyte[1] == this.sanitybytetwo)
            {
                return endposition;
            }
            /// else we have some problems here. The record was not end, need to be cut off. 
            for (int i = 0; i < 999; i++)
            {
                if (endposition - (i + 1) * 100 < 0)
                {
                    return lastend;
                }
                else
                {
                    lastend = endposition - (i + 1) * 100;
                }

                byte[] next100 = new byte[100];
                lock (_object)
                {
                    Stream.Position = lastend;
                    Stream.Read(next100, 0, 100);
                }

                for (int j = 0; j < 99; j++)
                {
                    if (next100[99 - j - 1] == this.sanitybyteone && next100[99 - j] == this.sanitybyteone)
                    {
                        return endposition - (i * 100 + j);
                    }
                }

            }
            return 0;
        }

        public void Close()
        {
            lock (_object)
            {
                if (_stream != null)
                {
                    _stream.Close();
                    _stream = null;
                }
            }
        }

        public void Flush()
        {
            lock (_object)
            {
                if (_stream != null)
                {
                    _stream.Flush();
                }
            }
        }

        public void DelSelf()
        {
            Close();
            System.IO.File.Delete(this.FullFileName);
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

                            _initialize();
                            _stream = StreamManager.GetFileStream(this.FullFileName);
                        }
                    }
                }
                return _stream;
            }
        }

    }

    public class SequenceQuery<TResult>
    {
        public Predicate<TResult> Predicate { get; set; }

        private Predicate<TResult> EndPredicate { get; set; }

        public bool Ascending { get; set; }

        private Sequence<TResult> _seq { get; set; }

        private int skip { get; set; }

        /// <summary>
        /// The time point that it will be stop query. 
        /// it can be the min time when DESC or max time when ASC. 
        /// </summary>
        private DateTime QueryEndTime { get; set; }

        public SequenceQuery(Sequence<TResult> Seq)
        {
            this._seq = Seq;
            this.skip = 0;
        }

        public SequenceQuery<TResult> Skip(int skipcount)
        {
            this.skip = skipcount;
            return this;
        }

        public SequenceQuery<TResult> EndQueryCondition(Predicate<TResult> EndCondition)
        {
            this.EndPredicate = EndCondition;
            return this;
        }

        public List<TResult> Take(int takecount)
        {
            List<TResult> col = new List<TResult>();

            int skipped = 0;
            int taken = 0;

            foreach (var item in this._seq.GetCollection(Ascending))
            {
                if (this.EndPredicate != null && this.EndPredicate(item))
                {
                    return col;
                }

                if (Predicate(item))
                {
                    if (skipped < skip)
                    {
                        skipped += 1;
                        continue;
                    }

                    if (taken >= takecount)
                    {
                        return col;
                    }

                    col.Add(item);

                    taken += 1;
                }


            }
            return col;

        }

        public int Count()
        {
            int counter = 0;

            foreach (var item in this._seq.GetCollection(Ascending))
            {
                if (this.EndPredicate != null && this.EndPredicate(item))
                {
                    return counter;
                }

                if (Predicate(item))
                {
                    counter += 1;
                }
            }
            return counter;

        }

    }
}
