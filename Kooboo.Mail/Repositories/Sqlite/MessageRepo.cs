using System;
using System.Collections.Generic;
using System.Linq;
using Kooboo.Mail.Imap;
using Kooboo.Mail.Models;
using MimeKit;

namespace Kooboo.Mail.Repositories.Sqlite
{
    public class MessageRepo
    {
        public string TableName
        {
            get
            {
                return "Message";
            }
        }

        private MailDb maildb { get; set; }

        public MessageRepo(MailDb db)
        {
            this.maildb = db;
        }

        public long LastKey
        {
            get
            {
                var folderId = Folder.ToId("inbox");
                var sql = $"SELECT Max(MsgId) FROM Message";
                return this.maildb.SqliteHelper.ExecuteScalar<int>(sql);
            }
        }

        public Message Get(long Id)
        {
            var msg = this.maildb.SqliteHelper.Get<Message>(this.TableName, nameof(Message.MsgId), Id);

            EnsureBody(msg);
            return msg;
        }
        public Message GetByMessageId(string messageId)
        {
            var msg = this.maildb.SqliteHelper.Get<Message>(this.TableName, nameof(Message.SmtpMessageId), messageId);

            EnsureBody(msg);
            return msg;
        }

        public List<Message> GetList()
        {
            var msg = this.maildb.SqliteHelper.All<Message>(TableName);

            foreach (var item in msg)
            {
                EnsureBody(item);
            }
            return msg;
        }

        public Message Get(string smtpMessageId, Guid userId, int addressId, int folderId)
        {
            var msg = maildb.SqliteHelper.Get<Message>(TableName, new Dictionary<string, object>
            {
                [nameof(Message.SmtpMessageId)] = smtpMessageId,
                [nameof(Message.UserId)] = userId,
                [nameof(Message.AddressId)] = addressId,
                [nameof(Message.FolderId)] = folderId
            });
            EnsureBody(msg);
            return msg;
        }


        private void EnsureBody(Message msg)
        {
            if (msg != null && string.IsNullOrEmpty(msg.Body))
            {
                msg.Body = this.maildb.MsgHandler.GetMsgBody(msg.MsgId, msg.BodyPosition);  // .MsgBody2.Get(msg.BodyPosition);
            }
        }

        public Message GetMeta(long id)
        {
            return this.maildb.SqliteHelper.Get<Message>(this.TableName, nameof(Message.MsgId), id);
        }


        public void Add(Message msg, string MessageBody)
        {
            msg.MsgId = 0; // reset id.
            //var bodypos = this.maildb.MsgBody2.Add(MessageBody);
            //msg.BodyPosition = bodypos;
            var newid = this.maildb.SqliteHelper.AddGetId<Message>(msg, this.TableName, nameof(Message.MsgId));
            msg.MsgId = newid;

            this.maildb.MsgHandler.SetMsgBody(newid, MessageBody);

        }
        public void AddAndWriteMimeMessage(Message msg, MimeMessage MessageBody)
        {
            msg.MsgId = 0; // reset id.
            //var bodypos = this.maildb.MsgBody2.Add(MessageBody);
            //msg.BodyPosition = bodypos;
            var newid = this.maildb.SqliteHelper.AddGetId<Message>(msg, this.TableName, nameof(Message.MsgId));
            msg.MsgId = newid;

            this.maildb.MsgHandler.SetMsgBody(newid, MessageBody);

        }

        public void UpdateSent(Message msg, string messageBody)
        {
            var find = this.maildb.SqliteHelper.Where<Message>(this.TableName)
                .Where(x => x.AddressId == msg.AddressId)
                .Where(x => x.SmtpMessageId == msg.SmtpMessageId).FirstOrDefault();
            if (find is not null)
                this.Update(find);
        }

        public void Update(Message msg, string MessageBody)
        {
            if (msg.MsgId == 0)
            {
                return;   // can not find record. 
            }

            var current = this.Get(msg.MsgId);

            if (current != null)
            {
                if (current.Body != MessageBody)
                {
                    //  msg.BodyPosition = this.maildb.MsgBody2.Add(MessageBody);
                    this.maildb.MsgHandler.SetMsgBody(current.MsgId, MessageBody);
                }

                this.maildb.SqliteHelper.Update<Message>(msg, this.TableName, nameof(Message.MsgId));
            }
        }

        public bool Add(Message value)
        {
            if (value == null)
            {
                return false;
            }

            this.Add(value, value.Body);
            return true;
        }

        public bool Add(Message value, MimeMessage rawMail)
        {
            if (value == null)
            {
                return false;
            }

            this.AddAndWriteMimeMessage(value, rawMail);
            return true;
        }

