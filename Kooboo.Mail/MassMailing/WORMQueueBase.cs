using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Kooboo.IndexedDB.WORM;
using Microsoft.Extensions.Caching.Memory;
using SharpCompress;

namespace Kooboo.Mail.MassMailing
{
    public class WORMQueueBase<T>
    {
        public WORMQueueBase()
        {
            InitReading();
        }

        public WORMQueueBase(string Folder)
        {
            this.RootFolder = Folder;
            Lib.Helper.IOHelper.EnsureDirectoryExists(this.RootFolder);
        }

        private static MemoryCache DbCache { get; set; } = new MemoryCache(new MemoryCacheOptions());

        /// <summary>
        /// Use as an unique folder name in order to separate from others
        /// </summary>
        protected virtual string UniqueIdentifier { get; set; } = "Daily";

        private string _folder;
        private object _locker = new object();
        private string RootFolder
        {
            get
            {
                if (_folder == null)
                {
                    lock (_locker)
                    {
                        if (_folder == null)
                        {
                            var root = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppData");
                            var tName = typeof(T).Name;
                            root = System.IO.Path.Combine(root, "WORMQueue", tName, UniqueIdentifier);
                            Kooboo.Lib.Helper.IOHelper.EnsureDirectoryExists(root);
                            _folder = root;
                        }
                    }
                }
                return _folder;
            }
            set
            {
                _folder = value;
            }
        }

        private int CurrentReadingFileId { get; set; }


        private long WormDBId { get; set; }


        private string FullFileName(int FileId)
        {
            return System.IO.Path.Combine(RootFolder, FileId.ToString() + ".data");
        }

        public T Dequeue()
        {
            var item = _Peek();
            if (item != null)
            {
                Process(item);
            }

            if (item != null)
            {
                return item.Value;
            }
            else
            {
                return default;
            }
        }


        public T Peek()
        {
            var item = _Peek();
            if (item != null)
            {
                return item.Value;
            }
            return default;
        }

        public MetaTValue<T> _Peek()
        {
            if (this.CurrentReadingFileId == 0)
            {
                this.InitReading();
            }

            if (this.CurrentReadingFileId == 0)
            {
                return null;
            }

            var db = GetDBFromFileId(this.CurrentReadingFileId, false);

            if (db != null)
            {
                long MaxId = 0;

                if (this.WormDBId < 1)
                {
                    this.WormDBId = 1;
                }

                var item = db.Get(this.WormDBId);

                if (item != null && !item.Processed)
                {
                    return item;
                }

                while (item == null || item.Processed)
                {
                    if (MaxId == 0)
                    {
                        MaxId = db.FindKey(false);
                    }

                    if (this.WormDBId < MaxId)
                    {
                        this.WormDBId += 1;
                        item = db.Get(this.WormDBId);
                    }
                    else
                    {
                        break;
                    }
                }


                if (item != null && !item.Processed)
                {
                    return item;
                }

                // it is max id, and also... and also not current date. 
                if (MaxId == 0)
                {
                    MaxId = db.FindKey(false);
                }
                var newFileId = DateToFileId(DateTime.Now);

                if (this.WormDBId >= MaxId && newFileId > this.CurrentReadingFileId)
                {
                    // can move ahead to next file. 
                    var allIds = ListAllFileIds();

                    var FilterIds = allIds.Where(o => o > this.CurrentReadingFileId).OrderBy(o => o);

                    if (FilterIds != null && FilterIds.Any())
                    {
                        var newId = FilterIds.FirstOrDefault();
                        if (newId > this.CurrentReadingFileId)
                        {
                            this.CurrentReadingFileId = newId;
                            this.WormDBId = 1;

                            return _Peek();
                        }
                    }
                }
            }

            return null;
        }

        public void Process(MetaTValue<T> value)
        {
            Process(value.TimeFileId, value.Id);
        }

        public void Process(int FileId, long WormDbId)
        {
            var db = GetDBFromFileId(FileId, false);
            if (db != null)
            {
                var item = db.Get(WormDbId);
                item.Processed = true;
                db.UpdateMeta(item, WormDbId);

                if (this.CurrentReadingFileId == FileId && this.WormDBId == WormDBId)
                {
                    this.WormDBId += 1;
                }
            }
        }

        private WormDb<MetaTValue<T>> GetDBFromFileId(int id, bool Create)
        {
            var FileName = FullFileName(id);

            if (!Create && !System.IO.File.Exists(FileName))
            {
                return null;
            }

            Guid key = Lib.Security.Hash.ComputeGuidIgnoreCase(FileName);

            if (DbCache.TryGetValue<WormDb<MetaTValue<T>>>(key, out var value))
            {
                return value;
            }
            else
            {
                lock (_locker)
                {
                    if (DbCache.TryGetValue<WormDb<MetaTValue<T>>>(key, out var value2))
                    {
                        return value2;
                    }
                    else
                    {
                        var store = new WormDb<MetaTValue<T>>(FileName, nameof(MetaTValue<T>.Id));
                        DbCache.Set(key, store, TimeSpan.FromHours(24));
                        return store;
                    }
                }
            }
        }

        private WormDb<MetaTValue<T>> _WritingDb;

        private int WriteId { get; set; }


