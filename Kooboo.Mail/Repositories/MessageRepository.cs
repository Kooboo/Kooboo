//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.IndexedDB;
using Kooboo.IndexedDB.Query;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Kooboo.Mail.Repositories
{
    public class MessageRepository : RepositoryBase<Message>
    {
        private MailDb maildb { get; set; }

        public MessageRepository(MailDb db) : base(db.Db)
        {
            this.maildb = db;
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

        public void Add(Message Message, string MessageBody)
        {
            Message.Id = 0; // to reset the message id. 
            var bodyposition = this.maildb.MsgBody.Add(MessageBody);
            Message.BodyPosition = bodyposition;
            this.maildb.MsgBody.Close();
            this.Add(Message);
        }

        public void Update(Message Message, string MessageBody)
        {
            var bodyposition = this.maildb.MsgBody.Add(MessageBody);
            Message.BodyPosition = bodyposition;
            this.maildb.MsgBody.Close();
            this.Update(Message);
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
            if (columnMsg !=null)
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

        public List<Message> ByFolder(string FolderName)
        {
            return FolderQuery(FolderName).SelectAll();
        }

        public WhereFilter<int, Message> FolderQuery(string FolderName)
        {
            var model = Kooboo.Mail.Utility.FolderUtility.ParseFolder(FolderName);

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

        public Message GetBySeqNo(string FolderName, int LastMaxId, int MessageCount, int SeqNo)
        {
            if (SeqNo < 1)
            {
                SeqNo = 1;
            }

            int skip = MessageCount - SeqNo;
            if (skip < 0)
            {
                skip = 0;
            }
            return FolderQuery(FolderName).Where(o => o.Id <= LastMaxId).Skip(skip).FirstOrDefault();
        }

        public List<Message> GetBySeqNos(string FolderName, int LastMaxId, int MessageCount, int lower, int upper)
        {
            if (lower < 1)
            {
                lower = 1;
            }

            int skip = MessageCount - upper;
            if (skip < 0)
            {
                skip = 0;
            }

            var result = FolderQuery(FolderName).Where(o => o.Id <= LastMaxId).Skip(skip).Take(upper - lower + 1);

            result.Reverse();
            return result;
        }

        public int GetSeqNo(string FolderName, int LastMaxId, int MessagetCount, int MsgId)
        {
            var query = FolderQuery(FolderName).Where(o => o.Id <= LastMaxId).Where(o => o.Id > MsgId);

            int count = query.Count();

            return MessagetCount - count;
        }

        public string GetContent(int id)
        {
            var mail = this.Get(id);

            if (mail != null)
            {
                return this.maildb.MsgBody.Get(mail.BodyPosition);
            }
            return null;
        }
         

        public void MarkAsRead(int MsgId, bool read = true)
        {
            this.Store.UpdateColumn<bool>(MsgId, o => o.Read, read);
        }

        public void UpdateRecent(int Id)
        {
            this.Store.UpdateColumn<bool>(Id, o => o.Recent, false);
        }

        // used with select.... select command should update the recent... 
        public void UpdateRecentByMaxId(int maxMsgId)
        {
            DateTime maxBeforeDate = DateTime.Now.AddMonths(-10);
            long tickbefore = maxBeforeDate.Ticks;
            var query = this.Query().OrderByAscending(o => o.Id).Where(o => o.CreationTimeTick > tickbefore && o.Recent == true && o.Id <= maxMsgId);

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

        public Models.MessageStat GetStat(string FolderName)
        {
            if (string.IsNullOrEmpty(FolderName))
            {
                return new Models.MessageStat();
            }
            int folderid = Folder.ToId(FolderName);
            if (FolderName.Contains("@"))
            {
                // our special folder with emailaddress.
                int index = FolderName.IndexOf("/");
                string realfolder = FolderName.Substring(0, index);
                folderid = Folder.ToId(realfolder);
                string emailaddress = FolderName.Substring(index + 1);
                var address = Kooboo.Mail.Utility.AddressUtility.GetLocalEmailAddress(emailaddress);

                if (address != null)
                {
                    return GetStat(folderid, address.Id);
                }
            }

            return GetStat(folderid);
        }

        public Models.MessageStat GetStat(int FolderId, int AddressId = 0)
        {
            DateTime maxBeforeDate = DateTime.Now.AddMonths(-10);
            long tickbefore = maxBeforeDate.Ticks;
            int maxrecord = 1000;

            var query = this.Query().UseColumnData();

            query.Where(o => o.FolderId == FolderId);

            if (AddressId != 0)
            {
                query.Where(o => o.AddressId == AddressId);
            }

            var lastestRecord = query.Take(maxrecord);

            var result = new Models.MessageStat();
            result.NextUid = this.Store.LastKey + 1;
            result.FolderUid = FolderId;
            if (lastestRecord == null || lastestRecord.Count == 0)
            {
                return result;
            }

            result.Exists = query.Count();
            result.UnSeen = lastestRecord.Where(o => !o.Read).Count();
            result.Recent = lastestRecord.Where(o => o.Recent).Count();

            if (result.UnSeen > 0)
            {
                var lastunseen = lastestRecord.Where(o => !o.Read).OrderByDescending(o => o.Id).First();
                result.FirstUnSeen = lastunseen.Id;
            }

            result.LastestMsgId = this.Store.LastKey;

            return result;
        }

        public string[] GetFlags(int MsgId)
        {
            var msg = this.Store.GetFromColumns(MsgId);
              
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

        public void AddFlags(int MsgId, string[] flags)
        {
            var msg = this.Store.GetFromColumns(MsgId);
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
