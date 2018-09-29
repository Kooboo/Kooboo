//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Sites.Repository;

namespace Kooboo.Sites.TaskQueue.TaskExecutor
{
    public class HttpPostStringContentExecutor : ITaskExecutor<HttpPostStringContent>
    {
        public bool Execute(SiteDb SiteDb, string JsonModel)
        { 
            var item = Lib.Helper.JsonHelper.Deserialize<HttpPostStringContent>(JsonModel);

           return Kooboo.Lib.Helper.HttpHelper.PostData(item.RemoteUrl, null, System.Text.Encoding.UTF8.GetBytes(item.StringContent), item.UserName, item.Password);  
        }
    }
}
