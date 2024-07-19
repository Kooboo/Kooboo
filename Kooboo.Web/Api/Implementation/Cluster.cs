//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Api;
using Kooboo.Data.Helper;
using Kooboo.Data.Models;
using Kooboo.Data.ViewModel;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Sites.Sync;
using Kooboo.Sites.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Web.Api.Implementation
{
    //public class Cluster : IApi
    //{
    //    public string ModelName
    //    {
    //        get { return "Cluster"; }
    //    }

    //    public bool RequireSite
    //    {
    //        get { return false; }
    //    }

    //    public bool RequireUser
    //    {
    //        get { return false; }
    //    }

    //    private void ValidateRequest(ApiCall call)
    //    {
    //        var isonlineServer = Kooboo.Data.Helper.ApiHelper.IsOnlineSever(call.Context.Request.IP);

    //        if (!isonlineServer)
    //        {
    //            throw new Exception(Data.Language.Hardcoded.GetValue("Request IP not allowed"));
    //        }
    //    }

    //    [Attributes.RequireModel(typeof(Data.Models.Cluster))]
    //    public bool ClusterUpdate(ApiCall call)
    //    {
    //        ValidateRequest(call);
    //        var model = call.Context.Request.Model as Data.Models.Cluster;
    //        return false;
    //    }

    //    [Attributes.RequireParameters("SiteId")]
    //    public ClusterEditViewModel Get(ApiCall call)
    //    {
    //        var sitedb = call.Context.WebSite.SiteDb();

    //        ClusterEditViewModel viewmodel = new ClusterEditViewModel() { EnableCluster = call.WebSite.EnableCluster };

    //        bool IsSlave = false;
    //        //if (!Kooboo.Data.AppSettings.Global.IsOnlineServer)
    //        //{

    //        foreach (var item in sitedb.SiteCluster.All())
    //        {
    //            if (item.IsRoot)
    //            {
    //                IsSlave = true;
    //            }
    //            DataCenter dc = new DataCenter();
    //            dc.Name = item.DataCenter;
    //            dc.DisplayName = item.Name;
    //            dc.Ip = item.ServerIp;
    //            dc.Port = item.Port;
    //            dc.IsSelected = item.IsSelected;
    //            viewmodel.DataCenter.Add(dc);
    //        }

    //        if (!viewmodel.DataCenter.Any(o => o.IsRoot))
    //        {
    //            DataCenter dc = new DataCenter();
    //            dc.Name = Data.Language.Hardcoded.GetValue("Root", call.Context);
    //            dc.DisplayName = Data.Language.Hardcoded.GetValue("Root", call.Context);
    //            dc.Ip = "127.0.0.1";
    //            dc.Port = Data.AppSettings.HttpPort;
    //            dc.IsSelected = true;
    //            dc.IsRoot = true;
    //            viewmodel.DataCenter.Add(dc);
    //        }

    //        viewmodel.IsSlave =  IsSlave; 

            
    //        return viewmodel;
    //        //}
    //        //else
    //        //{
    //        //    // online server must have the setting from online database, in order to load balancing..
    //        //    var site = call.Context.WebSite;
    //        //    var user = call.Context.User;
    //        //    if (user == null)
    //        //    {
    //        //        return null;
    //        //    }

    //        //    viewmodel.DataCenter.Add(new DataCenter() { Name = "CN", DisplayName = "China", IsCompleted = true, IsSelected = true, IsRoot = true });
    //        //    viewmodel.DataCenter.Add(new DataCenter() { Name = "HK", DisplayName = "HongKong", IsCompleted = false, IsSelected = false });
    //        //    viewmodel.DataCenter.Add(new DataCenter() { Name = "US", DisplayName = "America", IsCompleted = false, IsSelected = false });

    //        //    viewmodel.LocationRedirect.Add(new ClusterLocationRedirect() { Name = "CN", SubDomain = "xub", RootDomain = "kooboo.cn" });
    //        //    return viewmodel;
    //        //}
    //    }

    //    [Attributes.RequireParameters("SiteId")]
    //    public ClusterEditViewModel GetNew(ApiCall call)
    //    {
    //        var user = call.Context.User;
    //        if (user == null)
    //        {
    //            return null;
    //        }

    //        ClusterEditViewModel viewmodel = new ClusterEditViewModel();

    //        Dictionary<string, string> para = new Dictionary<string, string>();
    //        para.Add("SiteId", call.GetValue("SiteId"));
    //        para.Add("OrganizationId", user.CurrentOrgId.ToString());

    //        var datacenterlist = Lib.Helper.HttpHelper.Get<List<DataCenter>>(AccountUrlHelper.Cluster("GetDataCenter"), para, user.UserName, user.GetPasswordString());

    //        viewmodel.DataCenter = datacenterlist;

    //        viewmodel.EnableCluster = datacenterlist.Where(o => o.IsSelected).Count() >= 2;

    //        viewmodel.EnableLocationRedirect = datacenterlist.Where(o => !string.IsNullOrWhiteSpace(o.PrimaryDomain)).Any();

    //        if (viewmodel.EnableLocationRedirect)
    //        {
    //            foreach (var item in datacenterlist.Where(o => !string.IsNullOrWhiteSpace(o.PrimaryDomain)))
    //            {
    //                var domainresult = Kooboo.Data.Helper.DomainHelper.Parse(item.PrimaryDomain);
    //                if (domainresult != null)
    //                {
    //                    viewmodel.LocationRedirect.Add(new ClusterLocationRedirect() { Name = item.Name, SubDomain = domainresult.SubDomain, RootDomain = domainresult.Domain });
    //                }
    //            }
    //        }

    //        return viewmodel;
    //    }

    //    private void ValidatePort(ApiCall call)
    //    {
    //        var site = call.WebSite;

    //        if (site == null)
    //        {
    //            throw new Exception(Data.Language.Hardcoded.GetValue("Website not found", call.Context));
    //        }

    //        if (Data.AppSettings.HttpPort != 80)
    //        {
    //            throw new Exception(Data.Language.Hardcoded.GetValue("To be a host of web cluster, your kooboo instance must be listening on port 80", call.Context));
    //        }

    //    }

    //    public void Post(ClusterEditViewModel model, ApiCall call)
    //    {
    //        //TODO: Cluser is not enable now. 
    //        //ValidatePort(call);

    //        //var sitedb = call.Context.WebSite.SiteDb();

    //        //Data.GlobalDb.WebSites.UpdateBoolColumn(call.Context.WebSite.Id, o => o.EnableCluster, model.EnableCluster);

    //        //if (Data.AppSettings.IsOnlineServer)
    //        //{
    //        //    // check and update the SiteInfo of EnableCluster.  
    //        //    var siteid = call.GetValue<Guid>("SiteId");
    //        //    var user = call.Context.User;
    //        //    if (user == null)
    //        //    {
    //        //        throw new Exception(Data.Language.Hardcoded.GetValue("user required", call.Context));
    //        //    }
    //        //    // Set the website doamins and send to Account for update... 
    //        //    // Get a list of ServerId back for sync purpose... 
    //        //    string url = AccountUrlHelper.Cluster("SaveSetting") + "?SiteId=" + siteid.ToString() + "&OrganizatioinId=" + user.CurrentOrgId.ToString();

    //        //    var result = Lib.Helper.HttpHelper.Post<List<SiteClusterViewModel>>(url, Lib.Helper.JsonHelper.Serialize(model));

    //        //    if (result != null)
    //        //    {

    //        //    }
    //        //    /// var errro; 
    //        //}

    //        //else
    //        //{
    //        //    // local server, no need for location redirect.
    //        //    // local server does not support location redirect.. 
    //        //    List<SiteCluster> updates = new List<SiteCluster>();
    //        //    foreach (var item in model.DataCenter)
    //        //    {
    //        //        if (!item.IsRoot)
    //        //        {
    //        //            // can not contains itself... 
    //        //            SiteCluster cluster = new SiteCluster();
    //        //            cluster.ServerIp = item.Ip;
    //        //            cluster.Port = item.Port;
    //        //            cluster.Name = item.DisplayName;
    //        //            cluster.DataCenter = item.Name;
    //        //            cluster.IsRoot = item.IsRoot;
    //        //            cluster.IsSelected = item.IsSelected;
    //        //            updates.Add(cluster);
    //        //        }
    //        //    }

    //        //    // do the delection. 
    //        //    HashSet<Guid> deleteIds = new HashSet<Guid>();
    //        //    foreach (var item in sitedb.SiteCluster.All())
    //        //    {
    //        //        var find = updates.Find(o => o.Id == item.Id);
    //        //        if (find == null)
    //        //        {
    //        //            deleteIds.Add(item.Id);
    //        //        }
    //        //    }
    //        //    foreach (var item in deleteIds)
    //        //    {
    //        //        sitedb.SiteCluster.Delete(item);
    //        //    }

    //        //    foreach (var item in updates)
    //        //    {
    //        //        sitedb.SiteCluster.AddOrUpdate(item);
    //        //    }
    //        //}
    //    }

    //    private WebSite InitWebSite(string RemoteIp, Guid SiteId, int port = 80)
    //    {
    //        string modelurl = Sites.Sync.SiteClusterSync.ClusterUrl.SiteModel(RemoteIp, port) + "?siteid=" + SiteId.ToString();

    //        var model = Lib.Helper.HttpHelper.Get<ClusterSiteEditModel>(modelurl);
    //        if (model == null)
    //        {
    //            return null;
    //        }
    //        else
    //        {
    //            var site = Kooboo.Sites.Sync.SiteClusterSync.SiteSetting.AddOrUpdate(model);
    //            // add the site cluster...
    //            var sitedb = site.SiteDb();
    //            sitedb.SiteCluster.AddOrUpdate(new SiteCluster() { IsRoot = true, ServerIp = RemoteIp, Name = site.Name });
    //            return site;
    //        }
    //    }

    //    // receive the site object... 
    //    public bool Receive(Guid SiteId, ApiCall call)
    //    {
    //        var converter = new IndexedDB.Serializer.Simple.SimpleConverter<SyncObject>();
    //        SyncObject sync = converter.FromBytes(call.Context.Request.PostData);

    //        //must do the user validation here...  
    //        var website = Data.Config.AppHost.SiteRepo.Get(SiteId);

    //        if (website == null)
    //        {
    //            website = InitWebSite(call.Context.Request.IP, SiteId, sync.SenderPort);
    //        }

    //        var sitedb = website.SiteDb();

    //        var manager = sitedb.ClusterManager;
    //        if (manager != null)
    //        {
    //            var node = sitedb.SiteCluster.GetByIp(call.Context.Request.IP);
    //            if (node == null)
    //            {
    //                return false;
    //            }
    //            manager.Receive(sync, node);
    //        }
    //        return true;
    //    }

    //    // this is used for remote site cluster to query information from here.. 
    //    [Attributes.RequireParameters("SiteId")]
    //    public ClusterSiteEditModel SiteModel(ApiCall call)
    //    {
    //        Guid siteid = call.GetValue<Guid>("SiteId");

    //        var website = Data.Config.AppHost.SiteRepo.Get(siteid);

    //        if (website != null)
    //        {
    //            string remoteip = call.Context.Request.IP;

    //            var sitedb = website.SiteDb();

    //            var cluster = sitedb.SiteCluster.GetByIp(remoteip);

    //            if (cluster != null)
    //            {
    //                return Kooboo.Sites.Sync.SiteClusterSync.SiteSetting.GetModel(website, cluster.PrimaryDomain);
    //            }
    //        }
    //        return null;
    //    }

    //    [Attributes.RequireParameters("Ip", "Name")]
    //    public bool ValidateCustomServer(ApiCall call)
    //    {
    //        return true;
    //    }
    //}
}
