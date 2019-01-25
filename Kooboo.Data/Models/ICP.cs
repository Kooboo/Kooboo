//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Models
{
    public class ICP : IGolbalObject
    {
        public Guid Id
        {
            get { return this.DomainId; }
            set { this.DomainId = value; }
        }

        public Guid DomainId { get; set; }

        public string DomainName { get; set; }

        public string WebSiteName { get; set; }

        public string ICPNumber { get; set; }

        public string IP { get; set; }

        public string BeianUrl { get; set; }

        public string OrganizationName { get; set; }

    }
}
