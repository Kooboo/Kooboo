//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.IndexedDB;
using Kooboo.IndexedDB.Query;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Mail.Repositories
{
    public class MessageRepository : RepositoryBase<Message>
    {
        private MailDb Maildb { get; set; }

        public MessageRepository(MailDb db) : base(db.Db)
        {
            this.Maildb = db;
        }

        protected override ObjectStoreParameters StoreParameters
        {
            get
            {
                ObjectStoreParameters paras = new ObjectStoreParameters();
                paras.AddColumn<Message>(it => it.Id);
                paras.AddColumn<Message>(it => it.AddressId);
                paras.AddColumn<Message>(it => it.UserId);
                paras.AddColumn<Message>(it => it.FolderId);
                paras.AddColumn<Message>(it => it.CreationTime);
                paras.AddColumn<Message>(it => it.Date);
                paras.AddColumn<Message>(it => it.Size);
                paras.AddColumn<Message>(it => it.Recent);
                paras.AddColumn<Message>(it => it.Answered);
                paras.AddColumn<Message>(it => it.Flagged);
                paras.AddColumn<Message>(it => it.Read);
                paras.AddColumn<Message>(it => it.Deleted);
                paras.AddColumn<Message>(it => it.CreationTimeTick);
                paras.SetPrimaryKeyField<Message>(o => o.Id);
                return paras;
            }
        }

        public void Add(Message message, string messageBody)
        {
            message.Id = 0; // to reset the message id.
            var bodyposition = this.Maildb.MsgBody.Add(messageBody);
            message.BodyPosition = bodyposition;
            this.Maildb.MsgBody.Close();
            this.Add(message);
        }

        public void Update(Message message, string messageBody)
        {
            var bodyposition = this.Maildb.MsgBody.Add(messageBody);
            message.BodyPosition = bodyposition;
            this.Maildb.MsgBody.Close();
            this.Update(message);
        }

        public override bool Add(Message value)
        {
            if (value.BodyPosition <= 0)
            {
                throw new Exception("Message body not found");
            }
            return base.Add(value);
        }

        public override bool AddOrUpdate(Message value)
        {
            // this is strange for now, and should be solved by database.

            var columnMsg = this.Store.GetFromColumns(value.Id);
            if (columnMsg != null)
            {
                value.Read = columnMsg.Read;
                value.Flagged = columnMsg.Flagged;
                value.Answered = columnMsg.Answered;
                value.Deleted = columnMsg.Deleted;
                value.Recent = columnMsg.Recent;
            }
            return base.AddOrUpdate(value);
        }

        public override bool Update(Message value)
        {
            var columnMsg = this.Store.GetFromColumns(value.Id);
            if (columnMsg != null)
            {
                value.Read = columnMsg.Read;
                value.Flagged = columnMsg.Flagged;
                value.Answered = columnMsg.Answered;
                value.Deleted = columnMsg.Deleted;
                value.Recent = columnMsg.Recent;
            }
            return base.Update(value);
        }

        public List<Message> ByFolder(string folderName)
        {
            return FolderQuery(folderName).SelectAll();
        }

        public WhereFilter<int, Message> FolderQuery(string folderName)
        {
            var model = Kooboo.Mail.Utility.FolderUtility.ParseFolder(folderName);

            var query = this.Query().OrderByDescending(o => o.Id).Where(o => o.FolderId == model.FolderId);
            if (model.AddressId != default(int))
            {
                query.Where(o => o.AddressId == model.AddressId);
            }
            return query;
        }

        public List<Message> ByUidRange(string folderName, int minId, int maxId)
        {
            return FolderQuery(folderName)
                .Where(o => o.Id >= minId)
                .Where(o => o.Id <= maxId)
                .Take(Int32.MaxValue);
        }

        public Message GetBySeqNo(string folderName, int lastMaxId, int messageCount, int seqNo)
        {
            if (seqNo < 1)
            {
                seqNo = 1;
            }

            int skip = messageCount - seqNo;
            if (skip < 0)
            {
                skip = 0;
            }
            return FolderQuery(folderName).Where(o => o.Id <= lastMaxId).Skip(skip).FirstOrDefault();
        }

        public List<Message> GetBySeqNos(string folderName, int lastMaxId, int messageCount, int lower, int upper)
        {
            if (lower < 1)
            {
                lower = 1;
            }

            int skip = messageCount - upper;
            if (skip < 0)
            {
                skip = 0;
            }

            var result = FolderQuery(folderName).Where(o => o.Id <= lastMaxId).Skip(skip).Take(upper - lower + 1);

            result.Reverse();
            return result;
        }

        public int GetSeqNo(string folderName, int lastMaxId, int messagetCount, int msgId)
        {
            var query = FolderQuery(folderName).Where(o => o.Id <= lastMaxId).Where(o => o.Id > msgId);

            int count = query.Count();

            return messagetCount - count;
        }

        public string GetContent(int id)
        {
            var mail = this.Get(id);

            if (mail != null)
            {
                return this.Maildb.MsgBody.Get(mail.BodyPosition);
            }
            return null;
        }

        public void MarkAsRead(int msgId, bool read = true)
        {
            this.Store.UpdateColumn<bool>(msgId, o => o.Read, read);
        }

        public void UpdateRecent(int id)
        {
            this.Store.UpdateColumn<bool>(id, o => o.Recent, false);
        }

        // used with select.... select command should update the recent...
        public void UpdateRecentByMaxId(int maxMsgId)
        {
            DateTime maxBeforeDate = DateTime.Now.AddMonths(-10);
            long tickbefore = maxBeforeDate.Ticks;
            var query = this.Query().OrderByAscending(o => o.Id).Where(o => o.CreationTimeTick > tickbefore && o.Recent && o.Id <= maxMsgId);

            var all = query.SelectAll();

            foreach (var item in all)
            {
                UpdateRecent(item.Id);
            }
        }

        public void Move(Message message, Folder toFolder)
        {
            message.FolderId = toFolder.Id;
            Update(message);
        }

        public Models.MessageStat GetStat(string folderName)
        {
            if (string.IsNullOrEmpty(folderName))
            {
                return new Models.MessageStat();
            }
            int folderid = Folder.ToId(folderName);
            if (folderName.Contains("@"))
            {
                // our special folder with emailaddress.
                int index = folderName.IndexOf("/");
                string realfolder = folderName.Substring(0, index);
                folderid = Folder.ToId(realfolder);
                string emailaddress = folderName.Substring(index + 1);
                var address = Kooboo.Mail.Utility.AddressUtility.GetLocalEmailAddress(emailaddress);

                if (address != null)
                {
                    return GetStat(folderid, address.Id);
                }
            }

            return GetStat(folderid);
        }

        public Models.MessageStat GetStat(int folderId, int addressId = 0)
        {
            DateTime maxBeforeDate = DateTime.Now.AddMonths(-10);
            long tickbefore = maxBeforeDate.Ticks;
            int maxrecord = 1000;

            var query = this.Query().UseColumnData();

            query.Where(o => o.FolderId == folderId);

            if (addressId != 0)
            {
                query.Where(o => o.AddressId == addressId);
            }

            var lastestRecord = query.Take(maxrecord);

            var result = new Models.MessageStat();
            result.NextUid = this.Store.LastKey + 1;
            result.FolderUid = folderId;
            if (lastestRecord == null || lastestRecord.Count == 0)
            {
                return result;
            }

            result.Exists = query.Count();
            result.UnSeen = lastestRecord.Count(o => !o.Read);
            result.Recent = lastestRecord.Count(o => o.Recent);

            if (result.UnSeen > 0)
            {
                var lastunseen = lastestRecord.Where(o => !o.Read).OrderByDescending(o => o.Id).First();
                result.FirstUnSeen = lastunseen.Id;
            }

            result.LastestMsgId = this.Store.LastKey;

            return result;
        }

        public string[] GetFlags(int msgId)
        {
            var msg = this.Store.GetFromColumns(msgId);

            var flags = new List<string>();
            if (msg.Recent)
            {
                flags.Add("Recent");
            }
            if (msg.Read)
            {
                flags.Add("Seen");
            }
            if (msg.Answered)
            {
                flags.Add("Answered");
            }
            if (msg.Flagged)
            {
                flags.Add("Flagged");
            }
            if (msg.Deleted)
            {
                flags.Add("Deleted");
            }
            return flags.ToArray();
        }

        public void AddFlags(int msgId, string[] flags)
        {
            var msg = this.Store.GetFromColumns(msgId);
            if (msg == null)
            {
                return;
            }
            if (flags.Contains("Seen", StringComparer.OrdinalIgnoreCase))
            {
                if (!msg.Read)
                {
                    this.Store.UpdateColumn<bool>(msg.Id, o => o.Read, true);
                }
            }
            if (flags.Contains("Answered", StringComparer.OrdinalIgnoreCase))
            {
                if (!msg.Answered)
                {
                    this.Store.UpdateColumn<bool>(msg.Id, o => o.Answered, true);
                }
            }

            if (flags.Contains("Flagged", StringComparer.OrdinalIgnoreCase))
            {
                if (!msg.Flagged)
                {
                    this.Store.UpdateColumn<bool>(msg.Id, o => o.Flagged, true);
                }
            }
            if (flags.Contains("Deleted", StringComparer.OrdinalIgnoreCase))
            {
                if (!msg.Deleted)
                {
                    this.Store.UpdateColumn<bool>(msg.Id, o => o.Deleted, true);
                }
            }
        }

        public void ReplaceFlags(int msgid, string[] flags)
        {
            var msg = this.Store.GetFromColumns(msgid);
            if (msg == null)
            {
                return;
            }

            if (flags.Contains("Seen", StringComparer.OrdinalIgnoreCase))
            {
                if (!msg.Read)
                {
                    this.Store.UpdateColumn<bool>(msg.Id, o => o.Read, true);
                }
            }
            else
            {
                if (msg.Read)
                {
                    this.Store.UpdateColumn<bool>(msg.Id, o => o.Read, false);
                }
            }

            if (flags.Contains("Answered", StringComparer.OrdinalIgnoreCase))
            {
                if (!msg.Answered)
                {
                    this.Store.UpdateColumn<bool>(msg.Id, o => o.Answered, true);
                }
            }
            else
            {
                if (msg.Answered)
                {
                    this.Store.UpdateColumn<bool>(msg.Id, o => o.Answered, false);
                }
            }

            if (flags.Contains("Flagged", StringComparer.OrdinalIgnoreCase))
            {
                if (!msg.Flagged)
                {
                    this.Store.UpdateColumn<bool>(msg.Id, o => o.Flagged, true);
                }
            }
            else
            {
                if (msg.Flagged)
                {
                    this.Store.UpdateColumn<bool>(msg.Id, o => o.Flagged, false);
                }
            }

            if (flags.Contains("Deleted", StringComparer.OrdinalIgnoreCase))
            {
                if (!msg.Deleted)
                {
                    this.Store.UpdateColumn<bool>(msg.Id, o => o.Deleted, true);
                }
            }
            else
            {
                if (msg.Deleted)
                {
                    this.Store.UpdateColumn<bool>(msg.Id, o => o.Deleted, false);
                }
            }
        }

        public void RemoveFlags(int msgid, string[] flags)
        {
            var msg = this.Store.GetFromColumns(msgid);
            if (msg == null)
            {
                return;
            }
            if (flags.Contains("Seen", StringComparer.OrdinalIgnoreCase))
            {
                if (msg.Read)
                {
                    this.Store.UpdateColumn<bool>(msg.Id, o => o.Read, false);
                }
            }
            if (flags.Contains("Answered", StringComparer.OrdinalIgnoreCase))
            {
                if (msg.Answered)
                {
                    this.Store.UpdateColumn<bool>(msg.Id, o => o.Answered, false);
                }
            }

            if (flags.Contains("Flagged", StringComparer.OrdinalIgnoreCase))
            {
                if (msg.Flagged)
                {
                    this.Store.UpdateColumn<bool>(msg.Id, o => o.Flagged, false);
                }
            }
            if (flags.Contains("Deleted", StringComparer.OrdinalIgnoreCase))
            {
                if (msg.Deleted)
                {
                    this.Store.UpdateColumn<bool>(msg.Id, o => o.Deleted, false);
                }
            }
        }
    }
}