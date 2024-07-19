//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using Kooboo.IndexedDB.ByteConverter;
using Kooboo.IndexedDB.Schedule;

namespace Kooboo.IndexedDB
{
    public class Schedule<TValue> : ISchedule
    {

        /// <summary>
        /// The folder that contains this schedule. 
        /// </summary>
        public string Folder { get; set; }

        private IByteConverter<TValue> ValueConverter;

        private Dictionary<int, ScheduleContent<TValue>> _contentdictionary;
        private Dictionary<int, TimeSheet> _timesheetdictionary;
        private Dictionary<int, SecondItem> _itemsdictionary;

        internal int ReadingDay { get; set; }

        private int _readingDaySeconds;

        /// <summary>
        /// The reading seconds. 
        /// </summary>
        internal int ReadingSeconds
        {
            get
            {
                if (_readingDaySeconds <= 0)
                {
                    _readingDaySeconds = this.GetTimeFile(ReadingDay).GetCounter();
                    if (_readingDayCounter < 0)
                    {
                        _readingDayCounter = 0;
                    }
                }
                return _readingDaySeconds;
            }
            set
            {
                _readingDaySeconds = value;
            }
        }

        private int _readingDayCounter;  // the total seconds. 

        internal object _object = new object();

        /// <summary>
        /// The Full directory path of a folder name that will be created under global folder. 
        /// </summary>
        /// <param name="scheduleNameOrFolder"></param>
        public Schedule(string scheduleNameOrFolder)
        {
            scheduleNameOrFolder = scheduleNameOrFolder.ToValidPath();

            if (System.IO.Path.IsPathRooted(scheduleNameOrFolder))
            {
                this.Folder = scheduleNameOrFolder;
            }
            else
            {
                string globalschedulefolder = System.IO.Path.Combine(GlobalSettings.RootPath, GlobalSettings.schedulePath);

                if (!System.IO.Directory.Exists(globalschedulefolder))
                {
                    System.IO.Directory.CreateDirectory(globalschedulefolder);
                }

                this.Folder = System.IO.Path.Combine(globalschedulefolder, scheduleNameOrFolder);
            }

            this.ValueConverter = ObjectContainer.GetConverter<TValue>();

            _initialize();

        }


        /// <summary>
        /// Get list of int days in the current schedule directory. 
        /// </summary>
        /// <returns></returns>
        internal List<int> GetDayList()
        {
            List<int> list = new List<int>();
            string[] files = System.IO.Directory.GetFiles(this.Folder, "*.time");

            foreach (var item in files)
            {
                string temp = string.Empty;

                int lastSlash = Kooboo.IndexedDB.Helper.PathHelper.GetLastSlash(item);
                if (lastSlash > 0)
                {
                    temp = item.Substring(lastSlash + 1);
                }
                else
                {
                    temp = item;
                }

                temp = temp.Replace(".time", "");
                int dayint = Convert.ToInt32(temp);
                list.Add(dayint);
            }

            return list;
        }

        private void CleanOldFiles()
        {
            var daylist = GetDayList();

            foreach (var item in daylist)
            {
                if (item < this.ReadingDay)
                {
                    DelDayFile(item);
                }
            }

        }

        private void DelDayFile(int dayint)
        {
            lock (_object)
            {
                if (_timesheetdictionary.ContainsKey(dayint))
                {
                    _timesheetdictionary[dayint].Close();
                    _timesheetdictionary.Remove(dayint);

                    System.IO.File.Delete(FileNameGenerator.GetTimeFullFileName(dayint, this));
                }

                if (_itemsdictionary.ContainsKey(dayint))
                {
                    _itemsdictionary[dayint].Close();
                    _itemsdictionary.Remove(dayint);
                    System.IO.File.Delete(FileNameGenerator.GetItemFullFileName(dayint, this));
                }

                if (_contentdictionary.ContainsKey(dayint))
                {

                    _contentdictionary[dayint].Close();
                    _contentdictionary.Remove(dayint);
                    System.IO.File.Delete(FileNameGenerator.GetContentFullFileName(dayint, this));
                }


            }

        }

