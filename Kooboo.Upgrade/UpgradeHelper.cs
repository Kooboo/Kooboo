//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;    
using System.Collections.Generic;
using System.Runtime.InteropServices;    

namespace Kooboo.Upgrade
{
    public static class UpgradeHelper
    {
        [DllImport("user32")]
        public static extern int ShowWindow(IntPtr hwnd, int nCmdShow);

        static UpgradeHelper()
        {
            InitRootAndZip(); 
        }

        public static void InitRootAndZip()
        {
            RootPath = GetRoot();
            if (RootPath !=null)
            {
                DownloadZipFile = GetPackageZip(RootPath); 
            }
        }

        private static string GetRoot()
        {
            var basedir = AppDomain.CurrentDomain.BaseDirectory;
            if (IsKoobooDiskRoot(basedir))
            {
                return basedir;
            }
            var info = new System.IO.DirectoryInfo(basedir);
            var parent = info.Parent;
            if (IsKoobooDiskRoot(parent.FullName))
            {
                return parent.FullName;
            }
            parent = parent.Parent;
            if (IsKoobooDiskRoot(parent.FullName))
            {
                return parent.FullName;
            }
            return null;
        }

        private static string GetPackageZip(string root)
        {    
            string name = System.IO.Path.Combine(root, "upgrade", "Kooboo.zip"); 
            if (System.IO.File.Exists(name))
            {
                return name; 
            }
            name = System.IO.Path.Combine(root, "upgradePackage", "Kooboo.zip");
            if (System.IO.File.Exists(name))
            {
                return name;
            }        
            return null; 
        }

        public static string RootPath { get; set; }

        public static string DownloadZipFile { get; set; }
                                                             
        private static bool IsKoobooDiskRoot(string FullPath)
        {
            string ScriptFolder = System.IO.Path.Combine(FullPath, "_Admin", "Scripts");
            if (!Directory.Exists(ScriptFolder))
            {
                return false;
            }
            string ViewFolder = System.IO.Path.Combine(FullPath, "_Admin", "View");
            if (!Directory.Exists(ViewFolder))
            {
                return false;
            }
            return true;
        }

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

                    if (fullPath.StartsWith(RootPath))
                    {
                        //show Kooboo Window,if kooboo window is hidden.
                        ShowKoobooWindow(windows, process);

                        //closeMainWindow only trigger when Kooboo window is show
                        var close = process.CloseMainWindow();

                        process.Close();

                    }
                }
            }

        }

        private static void ShowKoobooWindow(List<IntPtr> windows, Process process)
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
            var dirs = Directory.GetDirectories(RootPath);

            var deleteFolders = new List<string>()
            {
                "_admin"
            };

            foreach (var dir in dirs)
            {
                var dirInfo = new DirectoryInfo(dir);

                if (dirInfo != null && deleteFolders.Contains(dirInfo.Name.ToLower()))
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

            var deleteLangFiles =new string[] { "en.xml", "zh.xml" };
            foreach(var file in deleteLangFiles)
            {
                var path= System.IO.Path.Combine(RootPath, "lang",file);
                try
                {
                    File.Delete(path);
                }
                catch(Exception ex)
                {

                }
            }


            var files = Directory.GetFiles(RootPath);
            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);

                if (fileInfo != null)
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

                        var path = Path.Combine(RootPath, name);

                        if (string.IsNullOrEmpty(entry.Name))
                        {
                            EnsureDirectoryExists(path);
                        }
                        else if (entry.Name.ToLower() == "kooboo.upgrade.exe")
                        {
                            continue;  // already updated... 
                        }    
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
            var path = Path.Combine(RootPath, "Kooboo.exe");

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
            if (dir != null)
            {
                EnsureDirectoryExists(dir);
            }
        }

    }
}
