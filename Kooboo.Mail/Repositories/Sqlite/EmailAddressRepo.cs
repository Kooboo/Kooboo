using System;
using System.Collections.Generic;
using System.Linq;
using Kooboo.Data.Models;

namespace Kooboo.Mail.Repositories.Sqlite
{
    public class EmailAddressRepo
    {
        private string TableName
        {
            get
            {
                return "EmailAddress";
            }
        }

        private OrgDb orgdb { get; set; }

        public EmailAddressRepo(OrgDb db)
        {
            this.orgdb = db;
        }

        public EmailAddress Get(int Id)
        {
            return this.orgdb.SqliteHelper.Get<EmailAddress>(this.TableName, nameof(EmailAddress.Id), Id);
        }

        public EmailAddress GetDefaultSender(User user)
        {
            var sql = $"SELECT * FROM {TableName} WHERE UserId='{user.Id}' AND IsDefault=1";
            return this.orgdb.SqliteHelper.Get<EmailAddress>(sql);
        }

        public bool UpdateName(int addressId, string newName)
        {
            var add = orgdb.Email.Get(addressId);

            if (add != null)
            {
                if (string.IsNullOrEmpty(newName))
                {
                    newName = "";
                }
                Dictionary<string, object> data = new Dictionary<string, object>();
                data.Add(nameof(EmailAddress.Name), newName);
                this.orgdb.SqliteHelper.Update(this.TableName, nameof(EmailAddress.Id), addressId, data);

                return true;
            }
            else
            {
                return false;
            }

        }

        public EmailAddress Get(string email)
        {
            var id = EmailAddress.ToId(email);
            return Get(id);
        }

        public List<EmailAddress> All()
        {
            return this.orgdb.SqliteHelper.All<EmailAddress>(this.TableName);
        }

        public bool Update(EmailAddress value)
        {
            return this.orgdb.SqliteHelper.AddOrUpdate(value, this.TableName, nameof(EmailAddress.Id));
        }

        public bool AddOrUpdate(EmailAddress value)
        {
            return this.Update(value);
        }

        public bool Delete(int id)
        {
            var item = Get(id);
            if (item == null)
                return false;

            this.orgdb.SqliteHelper.Delete(this.TableName, nameof(EmailAddress.Id), id);
            if (item.UserId == default)
            {
                return true;
            }

            //should also delete message.
            //delete all email address. 
            var maildb = Factory.DBFactory.UserMailDb(item.UserId, this.orgdb.OrganizationId);
            if (maildb == null)
            {
                return true;
            }
            // 1. delete relative job
            DeleteJobs(item, maildb);

            // 2. delete all messages.
            DeleteMessages(item, maildb);

            return true;
        }

        private static void DeleteJobs(EmailAddress emailAddress, MailDb maildb)
        {
            maildb.MailMigrationJob.DeleteByAddressId(emailAddress.Id);
        }

        private static void DeleteMessages(EmailAddress item, MailDb maildb)
        {
            long currentMsgId = -1;
            var MsgIdQuery = "SELECT MsgId FROM " + maildb.Message2.TableName + " WHERE MsgId> " + currentMsgId.ToString() + " AND " + nameof(Message.AddressId) + "=" + maildb.SqliteHelper.ObjToString(item.Id) + " Order By MsgId limit 0, 1000";

            var allmsgid = maildb.SqliteHelper.Query<long>(MsgIdQuery);

            while (allmsgid != null && allmsgid.Any())
            {
                foreach (var delMsgId in allmsgid)
                {
                    maildb.MsgHandler.DeleteMsg(delMsgId);
                }
                currentMsgId = allmsgid.Max();

                MsgIdQuery = "SELECT MsgId FROM " + maildb.Message2.TableName + " WHERE MsgId> " + currentMsgId.ToString() + " AND " + nameof(Message.AddressId) + "=" + maildb.SqliteHelper.ObjToString(item.Id) + " Order By MsgId limit 0, 1000";

                allmsgid = maildb.SqliteHelper.Query<long>(MsgIdQuery);
            }

            string sql = "DELETE FROM " + maildb.Message2.TableName + " WHERE " + nameof(Message.AddressId) + "=" + maildb.SqliteHelper.ObjToString(item.Id);
            maildb.SqliteHelper.Execute(sql);
        }

        public List<EmailAddress> ByUser(Guid userId)
        {
            return this.orgdb.SqliteHelper.FindAll<EmailAddress>(this.TableName, nameof(EmailAddress.UserId), userId).ToList();

        }

        public List<string> GetMembers(int addressId)
        {
            return Get(addressId)?.Members;
        }

        public void AddMember(int addressId, string memberAddress)
        {
            if (Utility.AddressUtility.IsValidEmailAddress(memberAddress))
            {
                var add = this.Get(addressId);

                if (add != null)
                {
                    if (!add.Members.Contains(memberAddress, StringComparer.OrdinalIgnoreCase))
                    {
                        add.Members.Add(memberAddress.ToLower());

                        Dictionary<string, object> updates = new Dictionary<string, object>();
                        updates.Add(nameof(EmailAddress.Members), System.Text.Json.JsonSerializer.Serialize(add.Members));

                        this.orgdb.SqliteHelper.Update(this.TableName, nameof(EmailAddress.Id), addressId, updates);

                    }
                }
            }
        }

        public void DeleteMember(int addressId, string memberAddress)
        {
            var addr = Get(addressId);
            if (addr != null)
            {
                addr.Members.Remove(memberAddress);

                Dictionary<string, object> updates = new Dictionary<string, object>();
                updates.Add(nameof(EmailAddress.Members), System.Text.Json.JsonSerializer.Serialize(addr.Members));

                this.orgdb.SqliteHelper.Update(this.TableName, nameof(EmailAddress.Id), addressId, updates);
            }
        }

        public EmailAddress Find(string emailaddress)
        {
            var Id = EmailAddress.ToId(emailaddress);
            var direct = this.Get(emailaddress);
            if (direct != null)
            {
                return direct;
            }

            var WildcardList = this.orgdb.SqliteHelper.FindAll<EmailAddress>(this.TableName, nameof(EmailAddress.AddressType), EmailAddressType.Wildcard);

            if (WildcardList != null)
            {
                var allMatches = WildcardList.ToList().FindAll(o => Utility.AddressUtility.WildcardMatch(emailaddress, o.Address));
                if (allMatches != null && allMatches.Any())
                {
                    return allMatches.OrderByDescending(o => o.Address.Length).FirstOrDefault();
                }

                // return WildcardList.ToList().Find(o => Utility.AddressUtility.WildcardMatch(emailaddress, o.Address));
            }
            return null;

        }

        public bool LoginByAuthorizationCode(string mailaddress, string AuthorizationCode)
        {
            var Id = EmailAddress.ToId(mailaddress);
            var direct = this.Get(mailaddress);

            if (string.IsNullOrEmpty(direct.AuthorizationCode))
                return false;

            if (direct.IsDefault && direct.AuthorizationCode.Equals(AuthorizationCode))
                return true;

            return false;
        }

    }





}
