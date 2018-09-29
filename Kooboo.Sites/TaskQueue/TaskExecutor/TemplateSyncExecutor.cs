//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Sites.TaskQueue.Model;
using System.Collections.Generic;
using Kooboo.Sites.Repository;
using Kooboo.Data;

namespace Kooboo.Sites.TaskQueue.TaskExecutor
{
    public class TemplateSyncExecutor : ITaskExecutor<TemplateSyncModel>
    {
        public bool Execute(SiteDb SiteDb, string JsonModel)
        {
            var model = Lib.Helper.JsonHelper.Deserialize<TemplateSyncModel>(JsonModel);
              
            List<string> ips = Data.Helper.TemplateHelpder.SyncServerIps;

            foreach (var item in ips)
            {
                if (!Lib.Helper.IPHelper.IsInSameCClass(item, Sites.Sync.Template.MyIp))
                {
                    string remoteurl = "http://" + item + "/_api/template/NotifyUpdate";
                    var getresult = Lib.Helper.HttpHelper.Get<bool>(remoteurl);
                }
            }
            return true;
        }   
    }
}
