using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kooboo.Lib.Storage
{
    // File base queue.
    // NOTE, this does not remove files...shoud move into logs or other
    public class FileBaseQueue<T>
    {
        public FileBaseQueue(String baseDir, string folderName)
        {
            this.BaseFolder = baseDir;
            this.FolderName = folderName;
        }

        public FileBaseQueue(string folderName)
        {
            this.FolderName = folderName;
        }

        public string FileExtension { get; set; } = ".json";

        private string FolderName { get; set; }

        private string _baseFolder;
        private string BaseFolder
        {
            get
            {
                if (string.IsNullOrEmpty(_baseFolder))
                {
                    var folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppData");
                    Helper.IOHelper.EnsureDirectoryExists(folder);
                    folder = Path.Combine(folder, "Queue");
                    Helper.IOHelper.EnsureDirectoryExists(folder);
                    _baseFolder = folder;
                }
                return _baseFolder;
            }
            set
            {
                _baseFolder = value;
                Helper.IOHelper.EnsureDirectoryExists(_baseFolder);
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

        //used for get next collection cache, so that do not need to read files every time. 
        private Queue<string> tempfilequeue { get; set; } = new Queue<string>();

        public FetchResult<T> Fetch()
        {
            if (tempfilequeue.Any())
            {
                var item = tempfilequeue.Dequeue();
                if (item != null && System.IO.File.Exists(item))
                {
                    var allText = System.IO.File.ReadAllText(item);

                    T Value = System.Text.Json.JsonSerializer.Deserialize<T>(allText, new System.Text.Json.JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

                    if (Value != null)
                    {
                        FetchResult<T> result = new FetchResult<T>();
                        result.Value = Value;
                        result.FullFileName = item;
                        result.FileName = System.IO.Path.GetFileName(item);

                        return result;
                    }

                }
                return Fetch();
            }

            else
            {
                var hasresult = LoadQueue();
                if (hasresult)
                {
                    return Fetch();
                }
                else { return null; }
            }

            // load queue; 
        }

        public List<FetchResult<T>> FetchAllFromDisk()
        {
            List<FetchResult<T>> result = new List<FetchResult<T>>();

            var files = System.IO.Directory.GetFiles(this.FullFolder);
            if (files != null && files.Any())
            {
                foreach (var item in files)
                {
                    var fetchItem = FetchFromFile(item);
                    if (fetchItem != null)
                    {
                        result.Add(fetchItem);
                    }
                }
            }
            return result;
        }

        public FetchResult<T> FetchFromFile(string item)
        {
            var allText = System.IO.File.ReadAllText(item);

            T Value = System.Text.Json.JsonSerializer.Deserialize<T>(allText, new System.Text.Json.JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

            if (Value != null)
            {
                FetchResult<T> fetchItem = new FetchResult<T>();
                fetchItem.Value = Value;
                fetchItem.FullFileName = item;
                fetchItem.FileName = System.IO.Path.GetFileName(item);
                return fetchItem;
            }
            return null;
        }

        private bool LoadQueue()
        {
            var files = System.IO.Directory.GetFiles(this.FullFolder);
            if (files != null && files.Any())
            {
                foreach (var item in files)
                {
                    this.tempfilequeue.Enqueue(item);
                }
                return true;
            }
            return false;
        }

        public void clear()
        {
            var files = System.IO.Directory.GetFiles(this.FullFolder);
            if (files != null && files.Any())
            {
                foreach (var item in files)
                {
                    System.IO.File.Delete(item);
                }
            }
            this.tempfilequeue.Clear();
        }

        public void Add(T TValue)
        {
            string key = System.Guid.NewGuid().ToString();

            Add(key, TValue);
        }

        public void Add(string key, T TValue)
        {
            var source = System.Text.Json.JsonSerializer.Serialize(TValue);

            var filename = key + this.FileExtension;

            var fullName = System.IO.Path.Combine(this.FullFolder, filename);

            if (!System.IO.File.Exists(fullName))
            {
                System.IO.File.WriteAllText(fullName, source);
            }
        }

        public string GetFullFileName(string key)
        {
            var filename = key + this.FileExtension;

            var fullName = System.IO.Path.Combine(this.FullFolder, filename);
            return fullName;
        }

        public void Update(string key, T TValue)
        {
            var source = System.Text.Json.JsonSerializer.Serialize(TValue);

            var filename = key + this.FileExtension;

            var fullName = System.IO.Path.Combine(this.FullFolder, filename);

            System.IO.File.WriteAllText(fullName, source);
        }

        public void UpdateByFileName(string FullFileName, T TValue)
        {
            var source = System.Text.Json.JsonSerializer.Serialize(TValue);
            System.IO.File.WriteAllText(FullFileName, source);
        }


        public void Delete(string FullFilName)
        {
            if (System.IO.File.Exists(FullFilName))
            {
                System.IO.File.Delete(FullFilName);
            }
        }
    }
    public class FetchResult<T>
    {
        public T Value { get; set; }
        public string FullFileName { get; set; }

        public string FileName { get; set; }
    }


}