        public bool AddOrUpdate(Message value)
        {
            if (value == null)
            {
                return false;
            }

            if (value.MsgId == 0)
            {
                return this.Add(value);
            }
            else
            {
                var exists = this.Get(value.MsgId);

                if (exists != null)
                {
                    if (exists.Body != value.Body && !string.IsNullOrEmpty(value.Body))
                    {
                        this.maildb.MsgHandler.SetMsgBody(exists.MsgId, value.Body);
                        //var bodypos = this.maildb.MsgBody2.Add(value.Body);
                        //value.BodyPosition = bodypos;
                    }

                    this.UpdateMeta(value);
                    return true;
                }
                else
                {
                    return this.Add(value);
                }
            }

        }
        public bool AddOrUpdate(Message value, MimeMessage rawMail)
        {
            if (value == null)
            {
                return false;
            }

            value.Body = null;
            if (value.MsgId == 0)
            {
                return this.Add(value, rawMail);
            }
            else
            {
                var exists = this.Get(value.MsgId);

                if (exists != null)
                {
                    if (rawMail != null)
                    {
                        this.maildb.MsgHandler.SetMsgBody(exists.MsgId, value.Body);
                    }

                    this.UpdateMeta(value);
                    return true;
                }
                else
                {
                    return this.Add(value, rawMail);
                }
            }

        }
        public bool Update(Message value)
        {
            if (value == null)
            {
                return false;
            }
            var exists = this.Get(value.MsgId);

            if (exists != null)
            {
                if (exists.Body != value.Body && !string.IsNullOrEmpty(value.Body))
                {
                    this.maildb.MsgHandler.SetMsgBody(exists.MsgId, value.Body);
                    //var bodypos = this.maildb.MsgBody2.Add(value.Body);
                    //value.BodyPosition = bodypos;
                }

                this.UpdateMeta(value);
                return true;
            }
            //this.worm.Update(value); return true; 
            return false;
        }

        public void UpdateMeta(Message msg)
        {
            if (msg == null)
            {
                return;
            }
            this.maildb.SqliteHelper.Update<Message>(msg, this.TableName, nameof(Message.MsgId));
            //this.worm.UpdateMeta(msg);
        }

        public void Delete(long MsgId)
        {
            this.maildb.SqliteHelper.Delete(this.TableName, nameof(Message.MsgId), MsgId);
            this.maildb.MsgHandler.DeleteMsg(MsgId);
        }

        public SqlWhere<Message> Query
        {
            get
            {
                return new SqlWhere<Message>(this.maildb.SqliteHelper, this.TableName);
            }
        }


        public List<Message> ToFullMessage(List<Message> metaMsgs)
        {
            List<Message> fullMsg = new List<Message>();
            foreach (var item in metaMsgs)
            {
                if (item != null && !item.Deleted)
                {
                    item.maildb = this.maildb;

                    fullMsg.Add(item);
                }
            }
            return fullMsg;
        }

        public Message ToFullMessage(Message msg)
        {
            if (msg == null || msg.Deleted) return null;

            msg.maildb = this.maildb;
            return msg;
        }

        public List<Message> ByUidRange(SelectFolder folder, int minId, int maxId)
        {
            var sql = "SELECT * FROM " + this.TableName + " WHERE ";
            sql += nameof(Message.FolderId) + "=" + this.maildb.SqliteHelper.ObjToString(folder.FolderId);

            if (folder.AddressId != 0)
            {
                sql += " AND " + nameof(Message.AddressId) + "=" + this.maildb.SqliteHelper.ObjToString(folder.AddressId);
            }

            sql += " AND " + nameof(Message.MsgId) + "<=" + maxId.ToString();
            sql += " AND " + nameof(Message.MsgId) + ">=" + minId.ToString();
            sql += " Order By " + nameof(Message.MsgId) + " ASC";

            var results = this.maildb.SqliteHelper.Query<Message>(sql).ToList();
            return ToFullMessage(results);
        }

        public Message GetBySeqNo(SelectFolder folder, int SeqNo)
        {
            if (SeqNo < 1)
            {
                SeqNo = 1;
            }

            int skip = SeqNo - 1;

            var sql = "SELECT * FROM " + this.TableName + " WHERE " + nameof(Message.FolderId) + "=" + this.maildb.SqliteHelper.ObjToString(folder.FolderId);
            if (folder.AddressId != 0)
            {
                sql += " AND " + nameof(Message.AddressId) + "=" + this.maildb.SqliteHelper.ObjToString(folder.AddressId);
            }

            sql += " Order By MsgId ASC Limit " + skip.ToString() + ", 1";

            var msg = this.maildb.SqliteHelper.Get<Message>(sql);
            return ToFullMessage(msg);

        }

