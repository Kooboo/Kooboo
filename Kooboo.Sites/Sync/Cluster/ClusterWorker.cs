//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
////Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
////All rights reserved.
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Kooboo.Data.Interface;
//using Kooboo.Data.Models;
//using Kooboo.Sites.Extensions;

//namespace Kooboo.Sites.Sync.Cluster
//{
//    public class ClusterWorker : IBackgroundWorker
//    {
//        public int Interval
//        {
//            get { return 30; }
//        }

//        public DateTime LastExecute
//        {
//            get; set;
//        } = DateTime.Now.AddSeconds(-100);

//        public string Name
//        {
//            get
//            {
//                return "ClusterSync";
//            }
//        }

//        public void Execute(Guid WebSiteId = default(Guid))
//        {
//            if (WebSiteId != default(Guid))
//            {
//                var website = Data.GlobalDb.WebSites.Get(WebSiteId);
//                ExecuteOneWebSite(website);
//                return;
//            }
//            else
//            {
//                var allsites = Data.GlobalDb.WebSites.AllSites.Where(o => o.Value.EnableCluster).ToList();

//                foreach (var item in allsites)
//                {
//                    ExecuteOneWebSite(item.Value);
//                }
//            }

//        }

//        private void ExecuteOneWebSite(Data.Models.WebSite website)
//        {
            
//            var SyncToServers = Cluster.ClusterService.GetClusterTo(website);

//            var sitedb = website.SiteDb();

//            foreach (var item in SyncToServers)
//            {
//                var store = Stores.NodeProgress(sitedb);

//                NodeProgress record = null;
//                record = store.FullScan(o => o.ServerId == item).FirstOrDefault();
//                if (record == null)
//                {
//                    record = new NodeProgress();
//                    record.ServerId = item;
//                    record.LastUpdate = DateTime.Now;
//                    store.add(record);
//                }

//                var currentversion = record.CurrentVersion;

//                var alllogs = sitedb.Log.Store.Where(o => o.Id > currentversion).SelectAll();

//                long Doneversion = 0;

//                foreach (var logitem in alllogs)
//                {

//                    try
//                    {
//                        var exeresult = ClusterService.PostToRemote(sitedb, logitem, item);
//                        if (!exeresult)
//                        {
//                            break;
//                        }

//                    }
//                    catch (Exception)
//                    {
//                        //TODO: log system errors. 
//                        break;
//                    }

//                    Doneversion = logitem.Id;
//                }


//                record.CurrentVersion = Doneversion;
//                record.LastUpdate = DateTime.Now;

//                store.update(record.ServerId, record);

//            }
//        }
//    } 
//}
