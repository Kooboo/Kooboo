//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using Kooboo.Lib;
using System.IO;
using System.Configuration;
using Kooboo.Lib.Helper;
using Kooboo.Data.Models;
using System.Collections.Generic;

namespace Kooboo.Data
{
    public static class AppSettings
    {
        static AppSettings()
        {
            Version = typeof(Kooboo.Data.Models.WebSite).Assembly.GetName().Version;

            RootPath = TryRootPath();
            IsTemplateServer = GetBool("IsTemplateServer");
            IsOnlineServer = GetBool("IsOnlineServer");
            Global = new GlobalInfo();
            Global.IsOnlineServer = GetBool("IsOnlineServer");
            Global.EnableLog = GetBool("Log");
            Global.LogPath = GetPhysicsPath(@"AppData\log");
            IOHelper.EnsureDirectoryExists(Global.LogPath);
        }

        public static string GetFileIORoot(WebSite website)
        {
            var orgid = website.OrganizationId;

            string orgfolder = GetOrganizationFolder(orgid);

            string websitefolder = System.IO.Path.Combine(orgfolder, website.Name);

            string fileiofolder = System.IO.Path.Combine(websitefolder, "__FileIO");

            Kooboo.Lib.Helper.IOHelper.EnsureDirectoryExists(fileiofolder);

            return fileiofolder;    
        }

        public static System.Version Version { get; set; }

        public static int MaxForEachLoop { get; set; } = 100;

        public static string GetMailDbName(Guid OrganizationId)
        {
            return GetDbName(OrganizationId, "__kooboomailstore");
        }

        public static string GetDbName(Guid OrganizationId, string websitename)
        {
            string orgfolder = GetOrganizationFolder(OrganizationId);
            return Path.Combine(orgfolder, websitename);
        }

