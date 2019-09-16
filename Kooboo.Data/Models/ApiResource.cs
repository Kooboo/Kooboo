//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Models
{
 public  class ApiResource
    {
        public string ThemeUrl { get; set; }

        public string AccountUrl { get; set;  }

        public string AcccountDomain { get; set; }

        public string ConvertUrl { get; set; } 

        public DateTime Expiration { get; set; } 

        public string MailServerIP { get; set; }
         
    }
}