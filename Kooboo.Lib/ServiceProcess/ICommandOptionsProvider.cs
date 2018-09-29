using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.ServiceProcess
{
    public interface ICommandOptionsProvider
    {
        ICommandOptions Parse(string[] args);

        string HelpText();
    }
}