        private void _initialize()
        {
            _contentdictionary = new Dictionary<int, ScheduleContent<TValue>>();
            _timesheetdictionary = new Dictionary<int, TimeSheet>();
            _itemsdictionary = new Dictionary<int, SecondItem>();

            this.ReadingDay = DateTime.Now.DayToInt();
            int currentchecking = this.ReadingDay;
            int nextchecking = this.ReadingDay;

            int foundday = this.ReadingDay;

            TimeSheet currentfile = GetTimeFile(currentchecking);

            if (!currentfile.isStartRead())
            {
                for (int i = 1; i < GlobalSettings.ScheduleExpirationDays; i++)
                {
                    nextchecking = nextchecking - 1;

                    if (!TimeSheet.isExists(nextchecking, this))
                    {
                        continue;
                    }

                    TimeSheet oldfile = GetTimeFile(nextchecking);

                    if (oldfile == null)
                    {
                        continue;
                    }

                    if (oldfile.isFinishRead())
                    {
                        break;    // return, use the last checking file. 

                    }

                    if (!oldfile.isStartRead())
                    {
                        // not start reading yet, set currentchecking. 
                        currentchecking = nextchecking;
                        continue;
                    }

                    if (oldfile.isBeingRead())
                    {
                        currentchecking = nextchecking;
                        break;
                    }
                }

                this.ReadingDay = currentchecking;

            }

            CleanOldFiles();

        }


        /// <summary>
        /// Add an item to the schedule. Note, an item can not be schedule to past.
        /// Only allowed to schedule an item for future. 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="scheduletime"></param>
        public void Add(TValue input, DateTime scheduletime)
        {
            lock (_object)
            {

                scheduletime = EnsureScheduleTime(scheduletime);

                int DayInt = scheduletime.DayToInt();

                Int64 blockposition = GetContentFile(DayInt).Add(input);

                Int64 TimeItemLocation = GetTimeFile(DayInt).GetItemPositionBySecondOfDay((int)scheduletime.TimeOfDay.TotalSeconds);

                if (TimeItemLocation > 0)
                {
                    GetItemFile(DayInt).Add(TimeItemLocation, blockposition);
                }
                else
                {
                    Int64 itemlocation = GetItemFile(DayInt).AddFirst(blockposition);
                    GetTimeFile(DayInt).Add(scheduletime, itemlocation);
                }
            }
        }


        public void Add(ScheduleItem<TValue> item)
        {
            lock (_object)
            {
                int dayint = item.DayInt;
                int secondofday = item.SecondOfDay;

                DateTime checkschedule = EnsureScheduleTime(item.ScheduleTime);

                if (checkschedule > item.ScheduleTime)
                {
                    dayint = checkschedule.DayToInt();
                    secondofday = (int)item.ScheduleTime.TimeOfDay.TotalSeconds;
                }

                Int64 blockposition = GetContentFile(dayint).Add(item.Item);

                Int64 TimeItemLocation = GetTimeFile(dayint).GetItemPositionBySecondOfDay(secondofday);

                if (TimeItemLocation > 0)
                {
                    GetItemFile(dayint).Add(TimeItemLocation, blockposition);
                }
                else
                {
                    Int64 itemlocation = GetItemFile(dayint).AddFirst(blockposition);
                    GetTimeFile(dayint).Add(checkschedule, itemlocation);
                }
            }
        }


        public void EnQueue(TValue input, DateTime scheduletime)
        {
            Add(input, scheduletime);
        }

