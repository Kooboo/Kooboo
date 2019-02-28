//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Xml.Linq;
using System.Xml.Serialization;
using Kooboo.App.Upgrade.Model;
using System.Threading;
using System.Diagnostics;

namespace Kooboo.App.Upgrade
{
    public static class KoobooUpgrade
    {
        static KoobooUpgrade()
        {
            InitRootAndZip();
        }
        public static string UpgradeLogPath { get; set; }

        private static string _rootPath;
        public static string RootPath
        {
            get { return _rootPath; }
            set
            {
                if(value!=null)
                {
                    _rootPath = value;
                    InitOthers();
                }
            }
        }

        public static string DownloadZipFile { get; set; }

        public static string UnzipDownloadFilePath { get; set; }

        public static string KoobooAppName { get; set; }

        public static List<string> UpgradeFolders { get; set; }

        public static List<string> UpgradeLangFiles { get; set; }

        public static List<string> ExcludeFiles { get; set; }

        public static string ConfigFile { get; set; }


        #region init
        public static void InitRootAndZip()
        {
            RootPath = GetRoot();
        }
        private static void InitOthers()
        {
            UpgradeLogPath = System.IO.Path.Combine(RootPath, "upgradePackage", "upgradeLog.txt");
            DownloadZipFile = GetPackageZip(RootPath);
            if (DownloadZipFile != null)
            {
                UnzipDownloadFilePath = Path.GetDirectoryName(DownloadZipFile);
            }
            KoobooAppName = "Kooboo.App.dll";
            ConfigFile = "Kooboo.App.dll.config";

            UpgradeFolders = new List<string>() { "_Admin" };
            UpgradeLangFiles = new List<string> { "en.xml", "zh.xml" };
            ExcludeFiles = new List<string>()
                        {
                        "Kooboo.App.Upgrade.dll",
                        ConfigFile,
                        "Kooboo.App.Upgrade.runtimeconfig.json",
                        "System.Buffers.dll"
                        };
        }

