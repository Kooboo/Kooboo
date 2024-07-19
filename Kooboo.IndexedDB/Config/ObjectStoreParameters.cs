//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Kooboo.IndexedDB
{
    [Serializable]
    public class ObjectStoreParameters
    {

        private int _maxkeylength;
        /// <summary>
        /// The max lenght of the primary key in bytes. For unicode string, 1 char = 2 bytes.  The lower the value is, the faster the system will be.  If the actually key key length is less than the length. it will append byte(value=0) till the defined length.
        /// </summary>
        public int PrimaryKeyLength
        {
            get
            {
                if (_maxkeylength < 1)
                {
                    _maxkeylength = GlobalSettings.defaultKeyLength;
                }
                return _maxkeylength;
            }

            set
            {
                _maxkeylength = value;
            }

        }


        /// <summary>
        /// If you want to allow  full table scan into some columns, put the field or property names in this dictionary. 
        /// Insert values into this ColumnList will automatically turn off AutoGenerateColumns and use your own list.
        /// string = FieldName,
        /// int = Fixed length of this Field data in bytes. This lenght is only required for String data type.
        /// </summary>
        public Dictionary<string, int> ColumnList = new Dictionary<string, int>();


        /// <summary>
        /// Add a new column, a column allow full table scan to search records. 
        /// An column can not be altered after created, an index can be deleted or rebuilt. 
        /// </summary>
        /// <param name="FieldOrPropertyName"></param>
        /// <param name="maxlength">The max column length, this value is only useful for string field. </param>
        public void AddColumn(string FieldOrPropertyName, int maxlength)
        {
            if (!ColumnList.ContainsKey(FieldOrPropertyName))
            {
                ColumnList.Add(FieldOrPropertyName, maxlength);
            }
        }

        /// <summary>
        /// Add a new column, if it is a string, consider using the overload method to specify a length.
        /// An column can not be altered after created, an index can be deleted or rebuilt. 
        /// </summary>
        /// <param name="FieldOrPropertyName"></param>
        public void AddColumn(string FieldOrPropertyName)
        {
            AddColumn(FieldOrPropertyName, GlobalSettings.defaultKeyLength);
        }

        public void AddColumn<TValue>(Expression<Func<TValue, object>> expression)
        {
            string fieldname = Helper.ExpressionHelper.GetFieldName<TValue>(expression);

            if (!string.IsNullOrEmpty(fieldname))
            {
                AddColumn(fieldname);
            }
        }

        public void AddColumn<TValue>(Expression<Func<TValue, object>> expression, int maxlength)
        {
            string fieldname = Helper.ExpressionHelper.GetFieldName<TValue>(expression);

            if (!string.IsNullOrEmpty(fieldname))
            {
                AddColumn(fieldname, maxlength);
            }
        }

        /// <summary>
        /// create additional index on some other column. Better use for non-string type of index. 
        /// string = FieldName,   int = Fixed length of this Field data in bytes. This lenght is only required for String data type.
        /// An column can not be altered after created, an index can be deleted or rebuilt. 
        /// </summary>
        public Dictionary<string, int> IndexList = new Dictionary<string, int>();

        /// <summary>
        /// Add an additional index. 
        /// An column can not be altered after created, an index can be deleted or rebuilt. 
        /// </summary>
        /// <param name="FieldOrPropertyName"></param>
        /// <param name="maxlength"></param>
        public void AddIndex(string FieldOrPropertyName, int maxlength)
        {
            if (!IndexList.ContainsKey(FieldOrPropertyName))
            {
                IndexList.Add(FieldOrPropertyName, maxlength);
            }
        }

        /// <summary>
        /// Add a new index, if it is a string, consider using the overload method to specify a length.
        /// An column can not be altered after created, an index can be deleted or rebuilt. 
        /// </summary>
        /// <param name="FieldOrPropertyName"></param>
        public void AddIndex(string FieldOrPropertyName)
        {
            AddIndex(FieldOrPropertyName, GlobalSettings.defaultKeyLength);
        }


        public void AddIndex<TValue>(Expression<Func<TValue, object>> expression)
        {
            string fieldname = Helper.ExpressionHelper.GetFieldName<TValue>(expression);

            if (!string.IsNullOrEmpty(fieldname))
            {
                AddIndex(fieldname);
            }
        }

        public void AddIndex<TValue>(Expression<Func<TValue, object>> expression, int maxlength)
        {
            string fieldname = Helper.ExpressionHelper.GetFieldName<TValue>(expression);

            if (!string.IsNullOrEmpty(fieldname))
            {
                AddIndex(fieldname, maxlength);
            }
        }

        public void SetPrimaryKeyField<TValue>(Expression<Func<TValue, object>> expression, int len = 0)
        {
            string fieldname = Helper.ExpressionHelper.GetFieldName<TValue>(expression);

            if (!string.IsNullOrEmpty(fieldname))
            {
                this.PrimaryKey = fieldname;
            }
        }

        public string PrimaryKey { get; set; } = "_id";
        /// <summary>
        /// enable log on every add/update/del. This allows recover from log in case of necessary. And save any single changes that allows database to revert back to past time. 
        /// </summary>
        public bool EnableLog { get; set; }
        /// <summary>
        /// enbable versioning on all objects within this store, when version is enable, during insert or update, store will try to insert a version number into the object property field "Version".
        /// Note: this object model must define a property named "Version". Log must be enabled first before version can be enable. 
        /// </summary>
        public bool EnableVersion { get; set; }

        /// <summary>
        /// use the default .NET binary serializer. more support but slower in speed. 
        /// </summary>
        public bool UseDefaultNETBinaryFormater { get; set; }

        public bool UseMsgPackSerializer { get; set; }

        public int MaxCacheLevel { get; set; }
    }
}
