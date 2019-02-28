//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using Kooboo.Extensions;
using System.Collections.Generic;

namespace Kooboo.Sites.Models
{
    public class SyncSetting : SiteObject
    {
        public SyncSetting()
        {
            this.ConstType = ConstObjectType.SyncSetting;
        }

        private Guid _id;
        public override Guid Id
        {
            set
            {
                _id = value;
            }
            get
            {
                if (_id == default(Guid))
                {
                    string unique = this.RemoteServerUrl + this.RemoteWebSiteId.ToString();
                    _id = unique.ToHashGuid();
                }
                return _id;
            }
        }

        public Guid RemoteWebSiteId { get; set; }

        // only for display purpose.... 
        public string RemoteSiteName { get; set; }

        /// <summary>
        /// The server or IP that host the remote website, plus the port if it is not port 80
        /// </summary>
        public string RemoteServerUrl { get; set; }
        
        public override int GetHashCode()
        {
            string uniquestring =this.RemoteServerUrl + this.RemoteWebSiteId.ToString() + this.RemoteSiteName; 
            return Lib.Security.Hash.ComputeIntCaseSensitive(uniquestring); 
        } 
    }
}
