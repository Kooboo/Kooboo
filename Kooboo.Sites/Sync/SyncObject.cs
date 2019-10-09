//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Sync
{
    public class SyncObject
    {
        public Guid ObjectId { get; set; }

        /// <summary>
        /// Action can be Add, Update or Delete, when = Delete, only objectId and ConstType is required;
        /// </summary>
        public bool IsDelete { get; set; }

        public byte ObjectConstType { get; set; }
        /// <summary>
        /// The Json data without the ContentByte[]; 
        /// </summary>
        public string JsonData { get; set; }

        /// <summary>
        /// The culture of text content....
        /// </summary>
        public string Language { get; set; }

        public string StoreName { get; set; }

        public string TableName { get; set; }

        public string TableColName { get; set; }

        public bool IsTable
        {
            get
            {
                return !string.IsNullOrWhiteSpace(TableName) && string.IsNullOrWhiteSpace(StoreName); 
            }
        }
        /// <summary>
        /// The encoding string of bytes.. It is base64 for now.
        /// </summary>
        public string EncodedByteString
        { get; set; }  

        /// <summary>
        /// Used for multi master synchronization. 
        /// </summary>
        public int Sender { get; set; }
        /// <summary>
        /// Used for multi master synchronization, this must be a sequence added number. 
        /// In the future, can make one node responsible for generating this number. 
        /// This is to ensure integrity in multi master situation. 
        /// </summary>
        public long SenderTick { get; set; }

        public long SenderVersion { get; set; }

        private Guid _checksum; 
        /// <summary>
        ///  A check code to make sure integrity during transfer... 
        /// </summary>
        public Guid CheckSum {
            get
            {
                if (_checksum == default(Guid))
                {
                    _checksum = GetCheckSum(); 
                }
                return _checksum; 
            }
            set
            {
                _checksum = value; 
            }
        }

        private Guid GetCheckSum()
        {
            string unique = this.ObjectId.ToString() + this.IsDelete.ToString() + this.JsonData + this.EncodedByteString;
            return unique.ToHashGuid(); 
        }

        public bool ValidateData()
        {
            var check = GetCheckSum();
            return check == CheckSum; 
        }  

        public int SenderPort { get; set; }
    }

    public static class DateTimeExtension
    {
        private static DateTime _startdatetime;
        /// <summary>
        /// The start date time to convert date time to int64, in official paper, they used 1970-1-1. I use 2000.
        /// </summary>
        private static DateTime UTCStartdate
        {
            get
            {
                if (_startdatetime == null || _startdatetime == default(DateTime))
                {
                    _startdatetime = new DateTime();
                    _startdatetime = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                }
                return _startdatetime;
            }
        }
        public static Int64 DayToInt(this DateTime date)
        {
            TimeSpan last = date - UTCStartdate;
            return (Int64)last.TotalSeconds;
        }

        /// <summary>
        /// use ID to represent a day, this is used for day key.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime FromInt64(this DateTime date, long datelong)
        {
            return UTCStartdate.AddMilliseconds(datelong);
        }

    }

}
