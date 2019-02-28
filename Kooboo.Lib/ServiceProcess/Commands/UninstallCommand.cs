//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration.Install;

namespace Kooboo.ServiceProcess
{
    public class UninstallCommand : ICommand
    {
        public ApplicationStarter Starter { get; set; }

        public bool Execute(ICommandOptions options)
        {
            if (!options.Uninstall)
                return false;

            var installArgs = new ServiceInstallEventArgs();
            Starter.OnServiceInstalling(installArgs);

            UninstallService(installArgs);

            return true;
        }

        public void UninstallService(ServiceInstallEventArgs args)
        {
            Console.WriteLine("Uninstalling service ......");
            using (var installer = Starter.CreateInstaller(args))
            {
                IDictionary state = new Hashtable();
                try
                {
                    installer.Uninstall(state);
                    Console.WriteLine("Service uninstalled.");
                }
                catch
                {
                    installer.Rollback(state);
                    throw;
                }
            }
        }

    }
}
