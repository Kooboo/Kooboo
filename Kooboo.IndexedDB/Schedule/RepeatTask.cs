//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Kooboo.IndexedDB.ByteConverter;

namespace Kooboo.IndexedDB.Schedule
{
    public class RepeatTask<TValue>
    {
        private object _object = new object();

        /// <summary>
        /// The record block position that are free to insert new records. 
        /// </summary>
        private List<long> FreePositions;

        /// <summary>
        /// The list of repeat task that is saved at memory. 
        /// </summary>
        private List<RepeatItem<TValue>> RepeatTaskList { get; set; }

        private byte startbyteone = 10;
        private byte startbytetwo = 13;

        /// <summary>
        /// To delete a reocord. update the isactive value to this value. 
        /// </summary>
        private byte recordDeletedByteValue = 99;

        /// <summary>
        /// The length per record. 
        /// </summary>
        private int recordlen = 44;


        /// <summary>
        /// The record file that save the meta info of all repeating task. 
        /// 
        /// </summary>
        private string RecordFile { get; set; }

        //The file that store the real schedule object.
        public string ContentFile { get; set; }

        private IByteConverter<TValue> ValueConverter;

        public RepeatTask(string FolderName)
        {
            this.RecordFile = FileNameGenerator.GetRepeatRecordFileName(FolderName);
            this.ContentFile = FileNameGenerator.GetRepeatContentFileName(FolderName);


            this.ValueConverter = ObjectContainer.GetConverter<TValue>();

            Verify();
        }

        private void Verify()
        {
            if (!File.Exists(this.ContentFile))
            {
                File.WriteAllText(this.ContentFile, "repeating schedule content file, do not modify.\r\n");
            }

            if (!File.Exists(this.RecordFile))
            {
                byte[] headerbytes = new byte[10];

                byte[] headers = System.Text.Encoding.ASCII.GetBytes("repeat");

                System.Buffer.BlockCopy(headers, 0, headerbytes, 0, headers.Count());

                File.WriteAllBytes(this.RecordFile, headerbytes);
            }
            RepeatTaskList = new List<RepeatItem<TValue>>();
            FreePositions = new List<long>();
            Init();
        }

        /// <summary>
        /// init the active items and free positions. 
        /// </summary>
        private void Init()
        {
            long current = 10;

            long length = this.RecordStream.Length;

            while (length >= current + this.recordlen)
            {
                RepeatItem<TValue> item = GetRecord(current);

                if (item == null)
                {
                    this.FreePositions.Add(current);
                }
                else
                {
                    item.Item = GetTask(item.BlockPosition);
                    this.RepeatTaskList.Add(item);
                }

                current = current + this.recordlen;
            }

            if (current != length)
            {
                // if the current is the not the end, there must be some bytes missing. 
                this.RecordStream.SetLength(current + this.recordlen);
            }


        }

        /// <summary>
        /// Add a new item to the repeating task list. 
        /// </summary>
        /// <param name="Item"></param>
        public void Add(RepeatItem<TValue> Item)
        {
            long blockposition = AddTask(Item.Item);
            Item.BlockPosition = blockposition;

            if (Item.NextExecute == default(DateTime))
            {
                Item.NextExecute = CalculateNextExecuteTime(Item);
            }

            AddRecord(Item);
            // also need to add to current list. 
            this.RepeatTaskList.Add(Item);
        }

        /// <summary>
        /// Used when an item is get out for execution, calcuate the next execution time. 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="LastExecute"></param>
        /// <returns></returns>
        public DateTime CalculateNextExecuteTime(RepeatItem<TValue> item)
        {
            return GetNextTryTime(item.Frequence, item.FrequenceUnit, item.StartTime, item.LastExecute);

        }

