//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;

namespace Kooboo.Mail
{
    public class EmailAddress : IMailObject
    {
        private int _id;
        public int Id
        {
            set { _id = value; }
            get
            {
                if (_id == default(int))
                {
                    _id = ToId(Address);
                }
                return _id;
            }
        }

        public Guid UserId { get; set; }

        public string Name { get; set; }

        // this does not save into db.  No need for this data because address only saved in one orgdb. 
        [Kooboo.IndexedDB.CustomAttributes.KoobooIgnore]
        public Guid OrgId { get; set; }

        /// <summary>
        /// The full email address, e.g. guoqi@kooboo.com. 
        /// </summary>
        public string Address { get; set; }

        public EmailAddressType AddressType { get; set; }

        public string ForwardAddress { get; set; }

        private List<string> _members;
        public List<string> Members
        {
            get
            {
                if (_members == null)
                {
                    _members = new List<string>();
                }
                return _members;
            }
            set
            {
                _members = value;
            }
        }

        public bool IsDefault { get; set; }

        public string AuthorizationCode { get; set; }

        public string Signature { get; set; }

        public static int ToId(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                return 0;
            }
            return Lib.Security.Hash.ComputeInt(address);
        }

        public override int GetHashCode()
        {
            string unique = this.Address + this.AddressType.ToString() + this.UserId.ToString() + this.ForwardAddress + this.Name;

            if (_members != null)
            {
                foreach (var item in _members)
                {
                    unique += item;
                }
            }

            return Lib.Security.Hash.ComputeIntCaseSensitive(unique);
        }
    }


    public enum EmailAddressType
    {
        Normal = 0,
        Forward = 1,
        Group = 3,
        Wildcard = 4
    }
}
