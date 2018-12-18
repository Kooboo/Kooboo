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

    public class KoobooAutoStartManager
    {
        public static bool IsFirstTimeAutoStart()
        {
            var autostartStr = AppSettings.AutoStart;
            if (string.IsNullOrEmpty(autostartStr))
                return true;
            return false;
        }
        public static bool IsAutoStart()
        {
            var autostartStr = AppSettings.AutoStart;
            bool autoStart;

            if (!bool.TryParse(autostartStr, out autoStart))
            {
                autoStart = false;
            }
            return autoStart;
        }
        public static void AutoStart(bool auto)
        {
            try
            {
                string path = System.Windows.Forms.Application.ExecutablePath;
                RegistryKey rk = Registry.LocalMachine;
                RegistryKey subKey = rk.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
                if (auto)
                {
                    subKey.SetValue("KoobooApp", path);
                }
                else
                {
                    subKey.DeleteValue("KoobooApp", false);
                }

                AppSettings.SetConfigValue("AutoStart", auto.ToString());
                subKey.Close();
                rk.Close();
            }
            catch (Exception ex)
            {

            }

        }
    }
}
