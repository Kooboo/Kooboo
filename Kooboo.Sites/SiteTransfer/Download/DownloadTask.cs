//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.SiteTransfer.Download
{
    public class DownloadTask
    {
        public string AbsoluteUrl { get; set; }

        public string RelativeUrl { get; set; }

        private Guid _routeid; 
        public Guid RouteId {
            get
            {
                if (_routeid == default(Guid))
                {
                    _routeid = Data.IDGenerator.Generate(this.RelativeUrl, ConstObjectType.Route);
                }
                return _routeid; 
            }
        }

        // page object id. 
        public Guid OwnerObjectId { get; set; }

        public byte ConstType { get; set; }

        public int RetryTimes { get; set; } = 0; 
    }
}
