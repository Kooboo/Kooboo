//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kooboo.Data;
using Kooboo.IndexedDB;
using Kooboo.Mail.Repositories.Sqlite;
using MimeKit;

namespace Kooboo.Mail
{
    public class MailDb : IDisposable
    {
        private object _lock = new object();

        public MailDb(Guid UserId, Guid OrganizationId)
        {
            this.UserId = UserId;
            string folder = AppSettings.GetMailDbName(OrganizationId);
            string dbname = System.IO.Path.Combine(folder, UserId.ToString());
            Db = DB.GetDatabase(dbname);
            this.OrganizationId = OrganizationId;

            this.MsgFolder = System.IO.Path.Combine(dbname, "MailContent");
            Lib.Helper.IOHelper.EnsureDirectoryExists(MsgFolder);

            var mailDbFolder = AppSettings.GetMailDbFolder(OrganizationId, UserId);
            sqlitedbpath = System.IO.Path.Combine(mailDbFolder, "mail.db");
            InitMailDb(SqliteHelper);
        }

        public string MsgFolder
        {
            get; set;
        }

        public Guid OrganizationId { get; set; }

        public Guid UserId { get; set; }

        public Database Db { get; set; }


        public void Dispose()
        {
            this.Db.Close();
        }


        #region SQliteTable


        private FolderRepo _folder;

        public FolderRepo Folder
        {
            get
            {
                if (_folder == null)
                {
                    _folder = new FolderRepo(this);
                }
                return _folder;
            }
        }

        private AddressBookRepo _addBookRepo;

        public AddressBookRepo AddBook
        {
            get
            {
                if (_addBookRepo == null)
                {
                    _addBookRepo = new AddressBookRepo(this);
                }
                return _addBookRepo;
            }
        }

        private SmtpReportRepo _smtpDelivery;
        public SmtpReportRepo SmtpDelivery
        {
            get
            {
                if (_smtpDelivery == null)
                {
                    _smtpDelivery = new SmtpReportRepo(this);
                }
                return _smtpDelivery;
            }
        }

        private MessageRepo _message2;
        public MessageRepo Message2
        {
            get
            {
                if (_message2 == null)
                {
                    _message2 = new MessageRepo(this);
                }
                return _message2;
            }
        }

        private CalendarRepo _calendar;
        public CalendarRepo Calendar
        {
            get
            {
                if (_calendar == null)
                {
                    _calendar = new CalendarRepo(this);
                }
                return _calendar;
            }
        }

        private MailMigrationJobRepo _mailMigrationJob;
        public MailMigrationJobRepo MailMigrationJob
        {
            get
            {
                if (_mailMigrationJob == null)
                {
                    _mailMigrationJob = new MailMigrationJobRepo(this);
                }
                return _mailMigrationJob;
            }
        }

        private MailMigrationProgressRepo _mailMigrationProgress;
        public MailMigrationProgressRepo MailMigrationProgress
        {
            get
            {
                if (_mailMigrationProgress == null)
                {
                    _mailMigrationProgress = new MailMigrationProgressRepo(this);
                }
                return _mailMigrationProgress;
            }
        }

        private Sequence<string> _MsgBody2;

        public Kooboo.IndexedDB.Sequence<string> MsgBody2
        {
            get
            {
                if (_MsgBody2 == null)
                {
                    lock (_lock)
                    {
                        if (_MsgBody2 == null)
                        {
                            _MsgBody2 = this.Db.GetSequence<string>("msgbody2.mail");
                        }
                    }
                }
                return _MsgBody2;
            }
        }

        private MsgHandler _handler;
        public MsgHandler MsgHandler
        {
            get
            {
                if (_handler == null)
                {
                    _handler = new MsgHandler(this);
                }
                return _handler;
            }
        }

        #endregion


        #region SQLITE

        private SqlLiteHelper _sqliteHelper;

        public SqlLiteHelper SqliteHelper
        {
            get
            {
                if (_sqliteHelper == null)
                {
                    _sqliteHelper = new SqlLiteHelper(this.sqlitedbpath);
                    _sqliteHelper.KeepConnectionAlive = true;
                }
                return _sqliteHelper;
            }
        }

