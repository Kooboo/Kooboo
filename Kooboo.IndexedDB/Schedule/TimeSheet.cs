//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.IO;

namespace Kooboo.IndexedDB.Schedule
{

    /// <summary>
    /// The scheduled objected. 
    /// </summary>
    public class TimeSheet
    {

        private ISchedule _schedule;

        private object _object = new object();

        private FileStream _stream;

        private int oldCounter;
        private int newCounter;
        private int _positionCacheCounter;
        private Dictionary<int, Int64> _positionCache;

        internal string FileName { get; set; }

        internal string FullFileName { get; set; }

        private int DayInt { get; set; }


        public bool Exists()
        {
            return File.Exists(FullFileName);
        }

        public static bool isExists(int DayInt, ISchedule schedule)
        {
            return File.Exists(FileNameGenerator.GetTimeFullFileName(DayInt, schedule));
        }

        public bool IsToday()
        {
            return (this.DayInt == DateTime.Now.DayToInt());
        }

        /// <summary>
        /// create the time file, set length = total seconds of a day. 
        /// </summary>
        private void Create()
        {
            if (!File.Exists(this.FullFileName))
            {
                using (FileStream fs = File.Create(this.FullFileName))
                {
                    int len = 24 * 60 * 60 * 8 + 8;   // The first 8 byte for the counter. 

                    fs.SetLength(len);
                    fs.Position = 4;
                    int counter = -1;
                    fs.Write(BitConverter.GetBytes(counter), 0, 4);
                    fs.Close();
                }
            }
        }

        public TimeSheet(int DayInt, ISchedule schedule)
        {
            this._schedule = schedule;

            _positionCache = new Dictionary<int, long>();
            _positionCacheCounter = 0;

            this.FileName = FileNameGenerator.GetTimeFileName(DayInt);
            this.DayInt = DayInt;
            this.FullFileName = FileNameGenerator.GetTimeFullFileName(DayInt, schedule);

            if (!File.Exists(this.FullFileName))
            {

                int currentDay = DateTime.Now.DayToInt();

                if (DayInt >= currentDay)
                {
                    Create();
                }
            }
        }

        /// <summary>
        /// Write the byte value and the schedule content disk position pointer.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public void Add(DateTime ExactScheduleTime, Int64 ContentPosition)
        {
            Int64 position = GetTimeSheetSecondPosition(ExactScheduleTime);

            lock (_object)
            {
                Stream.Position = position;
                Stream.Write(BitConverter.GetBytes(ContentPosition), 0, 8);
            }
        }

        public void Remove(DateTime ExactScheduleTime)
        {
            lock (_object)
            {
                Stream.Position = GetTimeSheetSecondPosition(ExactScheduleTime);
                Int64 removed = -1;
                Stream.Write(BitConverter.GetBytes(removed), 0, 8);
            }
        }


        public Int64 GetItemPositionBySecondOfDay(int SecondOfDay)
        {
            if (SecondOfDay < 0)
            {
                SecondOfDay = 0;
            }

            int timesheetPosition = SecondOfDay * 8 + 8;
            return GetItemPosition(timesheetPosition);
        }

        public Int64 GetItemPosition(int TimeSheetPosition)
        {
            Int64 ItemPosition = getcacheposition(TimeSheetPosition);

            //Return -1 if not found, to be read from disk. 
            if (ItemPosition < 0)
            {
                lock (_object)
                {
                    byte[] contentBytes = new byte[8];

                    Stream.Position = TimeSheetPosition;
                    Stream.Read(contentBytes, 0, 8);
                    ItemPosition = BitConverter.ToInt64(contentBytes, 0);

                    if (ItemPosition > 0)
                    {
                        positionaddtocache(TimeSheetPosition, ItemPosition);
                    }
                }
            }
            return ItemPosition;
        }



        public int GetCounterWithoutModify()
        {
            if (newCounter > 0)
            {
                return newCounter;
            }
            else
            {
                lock (_object)
                {
                    byte[] counterBytes = new byte[4];
                    Stream.Position = 4;
                    Stream.Read(counterBytes, 0, 4);

                    return BitConverter.ToInt32(counterBytes, 0);
                }
            }
        }

        /// <summary>
        /// Whether there have been dequeue reading on this file or not.
        /// </summary>
        /// <returns></returns>
        public bool isStartRead()
        {
            return this.GetCounter() >= 0;
        }

        /// <summary>
        /// this file has finished all reading. 
        /// </summary>
        /// <returns></returns>
        public bool isFinishRead()
        {
            return this.GetCounter() >= (24 * 60 * 60 - 1);
        }

        public bool isBeingRead()
        {
            return (this.isStartRead() == false && this.isFinishRead() == false);
        }

        /// <summary>
        /// Get the time of today for reading. 
        /// </summary>
        /// <returns></returns>
        public int GetCounter()
        {
            if (newCounter > 0)
            {
                return newCounter;
            }
            else
            {
                lock (_object)
                {
                    byte[] counterBytes = new byte[4];
                    Stream.Position = 4;
                    Stream.Read(counterBytes, 0, 4);

                    oldCounter = BitConverter.ToInt32(counterBytes, 0);
                    newCounter = oldCounter;
                    return oldCounter;
                }
            }
        }

        public void SetCounter(int counter)
        {
            newCounter = counter;
            // this should not be a problem to write every 10 reads, because every record is removed from schdulerecord after dequeue. .

            if ((newCounter - oldCounter) > 10)
            {
                lock (_object)
                {
                    Stream.Position = 4;
                    Stream.Write(BitConverter.GetBytes(counter), 0, 4);

                    oldCounter = newCounter;
                }
            }

        }

        public void Close()
        {
            if (_stream != null)
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
        }


        public void DelSelf()
        {
            this.Close();

            try
            {
                System.IO.File.Delete(this.FullFileName);
            }
            catch (Exception)
            {
                //
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

        public void Flush()
        {
            Stream.Flush();
        }

        #region cache

        private Int64 getcacheposition(int time)
        {
            if (_positionCache.ContainsKey(time))
            {
                return _positionCache[time];
            }
            else
            {
                return -1;
            }
        }

        private void positionaddtocache(int time, Int64 position)
        {
            if (_positionCache.ContainsKey(time))
            {
                _positionCache.Add(time, position);
                _positionCacheCounter++;
            }

            if (_positionCacheCounter > 300)
            {
                _positionCacheCounter = _positionCacheCounter - 150;
                List<int> keystoremove = new List<int>();
                int counter = 0;
                foreach (var item in _positionCache)
                {
                    keystoremove.Add(item.Key);
                    counter++;
                    if (counter > 149)
                    {
                        break;
                    }
                }

                foreach (var item in keystoremove)
                {
                    _positionCache.Remove(item);
                }
            }
        }

        #endregion


        private static int GetTimeSheetSecondPosition(DateTime ExactScheduleTime)
        {
            int timeofToday = (int)ExactScheduleTime.TimeOfDay.TotalSeconds;

            int position = timeofToday * 8;
            position = position + 8;  /// the first 8 bytes is the counter. 
            return position;
        }

    }
}
