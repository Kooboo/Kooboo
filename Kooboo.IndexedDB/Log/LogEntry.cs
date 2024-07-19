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

        private string _storeName;

        public string StoreName
        {
            get
            {
                return _storeName;
            }
            set
            {
                _storeName = value;
                _storeNameHash = _storeName.GetHashCode32();
            }
        }

        private int _storeNameHash;

        public int StoreNameHash
        {
            get
            {
                if (_storeNameHash == default(int))
                {
                    if (!string.IsNullOrEmpty(_storeName))
                    {
                        _storeNameHash = _storeName.GetHashCode32();
                    }
                    return _storeNameHash;
                }
                return _storeNameHash;
            }
            set
            {
                _storeNameHash = value;
            }
        }


        private string _tableName { get; set; }

        public string TableName
        {
            get
            {
                return _tableName;
            }
            set
            {
                _tableName = value;
                _tableNameHash = _tableName.GetHashCode32();
            }
        }


        private int _tableNameHash;

        public int TableNameHash
        {
            get
            {
                if (_tableNameHash == default(int))
                {
                    if (!string.IsNullOrEmpty(_tableName))
                    {
                        _tableNameHash = _tableName.GetHashCode32();
                    }
                    return _tableNameHash;
                }
                return _tableNameHash;
            }
            set
            {
                _tableNameHash = value;
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

        private byte[] _keyBytes;

        /// <summary>
        /// The key value in the format of byte array. 
        /// </summary>
        public byte[] KeyBytes
        {
            get
            {
                return _keyBytes;
            }
            set
            {
                _keyBytes = value;
                _keyHash = ToHashGuid(_keyBytes);
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
                    _keyHash = ToHashGuid(_keyBytes);
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

        private DateTime _updateTime;
        public DateTime UpdateTime
        {
            get
            {
                if (_updateTime == default(DateTime))
                {
                    _updateTime = DateTime.UtcNow;
                }
                return _updateTime;
            }
            set
            {
                _updateTime = DateTime.SpecifyKind(value, DateTimeKind.Utc);
            }
        }

        private Int64 _timeTick;

        /// <summary>
        /// use timetick instead of datatime to roll back items. Tick is smaller and more accurace than datetime. 
        /// </summary>
        public Int64 TimeTick
        {
            get
            {
                if (_timeTick == default(Int64))
                {
                    _timeTick = UpdateTime.Ticks;
                }
                return _timeTick;
            }
            set
            {
                _timeTick = value;
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