//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Models
{
    /// <summary>
    /// The log of visitor activity in this website. 
    /// </summary>
    public class VisitorLog
    {
        public string ClientIP { get; set; }

        public string Referer { get; set; }

        public string UserAgent { get; set; }

        /// <summary>
        /// The client user id, this can be for example used by Ecommerce, when client login to shop. 
        /// </summary>
        public Guid UserId { get; set; }

        private DateTime _begin;
        public DateTime Begin
        {
            get
            {
                if (_begin == default(DateTime))
                {
                    _begin = DateTime.UtcNow;
                }
                return _begin;
            }
            set
            {
                _begin = DateTime.SpecifyKind(value, DateTimeKind.Utc);
            }
        }

        private DateTime _end;
        public DateTime End
        {
            get
            {
                if (_end == default(DateTime))
                {
                    _end = DateTime.UtcNow;
                }
                return _end;
            }
            set
            {
                _end = DateTime.SpecifyKind(value, DateTimeKind.Utc);
            }
        }

        public int Size { get; set; }

        public double TimeSpan { get; set; }

        public int MillionSecondTake
        {
            get
            { 
                return Convert.ToInt32(TimeSpan);
            }
        }

        public string PageName { get; set; }

        /// <summary>
        ///  image\view\layout\htmlblock....
        /// </summary>
        public byte ConstType { get; set; }

        /// <summary>
        /// the id of image\view\layout\htmlblock....
        /// </summary>
        public Guid ObjectId { get; set; }

        /// <summary>
        /// The request  urls. 
        /// </summary>
        public string Url { get; set; }

        public Int16 StatusCode { get; set; }

        private List<VisitorLogItem> _entries;

        public List<VisitorLogItem> Entries
        {
            get
            {
                if (_entries == null)
                {
                    _entries = new List<VisitorLogItem>();
                }
                return _entries;
            }

            set
            {
                _entries = value;
            }

        }

        /// <summary>
        /// Add a new entry to the log, for example. 
        /// product: 234, DateTime.now, 200
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="StartTime"></param>
        /// <param name="StatusCode"></param>
        public void AddEntry(string Name, string Value, DateTime StartTime, DateTime EndTime, Int16 StatusCode, string Detail = null)
        {
            this.Entries.Add(new VisitorLogItem()
            {
                Name = Name,
                Value = Value,
                StartTime = StartTime,
                EndTime = EndTime,
                StatusCode = StatusCode,
                Detail = Detail
            });
        }


        public class VisitorLogItem
        {
            public string Name;

            public string Value;
             
            public Int16 StatusCode;

            public string Detail { get; set; }
             
            private DateTime _StartTime;
            public DateTime StartTime
            {
                get
                {
                    if (_StartTime == default(DateTime))
                    {
                        _StartTime = DateTime.UtcNow;
                    }
                    return _StartTime;
                }
                set
                {
                    _StartTime = DateTime.SpecifyKind(value, DateTimeKind.Utc);
                }
            }

            private DateTime _EndTime;
            public DateTime EndTime
            {
                get
                {
                    if (_EndTime == default(DateTime))
                    {
                        _EndTime = DateTime.UtcNow;
                    }
                    return _EndTime;
                }
                set
                {
                    _EndTime = DateTime.SpecifyKind(value, DateTimeKind.Utc);
                }
            }
        } 
    }
}
