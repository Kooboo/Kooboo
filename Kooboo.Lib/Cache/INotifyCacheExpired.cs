//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Cache
{
    public interface INotifyCacheExpired
    {
        void Notify(string objectCacheName, string key);
    }
}
