using System;
using System.Collections.Generic;
using System.Linq;
using Kooboo.Mail.Models;

namespace Kooboo.Mail.Repositories.Sqlite
{
    public class CalendarRepo
    {
        public string TableName
        {
            get
            {
                return "Calendar";
            }
        }

        private MailDb maildb { get; set; }

        public CalendarRepo(MailDb db)
        {
            this.maildb = db;
        }

        public void Add(CalendarInfo calendarInfo)
        {
            if (string.IsNullOrEmpty(calendarInfo.Id))
            {
                calendarInfo.Id = Guid.NewGuid().ToString();
            }
            this.maildb.SqliteHelper.Add<CalendarInfo>(calendarInfo, this.TableName);
        }

        public void AddOrUpdate(CalendarInfo calendarInfo)
        {
            if (string.IsNullOrEmpty(calendarInfo.Id))
            {
                calendarInfo.Id = Guid.NewGuid().ToString();
            }
            this.maildb.SqliteHelper.AddOrUpdate<CalendarInfo>(calendarInfo, this.TableName, "Id");
        }

        public void Update(CalendarInfo calendarInfo, string calendarBody)
        {
            if (string.IsNullOrEmpty(calendarInfo.Id))
            {
                return;
            }
            var info = this.maildb.SqliteHelper.Get<CalendarInfo>(calendarInfo.Id);
            if (info != null)
            {
                this.maildb.SqliteHelper.Update<CalendarInfo>(info, this.TableName, calendarBody);
            }
        }

        public void Delete(CalendarInfo calendarInfo)
        {
            if (string.IsNullOrEmpty(calendarInfo.Id))
            {
                return;
            }
            this.maildb.SqliteHelper.Delete(this.TableName, nameof(CalendarInfo.Id), calendarInfo.Id);
        }

        public CalendarInfo GetScheduleById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new CalendarInfo();
            }
            var info = this.maildb.SqliteHelper.Get<CalendarInfo>(this.TableName, nameof(CalendarInfo.Id), id);
            return info;
        }

        public List<CalendarInfo> GetAllSchedules()
        {
            var infos = this.maildb.SqliteHelper.All<CalendarInfo>(this.TableName).OrderBy(v => v.Start).ToList();
            return infos;
        }

        public List<CalendarInfo> GetSchedulesByTime(string start, string end, string userId)
        {
            DateTime Start = DateTime.ParseExact(start, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            if (string.IsNullOrEmpty(end))
            {
                DateTime End = Start.AddMonths(1);
                end = End.ToString("yyyy-MM-dd HH:mm:ss");
            }
            var sql = $"SELECT * FROM Calendar WHERE date(Calendar.Start) >= date('{Start.ToString("yyyy-MM-dd HH:mm:ss")}') And date(Calendar.Start) <= date('{end}') Order by End desc";
            var infos = this.maildb.SqliteHelper.Query<CalendarInfo>(sql);
            return infos.OrderBy(v => v.Start).ToList();
        }
    }
}
