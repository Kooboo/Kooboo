using System;
using Kooboo.Data;
using Kooboo.Lib; 
using Kooboo.IndexedDB;

namespace Kooboo.App.CrossPlatform
{
    class Program
    {
        static void Main(string[] args)
        {
     
            Kooboo.Lib.Compatible.CompatibleManager.Instance.Framework.RegisterEncoding();

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
                string message = Data.Language.Hardcoded.GetValue("Port") + " " + port.ToString() + " " + Data.Language.Hardcoded.GetValue("is in use");
                Console.WriteLine(message);
                return;
            }

            Kooboo.Data.AppSettings.DefaultLocalHost = "localkooboo.com";
            Kooboo.Data.AppSettings.StartHost = "127.0.01"; 

            AppSettings.CurrentUsedPort = port;

            Web.SystemStart.Start(port);
            Console.WriteLine("Web Server Started");
            Console.WriteLine("port:" + port);

            Mail.EmailWorkers.Start(); 

            if(UpgradeHelper.HasNewVersion())
            {
                UpgradeHelper.Download();
            } 
            Kooboo.Lib.Compatible.CompatibleManager.Instance.Framework.ConsoleWait(); 
        }

    }
}
