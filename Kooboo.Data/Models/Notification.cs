//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Models
{
    public class Notification
    {
        public Notification()
        {
            Id = Guid.NewGuid();
            UtcCreationDate = DateTime.UtcNow;
        }

        public Guid Id { get; set; }

        public Guid WebSiteId { get; set; }

        /// <summary>
        /// publisher
        /// </summary>
        public Guid UserId { get; set; }

        public DateTime UtcCreationDate { get; set; }

        /// <summary>
        /// 已读用户
        /// </summary>
        public List<Guid> ReadUsers
        {
            get
            {
                var list = new List<Guid>();
                if (String.IsNullOrWhiteSpace(ReadUserIds))
                {
                    return list;
                }
                var ids = ReadUserIds.Split(',');
                foreach (var id in ids)
                {
                    Guid tempId;
                    if (Guid.TryParse(id, out tempId))
                    {
                        list.Add(tempId);
                    }
                }
                return list;
            }
            set
            {
                ReadUserIds = String.Join(",", value);
            }
        }

        public string ReadUserIds { get; set; }

        public string Summary { get; set; }

        public string Message { get; set; }

        /// <summary>
        /// Info=0,Warning=1,Error=2
        /// </summary>
        public NotificationLevel Level { get; set; }
    }

    public enum NotificationLevel
    {
        Info = 0,
        Success = 1,
        Warning = 2,
        Error = 3
    }


    public class Notificationnew : IGolbalObject
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
         
        public Guid OrganizationId { get; set; }

        public Guid WebSiteId { get; set; }

        public Guid UserId { get; set;  }

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

        public DateTime UtcCreationDate
        {
            get; set;
        } = DateTime.Now;

        public DateTime UtcLastModified
        {
            get; set;
        } = DateTime.Now;

        [JsonConverter(typeof(StringEnumConverter))]
        public NotifyType NotifyType { get; set; }

    }

    public enum NotifyType
    {
        Info,
        Warning,
        Error
    }

}
