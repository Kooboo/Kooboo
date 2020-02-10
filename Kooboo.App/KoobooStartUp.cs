//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.App.Models;
using Kooboo.Data;
using Kooboo.Lib;
using System;
using System.Windows;
using Microsoft.Win32;
using System.Linq;

namespace Kooboo.App
{
    public static class KoobooStartUp
    {
    

        public static void StartAll()
        { 
            var initport = Data.AppSettings.InitPort();
            if (!initport.Ok)
            {
                MessageBox.Show(initport.ErrorMessage);
                return;
            }
            SystemStatus.Port = initport.HttpPort;
            Web.SystemStart.Start(initport.HttpPort);
            Mail.EmailWorkers.Start();
        }       

        public static void StopAll()
        {
            try
            {
                Web.SystemStart.Stop(); 
            }
            catch (Exception ex)
            {
            }
        }
    }

}
