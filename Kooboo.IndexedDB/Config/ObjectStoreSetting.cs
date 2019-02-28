//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using Kooboo.IndexedDB.Columns;

namespace Kooboo.IndexedDB
{
    [Serializable]
    public class ObjectStoreSetting
    {
        public ObjectStoreSetting()
        {
            this.IndexList = new List<IndexSetting>();
            this.ColumnList = new List<ColumnSetting>();
        }

        public string headerRemark;
        public Type primaryKeyType;
        public Type ValueType;
        public int primaryKeyLength;

        /// <summary>
        /// The primary key fieldname that will be used to calculated file path.
        /// </summary>
        public string primaryKeyFieldName = "_id";

        public List<IndexSetting> IndexList;

        /// <summary>
        /// The list of columns and the length can not be changed after it is initialized. 
        /// </summary>
        public List<ColumnSetting> ColumnList;

        public bool UseDefaultNETBinaryFormater { get; set; }

        public bool EnableLog { get; set; }

        public bool EnableVersion { get; set; }

        public int MaxCacheLevel { get; set; }

    }
}
