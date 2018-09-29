//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using Kooboo.IndexedDB; 
using System.Linq;

namespace Kooboo.Mail.Repositories
{
    public class EmailAddressRepository : RepositoryBase<EmailAddress>
    {
        private OrgDb orgdb { get; set; }

        public EmailAddressRepository(OrgDb db)
            : base(db.Db)
        {
            this.orgdb = db; 
        }

        protected override ObjectStoreParameters StoreParameters
        {
            get
            {
                ObjectStoreParameters paras = new ObjectStoreParameters();
                paras.AddColumn<EmailAddress>(it => it.Id);
                paras.AddColumn<EmailAddress>(it => it.UserId);  
                paras.AddColumn<EmailAddress>(it => it.AddressType);
                paras.AddColumn<EmailAddress>(it => it.Address);
                paras.AddColumn<EmailAddress>(it => it.ForwardAddress);
                paras.SetPrimaryKeyField<EmailAddress>(o => o.Id);
                return paras;
            }
        }
         
        public EmailAddress Get(string email)
        {
            var id = EmailAddress.ToId(email);
            return Get(id);
        }

        public override bool Update(EmailAddress value)
        {      
          return  this.AddOrUpdate(value);  
        }

        public override bool Delete(int id)
        {
            // Remove messages
            var item = Get(id);
            if (item == null)
                return false;

            var maildb = Kooboo.Mail.Factory.DBFactory.UserMailDb(item.UserId, this.orgdb.OrganizationId); 
             
            var all = maildb.Messages.Query().UseColumnData().Where(o => o.AddressId == id).SelectAll();

            foreach (var msg in all)
            {
                maildb.Messages.Delete(msg.Id); 
            } 

            return base.Delete(id);
        }

        public List<EmailAddress> ByUser(Guid userId)
        {
            return Query().UseColumnData().Where(o => o.UserId == userId).SelectAll();
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
                        this.AddOrUpdate(add); 
                    }
                } 
            } 
        }
         
        public void DeleteMember(int addressId, string memberAddress)
        {
            var addr = base.Get(addressId);
            addr.Members.Remove(memberAddress);
            this.AddOrUpdate(addr);  
        }

        public EmailAddress Find(string emailaddress)
        {
            var Id = EmailAddress.ToId(emailaddress);
            var direct = this.Get(emailaddress);
            if (direct != null)
            {
                return direct;
            }
            //find all find from wildcard.  
            return this.Store.FullScan(o => o.AddressType == EmailAddressType.Wildcard && Utility.AddressUtility.WildcardMatch(emailaddress, o.Address)).FirstOrDefault();
        }

    }
}