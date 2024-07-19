using System.Collections.Generic;
using System.Linq;
using Kooboo.Mail.Models;

namespace Kooboo.Mail.Repositories.Sqlite
{

    public class AddressBookRepo
    {
        private string TableName
        {
            get
            {
                return "AddressBook";
            }
        }
        private MailDb maildb { get; set; }

        public AddressBookRepo(MailDb db)
        {
            this.maildb = db;
        }

        public AddressBook Get(int Id)
        {
            return this.maildb.SqliteHelper.Get<AddressBook>(this.TableName, nameof(AddressBook.Id), Id);
        }

        public AddressBook GetInfoByAddress(string address)
        {
            return this.maildb.SqliteHelper.Get<AddressBook>(this.TableName, nameof(AddressBook.Address), address);
        }

        public bool AddOrUpdate(AddressBook addressbook)
        {
            return this.maildb.SqliteHelper.AddOrUpdate(addressbook, this.TableName, nameof(addressbook.Id));
        }

        public AddressBook GetByAddress(string fullAddress)
        {

            if (MimeKit.MailboxAddress.TryParse(fullAddress, out var mailbox))
            {
                var address = mailbox.GetAddress(false);

                if (string.IsNullOrWhiteSpace(address))
                {
                    return null;
                }

                AddressBook receiver = new();
                receiver.Address = address;

                return this.Get(receiver.Id);

            }
            return null;

        }

        public void Add(string FullAddress)
        {
            if (MimeKit.MailboxAddress.TryParse(FullAddress, out var mailbox))
            {
                var address = mailbox.GetAddress(false);

                if (string.IsNullOrWhiteSpace(address))
                {
                    return;
                }

                string name = mailbox.Name;
                string addressNamePart = null;

                var segments = Kooboo.Mail.Utility.AddressUtility.ParseSegment(address);

                addressNamePart = segments.Address;

                if (string.IsNullOrWhiteSpace(name))
                {
                    name = addressNamePart;
                }

                MimeKit.MailboxAddress newadd = new MimeKit.MailboxAddress(name, address);

                AddressBook receiver = new();
                receiver.Address = address;
                receiver.Name = name;
                receiver.FullAddress = newadd.ToString();

                var old = this.Get(receiver.Id);

                if (old != null)
                {
                    if (old.Name == addressNamePart)
                    {
                        if (!string.IsNullOrEmpty(name) && name != addressNamePart)
                        {
                            old.Name = name;
                            MimeKit.MailboxAddress oldFull = new MimeKit.MailboxAddress(name, address);
                            old.FullAddress = oldFull.ToString();
                            this.AddOrUpdate(old);
                        }
                    }
                }
                else
                {
                    this.AddOrUpdate(receiver);
                }
            }

        }

        public void AddList(List<string> Addresses)
        {
            foreach (var item in Addresses)
            {
                this.Add(item);
            }
        }

        public List<string> Contains(string part)
        {
            if (string.IsNullOrEmpty(part))
            {
                return null;
            }

            string sql = "SELECT * FROM AddressBook WHERE FullAddress like '%" + part + "%' COLLATE NOCASE limit 100";

            var list = this.maildb.SqliteHelper.Query<AddressBook>(sql);

            if (list != null && list.Any())
            {
                return list.Select(o => o.FullAddress.Replace("\"", "")).ToList();
            }

            return null;
        }

        public List<AddressBook> All()
        {
            string sql = "SELECT * FROM " + this.TableName;

            return this.maildb.SqliteHelper.Query<AddressBook>(sql).ToList();
        }

        public void Delete(int id)
        {
            this.maildb.SqliteHelper.Delete(this.TableName, nameof(AddressBook.Id), id);
        }

    }

}
