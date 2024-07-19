namespace Kooboo.Mail.Repositories.Sqlite
{
    public class DDL
    {

        #region maildb


        public string Folder()
        {
            return @"CREATE TABLE [Folder](
  [Id] INTEGER PRIMARY KEY ON CONFLICT IGNORE UNIQUE ON CONFLICT IGNORE, 
  [Name] TEXT, 
  [Subscribed] INTEGER DEFAULT (0));
";
        }


        public string SmtpReport()
        {
            return @"CREATE TABLE SmtpReport (
	Id TEXT,
	IsSuccess INTEGER,
	Log TEXT,
	MessageId TEXT,
	HeaderFrom TEXT,
	Json TEXT,
	DateTick INTEGER DEFAULT (0),
	Subject TEXT,
	CONSTRAINT SMTPREPORT_PK PRIMARY KEY (Id)
);

CREATE INDEX SmtpReport_DateTick_IDX ON SmtpReport (DateTick);";
        }


        public string AddressBook()
        {
            return @"CREATE TABLE AddressBook (
    Id          INTEGER PRIMARY KEY ON CONFLICT IGNORE
                        NOT NULL,
    FullAddress TEXT,
    Address     TEXT,
    Name        TEXT
);
";
        }


        public string Message()
        {

            return @"CREATE TABLE Message (
    MsgId         INTEGER PRIMARY KEY AUTOINCREMENT,
    SmtpMessageId TEXT,
    UserId        TEXT,
    AddressId     INTEGER,
    OutGoing      INTEGER,
    FolderId      INTEGER,
    MailFrom      TEXT,
    RcptTo        TEXT,
    [From]        TEXT,
    [To]          TEXT,
    Cc            TEXT,
    Bcc           TEXT,
    Subject       TEXT,
    BodyPosition  INTEGER,
    Body          TEXT,
    Summary       TEXT,
    Size          INTEGER,
    Read          INTEGER,
    Answered      INTEGER,
    Deleted       INTEGER,
    Flagged       INTEGER,
    Recent        INTEGER,
    CreationTime  TEXT,
    Date          TEXT,
    Draft         INTEGER,
    Attachments   TEXT, 
CreationTimeTick  INTEGER,
InviteConfirm  INTEGER
);
CREATE INDEX AID ON Message (AddressId);
CREATE INDEX FId ON Message (FolderId ASC);
CREATE INDEX CID ON Message (CreationTimeTick); 
";

        }

        public string ICalendar()
        {
            return @"CREATE TABLE Calendar (
    Id            TEXT PRIMARY KEY
                       UNIQUE
                       NOT NULL,
    CalendarTitle TEXT,
    Start         TEXT,
    End           TEXT,
    Location      TEXT,
    Organizer     TEXT,
    Attendees     TEXT,
    Mark          TEXT,
    UserId        TEXT,
    Status        INTEGER
);";
        }

        public string MailMigrationJob()
        {
            return @"CREATE TABLE [MailMigrationJob](
  [Id] TEXT PRIMARY KEY NOT NULL UNIQUE, 
  [Name] TEXT NOT NULL, 
  [StartTime] DATETIME, 
  [Active] BOOLEAN, 
  [Status] TINYINT, 
  [ErrorMessage] TEXT, 
  [UserId] TEXT NOT NULL, 
  [OrganizationId] TEXT NOT NULL, 
  [CreationDate] TIMESTAMP, 
  [LastModified] TIMESTAMP, 
  [EmailAddress] TEXT NOT NULL, 
  [Host] TEXT NOT NULL, 
  [ForceSSL] BOOLEAN DEFAULT TRUE, 
  [Port] INTEGER NOT NULL DEFAULT 993, 
  [Password] TEXT NOT NULL, 
  [AddressId] INTEGER NOT NULL);

CREATE UNIQUE INDEX [EmailAddress_AddressId_Unique]
ON [MailMigrationJob](
  [EmailAddress], 
  [AddressId]);
