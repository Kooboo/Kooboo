//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.IndexedDB.Queue
{
    public interface IQueue
    {
        void Close();

        void DelSelf();
    }
}
