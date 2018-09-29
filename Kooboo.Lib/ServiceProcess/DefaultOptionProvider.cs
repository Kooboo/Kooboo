//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.ServiceProcess
{
    public class DefaultOptionsProvider : ICommandOptionsProvider
    {
        public ICommandOptions Parse(string[] args)
        {
            var result = new Options();
            result.Install = args.Contains("-i") || args.Contains("-install");
            result.Uninstall = args.Contains("-u") || args.Contains("-uninstall");
            return result;
        }

        public string HelpText()
        {
            return
@"-i/-install Install as windows service
-u/-uninstall Uninstall the windows service";
        }

        class Options : ICommandOptions
        {
            public bool Install { get; set; }

            public bool Uninstall { get; set; }
        }

    }
}
