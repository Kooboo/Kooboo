//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceProcess;
using System.Threading;

namespace Kooboo.ServiceProcess
{
    public class ServiceCommand : ICommand
    {
        public ApplicationStarter Starter { get; set; }

        public bool Execute(ICommandOptions options)
        {
            if (Environment.UserInteractive)
                return false;

            ServiceBase.Run(new ServiceBase[] 
			{ 
				CreateService()
			});

            return true;
        }

        protected ServiceBase CreateService()
        {
            return new DefaultService(Starter);
        }
    }

    public class DefaultService : ServiceBase
    {
        private ApplicationStarter _starter;

        public DefaultService(ApplicationStarter starter)
        {
            _starter = starter;
        }

        protected override void OnStart(string[] args)
        {
            var options = _starter.OptionsProvider.Parse(args);
            var thread = new Thread(() =>
            {
                _starter.OnStart(options);
            });
            thread.Start();
        }

        protected override void OnStop()
        {
            _starter.OnStop();
        }
    }
}