        private DateTime GetNextTryTime(RepeatFrequence Frequence, int FrequenceUnit, DateTime StartTime, DateTime lastExecute)
        {
            DateTime NextExecuteTime = default(DateTime);

            if (lastExecute == default(DateTime))
            { NextExecuteTime = StartTime; }
            else
            {
                NextExecuteTime = lastExecute;
            }


            while (true)
            {
                if (NextExecuteTime > DateTime.Now)
                {
                    break;
                }
                switch (Frequence)
                {
                    case RepeatFrequence.Day:
                        NextExecuteTime = NextExecuteTime.AddDays(FrequenceUnit);
                        break;
                    case RepeatFrequence.Hour:
                        NextExecuteTime = NextExecuteTime.AddHours(FrequenceUnit);
                        break;
                    case RepeatFrequence.Minutes:
                        NextExecuteTime = NextExecuteTime.AddMinutes(FrequenceUnit);
                        break;
                    case RepeatFrequence.Week:
                        NextExecuteTime = NextExecuteTime.AddDays(FrequenceUnit * 7);
                        break;
                    case RepeatFrequence.Month:
                        NextExecuteTime = NextExecuteTime.AddMonths(FrequenceUnit);
                        break;
                    case RepeatFrequence.Second:
                        NextExecuteTime = NextExecuteTime.AddSeconds(FrequenceUnit);
                        break;
                    default:
                        break;
                }

                if (NextExecuteTime > DateTime.Now)
                {
                    break;
                }
                else
                {
                    DateTime now = DateTime.Now;
                    switch (Frequence)
                    {
                        case RepeatFrequence.Day:
                            NextExecuteTime = new DateTime(now.Year, now.Month, now.Day, StartTime.Hour, StartTime.Minute, StartTime.Second);
                            break;
                        case RepeatFrequence.Hour:
                            NextExecuteTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, StartTime.Minute, StartTime.Second);

                            break;
                        case RepeatFrequence.Minutes:
                            NextExecuteTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, StartTime.Second);

                            break;
                        case RepeatFrequence.Week:
                            break;
                        case RepeatFrequence.Second:
                            NextExecuteTime = now;
                            break;
                        case RepeatFrequence.Month:
                            break;
                        default:
                            break;
                    }
                }
            }

