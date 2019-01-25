//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Interface;
using Kooboo.Sites.Extensions;
using System;
using System.Linq;

namespace Kooboo.Sites.TaskQueue
{
    // This is a backup to ensure that some tasks are running. 
   
    public class EusureWorker : IBackgroundWorker
    {
        public static object _locker = new object();

        public int Interval
        {
            get
            {
                return 120;
            }
        }

        public DateTime LastExecute
        {
            get; set;
        }
         
        public void Execute()
        {
            EnsureCluster(); 
        }

        public void EnsureCluster()
        {
            foreach (var item in Kooboo.Data.GlobalDb.WebSites.All().Where(o=>o.EnableCluster))
            {
                item.SiteDb().ClusterManager.EnsureStart();  
            }
        }
      

    }
}



 