//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Extensions;
using System;

namespace Kooboo.Sites.Sync.Cluster
{
    public class ClusterNode
    {
        private Guid _id;

        public Guid Id
        {
            get
            {
                if (_id == default(Guid))
                {
                    string unique = this.ServerUrl + this.ServerWebSiteId.ToString();
                    _id = unique.ToHashGuid();
                }
                return _id;
            }
            set
            {
                _id = value;
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