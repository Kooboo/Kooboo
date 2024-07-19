//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.IO;

namespace Kooboo.IndexedDB.Schedule
{
    /// <summary>
    /// The items on every second record. 
    /// </summary>
    public class SecondItem
    {

        private object _object = new object();

        private string FullFileName;
        private string FileName;
        private int DayInt;

        private FileStream _indexstream;

        private ISchedule _schedule;


        public SecondItem(int DayInt, ISchedule schedule)
        {
            this._schedule = schedule;

            this.FileName = FileNameGenerator.GetItemFileName(DayInt);
            this.DayInt = DayInt;
            this.FullFileName = FileNameGenerator.GetItemFullFileName(DayInt, schedule);

            // create the file and write a header. 
            if (!File.Exists(this.FullFileName))
            {
                File.WriteAllText(this.FullFileName, "store items of every second");
            }
        }

        /// <summary>
        /// Add the first item and return the section position. 
        /// </summary>
        /// <param name="blockPosition"></param>
        /// <returns></returns>
        public long AddFirst(long blockPosition)
        {
            lock (_object)
            {
                ItemHeader header = CreateNewSection();
                Add(header, blockPosition);
                return header.BlockPosition;
            }
        }

        public void Add(long SectionPosition, long BlockPosition)
        {
            lock (_object)
            {
                ItemHeader header = loadHeader(SectionPosition);
                Add(header, BlockPosition);
            }
        }

        /// <summary>
        /// Del an item from the second item. Delete is expensive as it has to search throught all records. 
        /// </summary>
        /// <param name="SectionPosition"></param>
        /// <param name="BlockPosition"></param>
        public void Del(long SectionPosition, long BlockPosition)
        {
            ItemHeader header = loadHeader(SectionPosition);

            if (header.BlockPosition <= 0)
            {
                return;
            }

            Item nextitem = loadItem(header.FirstRecordPointer);

            if (nextitem.ContentBlockPosition == BlockPosition)
            {
                header.FirstRecordPointer = nextitem.NextRecord;
                header.counter = header.counter - 1;
                Stream.Position = header.BlockPosition;
                Stream.Write(header.ToBytes(), 0, 14);
            }
            else
            {
                bool delok = Del(nextitem, BlockPosition);
                if (delok)
                {
                    header.counter = header.counter - 1;
                    Stream.Position = header.BlockPosition;
                    Stream.Write(header.ToBytes(), 0, 14);
                }
            }
        }

        private bool Del(Item currentItem, long BlockPosition)
        {
            Item previousitem = currentItem;
            currentItem = loadItem(currentItem.NextRecord);

            while (true)
            {
                if (currentItem.ContentBlockPosition == BlockPosition)
                {
                    //Found, update previous record to point to nexg record directly.
                    previousitem.NextRecord = currentItem.NextRecord;

                    Stream.Position = previousitem.ItemDiskLocation;

                    Stream.Write(previousitem.ToBytes(), 0, 18);

                    return true;
                }

                else if (currentItem.NextRecord <= 0)
                {
                    return false;
                }

                previousitem = currentItem;
                currentItem = loadItem(previousitem.NextRecord);
            }


        }

        /// <summary>
        /// Dequeue one item
        /// </summary>
        /// <param name="SectionPosition"></param>
        /// <returns></returns>
        public long DeQueue(long SectionPosition)
        {
            lock (_object)
            {

                ItemHeader header = loadHeader(SectionPosition);

                if (header.counter < 1)
                {
                    return -1;
                }

                Item item = loadItem(header.FirstRecordPointer);

                header.counter = header.counter - 1;
                header.FirstRecordPointer = item.NextRecord;

                Stream.Position = header.BlockPosition;
                Stream.Write(header.ToBytes(), 0, 14);

                return item.ContentBlockPosition;
            }

        }

        public int Count(long sectionPosition)
        {
            lock (_object)
            {
                return loadHeader(sectionPosition).counter;
            }
        }


        public List<long> GetAll(long SectionPosition)
        {
            return ReadAll(SectionPosition);
        }

        public List<long> ReadAll(long SectionPosition)
        {

            lock (_object)
            {

                List<long> list = new List<long>();

                ItemHeader header = loadHeader(SectionPosition);

                if (header.FirstRecordPointer <= 0)
                {
                    return list;
                }

                Item item = loadItem(header.FirstRecordPointer);
                list.Add(item.ContentBlockPosition);

                while (item.NextRecord > 0)
                {
                    item = loadItem(item.NextRecord);
                    list.Add(item.ContentBlockPosition);
                }

                return list;
            }

        }

