//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.IndexedDB
{
    public interface ISequence
    {
        void Close();

        void DelSelf();

        void Flush();
    }
}