        internal string sqlitedbpath { get; set; }

        public static void InitMailDb(SqlLiteHelper sqlLiteHelper)
        {
            var tables = sqlLiteHelper.GetTables();
            if (tables == null)
            {
                tables = new List<string>();
            }

            if (!tables.Contains(nameof(Mail.Folder)))
            {
                sqlLiteHelper.Execute(new DDL().Folder());
            }

            if (!tables.Contains("AddressBook"))
            {
                sqlLiteHelper.Execute(new DDL().AddressBook());
            }

            if (!tables.Contains(nameof(Models.SmtpReport)))
            {
                sqlLiteHelper.Execute(new DDL().SmtpReport());
            }
            else
            {
                var columns = new[] {
                    new ColumnInfo("DateTick", "INTEGER DEFAULT(0)", true),
                    new ColumnInfo("Json"),
                    new ColumnInfo("Subject"),
                };
                EnsureColumns(sqlLiteHelper, nameof(Models.SmtpReport), columns);
            }

            if (!tables.Contains("Message"))
            {
                sqlLiteHelper.Execute(new DDL().Message());

                // System.Threading.Tasks.Task.Factory.StartNew(() => Importmsg());
                ///Importmsg();
            }
            else
            {
                EnsureColumn(sqlLiteHelper, nameof(Message), nameof(Message.InviteConfirm), "INTEGER DEFAULT 0");
            }

            if (!tables.Contains("Calendar"))
            {
                sqlLiteHelper.Execute(new DDL().ICalendar());
            }

            if (!tables.Contains("MailMigrationJob"))
            {
                sqlLiteHelper.Execute(new DDL().MailMigrationJob());
            }

            if (!tables.Contains("MailMigrationProgress"))
            {
                sqlLiteHelper.Execute(new DDL().MailMigrationProgress());
            }
            else
            {
                var columns = new[] {
                    new ColumnInfo(nameof(Models.MailMigrationProgress.AddressId), "INTEGER DEFAULT(0)", true),
                    new ColumnInfo(nameof(Models.MailMigrationProgress.Status), "TINYINT DEFAULT(0)", true),
                    new ColumnInfo(nameof(Models.MailMigrationProgress.Total), "INTEGER DEFAULT(0)"),
                    new ColumnInfo(nameof(Models.MailMigrationProgress.Completed), "INTEGER DEFAULT(0)"),
                    new ColumnInfo(nameof(Models.MailMigrationProgress.ErrorMessage)),
                };
                EnsureColumns(sqlLiteHelper, "MailMigrationProgress", columns);
            }
        }

        private static void EnsureColumn(SqlLiteHelper sqlLiteHelper, string tableName, string name, string type = "TEXT", bool withIndex = false)
        {
            EnsureColumns(sqlLiteHelper, tableName, new ColumnInfo(name, type, withIndex));
        }

        private static void EnsureColumns(SqlLiteHelper sqlLiteHelper, string tableName, params ColumnInfo[] columns)
        {
            var colums = sqlLiteHelper.GetColumns(tableName);
            var changed = false;
            var sqlBuilder = new StringBuilder();
            foreach (var column in columns)
            {
                var name = column.Name;
                var type = column.Type;
                var withIndex = column.WithIndex;
                var find = colums.Find(o => o.name == name);
                if (find == null)
                {
                    sqlBuilder.AppendLine(@$"ALTER TABLE [{tableName}] ADD [{name}] {type};");
                    if (withIndex)
                    {
                        sqlBuilder.AppendLine($"CREATE INDEX {tableName}_{name}_IDX ON {tableName}({name});");
                    }

                    changed = true;
                }
            }

            if (changed)
            {
                sqlLiteHelper.Execute(sqlBuilder.ToString());
                sqlLiteHelper.colCache.Clear();
            }
        }

        private class ColumnInfo
        {
            public string Name { get; set; }

            public string Type { get; set; }

