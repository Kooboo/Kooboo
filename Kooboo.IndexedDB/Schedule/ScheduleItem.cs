//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;

namespace Kooboo.IndexedDB.Schedule
{
    public class ScheduleItem<TValue>
    {

        /// <summary>
        /// The date part of schedule time. 
        /// </summary>
        public int DayInt { get; set; }

        public int SecondOfDay { get; set; }

        public DateTime ScheduleTime
        {
            get
            {
                return DateTimeService.ConvertIntToDateTime(this.DayInt, this.SecondOfDay);
            }
        }

        /// <summary>
        /// The schedule content block position, this is used to locate the 
        /// </summary>
        public Int64 BlockPosition { get; set; }

        /// <summary>
        /// The real task item of this schedul. 
        /// </summary>
        public TValue Item { get; set; }

    }
}
