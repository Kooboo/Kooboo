//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using Kooboo.Extensions;
using System.Collections;
using System.Collections.Generic;
using Kooboo.IndexedDB.CustomAttributes;

namespace Kooboo.Data.Models
{
    /// <summary>
    /// The top level domain. like kooboo.com. kooboo.org.cn. etc. 
    /// Binding can use subdomains. Domain only has one, sub domains can be multiple.
    /// </summary> 
    public class Domain : IGolbalObject
    {
        private Guid _id;
        public Guid Id
        {
            set { _id = value; }
            get
            {
                if (_id == default(Guid))
                {
                    _id = IDGenerator.GetDomainId(this.DomainName);
                }
                return _id;
            }
        }

        public string DomainName { get; set; }

        public Guid OrganizationId { get; set; }

        public DateTime ExpirationDate { get; set; }

        [KoobooIgnore]
        public bool IsKooboo { get; set; }

        public override int GetHashCode()
        {
            string unique = this.OrganizationId.ToString() + this.ExpirationDate.ToShortDateString();
            return Lib.Security.Hash.ComputeIntCaseSensitive(unique);
        }
    }
}
