//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Context
{
    public class BasicUser
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public Guid Id
        {
            get
            {
                return Lib.Security.Hash.ComputeGuidIgnoreCase(this.UserName); 
            }
        }
    }
}
