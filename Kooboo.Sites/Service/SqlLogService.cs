using Kooboo.Data.Context;
using Kooboo.Data.Logging;
using Kooboo.Data.Models;
using Kooboo.IndexedDB;
using Kooboo.IndexedDB.Dynamic;
using Kooboo.Lib.Helper;
using Kooboo.Lib.NETMultiplePart;
using Kooboo.Mail.Imap.Commands;
using KScript;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kooboo.Sites.Service
{
    public static class SqlLogService
    {

        static readonly ConcurrentQueue<SqlLog> _logs = new ConcurrentQueue<SqlLog>();
        static string _fullFilePath = null;
        static Sequence<SqlLog> _sequence = null;
        public static string LogFolder { get; private set; }
        public static Database GlobalDatabase { get; set; }

        static SqlLogService()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        if (_logs.TryDequeue(out var log))
                        {
                            var fullFilePath = GetFullFilePath(log.DateTime);

                            if (fullFilePath != _fullFilePath)
                            {
                                _fullFilePath = fullFilePath;
                                _sequence = GlobalDatabase.GetSequence<SqlLog>(_fullFilePath);
                            }

                            _sequence.Add(log);
                            _sequence.Flush();
                        }
                    }
                    catch (Exception ex)
                    {
                        Data.Log.Instance.Exception.WriteException(ex);
                    }

                    Thread.Sleep(50);
                }
            });


            GlobalDatabase = Kooboo.Data.DB.Global();
            LogFolder = System.IO.Path.Combine(Data.AppSettings.RootPath, "AppData", "SqlLog");
            IOHelper.EnsureDirectoryExists(LogFolder);
        }

        public static void AddLog(string sql, object @params, string type, Guid siteId)
        {
            try
            {
                _logs.Enqueue(new SqlLog
                {
                    Type = type,
                    Sql = sql,
                    Params = JsonHelper.Serialize(@params),
                    SiteId = siteId,
                    DateTime = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                Data.Log.Instance.Exception.WriteException(ex);
            }
        }


        public static List<SqlLog> QueryByWeek(string week, string keyword, string type, Guid siteId, int pageIndex, int pageSize, out int total)
        {
            var fullFilePath = GetFullFilePath(week);
            var sequence = GlobalDatabase.GetSequence<SqlLog>(fullFilePath);
            var emptyKeyword = string.IsNullOrWhiteSpace(keyword);
            var emptyType = string.IsNullOrWhiteSpace(type);
            var query = sequence.QueryDescending(o => o.SiteId == siteId && (emptyType || o.Type == type) && (emptyKeyword || o.Sql.Contains(keyword)));
            total = query.Count();
            var list = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);
            return list;
        }

        static string GetFullFilePath(DateTime datetime)
        {
            var weekname = _GetWeekName(datetime);
            var filename = weekname + ".log";
            return System.IO.Path.Combine(LogFolder, filename);
        }

        static string GetFullFilePath(string week)
        {
            var filename = week + ".log";
            return System.IO.Path.Combine(LogFolder, filename);
        }

        public static IEnumerable<string> GetWeeks()
        {
            var weeks = Directory.GetFiles(LogFolder).Select(s => Path.GetFileNameWithoutExtension(s)).ToList();
            if (!weeks.Any()) weeks.Add(_GetWeekName(DateTime.Now));
            return weeks;
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
    }
}
