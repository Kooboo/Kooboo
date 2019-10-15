//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System;

namespace Kooboo.Data.Models
{
    public class UserOrganization : IGolbalObject
    {
        private Guid _id;

        public Guid Id
        {
            get
            {
                if (_id == default(Guid))
                {
                    if (this.UserId != default(Guid) && this.OrganizationId != default(Guid))
                    {
                        string unique = this.UserId.ToString() + this.OrganizationId.ToString();
                        _id = Lib.Security.Hash.ComputeGuidIgnoreCase(unique);
                    }
                }
                return _id;
            }
            set => _id = value;
        }

        public Guid UserId { get; set; }

        public Guid OrganizationId { get; set; }

        public override int GetHashCode()
        {
            string unique = this.UserId.ToString() + this.OrganizationId.ToString();

            return Lib.Security.Hash.ComputeIntCaseSensitive(unique);
        }
    }
}