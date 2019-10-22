//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data;
using Kooboo.IndexedDB;
using Kooboo.Mail.Repositories;
using System;

namespace Kooboo.Mail
{
    public class OrgDb : IDisposable
    {
        public OrgDb(Guid organizationId)
        {
            OrganizationId = organizationId;
            var dbName = Kooboo.Data.AppSettings.GetMailDbName(OrganizationId);
            Db = DB.GetDatabase(dbName);
            EmailAddress = new EmailAddressRepository(this);
        }

        public Guid OrganizationId { get; set; }

        public Database Db { get; set; }

        public EmailAddressRepository EmailAddress
        {
            get; private set;
        }

        public void Dispose()
        {
            this.Db.Close();
        }
    }
}