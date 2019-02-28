//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Web.ViewModel
{
    public class SiteCultureViewModel
    {
        public SiteCultureViewModel()
        {
            this.Cultures = new Dictionary<string, string>(); 
        }
        public string Default { get; set;  }
        
        public string DefaultName { get; set; }

        public Dictionary<string, string> Cultures; 
    }

    public class SiteSummaryViewModel
    {
        public Guid SiteId { get; set; }
        public string SiteName { get; set; }

        public string SiteDisplayName { get; set; }

        public int PageCount { get; set; }
         
        public int ImageCount { get; set; }
 
        public int Visitors { get; set; }

        public bool InProgress { get; set; }

        public string HomePageLink { get; set; }

        public string EditPageLink { get; set; }

        public bool Online { get; set; }

    }

    public class SiteDiskSyncViewModel
    {
        public string Folder { get; set; }
        public bool EnableDiskSync { get; set; }

        public int DiskFileCount { get; set; }
         
    }

    public class CultureInfoViewModel
    {
        public string Name { get; set;  }

        public string DisplayName { get; set; }

    }

    public class SimpleSiteItemViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public long LastVersion { get; set; }
    }

    public class SiteBindingViewModel
    {
        public string Name { get; set; }

        public Guid Id { get; set; }

        public int BindingCount { get; set; }
    }

    public class ExportStoreNameViewModel
    { 
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public List<string> Related { get; set; }
    }


    public class SiteUpdate: WebSite
    {
        public List<string> Clusters { get; set; }

        public override Dictionary<string, string> Culture {
            get;set;
        }
    }
}