            public bool WithIndex { get; set; }

            public ColumnInfo(string name, string type = "TEXT", bool withIndex = false)
            {
                Name = name;
                Type = type;
                WithIndex = withIndex;
            }
        }

        public void Importmsg()
        {
            try
            {
                //int currentindex = 0;

                //long maxid = this.Msgstore.LastKey;

                //int nextmax = currentindex + 500;

                //var items = this.Msgstore.Query.SearchRange(currentindex, nextmax).SelectAll();

                //// var items = this.Msgstore.Query.SelectAll();
                //// this.SqliteHelper.KeepConnectionAlive = true; it is always true now.

                //while (items != null)
                //{
                //    foreach (var item in items.OrderBy(o => o.CreationTimeTick))
                //    {
                //        try
                //        {
                //            if (item != null && !item.Deleted)
                //            {
                //                this.Message2.Add(item);
                //            }
                //        }
                //        catch (Exception)
                //        {

                //        }
                //    }

                //    currentindex = nextmax + 1;
                //    nextmax = currentindex + 500;

                //    if (currentindex > maxid + 500)
                //    {
                //        break;
                //    }

                //    items = this.Msgstore.Query.SearchRange(currentindex, nextmax).SelectAll();

                //}

            }
            catch (Exception)
            {

            }
            finally
            {

            }


        }

        #endregion

    }


    public class MsgHandler
    {
        public MailDb _maildb { get; set; }

        public MsgHandler(MailDb maildb)
        {
            _maildb = maildb;
        }


        public string GetMsgFileName(long MsgId, long BodyPosition)
        {
            string filename = System.IO.Path.Combine(_maildb.MsgFolder, MsgId.ToString() + ".eml");
            if (System.IO.File.Exists(filename))
            {
                return filename;
            }
            else
            {
                try
                {
                    var body = _maildb.MsgBody2.Get(BodyPosition);
                    SetMsgBody(MsgId, body);
                }
                catch (Exception)
                {

                }

                if (System.IO.File.Exists(filename))
                {
                    return filename;
                }
            }

            return null;
        }


        public string GetMsgBody(long MsgId, long BodyPosition)
        {
            string filename = System.IO.Path.Combine(_maildb.MsgFolder, MsgId.ToString() + ".eml");
            if (System.IO.File.Exists(filename))
            {
                return System.IO.File.ReadAllText(filename);
            }
            else
            {
                try
                {
                    var msgbody = _maildb.MsgBody2.Get(BodyPosition);
                    SetMsgBody(MsgId, msgbody);
                    return msgbody;
                }
                catch (Exception)
                {

                }
                return null;
            }
        }

        public MimeMessage GetRawMsgBody(long MsgId, long BodyPosition)
        {
            string filename = System.IO.Path.Combine(_maildb.MsgFolder, MsgId.ToString() + ".eml");
            if (System.IO.File.Exists(filename))
            {
                return MimeMessage.Load(filename);
            }
            else
            {
                try
                {
                    var msgbody = _maildb.MsgBody2.Get(BodyPosition);
                    SetMsgBody(MsgId, msgbody);
                    return MimeMessage.Load(filename);
                }
                catch (Exception)
                {

                }
                return null;
            }
        }

        public void SetMsgBody(long MsgId, string body)
        {
            if (body != null)
            {
                string filename = System.IO.Path.Combine(_maildb.MsgFolder, MsgId.ToString() + ".eml");
                System.IO.File.WriteAllText(filename, body);
            }
        }
        public void SetMsgBody(long MsgId, MimeMessage body)
        {
            if (body != null)
            {
                string filename = System.IO.Path.Combine(_maildb.MsgFolder, MsgId.ToString() + ".eml");
                body.WriteTo(filename);
            }
        }
        public void DeleteMsg(long MsgId)
        {
            string filename = System.IO.Path.Combine(_maildb.MsgFolder, MsgId.ToString() + ".eml");
            if (System.IO.File.Exists(filename))
            {
                System.IO.File.Delete(filename);
            }
        }
    }
}
