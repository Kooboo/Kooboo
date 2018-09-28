using System;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;
using System.Configuration;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Kooboo.Upgrade
{
    public static class UpgradeHelper
    {
        [DllImport("user32")]
        public static extern int ShowWindow(IntPtr hwnd, int nCmdShow);

        static UpgradeHelper()
        {
            var exeRoot = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            ParentExePath = Directory.GetParent(exeRoot).FullName;

            string path = System.IO.Path.Combine(ParentExePath, "upgradePackage");  
            DownloadZipFile = System.IO.Path.Combine(path, "Kooboo.zip");
        }

        public static string DownloadZipFile { get; set; }

        //public static string ExeRoot { get; set; }

        public static string ParentExePath { get; set; }
         
        public static void Upgrade()
        {
            if (!File.Exists(DownloadZipFile)) return;

            CloseKooboo();

            DeleteOldFiles();

            UnzipDownloadPackage();

            // clean the download zip...
            if (File.Exists(DownloadZipFile))
            {
               File.Delete(DownloadZipFile);
            }
        }

        private static void CloseKooboo()
        {
            var koobooProcesses = Process.GetProcessesByName("Kooboo");

            var windows = WindowHelper.GetAllKoobooWindows();
            if (koobooProcesses != null)
            {
                foreach (var process in koobooProcesses)
                { 
                    // make sure does not close other instance.  
                    string fullPath = process.MainModule.FileName;

                    if (fullPath.StartsWith(ParentExePath))
                    {
                        //show Kooboo Window,if kooboo window is hidden.
                        ShowKoobooWindow(windows, process);

                        //closeMainWindow only trigger when Kooboo window is show
                        var close= process.CloseMainWindow();

                        process.Close();
                        
                    }
                }
            }

        }

        private static void ShowKoobooWindow(List<IntPtr> windows,Process process)
        {
            //hidden kooboo window,process.MainWindowHandle will be IntPrt.Zero
            var hwnd = process.MainWindowHandle;
            if (hwnd == IntPtr.Zero)
            {
                hwnd = WindowHelper.GetKoobooWindow(windows, process.Id);

                if (hwnd != IntPtr.Zero)
                {
                    //Show the window
                    var SW_SHOW = 5;
                    ShowWindow(hwnd, SW_SHOW);
                }
            }
            
        }

        private static void DeleteOldFiles()
        {
            var path = ParentExePath;

            var dirs = Directory.GetDirectories(path);

            var excludeFiles = new List<string>()
            {
                "appdata",
                "upgradepackage",
                "upgrade"
            };
            foreach (var dir in dirs)
            {
                var dirInfo = new DirectoryInfo(dir);

                if (dirInfo != null && !excludeFiles.Contains(dirInfo.Name.ToLower()))
                {
                    try
                    {
                        Directory.Delete(dir, true);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }

            var files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);

                if (fileInfo != null && fileInfo.Name.ToLower() != "kooboo.exe.config")
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch (Exception ex)
                    {

                    } 
                } 
            }
        }

        private static void UnzipDownloadPackage()
        {
            if (!File.Exists(DownloadZipFile)) return;

            var unzipPath = ParentExePath; 

            using (var archive = ZipFile.Open(DownloadZipFile, ZipArchiveMode.Read))
            {
                if (archive.Entries.Count > 0)
                {
                    foreach (var entry in archive.Entries)
                    {
                        //remove base dir// 
                        string basedir = "Kooboo/";  
                        string name = entry.FullName; 
                        if (name.StartsWith(basedir))
                        {
                            name = name.Substring(basedir.Length); 
                        } 

                        if (string.IsNullOrEmpty(name)) continue;

                        var path = Path.Combine(unzipPath, name);

                        if (string.IsNullOrEmpty(entry.Name))
                        {
                            EnsureDirectoryExists(path); 
                        }
                        else if (entry.Name.ToLower() == "kooboo.upgrade.exe")
                        {
                            continue;  // already updated... 
                        }
                        //else if (entry.Name.ToLower() == "kooboo.exe.config")
                        //{
                        //    var configPath =Path.Combine(ExeRoot, "kooboo.exe");
                        //    var config = ConfigurationManager.OpenExeConfiguration(configPath);
                        //    var settings = config.AppSettings;

                        //    var keyValues = new Dictionary<string, string>();
                        //    foreach (var key in settings.Settings.AllKeys)
                        //    {
                        //        keyValues.Add(key, settings.Settings[key].Value);
                        //    }
                        //    entry.ExtractToFile(path, true);

                        //    config = ConfigurationManager.OpenExeConfiguration(configPath);
                        //    foreach (var keyPair in keyValues)
                        //    {

                        //        if (config.AppSettings.Settings[keyPair.Key] != null)
                        //            config.AppSettings.Settings[keyPair.Key].Value = keyPair.Value;
                        //        else
                        //            config.AppSettings.Settings.Add(keyPair.Key, keyPair.Value);
                        //    }
                        //    config.Save(ConfigurationSaveMode.Modified);
                        //}
                        else
                        {
                            EnsureFileDirectoryExists(path);  
                            try
                            {
                                entry.ExtractToFile(path, true); 
                            }
                            catch (Exception ex)
                            { 
                            }
                        }
                    }
                }
            }
        }

        public static void OpenKoobooApp()
        {
            var path = Path.Combine(ParentExePath, "Kooboo.exe");

            try
            {
                Process process = Process.Start(path);
                if (process.HasExited)
                {
                    Process.Start(path);
                }
            }
            catch (Exception ex)
            {
                //Process.Start(path);
            }
        }
         
        private static void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        private static void EnsureFileDirectoryExists(string filePath)
        {
            var dir = Path.GetDirectoryName(filePath);
            if (dir !=null)
            {
                EnsureDirectoryExists(dir);
            } 
        }

    }
}
