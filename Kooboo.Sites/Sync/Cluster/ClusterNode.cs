//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Kooboo.Sites.Sync.Cluster
{
   public class ClusterNode
    {
        private Guid _Id; 
        public Guid Id
        {
            get
            {
                if (_Id == default(Guid))
                {
                    string unique =  this.ServerUrl + this.ServerWebSiteId.ToString();
                    _Id = unique.ToHashGuid();  
                }
                return _Id; 
            }
            set
            {
                _Id = value; 
            }
        } 
          
        /// <summary>
        /// includes http and port number. 
        /// </summary>
        public string ServerUrl { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public Guid ServerWebSiteId { get; set; }

        public bool IsLocal { get; set; }

        public override int GetHashCode()
        {
            string unique = this.ServerUrl + this.UserName + this.Password + this.ServerWebSiteId.ToString();
            return Lib.Security.Hash.ComputeIntCaseSensitive(unique); 

        }
    }
}
