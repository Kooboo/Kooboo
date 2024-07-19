//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.IndexedDB
{
    public interface ISchedule
    {
        void Close();

        void DelSelf();

        string Folder { get; set; }
    }
}
