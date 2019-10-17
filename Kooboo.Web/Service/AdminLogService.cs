//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Api.ApiResponse;
using Kooboo.Data.Models;
using Kooboo.IndexedDB;
using Kooboo.IndexedDB.Dynamic;
using Kooboo.Lib.Helper;
using Kooboo.Render;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Web.Service
{
    public class AdminLogService
    {
        static AdminLogService()
        {
            GlobalDatabase = Kooboo.Data.DB.Global();
            LogFolder = System.IO.Path.Combine(Data.AppSettings.RootPath, "AppData", "AdminLog"); 
            IOHelper.EnsureDirectoryExists(LogFolder);

            var setting = new Setting();
            setting.AppendColumn("Path", typeof(string), 800);

            LastPath = Kooboo.Data.DB.GetOrCreateTable(GlobalDatabase, "userpath", setting);
        }

        public static Database GlobalDatabase { get; set; }

        private static string _logfolder;
        public static string LogFolder
        {
            get
            {
                return _logfolder;
            }
            set
            {
                _logfolder = value;
                _logstore = null;
            }
        }

        public static Table LastPath { get; set; }

        public static Sequence<AdminLog> LogByWeek<AdminLog>(DateTime weektime)
        {
            string weekname = _GetWeekName(weektime);

            var filename = weekname + ".log";
            string FullFileName = System.IO.Path.Combine(LogFolder, filename);

            return GlobalDatabase.GetSequence<AdminLog>(FullFileName) as Sequence<AdminLog>;
        }

        private static int GetWeekOfYear(DateTime datetime)
        {
            return System.Globalization.CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(datetime, System.Globalization.CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
        }

        internal static string _GetWeekName(DateTime datetime)
        {
            var weekname = GetWeekOfYear(datetime);
            string strweekname = weekname > 9 ? weekname.ToString() : "0" + weekname.ToString();
            return datetime.Year.ToString() + "-" + strweekname;
        }

        private static object _lock = new object();

        private static Sequence<BackendLog> _logstore;
        private static DateTime _logtime;
        public static Sequence<BackendLog> LogStore
        {
            get
            {
                if (_logstore == null)
                {
                    lock (_lock)
                    {
                        if (_logstore == null)
                        {
                            _logtime = DateTime.Now;
                            _logstore = LogByWeek<BackendLog>(_logtime);
                        }
                    }
                }

                if (GetWeekOfYear(_logtime) != GetWeekOfYear(DateTime.Now))
                {
                    _logstore = null;
                    return LogStore;
                }
                return _logstore;
            }
        }

        public static void Add(BackendLog log)
        {
            LogStore.Add(log); 
            LogStore.Flush();
        }

        public static void AddAdminLog(Kooboo.Data.Context.RenderContext context, RenderRespnose response)
        {
            if (response != null && response.ContentType != null && response.ContentType.ToLower().Contains("html"))
            {
                BackendLog model = new BackendLog();
                model.IP = context.Request.IP;
                if (context.User != null)
                {
                    model.UserId = context.User.Id;
                    model.UserName = context.User.UserName;
                }
                model.Url = context.Request.RawRelativeUrl;
                model.StatusCode = 200;

                if (context.Request.Cookies != null)
                {
                    foreach (var item in context.Request.Cookies)
                    {
                        model.Data[item.Key] = item.Value;
                    }
                }

                if (context.Request.Forms != null)
                {
                    foreach (var item in context.Request.Forms.AllKeys)
                    {
                        string key = item;
                        string value = null;

                        var itemvalue = context.Request.Forms.GetValues(item);
                        if (itemvalue != null)
                        {
                            value = string.Join(";", itemvalue);
                        }

                        model.Data[key] = value;
                    }
                }

                if (context.Request.QueryString != null)
                {
                    foreach (var item in context.Request.QueryString.AllKeys)
                    {
                        string key = item;
                        string value = null;

                        var itemvalue = context.Request.QueryString.GetValues(item);
                        if (itemvalue != null)
                        {
                            value = string.Join(";", itemvalue);
                        }

                        model.Data[key] = value;
                    }
                }

                Add(model);
                Kooboo.Data.Service.UserLoginService.UpdateLastPath(model);
            }
        }

        public static void AddApiLog(Data.Context.RenderContext context, IResponse response)
        {
            if (response != null)
            {
                BackendLog model = new BackendLog();
                model.IsApi = true;
                model.IP = context.Request.IP;
                if (context.User != null)
                {
                    model.UserId = context.User.Id;
                    model.UserName = context.User.UserName;
                }
                model.Url = context.Request.RawRelativeUrl;
                model.StatusCode = 200;

                if (context.Request.Cookies != null)
                {
                    foreach (var item in context.Request.Cookies)
                    {
                        model.Data[item.Key] = item.Value;
                    }
                }

                if (context.Request.Forms != null)
                {
                    foreach (var item in context.Request.Forms.AllKeys)
                    {
                        string key = item;
                        string value = null;

                        var itemvalue = context.Request.Forms.GetValues(item);
                        if (itemvalue != null)
                        {
                            value = string.Join(";", itemvalue);
                        }

                        model.Data[key] = value;
                    }
                }

                if (context.Request.QueryString != null)
                {
                    foreach (var item in context.Request.QueryString.AllKeys)
                    {
                        string key = item;
                        string value = null;

                        var itemvalue = context.Request.QueryString.GetValues(item);
                        if (itemvalue != null)
                        {
                            value = string.Join(";", itemvalue);
                        }

                        model.Data[key] = value;
                    }
                }

                Add(model);
            }
        }

        public static List<BackendLog> ListByUser(Guid UserId, DateTime weektime)
        {
            var logbyweek = LogByWeek<BackendLog>(weektime);
            var all = logbyweek.AllItemList();
            return all.Where(o => o.UserId == UserId).ToList();
        }

        public static List<BackendLog> ListByUser(string UserName, DateTime weektime)
        {
            Guid userId = default(Guid);
            if (!string.IsNullOrEmpty(UserName))
            {
                userId = Lib.Security.Hash.ComputeGuidIgnoreCase(UserName);
            }
            return ListByUser(userId, weektime);
        }

        public static Dictionary<string, int> UrlSum(DateTime weektime)
        {
            var logbyweek = LogByWeek<BackendLog>(weektime);
            var all = logbyweek.AllItemList();

            Dictionary<string, int> tempresult = new Dictionary<string, int>();

            foreach (var item in all.GroupBy(o => o.Url))
            {
                tempresult[item.Key] = item.Count();
            }

            Dictionary<string, int> result = new Dictionary<string, int>();

            foreach (var item in tempresult.OrderByDescending(o => o.Value))
            {
                result[item.Key] = item.Value;
            }
            return result;
        }

        public static Dictionary<string, int> UrlSum(Guid UserId, DateTime weektime)
        {
            var logbyweek = LogByWeek<BackendLog>(weektime);
            var all = logbyweek.AllItemList().Where(o => o.UserId == UserId).ToList();

            Dictionary<string, int> tempresult = new Dictionary<string, int>();

            foreach (var item in all.GroupBy(o => o.Url))
            {
                tempresult[item.Key] = item.Count();
            }

            Dictionary<string, int> result = new Dictionary<string, int>();

            foreach (var item in tempresult.OrderByDescending(o => o.Value))
            {
                result[item.Key] = item.Value;
            }

            return result;
        }

        public static List<string> ListUserNames(DateTime weektime)
        {
            var logbyweek = LogByWeek<BackendLog>(weektime);
            var all = logbyweek.AllItemList();
            List<string> result = new List<string>();
            foreach (var item in all.GroupBy(o => o.UserName))
            {
                result.Add(item.Key);
            }
            return result;
        }
         

        public static string GetLastPath(Guid UserId)
        {

            var find = LastPath.Get(UserId);
            if (find != null && find.ContainsKey("Path"))
            {
                var obj = find["Path"];
                if (obj != null)
                {
                    return obj.ToString();
                }
            }
            return null;
        }
          
    }
}
