//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System;

namespace Kooboo.Data.Infrastructure
{
    public class LocalInfra : IInfrastructure
    {
        public void Add(Guid organizationId, InfraType infraType, long amount, string item)
        {
        }

        public bool Test(Guid organizationId, InfraType infraType, long amount)
        {
            return true;
        }
    }
}