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
        public void Add(Guid OrganizationId, InfraType InfraType, int Amount, string Item)
        {

        }

        public bool Test(Guid OrganizationId, InfraType InfraType, int amount)
        {
            return true;
        }
    }
}
