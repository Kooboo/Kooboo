//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Data.Models;
using Kooboo.Lib;
using Kooboo.Lib.Helper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace Kooboo.Data
{
    public static class AppSettings
    {
        static AppSettings()
        {
            LoadSetting();
        }

        public static void LoadSetting()
        {
            Version = typeof(Kooboo.Data.Models.WebSite).Assembly.GetName().Version;

            RootPath = TryRootPath();
            IsOnlineServer = GetBool("IsOnlineServer");

            string quotavalue = ConfigurationManager.AppSettings.Get("QuotaControl");
            QuotaControl = string.IsNullOrEmpty(quotavalue) || GetBool("QuotaControl");

            Global = new GlobalInfo
            {
                IsOnlineServer = GetBool("IsOnlineServer"),
                EnableLog = GetBool("Log"),
                LogPath = System.IO.Path.Combine(RootPath, "Log")
            };


            IOHelper.EnsureDirectoryExists(Global.LogPath);

            MaxVisitorLogRead = IsOnlineServer ? 3000 : 10000;

            // users
            SetUser();

            _accountApiUrl = ConfigurationManager.AppSettings.Get("AccountApiUrl");
            _themeurl = ConfigurationManager.AppSettings.Get("ThemeUrl");

            // for some servers that does not have hostfile.
            if (!System.IO.File.Exists(Kooboo.Data.Hosts.WindowsHost.HostFile))
            {
                DefaultLocalHost = "localkooboo.com";
            }

            _serversetting = null; // reset server setting.
        }

        private static void SetUser()
        {
            string defaultuser = ConfigurationManager.AppSettings.Get("DefaultUser");
            string alloweUsers = ConfigurationManager.AppSettings.Get("AllowUsers");

            if (!string.IsNullOrWhiteSpace(defaultuser))
            {
                int index = defaultuser.IndexOf(",");
                if (index > -1)
                {
                    var username = defaultuser.Substring(0, index);
                    var password = defaultuser.Substring(index + 1);
                    DefaultUser = new BasicUser() { UserName = username.Trim().ToLower(), Password = password.Trim() };
                }
            }

            if (!string.IsNullOrWhiteSpace(alloweUsers))
            {
                if (alloweUsers != "*")
                {
                    string[] users = alloweUsers.Split(',');
                    List<string> userlist = (from item in users where !string.IsNullOrWhiteSpace(item) select item.ToLower().Trim()).ToList();

                    if (userlist.Any())
                    {
                        AllowUsers = new List<Guid>();
                        foreach (var item in userlist)
                        {
                            AllowUsers.Add(Lib.Security.Hash.ComputeGuidIgnoreCase(item));
                        }
                    }
                }
            }
        }

        public static bool QuotaControl { get; set; }

        public static BasicUser DefaultUser { get; set; }

        public static List<Guid> AllowUsers { get; set; }

        public static int MaxVisitorLogRead { get; set; } = 3000;  // only read the last  3000

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

        public static bool CustomSslCheck { get; set; }

        public static void SetCustomSslCheck()
        {
            if (CustomSslCheck) return;
            Kooboo.Lib.Helper.HttpHelper.SetCustomSslChecker();
            CustomSslCheck = true;
        }

        public static string GetMailDbName(Guid organizationId)
        {
            return GetDbName(organizationId, "__kooboomailstore");
        }

        public static string GetDbName(Guid organizationId, string websitename)
        {
            string orgfolder = GetOrganizationFolder(organizationId);
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

        public static string GetOrganizationFolder(Guid organizationId)
        {
            return GetOrganizationFolder(organizationId, IsOnlineServer);
        }

        public static string GetOrganizationFolder(Guid organizationId, bool isOnlineServer)
        {
            var organizationName = GlobalDb.Organization.GetName(organizationId);

            string foldername = null;

            if (!string.IsNullOrWhiteSpace(organizationName))
            {
                foldername = Path.Combine(DatabasePath, organizationName);
                if (Directory.Exists(foldername))
                {
                    return foldername;
                }
            }

            string folderidname = Path.Combine(DatabasePath, organizationId.ToString()); ;

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

            if (!Directory.Exists(folderidname))
            {
                Directory.CreateDirectory(folderidname);
            }
            return folderidname;
        }

        private static bool IsKoobooDiskRoot(string fullPath)
        {
            string scriptFolder = System.IO.Path.Combine(fullPath, "_Admin", "Scripts");
            if (!Directory.Exists(scriptFolder))
            {
                return false;
            }
            string viewFolder = System.IO.Path.Combine(fullPath, "_Admin", "View");
            if (!Directory.Exists(viewFolder))
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

            List<string> trypaths = Kooboo.Lib.Compatible.CompatibleManager.Instance.System.GetTryPaths();

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

        public static string CmsLang => ConfigurationManager.AppSettings.Get("CmsLang");

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
            set => _starthost = value;
        }

        public static bool SupportSiteDeviceBinding => false;

        private static string _hostdomain;

        public static string HostDomain
        {
            get
            {
                if (IsOnlineServer)
                {
                    return ServerSetting.HostDomain;
                }

                return _hostdomain ??
                       (_hostdomain = ConfigurationManager.AppSettings.Get("HostDomain") ?? string.Empty);
            }
            set => _hostdomain = value;
        }

        private static string _screenShotUrl;

        public static string ScreenShotUrl
        {
            get =>
                _screenShotUrl ??
                (_screenShotUrl = ConfigurationManager.AppSettings["ScreenShotUrl"] ?? string.Empty);
            set => _screenShotUrl = value;
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

            if (localvalue != null && localvalue.Expiration > DateTime.Now && localvalue.HasKey("AccountUrl"))
            {
                var res = new ApiResource
                {
                    AccountUrl = localvalue.KeyValues["AccountUrl"],
                    ThemeUrl = localvalue.KeyValues["ThemeUrl"],
                    ConvertUrl = localvalue.KeyValues["ConvertUrl"],
                    Expiration = localvalue.Expiration
                };
                return res;
            }

            List<string> apis = new List<string>();

            // add the local value.
            if (localvalue != null && localvalue.HasKey("AccountUrl"))
            {
                var accounturl = localvalue.GetValue("AccountUrl");
                if (!string.IsNullOrWhiteSpace(accounturl))
                {
                    if (accounturl.ToLower().StartsWith("https://"))
                    {
                        accounturl = accounturl.Replace("https://", "http://");
                    }
                    apis.Add(accounturl);
                }
            }

            apis.Add("http://159.138.24.241");
            apis.Add("http://51.15.11.145");
            apis.Add("http://us.koobooapi.com");
            apis.Add("http://eu.koobooapi.com");

            ApiResource apires = null;

            string apiurl = "/account/system/apiresource";
            if (IsOnlineServer)
            {
                apiurl += apiurl += "?online=true";
            }

            foreach (var url in apis.Select(item => item + apiurl))
            {
                try
                {
                    apires = HttpHelper.Get<ApiResource>(url);
                }
                catch (Exception)
                {
                    // ignored
                }

                if (apires != null && !string.IsNullOrWhiteSpace(apires.AccountUrl))
                {
                    break;
                }
            }

            if (apires != null)
            {
                if (!CustomSslCheck)
                {
                    apires.AccountUrl = apires.AcccountDomain;
                }

                //Kooboo.Data.Helper.ApiHelper.EnsureAccountUrl(apires);

                var localsetting = new GlobalSetting() { Name = "ApiResource", LastModified = DateTime.Now };
                localsetting.KeyValues["AccountUrl"] = apires.AccountUrl;
                localsetting.KeyValues["ThemeUrl"] = apires.ThemeUrl;
                localsetting.KeyValues["ConvertUrl"] = apires.ConvertUrl;
                localsetting.Expiration = apires.Expiration;
                GlobalDb.GlobalSetting.AddOrUpdate(localsetting);
                return apires;
            }

            if (localvalue != null)
            {
                var res = new ApiResource
                {
                    AccountUrl = localvalue.KeyValues["AccountUrl"],
                    ThemeUrl = localvalue.KeyValues["ThemeUrl"],
                    ConvertUrl = localvalue.KeyValues["ConvertUrl"],
                    Expiration = DateTime.Now.AddDays(1)
                };
                return res;
            }

            return new ApiResource() { AccountUrl = "http://159.138.24.241", Expiration = DateTime.Now.AddMinutes(10) };
        }

        public static string RootUrl { get; set; }

        private static ServerSetting _serversetting;

        public static ServerSetting ServerSetting
        {
            get
            {
                if (_serversetting == null)
                {
                    if (IsOnlineServer)
                    {
                        string currentRooturl = RootUrl;

                        if (string.IsNullOrWhiteSpace(currentRooturl))
                        {
                            currentRooturl = ConfigurationManager.AppSettings.Get("RootUrl");
                        }

                        string url = null;

                        if (string.IsNullOrEmpty(currentRooturl))
                        {
                            url = Helper.AccountUrlHelper.System("GetSetting");
                        }
                        else
                        {
                            currentRooturl = currentRooturl + "/_api/";
                            url = Lib.Helper.UrlHelper.Combine(currentRooturl, "system/GetSetting");
                        }
                        try
                        {
                            _serversetting = HttpHelper.Get<ServerSetting>(url);
                        }
                        catch (Exception)
                        {
                            // ignored
                        }

                        if (_serversetting == null)
                        {
                            Console.WriteLine("Can not find server info from root server");
                            _serversetting = new ServerSetting();
                        }
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

            bool.TryParse(value, out var boolValue);

            return boolValue;
        }

        private static string _themeurl;

        public static string ThemeUrl
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_themeurl))
                {
                    return _themeurl;
                }

                if (ApiResource != null)
                {
                    return ApiResource.ThemeUrl;
                }

                return "http://5.thetheme.com";
            }
        }

        private static string _themedomain;

        public static string ThemeDomain =>
            _themedomain ?? (_themedomain =
                (!IsOnlineServer ? "kooboo" : ServerSetting.ServerId + "." + ServerSetting.HostDomain) ??
                "thetheme.com");

        private static string _accountApiUrl;

        public static string AccountApiUrl
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_accountApiUrl))
                {
                    return _accountApiUrl;
                }

                if (ApiResource != null)
                {
                    return ApiResource.AccountUrl;
                }

                return "http://us.koobooapi.com";
            }
        }

        public static string ConvertApiUrl
        {
            get
            {
                if (ApiResource != null && !string.IsNullOrWhiteSpace(ApiResource.ConvertUrl))
                {
                    return ApiResource.ConvertUrl;
                }

                return ThemeUrl;
            }
        }

        #endregion "resource url"

        private static string _databasepath;

        public static string DatabasePath
        {
            get
            {
                if (_databasepath == null)
                {
                    _databasepath = Path.Combine(AppSettings.RootPath, "AppData", "KoobooData");

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
                    _tempdatapath = System.IO.Path.Combine(Data.AppSettings.RootPath, "AppData", "TempData");
                    IOHelper.EnsureDirectoryExists(_tempdatapath);
                }
                return _tempdatapath;
            }
        }

        public static string ThemeFolder
        {
            get
            {
                var path = System.IO.Path.Combine(Data.AppSettings.RootPath, "AppData", "theme");
                IOHelper.EnsureDirectoryExists(path);
                return path;
            }
        }

        public static string AppFolder
        {
            get
            {
                var path = System.IO.Path.Combine(AppSettings.RootPath, "AppData", "app");
                IOHelper.EnsureDirectoryExists(path);
                return path;
            }
        }

        public static string GetPhysicsPath(string relativePath)
        {
            var path = relativePath.Replace("/", "\\").Replace("\\\\", "\\").TrimStart('\\', '/');
            return Path.Combine(AppSettings.RootPath, path);
        }

        public static int MaxTemplateSize => 1024 * 1024 * 20;

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