            return NextExecuteTime;
        }
        /// <summary>
        /// Delete a item. 
        /// </summary>
        /// <param name="Item"></param>
        public void Del(RepeatItem<TValue> Item)
        {
            Del(Item.id);
        }

        public void Del(long id)
        {
            if (id <= 0)
            {
                // error. 
                throw new Exception("wrong record position id or position id not assigned");
            }

            lock (_object)
            {
                this.RecordStream.Position = id + 2;

                this.RecordStream.WriteByte(this.recordDeletedByteValue);

                this.FreePositions.Add(id);

                var existingitem = this.RepeatTaskList.Find(o => o.id == id);

                if (existingitem != null)
                {
                    this.RepeatTaskList.Remove(existingitem);
                }
            }


        }

        /// <summary>
        /// Get all the items that in the repeating task list. 
        /// </summary>
        /// <returns></returns>
        public List<RepeatItem<TValue>> GetItems()
        {
            List<RepeatItem<TValue>> items = new List<RepeatItem<TValue>>();

            foreach (var item in RepeatTaskList)
            {
                items.Add(Helper.TypeHelper.DeepCopy<RepeatItem<TValue>>(item));
            }
            return items;
        }

        public RepeatItem<TValue> Get(Int64 id)
        {
            foreach (var item in RepeatTaskList)
            {
                if (item.id == id)
                {
                    return Helper.TypeHelper.DeepCopy<RepeatItem<TValue>>(item);
                }
            }

            RepeatItem<TValue> record = GetRecord(id);

            if (record != null)
            {
                this.RepeatTaskList.Add(record);
                return Helper.TypeHelper.DeepCopy<RepeatItem<TValue>>(record);
            }

            return null;
        }

        /// <summary>
        /// Get the list of  all repeating task items that are due to be executed. and update their last execute time accordingly. 
        /// </summary>
        /// <returns></returns>
        public List<RepeatItem<TValue>> DequeueItems(Func<RepeatItem<TValue>, bool> isIgnore = null)
        {
            List<RepeatItem<TValue>> items = new List<RepeatItem<TValue>>();
            foreach (var item in this.RepeatTaskList)
            {
                if (item.NextExecute <= DateTime.Now)
                {
                    if (isIgnore != null && isIgnore(item))
                    {
                        continue;
                    }
                    RepeatItem<TValue> one = Helper.TypeHelper.DeepCopy<RepeatItem<TValue>>(item);
                    items.Add(one);

                }
            }

            // mark every item as executed. 
            foreach (var item in items)
            {
                UpdateExecuteTime(item);
            }

            return items;
        }

        /// <summary>
        /// Dequeue one item from the list the due tasks, and update its last execute time. 
        /// </summary>
        /// <returns></returns>
        public RepeatItem<TValue> DequeueItem()
        {
            foreach (var item in this.RepeatTaskList)
            {
                if (item.NextExecute <= DateTime.Now)
                {
                    RepeatItem<TValue> one = Helper.TypeHelper.DeepCopy<RepeatItem<TValue>>(item);
                    UpdateExecuteTime(one);
                    return one;
                }
            }
            return null;
        }

        /// <summary>
        /// Get the position that can be used to insert one more record. 
        /// </summary>
        /// <returns></returns>
        private long GetRecordInsertPosition()
        {
            lock (_object)
            {
                int count = this.FreePositions.Count;
                long positon;
                if (count > 0)
                {
                    positon = this.FreePositions[count - 1];
                    this.FreePositions.Remove(positon);
                    return positon;
                }
                else
                {
                    positon = this.RecordStream.Length;
                    return positon;
                }
            }
        }

        /// <summary>
        /// Add new task object to the content block. 
        /// </summary>
        /// <param name="repeattask"></param>
        /// <returns></returns>
        private long AddTask(TValue repeattask)
        {
            byte[] bytes = this.ValueConverter.ToByte(repeattask);

            int contentbytelen = bytes.Length;

            lock (_object)
            {
                Int64 currentposition = this.ContentStream.Length;

                ContentStream.Position = currentposition;

                ContentStream.WriteByte(startbyteone);
                ContentStream.WriteByte(startbytetwo);

                ContentStream.Write(BitConverter.GetBytes(contentbytelen), 0, 4);

                ContentStream.Write(bytes, 0, contentbytelen);
                return currentposition;
            }

        }

        private TValue GetTask(long blockposition)
        {
            lock (_object)
            {
                ContentStream.Position = blockposition;

                byte[] check = new byte[2];

                ContentStream.Read(check, 0, 2);

                if (check[0] != startbyteone && check[1] != startbytetwo)
                {
                    return default(TValue);
                }

                byte[] counter = new byte[4];
                ContentStream.Read(counter, 0, 4);

                int len = BitConverter.ToInt32(counter, 0);

                byte[] content = new byte[len];

                ContentStream.Read(content, 0, len);

                return this.ValueConverter.FromByte(content);
            }
        }

        /// <summary>
        /// Add the record item to the record table. 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private long AddRecord(RepeatItem<TValue> item)
        {
            byte[] bytes = this.ToRecordBytes(item);
            int bytelen = bytes.Length;

            lock (_object)
            {
                Int64 insertposition = GetRecordInsertPosition();

                RecordStream.Position = insertposition;
                RecordStream.Write(bytes, 0, bytelen);
                item.id = insertposition;
                return insertposition;
            }
        }

        /// <summary>
        /// Update this task content of this record. 
        /// </summary>
        /// <param name="item"></param>
        public void UpdateTask(RepeatItem<TValue> item)
        {
            long blockposition = AddTask(item.Item);
            item.BlockPosition = blockposition;
            UpdateSchedule(item);
        }

        /// <summary>
        /// update the schedule info of this record. 
        /// </summary>
        /// <param name="item"></param>
        public void UpdateSchedule(RepeatItem<TValue> item)
        {
            byte[] bytes = this.ToRecordBytes(item);
            int bytelen = bytes.Length;
            lock (_object)
            {
                if (item.id > 0)
                {
                    ContentStream.Position = item.id;
                    ContentStream.Write(bytes, 0, bytelen);
                }

                var currentitem = this.RepeatTaskList.Find(o => o.id == item.id);

                if (currentitem != null)
                {
                    currentitem = item;
                }
            }

        }

        /// <summary>
        /// This item just get executed, and should be updated now. 
        /// </summary>
        /// <param name="item"></param>
        private void UpdateExecuteTime(RepeatItem<TValue> item)
        {
            DateTime executetime = DateTime.Now;
            lock (_object)
            {
                RepeatItem<TValue> currentitem = this.RepeatTaskList.Find(o => o.id == item.id);

                if (currentitem != null)
                {
                    currentitem.LastExecute = executetime;
                    currentitem.NextExecute = CalculateNextExecuteTime(currentitem);

                    long executetimeposition = item.id + 16;
                    long tickexecutetime = executetime.Ticks;

                    long NextExecutionPosition = item.id + 24;
                    long tickNextExecution = currentitem.NextExecute.Ticks;

                    this.RecordStream.Position = executetimeposition;
                    this.RecordStream.Write(BitConverter.GetBytes(tickexecutetime), 0, 8);

                    this.RecordStream.Position = NextExecutionPosition;
                    this.RecordStream.Write(BitConverter.GetBytes(tickNextExecution), 0, 8);
                }
            }
        }

        /// <summary>
        /// get the record schedule information.
        /// </summary>
        /// <param name="recordposition"></param>
        /// <returns></returns>
        private RepeatItem<TValue> GetRecord(long recordposition)
        {
            byte[] recordbytes = new byte[this.recordlen];

            lock (_object)
            {
                this.RecordStream.Position = recordposition;
                this.RecordStream.Read(recordbytes, 0, this.recordlen);
            }

            RepeatItem<TValue> record = ParseRecordBytes(recordbytes);

            if (record == null)
            {
                return null;
            }
            else
            {
                record.id = recordposition;
                return record;
            }
        }

        /// <summary>
        /// convert the meta info of a repeating task into bytes. 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private byte[] ToRecordBytes(RepeatItem<TValue> item)
        {
            byte[] bytearray = new byte[this.recordlen];

            bytearray[0] = startbyteone;
            bytearray[1] = startbytetwo;

            bytearray[this.recordlen - 2] = startbyteone;
            bytearray[this.recordlen - 1] = startbytetwo;

            if (item.IsActive)
            {
                bytearray[2] = 1;
            }
            else
            {
                bytearray[2] = 0;
            }

            bytearray[3] = (byte)item.Frequence;

            //frequence unit. 
            System.Buffer.BlockCopy(BitConverter.GetBytes(item.FrequenceUnit), 0, bytearray, 4, 4);

            long tickStartTime = item.StartTime.Ticks;
            long tickLastExecute = item.LastExecute.Ticks;
            long tickNextExecute = item.NextExecute.Ticks;

            //starttime 
            System.Buffer.BlockCopy(BitConverter.GetBytes(tickStartTime), 0, bytearray, 8, 8);

            //last execute 
            System.Buffer.BlockCopy(BitConverter.GetBytes(tickLastExecute), 0, bytearray, 16, 8);

            //next execute 
            System.Buffer.BlockCopy(BitConverter.GetBytes(tickLastExecute), 0, bytearray, 24, 8);


            // block position. 
            System.Buffer.BlockCopy(BitConverter.GetBytes(item.BlockPosition), 0, bytearray, 32, 8);

            return bytearray;
        }

        public RepeatItem<TValue> ParseRecordBytes(byte[] bytes)
        {
            if (bytes[0] != startbyteone && bytes[1] != startbytetwo)
            {
                return null;
            }

            if (bytes[2] == this.recordDeletedByteValue)
            {
                return null;
            }

            RepeatItem<TValue> item = new RepeatItem<TValue>();

            if (bytes[2] == 1)
            {
                item.IsActive = true;
            }
            else if (bytes[2] == 0)
            {
                item.IsActive = false;
            }
            else
            {
                /// potential errror.
                return null;
            }

            item.Frequence = (RepeatFrequence)bytes[3];

            item.FrequenceUnit = BitConverter.ToInt32(bytes, 4);

            long tickStartTime = BitConverter.ToInt64(bytes, 8);
            long tickLastExecute = BitConverter.ToInt64(bytes, 16);
            long tickNextExecute = BitConverter.ToInt64(bytes, 24);
            item.StartTime = new DateTime(tickStartTime);
            item.LastExecute = new DateTime(tickLastExecute);
            item.NextExecute = new DateTime(tickNextExecute);

            item.BlockPosition = BitConverter.ToInt64(bytes, 32);

            return item;
        }


        private FileStream _RecordStream;

        public FileStream RecordStream
        {
            get
            {
                if (_RecordStream == null || _RecordStream.CanRead == false)
                {
                    lock (_object)
                    {
                        if (_RecordStream == null || _RecordStream.CanRead == false)
                        {
                            if (!File.Exists(this.RecordFile))
                            {
                                Verify();
                            }

                            _RecordStream = StreamManager.GetFileStream(this.RecordFile);
                        }
                    }
                }

                return _RecordStream;
            }
        }

        private FileStream _ContentStream;

        public FileStream ContentStream
        {
            get
            {
                if (_ContentStream == null || _ContentStream.CanRead == false)
                {
                    lock (_object)
                    {
                        if (_ContentStream == null || _ContentStream.CanRead == false)
                        {
                            if (!File.Exists(this.ContentFile))
                            {
                                Verify();
                            }
                            _ContentStream = StreamManager.GetFileStream(this.ContentFile);
                        }
                    }
                }

                return _ContentStream;
            }


        }

        public void DelSelf()
        {
            lock (_object)
            {
                this.RecordStream.Close();
                this.ContentStream.Close();
                File.Delete(this.RecordFile);
                File.Delete(this.ContentFile);
            }
        }


        public void Close()
        {
            if (_ContentStream != null)
            {
                _ContentStream.Close();
                _ContentStream = null;
            }

            if (_RecordStream != null)
            {
                _RecordStream.Close();
                _RecordStream = null;
            }
        }
    }
}