        public List<Message> GetBySeqNos(SelectFolder folder, int lower, int upper)
        {
            if (lower < 1)
            {
                lower = 1;
            }
            int skip = lower - 1;

            var take = upper - lower + 1;


            var sql = "SELECT * FROM " + this.TableName + " WHERE " + nameof(Message.FolderId) + "=" + this.maildb.SqliteHelper.ObjToString(folder.FolderId);

            if (folder.AddressId != 0)
            {
                sql += " AND " + nameof(Message.AddressId) + "=" + this.maildb.SqliteHelper.ObjToString(folder.AddressId);
            }

            sql += " Order By MsgId ASC Limit " + skip.ToString() + "," + take.ToString();

            var msgs = this.maildb.SqliteHelper.Query<Message>(sql);

            if (msgs != null && msgs.Any())
            {
                return ToFullMessage(msgs.ToList());
            }

            return null;

        }

        public int GetSeqNo(SelectFolder folder, long MsgId)
        {

            string sql = "SELECT COUNT(*) FROM " + this.TableName;
            sql += " WHERE " + nameof(Message.FolderId) + "=" + this.maildb.SqliteHelper.ObjToString(folder.FolderId);

            sql += " AND " + nameof(Message.MsgId) + "<" + this.maildb.SqliteHelper.ObjToString(MsgId);

            if (folder.AddressId != 0)
            {
                sql += " AND " + nameof(Message.AddressId) + "=" + this.maildb.SqliteHelper.ObjToString(folder.AddressId);
            }

            var count = this.maildb.SqliteHelper.ExecuteScalar<int>(sql);

            return count + 1;

        }

        public string GetContent(long id)
        {
            var item = this.Get(id);
            return item != null ? item.Body : null;
        }
        public MimeMessage GetMimeMessageContent(long id)
        {
            var item = this.Get(id);
            return maildb.MsgHandler.GetRawMsgBody(id, item.BodyPosition);
        }

        public void MarkAsRead(long MsgId, bool read = true)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add(nameof(Message.Read), read);
            this.maildb.SqliteHelper.Update(this.TableName, nameof(Message.MsgId), MsgId, data);
        }

        public void UpdateRecent(long Id)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add(nameof(Message.Recent), false);

