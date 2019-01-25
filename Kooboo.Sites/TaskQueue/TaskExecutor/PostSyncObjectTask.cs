//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Sites.TaskQueue.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Sites.Repository;
using Kooboo.Sites.Sync;

namespace Kooboo.Sites.TaskQueue.TaskExecutor
{
    public class PostSyncObjectTask : ITaskExecutor<PostSyncObject>
    {
        public bool Execute(SiteDb SiteDb, string JsonModel)
        {
            var item = Lib.Helper.JsonHelper.Deserialize<PostSyncObject>(JsonModel);
             
           //var stringcontent = Newtonsoft.Json.JsonConvert.SerializeObject(item.SyncObject); 
            var converter = new IndexedDB.Serializer.Simple.SimpleConverter<SyncObject>();

            var bytes = converter.ToBytes(item.SyncObject); 

            Guid websiteid = item.RemoteSiteId;

            var hash = Lib.Security.Hash.ComputeGuid(bytes); 
             
            string fullurl = item.RemoteUrl + "?" + DataConstants.SiteId + "=" + item.RemoteSiteId.ToString()+"&hash=" + hash.ToString(); 

           return Kooboo.Lib.Helper.HttpHelper.PostData(fullurl, null, bytes, item.UserName, item.Password); 
        }
    }
}
