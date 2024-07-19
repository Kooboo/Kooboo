using System.Collections.Generic;

namespace Kooboo.Mail.Extension
{
    public class MailDbWrapper
    {
        public MailDb MailDb { get; set; }

        public OrgDb OrgDb { get; set; }

        public MailDbWrapper(MailDb maildb, OrgDb orgDb)
        {
            this.MailDb = maildb;
            this.OrgDb = orgDb;

        }

        public List<string> SearchAddressBook(string contains)
        {
            return this.MailDb.AddBook.Contains(contains);
        }

    }
}
