//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System;
using System.Collections.Generic;

namespace Kooboo.IndexedDB
{
    public class StoreSetting
    {
        public string _HeaderMark { get; set; } = "Kooboo Setting DO NOT Modify";

        public string ValueTypeFullName { get; set; }

        private Type _valuetype;

        [CustomAttributes.KoobooIgnore]
        public Type ValueType
        {
            get
            {
                if (_valuetype == null)
                {
                    if (!string.IsNullOrEmpty(this.ValueTypeFullName))
                    {
                        _valuetype = Helper.TypeHelper.GetType(this.ValueTypeFullName);
                    }
                }
                return _valuetype;
            }
            set
            {
                _valuetype = value;
            }
        }

        /// <summary>
        /// The primary key fieldname that will be used to calculated file path.
        /// </summary>
        public string PrimaryKey { get; set; } = "_id";

        public int PrimaryKeyLen { get; set; }

        private Dictionary<string, int> _indexs;

        public Dictionary<string, int> Indexs
        {
            get
            {
                if (_indexs == null)
                {
                    _indexs = new Dictionary<string, int>();
                }
                return _indexs;
            }
            set { _indexs = value; }
        }

        private Dictionary<string, int> _columns;

        public Dictionary<string, int> Columns
        {
            get { return _columns ?? (_columns = new Dictionary<string, int>()); }
            set { _columns = value; }
        }

        public bool UseDefaultNETBinaryFormater { get; set; }

        public bool EnableLog { get; set; }

        public bool EnableVersion { get; set; }

        public int MaxCacheLevel { get; set; }
    }
}