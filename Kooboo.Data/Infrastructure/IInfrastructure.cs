//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Infrastructure
{
  public  interface IInfrastructure : Kooboo.Lib.IOC.IPriority
    {
        bool Test(Guid OrganizationId, InfraType InfraType, long amount);

        void Add(Guid OrganizationId, InfraType InfraType, long Amount, string Item);

        int MaxSites(Guid organizationId);

        int MaxPages(Guid OrganizationId);  
    }
}
