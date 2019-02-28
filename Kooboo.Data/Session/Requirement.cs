//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Session
{
   public class Requirement
    {
        public bool RequireSsl { get; set; }

        public bool RequireRsa { get; set; }

        public bool RequireValidation { get; set; }

        public bool UniqueKey { get; set; }
    }
}