        public static void Log(string content)
        {
            var log = string.Format("{0}-----{1}{2}", DateTime.Now.ToString(), content, Environment.NewLine);
            var fileInfo = new FileInfo(UpgradeLogPath);
            if (!Directory.Exists(fileInfo.DirectoryName))
            {
                Directory.CreateDirectory(fileInfo.DirectoryName);
            }
            System.IO.File.AppendAllText(UpgradeLogPath, log);
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

        private static string GetPackageZip(string root)
        {
            string name = System.IO.Path.Combine(root, "upgradePackage", "KoobooLinux.zip");
            if (System.IO.File.Exists(name))
            {
                return name;
            }
            name = System.IO.Path.Combine(root, "upgradePackage", "Kooboo.zip");
            if (System.IO.File.Exists(name))
                return name;
            return null;
        }

        #endregion

        public static void CloseKooboo()
        {
            var dotnetProcesses = Process.GetProcessesByName("dotnet");
            foreach (var process in dotnetProcesses)
            {
                var processId = process.Id;
                var cmd = string.Format("ps -ef|grep {0}", processId);
                Log(processId.ToString());
                var str = CmdHelper.Excute(cmd, false);
                if (str.IndexOf(KoobooAppName, StringComparison.OrdinalIgnoreCase)> -1)
                {
                    var closeCmd = "kill " + processId;
                    CmdHelper.Excute(closeCmd, false);
                    //break;
                    //maybe open multi kooboo.app,so it continue to check kooboo.app process
                }
            }
            //wait koobo to close.
            WaitKoobooClosed();
        }

        private static void WaitKoobooClosed()
        {
            var isClosed = IsKoobooClosed();
            while (!isClosed)
            {
                Thread.Sleep(1000);
                isClosed = IsKoobooClosed();
            }
        }
        private static bool IsKoobooClosed()
        {
            var dotnetProcesses = Process.GetProcessesByName("dotnet");
            foreach (var process in dotnetProcesses)
            {
                var processId = process.Id;
                var cmd = string.Format("ps -ef|grep {0}", processId);

                var str = CmdHelper.Excute(cmd, false);
                if (str.IndexOf(KoobooAppName, StringComparison.OrdinalIgnoreCase) == -1) return true;
            }
            return false;
        }

        public static void Upgrade()
        {
            if (!UnzipDownloadPackage())
            {
                Log("unzip failed or don't exist zip");
                return;
            }
            if (File.Exists(DownloadZipFile))
            {
                File.Delete(DownloadZipFile);
            }
            UpgradeAfterUnzipPackage();

            if (Directory.Exists(UnzipDownloadFilePath))
            {
                Directory.Delete(UnzipDownloadFilePath,true);
            }
        }

        public static void UpgradeAfterUnzipPackage()
        {
            Log("DeleteOldFiles");
            DeleteOldFiles();

            Log("UpdateFiles");
            UpdateFiles();
        }

        #region delete old files
        //test file is delete
        public static void DeleteOldFiles()
        {
            //only delete _admin folder
            var deleteFolders = UpgradeFolders;
            DeleteFolders(deleteFolders);

            //only delete en.xml and zh.xml
            var deleteLangFiles = UpgradeLangFiles;
            DeleteLangFiles(deleteLangFiles);

            //not delete the follow files
            DeleteRootFiles(ExcludeFiles);
        }

        private static void DeleteFolders(List<string> deleteFolders)
        {
            var dirs = Directory.GetDirectories(RootPath);

            foreach (var dir in dirs)
            {
                var dirInfo = new DirectoryInfo(dir);

                if (dirInfo != null && deleteFolders.Contains(dirInfo.Name))
                {
                    try
                    {
                        Directory.Delete(dir, true);
                    }
                    catch (Exception ex)
                    {
                        Log(string.Format("delete dir execption:{0}{1}", dir, ex.Message));
                    }
                }
            }
        }

        private static void DeleteLangFiles(List<string> deleteLangFiles)
        {
            
            foreach (var file in deleteLangFiles)
            {
                var path = System.IO.Path.Combine(RootPath, "Lang", file);
                try
                {
                    File.Delete(path);
                }
                catch (Exception ex)
                {
                    Log(string.Format("delete lang file execption:{0}{1}", path, ex.Message));
                }
            }
        }

        private static void DeleteRootFiles(List<string> excludeFiles)
        {
            var files = Directory.GetFiles(RootPath);

            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);

                if (fileInfo != null)
                {
                    try
                    {
                        if (!excludeFiles.Contains(fileInfo.Name))
                        {
                            File.Delete(file);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log(string.Format("delete file execption:{0}{1}", file, ex.Message));
                    }
                }
            }
        }

        #endregion

        public static bool UnzipDownloadPackage()
        {
            return UnzipDownloadPackage(DownloadZipFile, UnzipDownloadFilePath);
        }
        public static bool UnzipDownloadPackage(string zipFile,string unzipPath)
        {
            if (!File.Exists(zipFile)) return false;

            try
            {
                ZipFile.ExtractToDirectory(zipFile, unzipPath);
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
            
        }

        #region update files
        public static void UpdateFiles()
        {
            ResetUnzipPath();
            var copyFolders = UpgradeFolders;
            CopyFolders(copyFolders);

            var langFiles = UpgradeLangFiles;
            CopyLangFiles(langFiles);

            CopyRootFiles(ExcludeFiles);
            UpdateConfigFile();
        }

        private static void ResetUnzipPath()
        {
            var path = System.IO.Path.Combine(UnzipDownloadFilePath, "Kooboo");
            if(System.IO.Directory.Exists(path))
            {
                UnzipDownloadFilePath = path;
            }

        }
        private static void CopyFolders(List<string> copyFolders)
        {
            try
            {
                foreach(var copyFolder in copyFolders)
                {
                    var folder = System.IO.Path.Combine(UnzipDownloadFilePath, copyFolder);
                    var dirs = Directory.GetDirectories(folder, "*", SearchOption.AllDirectories);
                    foreach (var dir in dirs)
                    {
                        Directory.CreateDirectory(dir.Replace(UnzipDownloadFilePath, RootPath));
                    }
                    var files = Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories);
                    foreach (string file in files)
                    {
                        File.Copy(file, file.Replace(UnzipDownloadFilePath, RootPath), true);
                    }
                }
               
                    
            }
            catch(Exception ex)
            {
                Log(string.Format("copy dir execption:{0}", ex.Message));
            }
        }

