
using System;
using System.Collections.Generic;
using System.Linq;
using Kooboo.Mail.Models;


namespace Kooboo.Mail.Repositories.Sqlite
{
    public class MailModuleRepo
    {

        private string TableName
        {
            get
            {
                return "MailModule";
            }
        }

        private OrgDb orgdb { get; set; }

        public MailModuleRepo(OrgDb db)
        {
            this.orgdb = db;
        }

        public List<MailModule> List()
        {
            return this.All();
        }

        public MailModule Get(string name)
        {
            var id = Lib.Security.Hash.ComputeGuidIgnoreCase(name);
            return Get(id);
        }

        public MailModule Get(Guid id)
        {
            return this.orgdb.SqliteHelper.Find<MailModule>(this.TableName, nameof(MailModule.Id), id);
        }

        public bool Delete(Guid id)
        {
            this.orgdb.SqliteHelper.Delete(this.TableName, nameof(MailModule.Id), id);
            return true;
        }

        public bool AddOrUpdate(MailModule value)
        {
            return this.orgdb.SqliteHelper.AddOrUpdate(value, this.TableName, nameof(MailModule.Id));
        }

        public int Count()
        {
            return this.All().Count();
        }

        public List<MailModule> All()
        {
            return this.orgdb.SqliteHelper.All<MailModule>(this.TableName);
        }

    }
}
