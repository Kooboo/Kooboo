//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Interface;
using Kooboo.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Lib.Extensions; 

namespace Kooboo.Sites.Models
{
  public  class Synchronization : SiteObject
    {
        public Synchronization()
        {
            this.ConstType = ConstObjectType.Synchronization; 
        }
        private Guid _id;
        public override Guid Id
        {
            /// for objectid = default(guid), there can only be one version nr. the rest objectid must have multiple. 
            get
            {
                if (_id == default(Guid))
                {
                    string unique = this.SyncSettingId.ToString() + this.In.ToString();
                    
                    if (this.ObjectId != default(Guid))
                    {
                        unique += this.StoreName;
                        unique += this.ObjectId.ToString(); 
                    } 
                    if (this.In)
                    {
                        unique += this.RemoteVersion.ToString(); 
                    }
                    else
                    {
                        unique += this.Version.ToString(); 
                    }
                    _id = unique.ToHashGuid();  
                }
                return _id;
            }

            set
            {
                _id = value;
            }
        }
        
        public Guid SyncSettingId { get; set; }

        public string StoreName { get; set; }

        // default(Guid) =  the main index... 
        public Guid ObjectId { get; set; } 

        // in or out.. 
        public bool In { get; set; }

        public long Version { get; set; }

        public long RemoteVersion { get; set; }
        public override int GetHashCode()
        {
            if (this.In)
            {
                return Lib.Security.Hash.ComputeIntCaseSensitive(this.RemoteVersion.ToString()); 
            }
            else
            {
                return Lib.Security.Hash.ComputeIntCaseSensitive(Version.ToString());
            }
        }

    }
}