            this.maildb.SqliteHelper.Update(this.TableName, nameof(Message.MsgId), Id, data);
        }

        // used with select.... select command should update the recent... 
        public void UpdateRecentByMaxId(long maxMsgId)
        {
            string sql = "SELECT * FROM " + this.TableName + " WHERE " + nameof(Message.Recent) + "=" + this.maildb.SqliteHelper.ObjToString(true) + " AND " + nameof(Message.MsgId) + "<" + maxMsgId.ToString();

            var allitems = this.maildb.SqliteHelper.Query<Message>(sql);

            foreach (var item in allitems)
            {
                this.UpdateRecent(item.MsgId);
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
            var lastkey = this.LastKey;

            string Sql = "SELECT COUNT(*)  as [Exists], SUM(Read) as Seen, SUM(Recent) as Recent FROM " + this.TableName + " WHERE " + nameof(Message.Deleted) + "=" + this.maildb.SqliteHelper.ObjToString(false) + " AND " + nameof(Message.FolderId) + "=" + this.maildb.SqliteHelper.ObjToString(FolderId);

            if (AddressId != 0)
            {
                Sql += " AND " + nameof(Message.AddressId) + "=" + this.maildb.SqliteHelper.ObjToString(AddressId);
            }


            var result = this.maildb.SqliteHelper.Get<Models.MessageStat>(Sql);

            result.NextUid = Convert.ToInt32(lastkey) + 1;
            result.FolderUid = FolderId;

            if (result.UnSeen > 0)
            {
                var sqlLast = "SELECT " + nameof(Message.MsgId) + " FROM " + this.TableName + " WHERE " + nameof(Message.Read) + "=" + this.maildb.SqliteHelper.ObjToString(false) + " Order By " + nameof(Message.MsgId) + " DESC Limit 1";

                result.FirstUnSeen = this.maildb.SqliteHelper.ExecuteScalar<int>(sqlLast);
            }

            result.LastestMsgId = (int)lastkey;

            return result;
        }

        private List<string> flaglist()
        {
            List<string> flags = new List<string>();
            flags.Add("Recent");
            flags.Add("Seen");
            flags.Add("Answered");
            flags.Add("Flagged");
            flags.Add("Deleted");
            return flags;
        }

        public string[] GetFlags(long MsgId)
        {
            var msg = this.GetMeta(MsgId);

            return GetFlags(msg);
        }
        public string[] GetFlags(Message msg)
        {
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

        public void AddFlags(long MsgId, string[] flags)
        {
            var msg = this.GetMeta(MsgId);
            if (msg == null)
            {
                return;
            }

            Dictionary<string, object> updatedata = new Dictionary<string, object>();

            if (flags.Contains("Seen", StringComparer.OrdinalIgnoreCase))
            {
                //msg.Read = true;
                updatedata.Add(nameof(Message.Read), true);
            }
            if (flags.Contains("Answered", StringComparer.OrdinalIgnoreCase))
            {
                //msg.Answered = true;
                updatedata.Add(nameof(Message.Answered), true);
            }

            if (flags.Contains("Flagged", StringComparer.OrdinalIgnoreCase))
            {
                //msg.Flagged = true;
                updatedata.Add(nameof(Message.Flagged), true);
            }

            if (flags.Contains("Deleted", StringComparer.OrdinalIgnoreCase))
            {
                //msg.Deleted = true;
                updatedata.Add(nameof(Message.Deleted), true);
            }
            //this.worm.UpdateMeta(msg); 
            this.maildb.SqliteHelper.Update(this.TableName, nameof(Message.MsgId), MsgId, updatedata);
        }

        public void ReplaceFlags(long msgid, string[] flags)
        {
            var msg = this.GetMeta(msgid);
            if (msg == null) return;

            Dictionary<string, object> updatedata = new Dictionary<string, object>();
            updatedata.Add(nameof(Message.Read), flags.Contains("Seen", StringComparer.OrdinalIgnoreCase));
            updatedata.Add(nameof(Message.Answered), flags.Contains("Answered", StringComparer.OrdinalIgnoreCase));
            updatedata.Add(nameof(Message.Flagged), flags.Contains("Flagged", StringComparer.OrdinalIgnoreCase));
            updatedata.Add(nameof(Message.Deleted), flags.Contains("Deleted", StringComparer.OrdinalIgnoreCase));

            //msg.Read = flags.Contains("Seen", StringComparer.OrdinalIgnoreCase);
            //msg.Answered = flags.Contains("Answered", StringComparer.OrdinalIgnoreCase);
            //msg.Flagged = flags.Contains("Flagged", StringComparer.OrdinalIgnoreCase);
            //msg.Deleted = flags.Contains("Deleted", StringComparer.OrdinalIgnoreCase); 
            //this.worm.UpdateMeta(msg);
            this.maildb.SqliteHelper.Update(this.TableName, nameof(Message.MsgId), msgid, updatedata);
        }

        public void RemoveFlags(long msgid, string[] flags)
        {
            var msg = this.GetMeta(msgid);
            if (msg == null)
            {
                return;
            }
            Dictionary<string, object> updatedata = new Dictionary<string, object>();

            if (flags.Contains("Seen", StringComparer.OrdinalIgnoreCase))
            {
                //msg.Read = false;
                updatedata.Add(nameof(Message.Read), false);
            }
            if (flags.Contains("Answered", StringComparer.OrdinalIgnoreCase))
            {
                //msg.Answered = false;
                updatedata.Add(nameof(Message.Answered), false);
            }

            if (flags.Contains("Flagged", StringComparer.OrdinalIgnoreCase))
            {
                //msg.Flagged = false;
                updatedata.Add(nameof(Message.Flagged), false);
            }
            if (flags.Contains("Deleted", StringComparer.OrdinalIgnoreCase))
            {
                //msg.Deleted = false;
                updatedata.Add(nameof(Message.Deleted), false);
            }
            //this.worm.UpdateMeta(msg);
            this.maildb.SqliteHelper.Update(this.TableName, nameof(Message.MsgId), msgid, updatedata);
        }

        public List<UnreadCounter> AddressUnread(int FolderId)
        {
            var sql = "SELECT " + nameof(Message.AddressId) + ", COUNT(*)  as [Exists], SUM(Read) as Read FROM " + this.TableName;
            sql += " WHERE " + nameof(Message.FolderId) + "=" + this.maildb.SqliteHelper.ObjToString(FolderId);
            sql += " Group By " + nameof(Message.AddressId);

            return this.maildb.SqliteHelper.Query<UnreadCounter>(sql).ToList();
        }

        public UnreadCounter FolderUnread(int FolderId)
        {
            var sql = "SELECT " + nameof(Message.AddressId) + ", COUNT(*)  as [Exists], SUM(Read) as Read FROM " + this.TableName;
            sql += " WHERE " + nameof(Message.FolderId) + "=" + this.maildb.SqliteHelper.ObjToString(FolderId);

            return this.maildb.SqliteHelper.Get<UnreadCounter>(sql);
        }

        public Message GetBySmtpMessageId(string smtpMessageId)
        {
            var msg = this.maildb.SqliteHelper.Get<Message>(this.TableName, nameof(Message.SmtpMessageId), smtpMessageId);

            EnsureBody(msg);
            return msg;
        }
    }
}
