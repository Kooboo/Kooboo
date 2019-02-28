//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Sync.SiteClusterSync
{
  public  static class ClusterUrl
    {

        public static string Push(SiteCluster cluster)
        {
            string url = "http://" + cluster.ServerIp;
            
            if (cluster.Port != 80)
            {
                url += ":" + cluster.Port.ToString(); 
            }
             
             url +=  "/_api/cluster/receive"; 
            return url;  
        }

        public static string SiteModel(string remoteIp, int port)
        {
            string url = "http://" + remoteIp;

            if (port != 80 & port >=0)
            {
                url += ":" + port.ToString();
            }

            return url + "/_api/cluster/sitemodel"; 
        }
    }
}
