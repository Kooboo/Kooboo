//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Infrastructure
{
  public  interface IInfrastructure
    {
        bool Test(Guid OrganizationId, InfraType InfraType, int amount);

        void Add(Guid OrganizationId, InfraType InfraType, int Amount, string Item); 
    }
}
