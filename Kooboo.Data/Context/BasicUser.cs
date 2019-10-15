//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System;

namespace Kooboo.Data.Context
{
    public class BasicUser
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public Guid Id => Lib.Security.Hash.ComputeGuidIgnoreCase(UserName);
    }
}