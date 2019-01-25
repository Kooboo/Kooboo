//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Web.ViewModel
{
   public class DomainSummaryViewModel
    {

        public Guid Id { get; set; }

        public string DomainName { get; set; }

        public string Expires { get; set; }

        public int Records { get; set; }

        public int Sites { get; set; }

        public int Emails { get; set; } = 2;

        public bool UseEmail { get { return this.Emails > 0;  } } 
    }
     
    public class DomainBindingViewModel
    {
        public Guid Id { get; set; }
        public string SubDomain { get; set; }

        public string WebSiteName { get; set; }
    }
}
