//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.ViewModel
{
  public  class ClusterSiteEditModel 
    {
        public SiteSetting Settiing { get; set; }

        public string Name { get; set; }

        //reserved for future use.
        public string DefaultCulture { get; set; }

        public Guid OrganizationId { get; set; }

        public string PrimaryDomain { get; set; }

        public List<string> Domains { get; set; } = new List<string>(); 
    }
}
