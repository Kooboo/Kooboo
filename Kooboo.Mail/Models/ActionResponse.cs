//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Mail.Models
{
    public class ActionResponse
    {
        public bool Success { get; set; }

        // Should retry later or not.
        public bool ShouldRetry { get; set; }

        // in case of not success. 
        public string Message { get; set; }
    }

}
