//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;

namespace Kooboo.IndexedDB.Schedule
{

    public class ScheduleReadCollection<TValue> : IEnumerable<TValue>
    {

        private Schedule<TValue> schedule;

        /// <summary>
        /// read the collection of schedules, can be read only or dequeue. 
        /// </summary>
        /// <param name="schedule"></param>
        /// <param name="dequeue"></param>
        public ScheduleReadCollection(Schedule<TValue> schedule)
        {
            this.schedule = schedule;
        }

        IEnumerator<TValue> GetEnumerator()
        {
            return new Enumerator<TValue>(schedule);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public class Enumerator<EValue> : IEnumerator<EValue>
        {
            private Schedule<EValue> schedule;

            private int currentdate;
            private int currentcounter;


            private List<long> CurrentList;
            private int ListIndex;
            private int ListCount;

            private SortedSet<int> DaySet;

            public Enumerator(Schedule<EValue> schedule)
            {
                this.schedule = schedule;
                this.currentdate = this.schedule.ReadingDay;
                this.currentcounter = this.schedule.ReadingSeconds;
                this.CurrentList = new List<long>();
                this.ListIndex = 0;
                this.ListCount = 0;

                this.DaySet = new SortedSet<int>();

                foreach (var item in this.schedule.GetDayList())
                {
                    if (item > this.currentdate)
                    {
                        this.DaySet.Add(item);
                    }
                }
            }


            public EValue Current
            {
                get
                {
                    long blockposition = this.CurrentList[this.ListIndex];
                    return this.schedule.GetValue(blockposition, currentdate);
                }
            }

            public void Dispose()
            {
                this.schedule = null;
            }


            public bool MoveNext()
            {

                if (this.ListIndex < (this.ListCount - 1))
                {
                    this.ListIndex = this.ListIndex + 1;
                    return true;
                }

                int maxcount = 24 * 60 * 60 - 1;

                for (int i = this.currentcounter; i <= maxcount; i++)
                {
                    Int64 itemposition = this.schedule.GetTimeFile(this.currentdate).GetItemPositionBySecondOfDay(this.currentcounter);

                    if (itemposition > 0)
                    {

                        List<long> list = this.schedule.GetItemFile(this.currentdate).ReadAll(itemposition);

                        this.ListCount = list.Count;

                        if (this.ListCount > 0)
                        {
                            this.CurrentList = list;
                            this.ListIndex = 0;
                            this.currentcounter = i + 1;
                            return true;
                        }

                    }

                    this.currentcounter = i + 1;
                }


                // advance one day if any. 
                int nextday = GetNextDay(this.currentdate);

                if (nextday <= 0)
                {
                    return false;
                }
                else
                {
                    this.currentdate = nextday;
                    this.currentcounter = 0;
                }

                return MoveNext();
            }

            private int GetNextDay(int currentday)
            {
                foreach (var item in this.DaySet)
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
