//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.IO;
using System.Linq;
using System.Runtime;
using System.Threading;
using Kooboo.Data;
using Kooboo.Data.Hosts;
using Kooboo.IndexedDB;
using Kooboo.Lib.Helper;

namespace Kooboo.Web.CrossPlatform
{
    class Program
    {
        static void Main(string[] args)
        {
            new KoobooCommandLine(args).Invoke();
            Console.WriteLine("Kooboo AppData path: " + AppSettings.AppDataFolder);
            System.Threading.Tasks.Task.Run(() =>
            {
                while (true)
                {
                    if (GCSettings.LargeObjectHeapCompactionMode == GCLargeObjectHeapCompactionMode.Default)
                    {
                        GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                        GC.Collect();
                    }
                    Thread.Sleep(5000);
                }
            });


            Lib.Compatible.CompatibleManager.Instance.Framework.RegisterEncoding();
            GlobalSettings.RootPath = Data.AppSettings.DatabasePath;

            var initPort = Data.AppSettings.InitPort();
            if (!initPort.Ok)
            {
                Console.WriteLine(initPort.ErrorMessage);
                return;
            }

            Host.Instance.HasAccess = false;
            SystemStart.Start(initPort.HttpPort);
            Console.WriteLine("Web Server Started at port:" + initPort.HttpPort.ToString());
            if (args.Contains("--daemon"))
            {
                HandleDaemon(initPort.HttpPort);
            }

            Mail.EmailWorkers.Start();

            // set forward for debug..
            // Mail.Settings.ForwardRequired = true;

            SystemStart.WebServer.WaitForShutdown();
        }

        private static void HandleDaemon(int port)
        {
            var uriBuilder = new UriBuilder("http", "localhost", port, "/_Admin/");
            var url = uriBuilder.Uri.AbsoluteUri;
            var pid = Environment.ProcessId;
            var target = Path.Combine(AppSettings.AppDataFolder, "kooboo.daemon");
            File.WriteAllText(target, JsonHelper.Serialize(new { pid, url }));
        }
    }
}
