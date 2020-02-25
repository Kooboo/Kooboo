//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Infrastructure
{
    public class LocalInfra : IInfrastructure
    {
        public long Priority => 1;

        public void Add(Guid OrganizationId, InfraType InfraType, long Amount, string Item)
        {

        }

        public int MaxPages(Guid OrganizationId)
        {
            return 9999;
        }

        public int MaxSites(Guid organizationId)
        {
            return 9999;
        }

        public bool Test(Guid OrganizationId, InfraType InfraType, long amount)
        {
            return true;
        }
    }
}
