//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using Kooboo.IndexedDB;

namespace Kooboo.App.CrossPlatform
{
    class Program
    {
        static void Main(string[] args)
        {
            Kooboo.Lib.Compatible.CompatibleManager.Instance.Framework.RegisterEncoding(); 
            GlobalSettings.RootPath = Kooboo.Data.AppSettings.DatabasePath;

            var initport = Data.AppSettings.InitPort();
            if (!initport.Ok)
            {
                Console.WriteLine(initport.ErrorMessage);
                return;
            }

            Kooboo.Data.Hosts.WindowsHost.change = new Data.Hosts.HostChange() { NoChange = true }; 
            Kooboo.Data.AppSettings.DefaultLocalHost = "localkooboo.com";
            Kooboo.Data.AppSettings.StartHost = "127.0.0.1"; 
             
            Web.SystemStart.Start(initport.HttpPort);
            Console.WriteLine("Web Server Started at port:" + initport.HttpPort.ToString());
        
            if (!Kooboo.Data.AppSettings.DisableMail)
            {
                Mail.EmailWorkers.Start();
            }

            Kooboo.Lib.Compatible.CompatibleManager.Instance.Framework.ConsoleWait(); 
        } 
    }
}
