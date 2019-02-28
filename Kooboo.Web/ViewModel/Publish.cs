//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Web.ViewModel
{
    public class PushItemViewModel
    {
        public long Id { get; set; }

        public string Name { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ChangeType ChangeType { get; set; }

        public string ObjectType { get; set; }
          
        public string Thumbnail { get; set; }

        public bool Selected { get; set; }

        public DateTime LastModified { get; set; }

        public string Size { get; set; }

        public long Version { get; set; }

        public byte KoobooType { get; set; }

        public Guid SyncItemId { get; set; }

        public long LogId { get; set; }
    }

    public class SyncLogItemViewModel
    {
        public long Id { get; set; }

        public string Name { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ChangeType ChangeType { get; set; }

        public string ObjectType { get; set; }
           
        public DateTime LastModified { get; set; }

        public string Size { get; set; }

        public long Version { get; set; }
  
        public long LogId { get; set; }
    }


    public class SyncCountViewModel
    {
        public Guid Id { get; set; }

        public string Name { set; get;  }

        public int Count { get; set; }
    }


    public class SyncItemViewModel
    {
        public Guid Id { get; set; }
   
        // only for display purpose.... 
        public string RemoteSiteName { get; set; }

        /// <summary>
        /// The server or IP that host the remote website, plus the port if it is not port 80
        /// </summary>
        public string RemoteServerUrl { get; set; }

         public  int Difference { get; set; }

    }

    public class PullResult
    {
        public bool IsFinish { get; set; }

        public long SiteLogId { get; set; }

        public long SenderVersion { get; set; }
    }

    public class UserPublishServer
    {
        private Guid _id; 
        public Guid Id {
            get
            {
                if (_id == default(Guid))
                {
                    _id = Guid.NewGuid();  
                }
                return _id;  
            }
            set { _id = value;  }
        }

        public string Name { get; set; }

        public string ServerUrl { get; set; }

        public bool Reserved { get; set; }
    }
}
