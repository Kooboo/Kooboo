//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Models;
using System;

namespace Kooboo.Data.Authorization
{
    public static class RightControl
    {
        public static bool CanSendEmail(Organization org)
        {
            if (org.ServiceLevel < 3)
            {
            }
            return true;
        }
    }

    public class EmailCounter
    {
        public Guid OrgId { get; set; }

        public int Count { get; set; }
    }
}