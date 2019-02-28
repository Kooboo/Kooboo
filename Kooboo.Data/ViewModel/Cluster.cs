//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.ViewModel
{
    public class ClusterEditViewModel
    {
        public bool EnableCluster { get; set; }

        public List<DataCenter> DataCenter { get; set; } = new List<DataCenter>();

        public bool EnableLocationRedirect { get; set; }

        public bool IsSlave { get; set; }

        public List<ClusterLocationRedirect> LocationRedirect { get; set; } = new List<ClusterLocationRedirect>();

        public List<string> SiteDomains { get; set; }
    }

    public class ClusterLocationRedirect
    {
        public string Name { get; set; }

        public string SubDomain { get; set; }

        public string RootDomain { get; set; }
    }

    public class SiteClusterViewModel
    {
        public int ServerId { get; set; }

        public string ServerIp { get; set; }

        // The request server... 
        public bool IsRoot { get; set; }
    }

}