        private void Add(ItemHeader header, long BlockPosition)
        {
            Item item = new Item();
            item.ContentBlockPosition = BlockPosition;
            item.NextRecord = header.FirstRecordPointer;

            long writeposition = Stream.Length;

            Stream.Position = writeposition;
            Stream.Write(item.ToBytes(), 0, 18);

            header.FirstRecordPointer = writeposition;
            header.counter = header.counter + 1;

            Stream.Position = header.BlockPosition;
            Stream.Write(header.ToBytes(), 0, 14);
        }

        private Item loadItem(long itemBlockPosition)
        {
            byte[] itembytes = new byte[18];

            Stream.Position = itemBlockPosition;
            Stream.Read(itembytes, 0, 18);

            Item item = new Item();

            item.ParseBytes(itembytes);

            item.ItemDiskLocation = itemBlockPosition;

            return item;
        }

        private ItemHeader loadHeader(long SectionPosition)
        {
            byte[] headerbytes = new byte[14];

            Stream.Position = SectionPosition;
            Stream.Read(headerbytes, 0, 14);

            ItemHeader header = new ItemHeader();

            header.ParseBytes(headerbytes);

            header.BlockPosition = SectionPosition;

            return header;
        }


        private ItemHeader CreateNewSection()
        {
            ItemHeader header = new ItemHeader();
            header.counter = 0;
            header.FirstRecordPointer = 0;
            Int64 writeposition;

            writeposition = Stream.Length;

            Stream.Position = writeposition;

            Stream.Write(header.ToBytes(), 0, 14);

            header.BlockPosition = writeposition;

            return header;
        }

        private FileStream Stream
        {
            get
            {
                if (_indexstream == null || _indexstream.CanRead == false)
                {
                    lock (_object)
                    {
                        if (_indexstream == null || _indexstream.CanRead == false)
                        {
                            _indexstream = StreamManager.GetFileStream(this.FullFileName);
                        }
                    }

                }
                return _indexstream;
            }
        }


        public void Close()
        {
            if (_indexstream != null)
            {
                lock (_object)
                {
                    if (_indexstream != null)
                    {
                        _indexstream.Close();
                        _indexstream = null;
                    }
                }
            }
        }

    }


    public class ItemHeader
    {
        /// <summary>
        /// The total counter of this section. 
        /// </summary>
        public int counter { get; set; }

        /// <summary>
        /// The block position of first record. 
        /// </summary>
        public long FirstRecordPointer { get; set; }

        /// <summary>
        /// The block position of this header.
        /// </summary>
        public long BlockPosition { get; set; }

        public byte[] ToBytes()
        {
            byte[] bytearray = new byte[14];

            bytearray[0] = 10;

            System.Buffer.BlockCopy(BitConverter.GetBytes(this.counter), 0, bytearray, 1, 4);
            System.Buffer.BlockCopy(BitConverter.GetBytes(this.FirstRecordPointer), 0, bytearray, 5, 8);

            bytearray[13] = 10;

            return bytearray;
        }

        public void ParseBytes(byte[] bytes)
        {

            this.counter = BitConverter.ToInt32(bytes, 1);

            this.FirstRecordPointer = BitConverter.ToInt64(bytes, 5);
        }

    }

    public class Item
    {
        public long ContentBlockPosition { get; set; }

        public long ItemDiskLocation { get; set; }

        public long NextRecord { get; set; }

        public byte[] ToBytes()
        {
            byte[] bytearray = new byte[18];

            bytearray[0] = 10;

            System.Buffer.BlockCopy(BitConverter.GetBytes(this.ContentBlockPosition), 0, bytearray, 1, 8);
            System.Buffer.BlockCopy(BitConverter.GetBytes(this.NextRecord), 0, bytearray, 9, 8);

            bytearray[17] = 10;

            return bytearray;
        }

        public void ParseBytes(byte[] bytes)
        {
            this.ContentBlockPosition = BitConverter.ToInt64(bytes, 1);
            this.NextRecord = BitConverter.ToInt64(bytes, 9);
        }

    }

}