        public ScheduleItem<TValue> DeQueue()
        {
            lock (_object)
            {
                TimeSheet currentreading = GetTimeFile(this.ReadingDay);

                int currentReadingSecond = this.ReadingSeconds;
                int maxcount;
                if (currentreading.IsToday())
                {
                    maxcount = (int)DateTime.Now.TimeOfDay.TotalSeconds;
                }
                else
                {
                    maxcount = 24 * 60 * 60 - 1;
                }

                for (int i = currentReadingSecond; i <= maxcount; i++)
                {
                    Int64 itemposition = currentreading.GetItemPositionBySecondOfDay(i);

                    if (itemposition > 0)
                    {
                        Int64 oneBlockPosition = GetItemFile(this.ReadingDay).DeQueue(itemposition);

                        if (oneBlockPosition > 0)
                        {

                            TValue onevalue = GetContentFile(this.ReadingDay).Get(oneBlockPosition);

                            return new ScheduleItem<TValue>()
                            {
                                DayInt = this.ReadingDay,
                                SecondOfDay = i,
                                BlockPosition = oneBlockPosition,
                                Item = onevalue
                            };

                        }

                    }

                    // reset counter and move ahead. 
                    currentreading.SetCounter(i + 1);
                    this.ReadingSeconds = i + 1;
                }

                // after reading all seconds.
                if (currentreading.IsToday())
                {
                    return null;  // no more, return null. 
                }
                else
                {
                    // advanced one day and continue. 
                    this.ReadingDay = GetNextDay(this.ReadingDay);
                    this.ReadingSeconds = -1;
                    //TODO: check whether it is necessary to remove old file or not. 
                    return DeQueue();
                }
            }

        }

        public void Delete(ScheduleItem<TValue> item)
        {
            Int64 TimeItemLocation = GetTimeFile(item.DayInt).GetItemPositionBySecondOfDay(item.SecondOfDay);
            GetItemFile(item.DayInt).Del(TimeItemLocation, item.BlockPosition);
        }

        public void Delete(int DayInt, int SecondOfDay, Int64 blockposition)
        {
            Int64 TimeItemLocation = GetTimeFile(DayInt).GetItemPositionBySecondOfDay(SecondOfDay);
            GetItemFile(DayInt).Del(TimeItemLocation, blockposition);
        }

        public ScheduleItem<TValue> Get(int dayint, int SecondOfDay, Int64 BlockPosition)
        {
            TValue content = GetValue(BlockPosition, dayint);
            return new ScheduleItem<TValue>()
            {
                DayInt = dayint,
                SecondOfDay = SecondOfDay,
                BlockPosition = BlockPosition,
                Item = content
            };
        }

        internal TValue GetValue(long BlockPosition, int DayInt)
        {
            return GetContentFile(DayInt).Get(BlockPosition);
        }

        /// <summary>
        /// Count all records in the dequeue. 
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            lock (_object)
            {

                var daylist = GetDayList();
                int counter = 0;

                foreach (var item in daylist)
                {
                    if (item >= this.ReadingDay)
                    {
                        counter += CountOneDay(item);
                    }
                }

                return counter;
            }
        }

        /// <summary>
        /// Read the schedule items collections.
        /// NOTE: this only read the task object, without meta info. You can not delete an item without meta info.
        /// </summary>
        /// <returns></returns>
        public ScheduleReadCollection<TValue> GetReadCollection()
        {
            return new ScheduleReadCollection<TValue>(this);
        }

        /// <summary>
        /// Read items from the schedule list. It can be both due or not yet due items. 
        /// This read contains meta info that can be used to delete one schedule item. 
        /// </summary>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        public List<ScheduleItem<TValue>> Read(int take, int skip = 0)
        {
            if (skip < 0)
            {
                skip = 0;
            }

            int skipped = 0;
            int taken = 0;

            List<ScheduleItem<TValue>> Items = new List<ScheduleItem<TValue>>();

            int currentdate = this.ReadingDay;
            int currentcounter = this.ReadingSeconds;

            while (true)
            {
                int maxcount = 24 * 60 * 60 - 1;
                for (int i = currentcounter; i <= maxcount; i++)
                {
                    Int64 itemposition = this.GetTimeFile(currentdate).GetItemPositionBySecondOfDay(currentcounter);

                    if (itemposition > 0)
                    {
                        List<long> list = this.GetItemFile(currentdate).ReadAll(itemposition);

                        foreach (var item in list)
                        {
                            if (skipped < skip)
                            {
                                skipped = skipped + 1;
                                continue;
                            }

                            var content = this.GetValue(item, currentdate);

                            ScheduleItem<TValue> scheduleitem = new ScheduleItem<TValue>()
                            {
                                DayInt = currentdate,
                                SecondOfDay = i,
                                BlockPosition = item,
                                Item = content
                            };

                            Items.Add(scheduleitem);

                            taken = taken + 1;

                            if (taken >= take)
                            {
                                return Items;
                            }

                        }
                    }

                    currentcounter = i + 1;
                }

                // advance one day if any. 
                int nextday = GetNextDay(currentdate);

                if (nextday <= 0)
                {
                    return Items;
                }
                else
                {
                    currentdate = nextday;
                    currentcounter = 0;
                }

            }

        }

