//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Models
{
    public class Notification : Kooboo.Data.Interface.ISiteObject
    {
        private Guid _id;
        public Guid Id
        {
            get
            {
                if (_id == default(Guid))
                {
                    _id = new Guid();
                }
                return _id;
            }
            set { _id = value; }
        }

        public string Message { get; set; }

        public byte ConstType
        {
            get; set;
        } = ConstObjectType.Notification;

        private string _name;
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name))
                {
                    if (!string.IsNullOrEmpty(this.Message))
                    {
                        return Kooboo.Lib.Helper.StringHelper.GetSummary(this.Message);
                    }
                }
                return _name;
            }
            set { _name = value; }
        }

        public bool IsRead { get; set; }

        public DateTime CreationDate
        {
            get; set;
        } = DateTime.Now;
  
       [JsonConverter(typeof(StringEnumConverter))]
        public NotifyType NotifyType { get; set; }


        private DateTime _lastmodify; 
        public DateTime LastModified
        {
            get
            {
                if (_lastmodify == default(DateTime))
                {
                    _lastmodify = DateTime.Now;
                }
                return _lastmodify;
            }
            set
            {
                _lastmodify = value;
                _lastmodifytick = default(long);
            }
        }

        private long _lastmodifytick;
        public long LastModifyTick
        {
            get
            {
                if (_lastmodifytick == default(long))
                {
                    _lastmodifytick = this.LastModified.Ticks;
                }
                return _lastmodifytick;
            }
            set
            {
                _lastmodifytick = value;
            }
        }

    }

    public enum NotifyType
    {
        Info,
        Warning,
        Error
    }
}
