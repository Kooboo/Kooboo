//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System;

namespace Kooboo.Data.Authorization
{
    public static class LogStorage
    {
        static LogStorage()
        {
            CurrentDate = DateTime.Now;
            // email setting.
            Kooboo.IndexedDB.Dynamic.Setting setting = new IndexedDB.Dynamic.Setting();
            setting.SetPrimaryKey("OrgId", typeof(Guid));

            IndexedDB.Dynamic.TableColumn col = new IndexedDB.Dynamic.TableColumn
            {
                Name = "Count", DataType = typeof(int).FullName
            };
            setting.AddColumn(col);

            Emailsetting = setting;
        }

        private static object _locker = new object();

        private static object _dblocker = new object();

        private static DateTime CurrentDate { get; set; }

        private static Kooboo.IndexedDB.Dynamic.Setting Emailsetting { get; set; }

        private static string Folder()
        {
            return CurrentDate.Year.ToString() + CurrentDate.Month.ToString() + CurrentDate.Day.ToString();
        }

        private static Kooboo.IndexedDB.Database _db;

        private static Kooboo.IndexedDB.Database GetDb(DateTime date)
        {
            var folder = Folder();
            folder = "global" + "\\logging\\" + folder;
            return new Kooboo.IndexedDB.Database(folder);
        }

        private static Kooboo.IndexedDB.Database Db
        {
            get
            {
                var now = DateTime.Now;
                if (now.DayOfYear != CurrentDate.DayOfYear)
                {
                    lock (_locker)
                    {
                        if (now.DayOfYear != CurrentDate.DayOfYear)
                        {
                            if (_db != null)
                            {
                                _db.Close();
                                _db = null;
                            }

                            CurrentDate = now;
                            _db = GetDb(CurrentDate);
                        }
                    }
                }

                return _db ?? (_db = GetDb(now));
            }
            set => _db = value;
        }

        public static Kooboo.IndexedDB.Dynamic.Table EmailLog => Db.GetOrCreateTable("EmailLog", Emailsetting);
    }
}