        private void CleanOldFile(int MaxWriteId)
        {
            var AllIds = ListAllFileIds();

            var OldId = AllIds.Where(o => o < MaxWriteId);

            if (OldId != null && OldId.Any())
            {
                var TooOldShouldDelete = OldId.OrderByDescending(o => o).Skip(3).Take(999);

                if (TooOldShouldDelete != null && TooOldShouldDelete.Any())
                {
                    foreach (var item in TooOldShouldDelete)
                    {
                        var db = GetDBFromFileId(item, false);
                        if (db != null)
                        {
                            db.Close();
                        }
                        var fileName = FullFileName(item);
                        Lib.Helper.IOHelper.DeleteFile(fileName);
                    }
                }
            }
        }

        public WormDb<MetaTValue<T>> WritingDb(int CurrentTimeFileId)
        {
            if (CurrentTimeFileId > WriteId)
            {
                lock (_locker)
                {
                    if (CurrentTimeFileId > WriteId)
                    {
                        _WritingDb?.Close();
                    }
                    var db = GetDBFromFileId(CurrentTimeFileId, true);
                    _WritingDb = db;
                    WriteId = CurrentTimeFileId;

                    Task.Run(() => { CleanOldFile(WriteId); });
                }
            }

            if (_WritingDb == null)
            {
                lock (_locker)
                {
                    if (_WritingDb == null)
                    {
                        var db = GetDBFromFileId(CurrentTimeFileId, true);
                        _WritingDb = db;
                        WriteId = CurrentTimeFileId;
                    }
                }
            }
            return _WritingDb;
        }

        public void Enqueue(T value)
        {
            var currentFileId = DateToFileId(DateTime.Now);
            var db = WritingDb(currentFileId);

            MetaTValue<T> model = new MetaTValue<T>();
            model.Value = value;
            model.TimeFileId = currentFileId;
            var id = db.Add(model);
        }

        private void InitReading()
        {

            var allIds = ListAllFileIds();

            foreach (var id in allIds.OrderByDescending(o => o))
            {
                var fileName = FullFileName(id);

                if (System.IO.File.Exists(fileName))
                {
                    WormDb<MetaTValue<T>> DB = new WormDb<MetaTValue<T>>(fileName, "Id", true);

                    var firstKey = DB.FindKey(true);

                    var item = DB.Get(firstKey);

                    //If first item has bene processed,this is the file being processed. 
                    if (item != null && item.Processed)
                    {
                        this.CurrentReadingFileId = id;
                        this.WormDBId = FindFirstUnProcessed(DB);
                        return;
                    }
                    else
                    {
                        this.CurrentReadingFileId = id;
                        this.WormDBId = firstKey;  //will continue reading.
                    }
                }
            }

        }

        public List<int> ListAllFileIds()
        {
            List<int> Result = new List<int>();

            var allFiles = System.IO.Directory.GetFiles(this.RootFolder);

            foreach (var item in allFiles)
            {
                var info = new System.IO.FileInfo(item);

                var name = info.Name.Replace(".data", "");

                if (IsIdStringRightFormat(name))
                {
                    if (int.TryParse(name, out var id))
                    {
                        Result.Add(id);
                    }
                }
            }
            return Result;
        }


        public virtual bool IsIdStringRightFormat(string IdString)
        {
            var currentFormat = DateToFileId(DateTime.Now).ToString();
            return IsSameFormat(currentFormat, IdString);
        }

        private bool IsSameFormat(string A, string B)
        {
            if (A == null || B == null)
            {
                return false;
            }
            if (A.Length != B.Length)
            {
                return false;
            }
            for (int i = 0; i < A.Length; i++)
            {
                var x = A[i];
                var y = B[i];
                if (x == y)
                {
                    return true;
                }
            }
            return false;
        }

        private long FindFirstUnProcessed(WormDb<MetaTValue<T>> db)
        {
            foreach (var item in db.MetaQuery.All(true))
            {
                if (!item.Processed)
                {
                    return item.Id;
                }
            }
            return 0;
        }

        protected virtual int DateToFileId(DateTime time)
        {
            return Lib.Helper.DateTimeHelper.DayToInt32(time);
        }


        protected virtual DateTime AddInterval(DateTime time)
        {
            return time.AddHours(1);
        }

        protected virtual DateTime DeductInterval(DateTime time)
        {
            return time.AddHours(-1);
        }

    }

    public class MetaTValue<T> : Kooboo.IndexedDB.WORM.MetaObject.IMetaObject
    {
        public long Id { get; set; }

        public int MetaByteLen => 10;

        public long MetaKey { get; set; }

        public bool SkipValueBlock => false;

        public bool Processed { get; set; }

        public int TimeFileId { get; set; }

        public T Value { get; set; }

        public long CheckTimeTick { get; set; }

        public byte[] GetMetaBytes()
        {
            byte[] bytes = new byte[10];

            System.Buffer.BlockCopy(BitConverter.GetBytes(CheckTimeTick), 0, bytes, 0, 8);
            if (this.Processed)
            {
                bytes[8] = 1;
            }
            else
            {
                bytes[8] = 0;
            }
            return bytes;
        }

        public void ParseMetaBytes(byte[] bytes)
        {
            this.CheckTimeTick = BitConverter.ToInt64(bytes, 0);
            this.Processed = bytes[8] == 1;
        }
    }
}
