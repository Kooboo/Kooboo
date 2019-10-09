//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Security.Cryptography;

namespace Kooboo.IndexedDB
{
    /// <summary>
    /// The cms editing log, this can be used to restore the entire cms entries
    /// </summary>
    public class LogEntry
    {
        /// <summary>
        /// sequence version id. incremental. this is also the key. 
        /// </summary>
        public Int64 Id
        {
            get; set;
        }

        private string _storename;

        public string StoreName
        {
            get
            {
                return _storename;
            }
            set
            {
                _storename = value;
                _storenamehash = _storename.GetHashCode32();
            }
        }

        private int _storenamehash;

        public int StoreNameHash
        {
            get
            {
                if (_storenamehash == default(int))
                {
                    if (!string.IsNullOrEmpty(_storename))
                    {
                        _storenamehash = _storename.GetHashCode32();
                    }
                    return _storenamehash;
                }
                return _storenamehash;
            }
            set
            {
                _storenamehash = value;
            }
        }


        private string _tablename { get; set; }

        public string TableName
        {
            get
            {
                return _tablename;
            }
            set
            {
                _tablename = value;
                _tablenamehash = _tablename.GetHashCode32();
            }
        }


        private int _tablenamehash;

        public int TableNameHash
        {
            get
            {
                if (_tablenamehash == default(int))
                {
                    if (!string.IsNullOrEmpty(_tablename))
                    {
                        _tablenamehash = _tablename.GetHashCode32();
                    }
                    return _tablenamehash;
                }
                return _tablenamehash;
            }
            set
            {
                _tablenamehash = value;
            }
        }

        public string TableColName { get; set; }

        [Kooboo.IndexedDB.CustomAttributes.KoobooIgnore]
        public bool IsTable
        {
            get
            {
                return !string.IsNullOrWhiteSpace(TableName);
            }
        }

        public Guid UserId { get; set; }

        private byte[] _keybytes;

        /// <summary>
        /// The key value in the format of byte array. 
        /// </summary>
        public byte[] KeyBytes
        {
            get
            {
                return _keybytes;
            }
            set
            {
                _keybytes = value;
                _keyHash = ToHashGuid(_keybytes);
            }
        }

        private Guid _keyHash;
        /// <summary>
        /// The KeyBytes HasKey; 
        /// </summary>
        public Guid KeyHash
        {
            get
            {
                if (_keyHash == default(Guid))
                {
                    _keyHash = ToHashGuid(_keybytes);
                }
                return _keyHash;
            }
            set
            {
                _keyHash = value;
            }

        }


        /// <summary>
        ///The kind of action, Add =0, update =1, del =2. 
        /// </summary>
        public EditType EditType { get; set; }

        private DateTime _updatetime;
        public DateTime UpdateTime
        {
            get
            {
                if (_updatetime == default(DateTime))
                {
                    _updatetime = DateTime.UtcNow;
                }
                return _updatetime;
            }
            set
            {
                _updatetime = DateTime.SpecifyKind(value, DateTimeKind.Utc);
            }
        }

        private Int64 _timetick;

        /// <summary>
        /// use timetick instead of datatime to roll back items. Tick is smaller and more accurace than datetime. 
        /// </summary>
        public Int64 TimeTick
        {
            get
            {
                if (_timetick == default(Int64))
                {
                    _timetick = UpdateTime.Ticks;
                }
                return _timetick;
            }
            set
            {
                _timetick = value;
            }
        }

        public Int64 OldBlockPosition { get; set; }

        public Int64 NewBlockPosition { get; set; }

        public static Guid ToHashGuid(byte[] bytes)
        {
            if (bytes == null)
            {
                return default(Guid);
            }
            MD5 md5Hasher = MD5.Create();
            byte[] data = md5Hasher.ComputeHash(bytes);
            return new Guid(data);
        }
    }

    public enum EditType
    {
        Add = 1,
        Update = 2,
        Delete = 3
    }

}