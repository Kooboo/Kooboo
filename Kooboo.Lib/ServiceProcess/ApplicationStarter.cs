//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceProcess;
using System.Configuration.Install;

namespace Kooboo.ServiceProcess
{
    public class ApplicationStarter
    {
        public ApplicationStarter()
            : this(new DefaultOptionsProvider())
        {
        }

        public ApplicationStarter(ICommandOptionsProvider optionsProvider)
        {
            OptionsProvider = optionsProvider;

            Commands.Add(new InstallCommand());
            Commands.Add(new UninstallCommand());
            Commands.Add(new ServiceCommand());
            Commands.Add(new ConsoleCommand());
        }

        public ICommandOptionsProvider OptionsProvider { get; private set; }

        private IList<ICommand> _commands;
        public IList<ICommand> Commands
        {
            get
            {
                if (_commands == null)
                {
                    _commands = new CommandCollection(this);
                }
                return _commands;
            }
        }
        
        public void Run(string[] args)
        {
            if (args.Contains("/?", StringComparer.OrdinalIgnoreCase))
            {
                Console.WriteLine(OptionsProvider.HelpText());
                return;
            }

            var options = OptionsProvider.Parse(args);
            if (options == null)
            {
                Console.WriteLine(OptionsProvider.HelpText());
                return;
            }

            foreach (var each in Commands)
            {
                if (each.Execute(options))
                    break;
            }
        }

        public Installer CreateInstaller(ServiceInstallEventArgs args)
        {
            var installArgs = new List<string>
            {
                "/LogFile=" + args.Install.LogFile,
                "/LogToConsole=false" + args.Install.LogToConsole,
            };
            if (!String.IsNullOrEmpty(args.Install.StateDir))
            {
                installArgs.Add("/InstallStateDir=" + args.Install.StateDir);
            }
            var installer = new IntegratedInstaller(installArgs.ToArray());
            installer.Installers.Add(CreateServiceInstaller(args));
            installer.Installers.Add(CreateServiceProcessInstaller(args));
            return installer;
        }

        public ServiceInstaller CreateServiceInstaller(ServiceInstallEventArgs args)
        {
            return new ServiceInstaller
            {
                ServiceName = args.Service.Name,
                Description = args.Service.Description,
                StartType = args.Service.StartMode,
            };
        }

        public ServiceProcessInstaller CreateServiceProcessInstaller(ServiceInstallEventArgs args)
        {
            return new ServiceProcessInstaller
            {
                Account = args.Service.Account
            };
        }

        public virtual void OnServiceInstalling(ServiceInstallEventArgs args)
        {
            ServiceInstalling?.Invoke(this, args);
        }

        public virtual void OnStart(ICommandOptions options)
        {
            if (Start != null)
            {
                Start(this, new ServiceStartEventArgs { Options = options });
            }
        }

        public virtual void OnStop()
        {
            if (Stop != null)
            {
                Stop(this, new EventArgs());
            }
        }

        public event EventHandler<ServiceInstallEventArgs> ServiceInstalling;

        public event EventHandler<ServiceStartEventArgs> Start;

        public event EventHandler<EventArgs> Stop;
    }

    public class CommandCollection : List<ICommand>, IList<ICommand>, ICollection<ICommand>
    {
        private ApplicationStarter _starter;

        public CommandCollection(ApplicationStarter starter)
        {
            _starter = starter;
        }

        public new void Add(ICommand item)
        {
            item.Starter = _starter;
            base.Add(item);
        }

        public new void Insert(int index, ICommand item)
        {
            item.Starter = _starter;
            base.Insert(index, item);
        }

        public new ICommand this[int index]
        {
            get
            {
                return base[index];
            }
            set
            {
                value.Starter = _starter;
                base[index] = value;
            }
        }

        void ICollection<ICommand>.Add(ICommand item)
        {
            Add(item);
        }

        void IList<ICommand>.Insert(int index, ICommand item)
        {
            Insert(index, item);
        }

        ICommand IList<ICommand>.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                this[index] = value;
            }
        }
    }

    public class ServiceInstallEventArgs : EventArgs
    {
        public ServiceInstallEventArgs()
        {
            Install = new InstallArgs();
            Service = new ServiceArgs();
        }

        public InstallArgs Install { get; set; }

        public ServiceArgs Service { get; set; }

        public ICommandOptions Options { get; set; }

        public class InstallArgs
        {
            public string StateDir { get; set; }

            public bool LogToConsole { get; set; }

            public string LogFile { get; set; }
        }

        public class ServiceArgs
        {
            public ServiceArgs()
            {
                Account = ServiceAccount.NetworkService;
                StartMode = ServiceStartMode.Automatic;
            }

            public string Name { get; set; }

            public string Description { get; set; }

            public ServiceAccount Account { get; set; }

            public ServiceStartMode StartMode { get; set; }
        }
    }

    public class ServiceStartEventArgs : EventArgs
    {
        public ICommandOptions Options { get; set; }
    }
}
