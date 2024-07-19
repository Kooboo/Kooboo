//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Linq;
using Kooboo.Mail.Models;
using Kooboo.Mail.Repositories.Sqlite;

namespace Kooboo.Mail
{
    public class OrgDb : IDisposable
    {
        public OrgDb(Guid organizationId)
        {
            OrganizationId = organizationId;
            this.DbFolder = Kooboo.Data.AppSettings.GetMailDbName(OrganizationId);
            InitSqliteDb(DbFolder);
        }

        private string DbFolder { get; set; }

        public Guid OrganizationId { get; set; }


        private EmailAddressRepo _email;
        public EmailAddressRepo Email
        {
            get
            {
                if (_email == null)
                {
                    _email = new EmailAddressRepo(this);
                }
                return _email;
            }
        }

        private MailModuleRepo _module;
        public MailModuleRepo Module
        {
            get
            {
                if (_module == null)
                {
                    _module = new MailModuleRepo(this);
                }
                return _module;
            }
        }

        private DmarcReportRepo _dmarc;

        public DmarcReportRepo Dmarc
        {
            get
            {
                if (_dmarc == null)
                {
                    _dmarc = new DmarcReportRepo(this);
                }
                return _dmarc;
            }
        }

        public void Dispose()
        {

        }


        // only for local.
        public SmtpSetting SmtpGet()
        {
            string FileName = System.IO.Path.Combine(this.DbFolder, "smtpout.config");

            var setting = new SmtpSetting();

            if (System.IO.File.Exists(FileName))
            {
                var allText = System.IO.File.ReadAllText(FileName);
                setting = System.Text.Json.JsonSerializer.Deserialize<SmtpSetting>(allText);
            }

            return setting;
        }

        public void SmtpUpdate(SmtpSetting setting)
        {
            string FileName = System.IO.Path.Combine(this.DbFolder, "smtpout.config");

            var json = System.Text.Json.JsonSerializer.Serialize(setting);

            System.IO.File.WriteAllText(FileName, json);
        }




        #region SQLITE

        private SqlLiteHelper _sqliteHelper;

        public SqlLiteHelper SqliteHelper
        {
            get
            {
                if (_sqliteHelper == null)
                {
                    _sqliteHelper = new SqlLiteHelper(this.sqlitedbpath);
                }
                return _sqliteHelper;
            }
        }

        private string sqlitedbpath { get; set; }
        public void InitSqliteDb(string dbPath)
        {
            sqlitedbpath = System.IO.Path.Combine(dbPath, "org.db");

            var tables = this.SqliteHelper.GetTables();

            if (tables == null || !tables.Contains("EmailAddress"))
            {
                this.SqliteHelper.Execute(new DDL().EmailAddress());
            }
            else
            {
                var colums = this.SqliteHelper.GetColumns("EmailAddress");
                bool change = false;
                var find = colums.Find(o => o.name == "IsDefault");
                if (find == null)
                {
                    var sql = @"ALTER TABLE EmailAddress ADD IsDefault INTEGER DEFAULT 0;";
                    this.SqliteHelper.Execute(sql);
                    change = true;
                }

                find = colums.Find(o => o.name == "Signature");
                if (find == null)
                {
                    var sql = @"ALTER TABLE EmailAddress ADD Signature TEXT;";
                    this.SqliteHelper.Execute(sql);
                    change = true;
                }

                find = colums.Find(o => o.name == "AuthorizationCode");
                if (find == null)
                {
                    var sql = @"ALTER TABLE EmailAddress ADD AuthorizationCode TEXT;";
                    this.SqliteHelper.Execute(sql);
                    change = true;
                }

                if (change)
                {
                    this.SqliteHelper.colCache.Clear();
                }
            }

            if (tables == null || !tables.Contains("MailModule"))
            {
                this.SqliteHelper.Execute(new DDL().MailModule());
            }

            if (tables == null || !tables.Contains("DmarcReport"))
            {
                this.SqliteHelper.Execute(new DDL().DmarcReport());

            }
        }

        #endregion
    }
}