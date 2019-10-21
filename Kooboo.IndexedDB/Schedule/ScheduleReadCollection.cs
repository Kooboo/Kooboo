//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System;
using System.Collections.Generic;

namespace Kooboo.IndexedDB.Schedule
{
    public class ScheduleReadCollection<TValue> : IEnumerable<TValue>
    {
        private Schedule<TValue> _schedule;

        /// <summary>
        /// read the collection of schedules, can be read only or dequeue.
        /// </summary>
        /// <param name="schedule"></param>
        public ScheduleReadCollection(Schedule<TValue> schedule)
        {
            this._schedule = schedule;
        }

        private IEnumerator<TValue> GetEnumerator()
        {
            return new Enumerator<TValue>(_schedule);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public class Enumerator<TEValue> : IEnumerator<TEValue>
        {
            private Schedule<TEValue> _schedule;

            private int _currentdate;
            private int _currentcounter;

            private List<long> _currentList;
            private int _listIndex;
            private int _listCount;

            private SortedSet<int> _daySet;

            public Enumerator(Schedule<TEValue> schedule)
            {
                this._schedule = schedule;
                this._currentdate = this._schedule.ReadingDay;
                this._currentcounter = this._schedule.ReadingSeconds;
                this._currentList = new List<long>();
                this._listIndex = 0;
                this._listCount = 0;

                this._daySet = new SortedSet<int>();

                foreach (var item in this._schedule.GetDayList())
                {
                    if (item > this._currentdate)
                    {
                        this._daySet.Add(item);
                    }
                }
            }

            public TEValue Current
            {
                get
                {
                    long blockposition = this._currentList[this._listIndex];
                    return this._schedule.GetValue(blockposition, _currentdate);
                }
            }

            public void Dispose()
            {
                this._schedule = null;
            }

            public bool MoveNext()
            {
                if (this._listIndex < (this._listCount - 1))
                {
                    this._listIndex += 1;
                    return true;
                }

                int maxcount = 24 * 60 * 60 - 1;

                for (int i = this._currentcounter; i <= maxcount; i++)
                {
                    Int64 itemposition = this._schedule.GetTimeFile(this._currentdate).GetItemPositionBySecondOfDay(this._currentcounter);

                    if (itemposition > 0)
                    {
                        List<long> list = this._schedule.GetItemFile(this._currentdate).ReadAll(itemposition);

                        this._listCount = list.Count;

                        if (this._listCount > 0)
                        {
                            this._currentList = list;
                            this._listIndex = 0;
                            this._currentcounter = i + 1;
                            return true;
                        }
                    }

                    this._currentcounter = i + 1;
                }

                // advance one day if any.
                int nextday = GetNextDay(this._currentdate);

                if (nextday <= 0)
                {
                    return false;
                }
                else
                {
                    this._currentdate = nextday;
                    this._currentcounter = 0;
                }

                return MoveNext();
            }

            private int GetNextDay(int currentday)
            {
                foreach (var item in this._daySet)
                {
                    if (item > currentday)
                    {
                        return item;
                    }
                }
                return 0;
            }

            /// <summary>
            /// Sorry,not reset available or needed here.
            /// </summary>
            public void Reset()
            {
                // this.current = this.start;
            }

            object System.Collections.IEnumerator.Current
            {
                get { return Current; }
            }
        }
    }
}