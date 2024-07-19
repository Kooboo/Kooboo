//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.ComponentModel;

namespace Kooboo.IndexedDB.Schedule
{


    /// <summary>
    /// The items that will be repeating. 
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    [Serializable]
    public class RepeatItem<TValue>
    {

        public RepeatItem()
        {
            ///some default values
            this.FrequenceUnit = 1;
            this.Frequence = RepeatFrequence.Day;
            this.IsActive = true;
        }
        /// <summary>
        /// The id that will be used to update this record. 
        /// this is the record table block position. 
        /// </summary>
        public long id { get; set; }

        public DateTime StartTime { get; set; }

        /// <summary>
        /// The last execute time. 
        /// </summary>
        public DateTime LastExecute { get; set; }

        public DateTime NextExecute { get; set; }

        /// <summary>
        /// The block to get task object. 
        /// </summary>
        public long BlockPosition { get; set; }

        public RepeatFrequence Frequence { get; set; }

        public int FrequenceUnit { get; set; }

        /// <summary>
        /// whether this task is active or not. 
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// The real task item of this repeating task. 
        /// </summary>
        public TValue Item { get; set; }

    }

    [Serializable]
    public enum RepeatFrequence
    {
        [Description("days")]
        Day = 0,

        [Description("hours")]
        Hour = 1,

        [Description("minutes")]
        Minutes = 3,

        [Description("weeks")]
        Week = 4,

        [Description("months")]
        Month = 5,

        [Description("seconds")]
        Second = 6
    }
}
