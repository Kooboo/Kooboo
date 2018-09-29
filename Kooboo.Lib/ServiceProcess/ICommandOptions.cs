//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.ServiceProcess
{
    public interface ICommandOptions
    {
        bool Install { get; }

        bool Uninstall { get; }
    }
}