        private static void CopyLangFiles(List<string> copyLangFiles)
        {
            foreach (var file in copyLangFiles)
            {
                try
                {
                    var sourceFile = System.IO.Path.Combine(UnzipDownloadFilePath, "Lang", file);
                    var destFile = System.IO.Path.Combine(RootPath, "Lang", file);
                    if(System.IO.File.Exists(sourceFile))
                    {
                        File.Copy(sourceFile, destFile, true);
                    }
                    
                }
                catch (Exception ex)
                {
                    Log(string.Format("copy lang file execption:{0}", ex.Message));
                }
            }
        }

        private static void CopyRootFiles(List<string> excludeFiles)
        {
            var files = Directory.GetFiles(UnzipDownloadFilePath);

            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);

                if (fileInfo != null)
                {
                    try
                    {
                        if (!excludeFiles.Contains(fileInfo.Name))
                        {
                            var destFile = Path.Combine(RootPath, fileInfo.Name);
                            File.Copy(fileInfo.FullName, destFile, true);
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        Log(string.Format("copy file execption:{0}{1}", file, ex.Message));
                    }
                }
            }
        }

        private static void UpdateConfigFile()
        {
            var oldConfig = Path.Combine(RootPath, ConfigFile);
            var newConfig = Path.Combine(UnzipDownloadFilePath, ConfigFile);
            UpdateConfigFile(oldConfig, newConfig);
        }
        public static void UpdateConfigFile(string oldConfig,string newConfig)
        {
            XmlSerializer serializer = XmlSerializer.FromTypes(new[] { typeof(AppConfigModel) })[0];
            AppConfigModel oldSetting = ReadConfig(oldConfig);
            
            if(oldSetting==null)
            {
                if (File.Exists(newConfig))
                {
                    File.Copy(newConfig, oldConfig);
                    return;
                }
            }
            AppConfigModel newSetting = ReadConfig(newConfig);

            if (newSetting == null)
                return;

            foreach (var setting in newSetting.AppSettings)
            {
                var exist = oldSetting.AppSettings.Exists((s) => s.Key == setting.Key);
                if (!exist)
                {
                    oldSetting.AppSettings.Add(setting);
                }
            }
            var ns = new XmlSerializerNamespaces();
            //Add an empty namespace and empty value
            ns.Add("", "");
            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            serializer.Serialize(stream, oldSetting, ns);
            string xmlstring = System.Text.Encoding.UTF8.GetString(stream.ToArray());

            System.IO.File.WriteAllText(oldConfig,xmlstring);
        }

        public static AppConfigModel ReadConfig(string path)
        {
            if (!System.IO.File.Exists(path)) return null;
            XmlSerializer serializer = XmlSerializer.FromTypes(new[] { typeof(AppConfigModel) })[0];
            AppConfigModel setting = null;
            try
            {
                using (var reader = new System.IO.StreamReader(path))
                {
                    setting = serializer.Deserialize(reader) as AppConfigModel;
                }
            }
            catch (Exception ex)
            {

            }
            return setting;
        }
        #endregion

        public static void OpenKoobooApp()
        {
            var path = Path.Combine(RootPath, KoobooAppName);
            CmdHelper.StartDotnetApp(path);
        }


    }
}
