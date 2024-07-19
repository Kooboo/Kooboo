//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;

namespace Kooboo.IndexedDB
{
    /// <summary>
    /// Some database configuration settins, feel free to change that. 
    /// </summary>
    public static class GlobalSettings
    {

        private static string _rootpath;
        public static string RootPath
        {
            get
            {
                if (_rootpath == null)
                {
                    _rootpath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppData", "KoobooDB");
                }
                return _rootpath;
            }
            set
            {
                _rootpath = value;
            }
        }

        /// <summary>
        /// the folder that contains all queue of this database. 
        /// each queue has its own folder under this folder
        /// </summary>
        public static string QueuePath { get; set; } = "__koobooqueue";

        public static bool EnableTableLog { get; set; } = true;

        /// <summary>
        /// The field name that must be in the object when enable versioning. 
        /// </summary>
        public static string VersionFieldName { get; set; } = "Version";

        public static string EditLogUniqueName { get; set; } = "_koobooeditlog";

        public static string TableLogName { get; set; } = "_koobootablelog";

        /// <summary>
        /// the folder that contains all sequence of this database. 
        /// each sequence has its own file under this folder
        /// </summary>
        public static string SequencePath { get; set; } = "__kooboosequence";

        /// <summary>
        /// the folder that contains all sequence of this database. 
        /// each schedule has its own sub folder
        /// </summary>
        public static string schedulePath = "_koobooschedule";

        public static string scheduleRepeatingPath = "_koobooschedulerepeat";

        /// <summary>
        /// The days that a schedule item will not be queuued out any more. 
        /// Also the day that all items will be removed. 
        /// </summary>
        public static int ScheduleExpirationDays { get; set; } = 21;

        /// <summary>
        /// record start and end byte, to make them readable, \r\n is appended. 
        /// </summary>
        public static readonly byte[] startBytes = new byte[4] { 10, 13, 65, 65 };
        public static readonly byte[] endBytes = new byte[4] { 10, 13, 65, 65 };


        private static DateTime _startdatetime;
        /// <summary>
        /// The start date time to convert date time to int64, in official paper, they used 1970-1-1. I use 2000.
        /// </summary>
        public static DateTime UTCStartdate
        {
            get
            {
                if (_startdatetime == null || _startdatetime == default(DateTime))
                {
                    _startdatetime = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                }
                return _startdatetime;
            }
        }

        // The records section on one time slot. = sanity 2, + counter 4, 
        // + 4 * 8  + 8 * 25. + 2 for nothing now.. 
        public static int ScheduleRecordsSectorLen = 236;

        private static System.Text.Encoding _defaultEncoding;
        /// <summary>
        /// Encoding of your string value, used to convert string to bytes and bytes to string.
        /// </summary>
        /// <returns></returns>
        public static System.Text.Encoding DefaultEncoding
        {
            get
            {
                if (_defaultEncoding != null)
                {
                    return _defaultEncoding;
                }
                else
                {
                    return System.Text.Encoding.UTF8;
                }
            }
            set
            {
                _defaultEncoding = value;
            }
        }

        /// <summary>
        /// The multiplier used to muliple disksectorsize for the size to write block file to disk. 
        /// This might impact your read/write performance. 
        /// To maxmize the read performance, make a size that is enough to cover most of your record.
        /// </summary>
        /// <returns></returns>
        public static int blockWriterSizeFactor = 2;

        /// <summary>
        /// The sector size of your disk, 
        /// </summary>
        public static int SectorSize = 512;

        /// <summary>
        /// this determine the size of keys per nodes.
        /// </summary>
        public static int MaxTreeNodeSizeMultiplier = 12;

        /// <summary>
        /// The max * times factors to times the disk sector size.  = buffersize to be used for node.
        /// </summary>
        public static int MaxTreeNodeSize = MaxTreeNodeSizeMultiplier * SectorSize;

        public static Int16 defaultKeyLength = 50; // the default max len of a string key.

        //NOTE. To delete a record, only need to mark the first two bytes as 0. 
        //NOTE. See the findpointer method. searched Key is on the right side of pointer.

        //change the merge or split may break the system when merge + split ratio > 1;
        public const double MergeRatio = 0.22;      // when to merge with other leaf/node.
        public const double SplitRatio = 0.60;      // when to split leaf/node. 

        /// <summary>
        /// The max level of cache...
        /// </summary>
        public static int DefaultTreeNodeCacheMaxLevel = 99;

    }
}