        public static bool SetConfigValue(string key, string value)
        {
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                if (config.AppSettings.Settings[key] != null)
                    config.AppSettings.Settings[key].Value = value;
                else
                    config.AppSettings.Settings.Add(key, value);
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string GetOrganizationFolder(Guid OrganizationId)
        {
            return GetOrganizationFolder(OrganizationId, IsOnlineServer);
        }

        public static string GetOrganizationFolder(Guid OrganizationId, bool IsOnlineServer)
        {                  
            var organizationName = GlobalDb.Organization.GetName(OrganizationId);

            string foldername = null;
                                                  
            if (!string.IsNullOrWhiteSpace(organizationName))
            {
                foldername = Path.Combine(DatabasePath, organizationName);
                if (Directory.Exists(foldername))
                {
                    return foldername; 
                }
            }

            string folderidname = Path.Combine(DatabasePath, OrganizationId.ToString()); ;

            if (Directory.Exists(folderidname))
            {
                return folderidname;
            }

            if (!string.IsNullOrWhiteSpace(foldername))
            {
                if (!Directory.Exists(foldername))
                {
                    Directory.CreateDirectory(foldername);
                }
                return foldername;
            }

            else
            {
                if (!Directory.Exists(folderidname))
                {
                    Directory.CreateDirectory(folderidname);
                }
                return folderidname;
            }   
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

        private static string TryRootPath()
        {
            var basefolder = AppDomain.CurrentDomain.BaseDirectory;
            if (IsKoobooDiskRoot(basefolder))
            {
                return basefolder;
            }

            List<string> trypaths = new List<string>();
            trypaths.Add(@"..\..\..\Kooboo.Web");
            trypaths.Add(@"..\..\..\Github\Kooboo.Web"); 
            trypaths.Add(@"..\");
            trypaths.Add(@"..\..\");
            trypaths.Add(@"..\..\..\");

            foreach (var item in trypaths)
            {
                basefolder = System.IO.Path.GetFullPath(item);
                if (basefolder != null && IsKoobooDiskRoot(basefolder))
                {
                    return basefolder;
                }
            }

            return AppDomain.CurrentDomain.BaseDirectory;
        }

        public static string RootPath
        {
            get; set;
        }

        public static string CmsLang
        {
            get
            {
                return ConfigurationManager.AppSettings.Get("CmsLang");
            }
        }

        public static string AutoStart
        {
            get
            {
                return ConfigurationManager.AppSettings.Get("AutoStart");
            }
        }

        public static string AutoUpgrade
        {
            get
            {
                return ConfigurationManager.AppSettings.Get("AutoUpgrade");
            }
        }


        public static string DefaultLocalHost { get; set; } = "kooboo";

        public static int CurrentUsedPort { get; set; } = 80;

        private static string _starthost;
        public static string StartHost
        {
            get
            {
                if (string.IsNullOrEmpty(_starthost))
                {
                    _starthost = AppSettingsUtility.Get("ServerHost", "local.kooboo");
                }
                return _starthost;
            }
        }

        public static bool SupportSiteDeviceBinding
        {
            get { return false; }
        }

        private static string _hostdomain;

        public static string HostDomain
        {
            get
            {
                if (IsOnlineServer)
                {
                    return ServerSetting.HostDomain;
                }
                else
                {
                    if (_hostdomain == null)
                    {
                        _hostdomain = ConfigurationManager.AppSettings.Get("HostDomain");
                        if (_hostdomain == null)
                        {
                            _hostdomain = string.Empty;
                        }
                    }
                    return _hostdomain;
                }
            }
            set { _hostdomain = value; }
        }

        private static string _screenShotUrl;
        public static string ScreenShotUrl
        {
            get
            {
                if (_screenShotUrl == null)
                {
                    _screenShotUrl = ConfigurationManager.AppSettings["ScreenShotUrl"];
                    if (_screenShotUrl == null)
                    {
                        _screenShotUrl = string.Empty;
                    }
                }
                return _screenShotUrl;
            }
            set { _screenShotUrl = value; }
        }

        #region "resource url"

        public static void ResetLocation()
        {
            _resource = null;
            _themeurl = null;
            _accountApiUrl = null;
        }

        private static ApiResource _resource;

        public static ApiResource ApiResource
        {
            get
            {
                if (_resource == null)
                {
                    _resource = GetSetApiResponse();
                }
                else if (_resource.Expiration < DateTime.Now)
                {
                    var newres = GetSetApiResponse();
                    if (newres != null)
                    {
                        _resource = newres;
                    }
                    return _resource;
                }
                return _resource;
            }
        }

        private static ApiResource GetSetApiResponse()
        {
            var localvalue = Kooboo.Data.GlobalDb.GlobalSetting.Store.FullScan(o => o.Name == "ApiResource").FirstOrDefault();

            if (localvalue != null && localvalue.Expiration > DateTime.Now)
            {
                var res = new ApiResource();
                res.AccountUrl = localvalue.KeyValues["AccountUrl"];
                res.ThemeUrl = localvalue.KeyValues["ThemeUrl"];
                res.ConvertUrl = localvalue.KeyValues["ConvertUrl"];
                res.Expiration = localvalue.Expiration;
                return res;
            }
            else
            {
                List<string> apis = new List<string>();
                apis.Add("http://us.koobooapi.com");
                apis.Add("http://eu.koobooapi.com");
                apis.Add("http://hk.koobooapi.com");
                apis.Add("http://cn.koobooapi.com");

                ApiResource apires = null;

                foreach (var item in apis)
                {
                    string url = item + "/account/system/apiresource";
                    try
                    {
                        apires = HttpHelper.Get<ApiResource>(url);
                    }
                    catch (Exception)
                    {

                    }
                    if (apires != null && !string.IsNullOrWhiteSpace(apires.AccountUrl))
                    {
                        break;
                    }
                }

                if (apires != null)
                {
                    var localsetting = new GlobalSetting() { Name = "ApiResource", LastModified = DateTime.Now };
                    localsetting.KeyValues["AccountUrl"] = apires.AccountUrl;
                    localsetting.KeyValues["ThemeUrl"] = apires.ThemeUrl;
                    localsetting.KeyValues["ConvertUrl"] = apires.ConvertUrl;
                    localsetting.Expiration = apires.Expiration;
                    GlobalDb.GlobalSetting.AddOrUpdate(localsetting);
                    return apires;
                }
            }

            return null;
        }

        private static ServerSetting _serversetting;

        public static ServerSetting ServerSetting
        {
            get
            {
                if (_serversetting == null)
                {
                    if (IsOnlineServer)
                    {
                        // if there is a root url.. 
                        string rooturl = ConfigurationManager.AppSettings.Get("RootUrl");
                        string url = null;
                        if (string.IsNullOrEmpty(rooturl))
                        {
                            url = Helper.AccountUrlHelper.System("GetSetting");
                        }
                        else
                        {
                            url = Lib.Helper.UrlHelper.Combine(rooturl, "system/GetSetting");
                        }
                        _serversetting = HttpHelper.Get<ServerSetting>(url);
                    }
                    else
                    {
                        _serversetting = new ServerSetting();
                    }
                }
                return _serversetting;
            }
        }

        public static bool GetBool(string settingKey)
        {
            string value = ConfigurationManager.AppSettings.Get(settingKey);
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }
            else
            {
                bool boolValue;
                bool.TryParse(value, out boolValue);

                return boolValue;
            }
        }

        private static string _themeurl;
        public static string ThemeUrl
        {
            get
            {
                if (_themeurl == null)
                {
                    _themeurl = ConfigurationManager.AppSettings.Get("ThemeUrl");
                    if (string.IsNullOrEmpty(_themeurl) && ApiResource != null)
                    {
                        _themeurl = ApiResource.ThemeUrl;
                    }
                    if (_themeurl != null && !_themeurl.ToLower().StartsWith("http://"))
                    {
                        _themeurl = "http://" + _themeurl;
                    }
                }

                if (_themeurl == null)
                {
                    return "http://ustheme.thetheme.com";
                }
                return _themeurl;
            }
        }

        private static string _themedomain;
        public static string ThemeDomain
        {
            get
            {
                if (_themedomain == null)
                {
                    if (!IsOnlineServer)
                    {
                        _themedomain = "kooboo";
                    }
                    else
                    {
                        // template server must be an online server as well.  
                        _themedomain = ServerSetting.ServerId + "." + ServerSetting.HostDomain;
                    }

                    if (_themedomain == null)
                    {
                        _themedomain = "thetheme.com";
                    }
                }
                return _themedomain;
            }
            set
            {
                _themedomain = value;
            }
        }

        private static string _accountApiUrl;

        public static string AccountApiUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_accountApiUrl))
                {
                    _accountApiUrl = ConfigurationManager.AppSettings.Get("AccountApiUrl");

                    if (!string.IsNullOrEmpty(_accountApiUrl))
                    {
                        return _accountApiUrl;
                    }

                    if (ApiResource != null)
                    {
                        _accountApiUrl = ApiResource.AccountUrl;
                    }
                    else
                    {
                        return "http://us.koobooapi.com";
                    }
                }
                return _accountApiUrl;
            }
        }