        private int CountOneDay(int dayint)
        {
            int counter = 0;
            int maxcount = 24 * 60 * 60 - 1;

            int starti = 0;

            if (dayint == this.ReadingDay)
            {
                starti = this.ReadingSeconds;
            }

            for (int i = starti; i <= maxcount; i++)
            {
                Int64 itemposition = GetTimeFile(dayint).GetItemPositionBySecondOfDay(i);

                if (itemposition > 0)
                {
                    int count = GetItemFile(dayint).Count(itemposition);
                    counter = counter + count;
                }
            }

            return counter;

        }

        private int GetNextDay(int currentReadingDay)
        {
            int nextday = currentReadingDay + 1;

            // if there is a timesheet for this. return it. 
            if (TimeSheet.isExists(nextday, this))
            {
                return nextday;
            }
            else
            {
                int today = DateTime.Now.DayToInt();
                // if it is today, return it. 
                if (nextday == today)
                {
                    return nextday;
                }
                else if (nextday > today)
                {
                    return -1;
                }
            }

            return GetNextDay(nextday);
        }


        internal TimeSheet GetTimeFile(int DayInt)
        {
            if (!_timesheetdictionary.ContainsKey(DayInt))
            {
                lock (_object)
                {
                    if (!_timesheetdictionary.ContainsKey(DayInt))
                    {
                        TimeSheet time = new TimeSheet(DayInt, this);
                        _timesheetdictionary.Add(DayInt, time);
                    }
                }
            }

            return _timesheetdictionary[DayInt];
        }


        internal SecondItem GetItemFile(int DayInt)
        {
            if (!_itemsdictionary.ContainsKey(DayInt))
            {
                lock (_object)
                {
                    if (!_itemsdictionary.ContainsKey(DayInt))
                    {
                        SecondItem item = new SecondItem(DayInt, this);
                        _itemsdictionary.Add(DayInt, item);
                    }
                }
            }

            return _itemsdictionary[DayInt];
        }


        internal ScheduleContent<TValue> GetContentFile(int DayInt)
        {
            if (!_contentdictionary.ContainsKey(DayInt))
            {
                lock (_object)
                {
                    if (!_contentdictionary.ContainsKey(DayInt))
                    {
                        ScheduleContent<TValue> content = new ScheduleContent<TValue>(DayInt, this);
                        _contentdictionary.Add(DayInt, content);
                    }
                }
            }

            return _contentdictionary[DayInt];

        }

        /// <summary>
        /// Make sure that the schedule time is valid, not in the past that might not queue out any more. 
        /// </summary>
        /// <param name="scheduletime"></param>
        /// <returns></returns>
        private DateTime EnsureScheduleTime(DateTime scheduletime)
        {
            int DayScheduleTime = scheduletime.DayToInt();

            if (DayScheduleTime > this.ReadingDay)
            {
                return scheduletime;
            }
            else if (DayScheduleTime == this.ReadingDay)
            {
                if (scheduletime.TimeOfDay.TotalSeconds > this.ReadingSeconds)
                {
                    return scheduletime;
                }
                else
                {
                    int secondsToAdd = this.ReadingSeconds - (int)scheduletime.TimeOfDay.TotalSeconds + 2;

                    return scheduletime.AddSeconds(secondsToAdd);

                }
            }
            else
            {
                /// schedule in the past is not allowed. Make it schduled to now and 10 seconds. 
                return DateTime.Now.AddSeconds(2);
            }

        }


        public void Close()
        {
            foreach (var item in this._contentdictionary)
            {
                item.Value.Close();
            }

            foreach (var item in this._itemsdictionary)
            {
                item.Value.Close();
            }

            foreach (var item in this._timesheetdictionary)
            {
                item.Value.Close();
            }
        }


        public void DelSelf()
        {
            this.Close();

            System.IO.Directory.Delete(this.Folder, true);
        }

    }
}
