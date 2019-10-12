//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data;
using Kooboo.IndexedDB;
using Kooboo.Lib;
using System;

namespace Kooboo.App.CrossPlatform
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Kooboo.Lib.Compatible.CompatibleManager.Instance.Framework.RegisterEncoding();

            Kooboo.Data.AppSettings.SetCustomSslCheck();

            GlobalSettings.RootPath = Kooboo.Data.AppSettings.DatabasePath;
            var settingPort = AppSettingsUtility.Get("Port");
            if (string.IsNullOrEmpty(settingPort))
            {
                settingPort = AppSettingsUtility.Get("port");
            }
            var port = DataConstants.DefaultPort;

            if (string.IsNullOrEmpty(settingPort))
            {
                port = Kooboo.Lib.Compatible.CompatibleManager.Instance.System.GetPort(port);
            }
            else if (int.TryParse(settingPort, out port) && Kooboo.Lib.Compatible.CompatibleManager.Instance.System.IsPortInUsed(port))
            {
                string message = Data.Language.Hardcoded.GetValue("Port") + " " + port + " " + Data.Language.Hardcoded.GetValue("is in use");
                Console.WriteLine(message);
                return;
            }

            Kooboo.Data.Hosts.WindowsHost.change = new Data.Hosts.HostChange() { NoChange = true };
            Kooboo.Data.AppSettings.DefaultLocalHost = "localkooboo.com";
            Kooboo.Data.AppSettings.StartHost = "127.0.0.1";

            AppSettings.CurrentUsedPort = port;

            Web.SystemStart.Start(port);
            Console.WriteLine("Web Server Started");
            Console.WriteLine("port:" + port);

            Mail.EmailWorkers.Start();

            //if(UpgradeHelper.HasNewVersion())
            //{
            //    UpgradeHelper.Download();
            //}
            Kooboo.Lib.Compatible.CompatibleManager.Instance.Framework.ConsoleWait();
        }
    }
}