        public static string ConvertApiUrl
        {
            get
            {
                return ThemeUrl;
            }
        }

        #endregion

        private static string _databasepath;
        public static string DatabasePath
        {
            get
            {
                if (_databasepath == null)
                {
                    _databasepath = GetPhysicsPath(@"AppData\KoobooData");
                    IOHelper.EnsureDirectoryExists(_databasepath);
                }
                return _databasepath;
            }
        }

        private static string _tempdatapath;
        public static string TempDataPath
        {
            get
            {
                if (_tempdatapath == null)
                {
                    _tempdatapath = GetPhysicsPath(@"AppData\TempData");
                    IOHelper.EnsureDirectoryExists(_tempdatapath);
                }
                return _tempdatapath;
            }
        }

        public static string ThemeFolder
        {
            get
            {
                var path = GetPhysicsPath(@"AppData\theme");
                IOHelper.EnsureDirectoryExists(path);
                return path;
            }
        }
        public static string AppFolder
        {
            get
            {
                var path = GetPhysicsPath(@"AppData\app");
                IOHelper.EnsureDirectoryExists(path);
                return path;
            }
        }

        public static string GetPhysicsPath(string relativePath)
        {
            var path = relativePath.Replace("/", "\\").Replace("\\\\", "\\").TrimStart('\\', '/');
            return Path.Combine(System.IO.Path.GetFullPath(AppSettings.RootPath), path);
        }

        public static int MaxTemplateSize
        {
            get
            {
                return 1024 * 1024 * 20;
            }
        }

        public static bool IsTemplateServer { get; set; }

        public static bool IsOnlineServer { get; set; }

        public static GlobalInfo Global { get; set; }

        public class GlobalInfo
        {
            public bool IsOnlineServer { get; set; }    

            public bool EnableLog { get; set; }

            public string LogPath { get; set; }
        }
    }
}
