using System;
using Kooboo.Data;
using Kooboo.Lib;
using Kooboo.Lib.Helper;
using Kooboo.IndexedDB;

namespace Kooboo.App.CrossPlatform
{
    class Program
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, arguments) =>
            {
                System.IO.File.AppendAllText("log.txt", "Unhandled exception: " + arguments.ExceptionObject);
                Environment.Exit(1);
            };

            TextEncodingHelper.RegisterEncoding();

            GlobalSettings.RootPath = Kooboo.Data.AppSettings.DatabasePath;
            var settingPort = AppSettingsUtility.Get("Port");
            if (string.IsNullOrEmpty(settingPort))
            {
                settingPort = AppSettingsUtility.Get("port");
            }
            var port = Kooboo.DataConstants.DefaultPort;

            if (string.IsNullOrEmpty(settingPort))
            {
                while (Kooboo.Lib.Helper.NetworkHelper.IsPortInUse(port) && port < 65535)
                {
                    port += 1;
                }
            }
            else if (int.TryParse(settingPort, out port) && Kooboo.Lib.Helper.NetworkHelper.IsPortInUse(port))
            {
                string message = Kooboo.Data.Language.Hardcoded.GetValue("Port") + " " + port.ToString() + " " + Kooboo.Data.Language.Hardcoded.GetValue("is in use");
                Console.WriteLine(message);
               //Console.ReadKey();
                return;
            }
            AppSettings.CurrentUsedPort = port;


            if (RequireIpData())
            {
                Console.WriteLine("IP data missing...");
            }
            else
            {
                Kooboo.Web.SystemStart.Start(port);

                // GenerateSsl();

                Kooboo.Mail.EmailWorkers.Start();
                Console.WriteLine($"Server started on http://localhost:{port}");
            }

            while (true)
            {
                System.Threading.Thread.Sleep(1000);
            }
            //Console.ReadKey();
        }

        public static bool RequireIpData()
        {
            if (Kooboo.Data.AppSettings.IsOnlineServer)
            {
                if (Kooboo.Data.GeoLocation.IPLocation.IpCityStore == null)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
