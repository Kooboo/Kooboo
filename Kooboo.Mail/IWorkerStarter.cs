//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kooboo.Tasks
{
    public interface IWorkerStarter
    {
        string Name { get; }

        void Start();

        void Stop();
    }
}
