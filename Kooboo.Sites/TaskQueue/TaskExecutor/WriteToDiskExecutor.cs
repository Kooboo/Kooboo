//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Sites.Repository;
using Kooboo.Sites.TaskQueue.Model;

namespace Kooboo.Sites.TaskQueue.TaskExecutor
{
    public class WriteToDiskExecutor : ITaskExecutor<WriteToDisk>
    {
        public bool Execute(SiteDb siteDb, string jsonModel)
        {
            var item = Lib.Helper.JsonHelper.Deserialize<WriteToDisk>(jsonModel);

            return Kooboo.Sites.Sync.SyncService.WriteToDisk(siteDb, item);
        }
    }
}