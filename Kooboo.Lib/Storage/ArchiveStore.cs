using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Kooboo.Lib.Helper;

namespace Kooboo.Lib.Storage
{
    public class ArchiveStore<T>
    {

        public ArchiveStore(String baseDir, string folderName)
        {
            this.BaseFolder = baseDir;
            this.FolderName = folderName;
        }

        public ArchiveStore(string folderName)
        {
            this.FolderName = folderName;
        }

        public string FileExtension { get; set; } = ".json";

        public int ExpiresDays { get; set; } = 10;

        public StoreFolderType FolderType { get; set; } = StoreFolderType.Weekly;

        private string FolderName { get; set; }

        private string _BaseFolder;
        private string BaseFolder
        {
            get
            {
                if (string.IsNullOrEmpty(_BaseFolder))
                {
                    _BaseFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppData");
                    if (!Directory.Exists(_BaseFolder)) _BaseFolder = IOHelper.KoobooAppData;
                    _BaseFolder = System.IO.Path.Combine(_BaseFolder, "Archive");

                    Helper.IOHelper.EnsureDirectoryExists(_BaseFolder);
                }
                return _BaseFolder;
            }
            set
            {
                _BaseFolder = value;
                Helper.IOHelper.EnsureDirectoryExists(_BaseFolder);
            }
        }

        private string _fullFolder;
        public string FullFolder
        {
            get
            {
                if (_fullFolder == null)
                {
                    if (System.IO.Path.IsPathRooted(this.FolderName))
                    {
                        _fullFolder = this.FolderName;
                    }
                    else
                    {

                        _fullFolder = System.IO.Path.Combine(this.BaseFolder, this.FolderName);
                        Kooboo.Lib.Helper.IOHelper.EnsureDirectoryExists(_fullFolder);
                    }
                }
                return _fullFolder;
            }


        }

        public void Add(T TValue)
        {
            string key = System.Guid.NewGuid().ToString();

            Add(key, TValue);
        }

        public void Add(string key, T TValue)
        {
            var source = System.Text.Json.JsonSerializer.Serialize(TValue);

            if (string.IsNullOrWhiteSpace(key))
            {
                key = System.Guid.NewGuid().ToString();
            }

            var FullFileName = this.GetFullFileName(key);

            if (!System.IO.File.Exists(FullFileName))
            {
                System.IO.File.WriteAllText(FullFileName, source);
            }
        }

        private string GetFullFileName(string key)
        {
            string FileName = key.Contains(".") ? key : key + this.FileExtension;

            var TimeFolder = this.GetTimeFolder();

            var FullPath = System.IO.Path.Combine(this.FullFolder, TimeFolder);
            Helper.IOHelper.EnsureDirectoryExists(FullPath);
            return System.IO.Path.Combine(FullPath, FileName);
        }

        private string GetTimeFolder()
        {
            DateTime CurrentTime = DateTime.Now;

            if (this.FolderType == StoreFolderType.Daily)
            {
                return CurrentTime.ToString("yyyy-MM-dd");
            }
            else if (this.FolderType == StoreFolderType.Weekly)
            {
                return Kooboo.Lib.Helper.DateTimeHelper.GetWeekName(CurrentTime);
            }
            else if (this.FolderType == StoreFolderType.Monthly)
            {
                return CurrentTime.ToString("yyyy-MM");
            }
            return null;
        }

        internal bool IsFolderOlderThan(string Folder, StoreFolderType type, DateTime CompareTime)
        {
            if (type == StoreFolderType.Daily)
            {
                if (DateTime.TryParse(Folder, out var FolderTime))
                {
                    if (FolderTime.Year < CompareTime.Year)
                    {
                        return true;
                    }
                    else if (FolderTime.Year > CompareTime.Year)
                    {
                        return false;
                    }
                    else if (FolderTime.Month < CompareTime.Month)
                    {
                        return true;
                    }
                    else if (FolderTime.Month > CompareTime.Month)
                    {
                        return false;
                    }
                    else if (FolderTime.Day < CompareTime.Day)
                    {
                        return true;
                    }
                }
                return false;
            }
            else if (type == StoreFolderType.Weekly)
            {
                var (FolderYear, FolderWeek) = Kooboo.Lib.Helper.DateTimeHelper.ParseYearWeek(Folder);

                var CompareYear = CompareTime.Year;
                var CompareWeek = Helper.DateTimeHelper.GetWeekOfYear(CompareTime);

                if (FolderYear < CompareYear)
                {
                    return true;
                }
                else if (FolderYear > CompareYear)
                {
                    return false;
                }
                else if (FolderWeek < CompareWeek)
                {
                    return true;
                }
                return false;
            }
            else if (type == StoreFolderType.Monthly)
            {
                var (FolderYear, FolderMonth) = Kooboo.Lib.Helper.DateTimeHelper.ParseYearMonth(Folder);

                var CompareYear = CompareTime.Year;
                var CompareMonth = CompareTime.Month;

                if (FolderYear < CompareYear)
                {
                    return true;
                }
                else if (FolderYear > CompareYear)
                {
                    return false;
                }
                else if (FolderMonth < CompareMonth)
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        /// <summary>
        /// clean outdated folders. 
        /// </summary>
        public void CleanOutDate()
        {
            var compareTime = DateTime.Now.AddDays(-this.ExpiresDays);

            var folder = System.IO.Directory.GetDirectories(this.FullFolder).ToList();
            foreach (var item in folder)
            {
                var Info = new System.IO.DirectoryInfo(item);
                var FolderName = Info.Name;

                if (IsFolderOlderThan(FolderName, this.FolderType, compareTime))
                {
                    try
                    {
                        System.IO.Directory.Delete(item, true);
                    }
                    catch (Exception)
                    {

                    }
                }
            }
        }

        public List<string> Folders()
        {
            List<string> Result = new List<string>();
            var folders = System.IO.Directory.GetDirectories(this.FullFolder).ToList();
            foreach (var item in folders)
            {
                var Info = new System.IO.DirectoryInfo(item);
                Result.Add(Info.Name);
            }
            return Result;
        }

        public string[] Files(string FolderName)
        {
            var folders = System.IO.Directory.GetDirectories(this.FullFolder).ToList();
            foreach (var item in folders)
            {
                var Info = new System.IO.DirectoryInfo(item);

                if (Info.Name == FolderName)
                {
                    return System.IO.Directory.GetFiles(Info.FullName);
                }
            }
            return null;
        }
    }

    public enum StoreFolderType
    {
        Weekly = 0,
        Daily = 1,
        Monthly = 2
    }
}
