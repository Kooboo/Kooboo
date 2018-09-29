//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Models
{
    // change this into only the sync related info. 
    public class SiteCluster : ISiteObject
    {
        private Guid _id;
        public Guid Id
        {
            get
            {
                if (_id == default(Guid))
                {
                    if (ServerId != default(int))
                    {
                        _id = Lib.Security.Hash.ComputeGuidIgnoreCase(this.ServerId.ToString());
                    }
                    else if (!string.IsNullOrEmpty(ServerIp))
                    {
                        string unique = this.ServerIp; 
                        if (this.Port != 80)
                        {
                            unique += this.Port.ToString(); 
                        }
                        _id = Lib.Security.Hash.ComputeGuidIgnoreCase(unique); 
                    }
                }
                return _id;
            }
            set { _id = value; }
        }

        public int ServerId { get; set; }

        public string ServerIp { get; set; }

        public int Port { get; set; } = 80;

        public  bool IsSelected { get; set; }

        public string DataCenter { get; set; }

        // The root server...
        public bool IsRoot { get; set; }
          
        public string PrimaryDomain { get; set; }

        public long Version { get; set; }

        public byte ConstType { get; set; } = ConstObjectType.SiteCluster;

        public DateTime CreationDate { get; set; }
        public DateTime LastModified { get; set; }
        public string Name { get; set; }

    }

}
