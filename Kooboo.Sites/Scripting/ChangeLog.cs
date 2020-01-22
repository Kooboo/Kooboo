using Kooboo.IndexedDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Scripting
{
    public class ChangeLog
    {
        public ChangeLog(LogEntry log)
        {
            this.UserId = log.UserId;
            this.editType = log.EditType;
            this.LogId = log.Id;
            this.EditTime = log.UpdateTime;
            this.StoreName = log.StoreName;
            this.TableName = log.TableName;
            if (this.UserId != default(Guid))
            {
                var user = Kooboo.Data.GlobalDb.Users.Get(this.UserId);
                if (user != null)
                {
                    this.UserName = user.UserName;
                }
            }
        }

        public string StoreName { get; set; }

        public string TableName { get; set; }

        public string UserName { get; set; }
        public Guid UserId { get; set; }

        public DateTime EditTime { get; set; }

        public EditType editType { get; set; }

        public long LogId { get; set; }
    }
}
