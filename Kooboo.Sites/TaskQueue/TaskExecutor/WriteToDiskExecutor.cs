//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Sites.TaskQueue.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Sites.Repository;

namespace Kooboo.Sites.TaskQueue.TaskExecutor
{
    public class WriteToDiskExecutor : ITaskExecutor<WriteToDisk>
    {
        public bool Execute(SiteDb SiteDb, string JsonModel)
        {
            var item = Lib.Helper.JsonHelper.Deserialize<WriteToDisk>(JsonModel);

            return Kooboo.Sites.Sync.SyncService.WriteToDisk(SiteDb, item);

        }
    }
}
