using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kooboo.ServiceProcess
{
    public interface ICommand
    {
        ApplicationStarter Starter { get; set; }

        bool Execute(ICommandOptions options);
    }
}
