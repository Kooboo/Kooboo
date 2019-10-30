//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Sites.Repository;

namespace Kooboo.Sites.TaskQueue.TaskExecutor
{
    public interface ITaskExecutor<T> : IExecutor
    {
    }

    public interface IExecutor
    {
        bool Execute(SiteDb siteDb, string jsonModel);
    }
}