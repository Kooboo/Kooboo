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
        private static System.Diagnostics.Process ScreenShotProcess = null;
        public static void StartAll()
        {
            Kooboo.Data.AppSettings.SetCustomSslCheck();
            var settingPort = AppSettingsUtility.Get("Port");
            if (string.IsNullOrEmpty(settingPort))
            {
                settingPort = AppSettingsUtility.Get("port");
            }
            var port = DataConstants.DefaultPort;

            if (string.IsNullOrEmpty(settingPort))
            {
                while (Lib.Helper.NetworkHelper.IsPortInUse(port) && port < 65535)
                {
                    port += 1;
                }
            }
            else if (int.TryParse(settingPort, out port) && Lib.Helper.NetworkHelper.IsPortInUse(port))
            {
                string message = Data.Language.Hardcoded.GetValue("Port") + " " + port.ToString() + " " + Data.Language.Hardcoded.GetValue("is in use");
                MessageBox.Show(message);
                return;
            }
            AppSettings.CurrentUsedPort = port;
            SystemStatus.Port = port;
                                      
            Web.SystemStart.Start(port);     
            Mail.EmailWorkers.Start();   
        }       

        public static void StopAll()
        {
            try
            {
                if (ScreenShotProcess != null)
                {
                    ScreenShotProcess.CloseMainWindow();
                    ScreenShotProcess.Close();
                }
            }
            catch (Exception ex)
            {
            }
        }
    }

}
