using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace Kooboo.App
{
    public class KoobooAutoStart
    {
        private static string GetAutoStartPath()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var dir = System.IO.Path.GetDirectoryName(assembly.Location);
            dir = System.IO.Path.Combine(dir, "_Admin");
            var autoStartPath = System.IO.Path.Combine(dir, "AutoStart.txt");

            return autoStartPath;
        }
        public static bool IsFirstTimeAutoStart()
        {
            var autoStartPath = GetAutoStartPath();
            if (!File.Exists(autoStartPath))
                return true;
            return false;
        }
        public static bool IsAutoStart()
        {
            var autoStartPath = GetAutoStartPath();
            if (!File.Exists(autoStartPath))
            {
                return true;
            }

            var autoStart = false;
            var result = File.ReadAllText(autoStartPath);
            bool.TryParse(result,out autoStart);
            
            return autoStart;
        }
        private static void SetAutoStart(string autoStart)
        {
            var autoStartPath = GetAutoStartPath();
            var dir = System.IO.Path.GetDirectoryName(autoStartPath);
            if(!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            File.WriteAllText(autoStartPath, autoStart);
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

                SetAutoStart(auto.ToString());

                subKey.Close();
                rk.Close();
            }
            catch (Exception ex)
            {

            }

        }
    }
}