";
        }

        public string MailMigrationProgress()
        {
            return @"CREATE TABLE [MailMigrationProgress](
  [Id] TEXT NOT NULL UNIQUE, 
  [JobId] TEXT NOT NULL REFERENCES [MailMigrationJob]([Id]) ON DELETE CASCADE, 
  [FolderId] INTEGER NOT NULL, 
  [StartIndex] INTEGER DEFAULT 0, 
  [AddressId] INTEGER DEFAULT (0), 
  [Status] TINYINT DEFAULT (0), 
  [Total] INTEGER DEFAULT (0), 
  [Completed] INTEGER DEFAULT (0), 
  [ErrorMessage] TEXT);

CREATE INDEX [MailMigrationProgress_Status_IDX]
ON [MailMigrationProgress]([Status]);

CREATE INDEX [MailMigrationProgress_JobId_FolderId_AddressId_IDX]
ON [MailMigrationProgress](
  [JobId], 
  [FolderId], 
  [AddressId]);

CREATE INDEX [MailMigrationProgress_FolderId_IDX]
ON [MailMigrationProgress]([FolderId]);

CREATE INDEX [MailMigrationProgress_AddressId_IDX]
ON [MailMigrationProgress]([AddressId]);

CREATE INDEX [MailMigrationProgress_JobId_IDX]
ON [MailMigrationProgress]([JobId]);
";
        }

        #endregion



        #region Organization



        public string EmailAddress()
        {
            var sql = @"CREATE TABLE EmailAddress (
    Id             INTEGER PRIMARY KEY
                           NOT NULL,
    Name           TEXT,
    Address        TEXT    UNIQUE
                           NOT NULL,
    AddressType    TEXT    NOT NULL,
    UserId         TEXT,
    OrgId          TEXT,
    ForwardAddress TEXT,
    Members        TEXT,
    Signature      TEXT,
    IsDefault      INTEGER,
    AuthorizationCode TEXT
);
";
            return sql;
        }



        public string MailModule()
        {

            return @"CREATE TABLE MailModule (
    Id       TEXT    PRIMARY KEY
                     NOT NULL,
    Name     TEXT    UNIQUE,
    Settings TEXT,
    TaskJs   TEXT,
    Online   INTEGER
);";

        }


        public string DmarcReport()
        {

            return @"CREATE TABLE DmarcReport (
    Id           INTEGER PRIMARY KEY AUTOINCREMENT
                         UNIQUE,
    ReportDomain TEXT,
    OurDomain    TEXT,
    Start        TEXT,
    [End]        TEXT,
    Spf          TEXT,
    Dkim         TEXT,
    Count        INTEGER,
    SourceIP     TEXT,
    MailFrom     TEXT,
    RcptTo       TEXT,
    MsgFrom      TEXT
);";

        }

        #endregion


    }


}



/*
 *         private string EmailAddressDDL
        {
            get
            {

                var sql = @"CREATE TABLE EmailAddress (
    Id             INTEGER PRIMARY KEY
                           NOT NULL,
    Name           TEXT,
    Address        TEXT    UNIQUE
                           NOT NULL,
    AddressType    TEXT    NOT NULL,
    UserId         TEXT,
    OrgId          TEXT,
    ForwardAddress TEXT,
    Members        TEXT
);
";

                return sql;

            }
        }

        private string MailModuleDDL
        {
            get
            {
                return @"CREATE TABLE MailModule (
    Id       TEXT    PRIMARY KEY
                     NOT NULL,
    Name     TEXT    UNIQUE,
    Settings TEXT,
    TaskJs   TEXT,
    Online   INTEGER
);";
            }
        }


        private string DmarcReportDDL
        {
            get
            {
                return @"CREATE TABLE DmarcReport (
    Id           INTEGER PRIMARY KEY AUTOINCREMENT
                         UNIQUE,
    ReportDomain TEXT,
    OurDomain    TEXT,
    Start        TEXT,
    [End]        TEXT,
    Spf          TEXT,
    Dkim         TEXT,
    Count        INTEGER,
    SourceIP     TEXT,
    MailFrom     TEXT,
    RcptTo       TEXT,
    MsgFrom      TEXT
);"; 

            }
        }

        #endregion

*/