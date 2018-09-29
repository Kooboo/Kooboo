//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Models;
using Kooboo.Sites.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq; 

namespace Kooboo.Sites.Sync.SiteClusterSync
{
    public class SiteSetting
    {
        // update site based on the remote host checkin.  // this will be updated instantly... 
        public static WebSite AddOrUpdate(ClusterSiteEditModel editModel)
        {
            WebSite site = new WebSite() { Name = editModel.Name, OrganizationId = editModel.OrganizationId };

            ImportExport.SetSiteSetting(site, editModel.Settiing);

            site.Id = default(Guid); // reset. 
            site.EnableCluster = true; 

            Data.GlobalDb.WebSites.AddOrUpdate(site);

            HashSet<string> domains = new HashSet<string>(editModel.Domains);

            if (!string.IsNullOrEmpty(editModel.PrimaryDomain))
            {
                domains.Add(editModel.PrimaryDomain);
            }

            foreach (var item in domains)
            {
                Data.GlobalDb.Bindings.AddOrUpdate(item, site.Id, site.OrganizationId);
            }

            return site;
        }

        public static ClusterSiteEditModel GetModel(WebSite site, string destPrimaryDomain)
        {
            ClusterSiteEditModel editmodel = new ClusterSiteEditModel();

            editmodel.PrimaryDomain = destPrimaryDomain;
            editmodel.Name = site.Name;
            editmodel.OrganizationId = site.OrganizationId;

            var bindings = Kooboo.Data.GlobalDb.Bindings.GetByWebSite(site.Id);

            foreach (var item in bindings)
            {
                editmodel.Domains.Add(item.FullName);
            }

            editmodel.Settiing = ImportExport.GetSiteSetting(site);

            return editmodel;
        }
    } 
}