//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.IndexedDB.Schedule
{
    public static class FileNameGenerator
    {

        public static string GetTimeFileName(int dayint)
        {
            return dayint.ToString() + ".time";
        }

        public static string GetTimeFullFileName(int dayint, ISchedule schedule)
        {
            if (!System.IO.Directory.Exists(schedule.Folder))
            {
                System.IO.Directory.CreateDirectory(schedule.Folder);
            }
            return System.IO.Path.Combine(schedule.Folder, GetTimeFileName(dayint));
        }


        public static string GetContentFileName(int dayint)
        {
            return dayint.ToString() + ".block";
        }

        public static string GetContentFullFileName(int dayint, ISchedule schedule)
        {
            if (!System.IO.Directory.Exists(schedule.Folder))
            {
                System.IO.Directory.CreateDirectory(schedule.Folder);
            }
            return System.IO.Path.Combine(schedule.Folder, GetContentFileName(dayint));
        }


        public static string GetItemFileName(int dayint)
        {
            return dayint.ToString() + ".items";
        }

        public static string GetItemFullFileName(int dayint, ISchedule schedule)
        {
            if (!System.IO.Directory.Exists(schedule.Folder))
            {
                System.IO.Directory.CreateDirectory(schedule.Folder);
            }
            return System.IO.Path.Combine(schedule.Folder, GetItemFileName(dayint));
        }


        public static string GetRepeatRecordFileName(string Folder)
        {
            string repeatfolder = System.IO.Path.Combine(GlobalSettings.RootPath, GlobalSettings.scheduleRepeatingPath);

            repeatfolder = System.IO.Path.Combine(repeatfolder, Folder);

            if (!System.IO.Directory.Exists(repeatfolder))
            {
                System.IO.Directory.CreateDirectory(repeatfolder);
            }


            return System.IO.Path.Combine(repeatfolder, "record.repeat");
        }

        public static string GetRepeatContentFileName(string Folder)
        {
            string repeatfolder = System.IO.Path.Combine(GlobalSettings.RootPath, GlobalSettings.scheduleRepeatingPath);

            repeatfolder = System.IO.Path.Combine(repeatfolder, Folder);

            if (!System.IO.Directory.Exists(repeatfolder))
            {
                System.IO.Directory.CreateDirectory(repeatfolder);
            }

            return System.IO.Path.Combine(repeatfolder, "content.repeat");
        }


    }
}
