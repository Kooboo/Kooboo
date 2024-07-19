//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Tasks
{
    public interface IWorkerStarter
    {
        string Name { get; }

        void Start();

        void Stop();
    }
}
