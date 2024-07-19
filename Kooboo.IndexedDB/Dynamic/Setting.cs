//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static Kooboo.IndexedDB.Helper.StringHelper;

namespace Kooboo.IndexedDB.Dynamic
{
    public class Setting
    {
        public Setting()
        {
        }

        public bool EnableLog { get; set; } = true;

        private HashSet<TableColumn> _columns;
        public HashSet<TableColumn> Columns
        {
            get
            {
                if (_columns == null)
                {
                    var newCol = new HashSet<TableColumn>();

                    newCol.Add(new TableColumn() { Name = Constants.DefaultIdFieldName, DataType = typeof(Guid).FullName, relativePosition = 0, Length = 16, IsPrimaryKey = true, IsSystem = true, IsUnique = true, IsIndex = true });

                    _columns = newCol;
                }
                return _columns;
            }
            set
            {
                _columns = value;
            }
        }

        private object _locker = new object();

        public void AddIndex(string FieldName, Type DataType, int length = 0, bool IsUnique = false)
        {
            lock (_locker)
            {
                var find = this.Columns.FirstOrDefault(o => IsSameValue(o.Name, FieldName));
                if (find == null)
                {
                    if (length == 0)
                    {
                        length = Helper.KeyHelper.GetKeyLen(DataType, Constants.DefaultKeyLen);
                    }

                    TableColumn col = new TableColumn();
                    col.DataType = DataType.FullName;
                    col.Length = length;
                    col.Name = FieldName;
                    col.IsIndex = true;
                    col.IsUnique = IsUnique;

                    col.isComplex = SettingHelper.IsComplexType(DataType);

                    AddColumn(col);
                }
                else
                {
                    find.IsUnique = IsUnique;
                    find.IsIndex = true;
                }
            }
        }

        public void AddIndex<TValue>(Expression<Func<TValue, object>> expression, int len = 0)
        {
            string fieldname = Helper.ExpressionHelper.GetFieldName<TValue>(expression);
            if (!string.IsNullOrEmpty(fieldname))
            {
                var type = Helper.TypeHelper.GetFieldType(typeof(TValue), fieldname);
                if (type != null)
                {
                    if (len == 0)
                    {
                        len = Helper.KeyHelper.GetKeyLen(type, Constants.DefaultKeyLen);
                    }
                    this.AddIndex(fieldname, type, len);
                }
            }
        }

        public void SetPrimaryKey<TValue>(Expression<Func<TValue, object>> expression, int len = 0)
        {
            string fieldname = Helper.ExpressionHelper.GetFieldName<TValue>(expression);
            if (!string.IsNullOrEmpty(fieldname))
            {
                var type = Helper.TypeHelper.GetFieldType(typeof(TValue), fieldname);
                if (type != null)
                {
                    if (len == 0)
                    {
                        len = Helper.KeyHelper.GetKeyLen(type, Constants.DefaultKeyLen);
                    }
                    this.SetPrimaryKey(fieldname, type, len);
                }
            }
        }

        public void SetPrimaryKey(string FieldName, Type DataType, int length = 0)
        {
            var find = this.Columns.FirstOrDefault(o => o.Name == FieldName);
            if (find == null)
            {
                length = Helper.KeyHelper.GetKeyLen(DataType, length);
                TableColumn newcol = new TableColumn();
                newcol.isComplex = SettingHelper.IsComplexType(DataType);
                if (newcol.isComplex)
                {
                    throw new Exception("Index must be a value datatype with fixed length.");
                }

                newcol.DataType = DataType.FullName;
                newcol.Length = length;
                newcol.Name = FieldName;
                newcol.IsUnique = true;
                newcol.IsPrimaryKey = true;
                AddColumn(newcol);
            }

            EnsurePrimaryKey(FieldName);
        }

        public void EnsurePrimaryKey(string FieldName)
        {
            var find = this.Columns.FirstOrDefault(o => IsSameValue(o.Name, FieldName));

            if (find != null)
            {
                var currents = this.Columns.Where(o => o.IsPrimaryKey);

                foreach (var item in currents)
                {
                    if (!IsSameValue(item.Name, FieldName))
                    {
                        item.IsPrimaryKey = false;
                        if (item.Name != Constants.DefaultIdFieldName)
                        {
                            item.IsUnique = false;
                            item.IsIndex = false;
                        }
                    }
                }

                find.IsPrimaryKey = true;
                find.IsUnique = true;
            }

            if (!this.Columns.Any(o => o.IsPrimaryKey))
            {
                var id = this.Columns.FirstOrDefault(o => o.IsSystem);

                id.IsPrimaryKey = true;
                id.IsUnique = true;
            }
        }

        public void AddColumn(TableColumn col)
        {
            lock (_locker)
            {
                if (col.IsIncremental)
                {
                    col.DataType = typeof(long).FullName;
                    col.IsUnique = true;
                    col.IsIndex = true;
                }

                // double verify..
                if (col.isComplex)
                {
                    col.relativePosition = int.MaxValue;
                    col.Length = int.MaxValue;
                }
                else
                {
                    col.Length = SettingHelper.GetColumnLen(col.ClrType, col.Length);
                    // get all the relative positive without the complex flxiable position.

                    if (col.Length == int.MaxValue)
                    {
                        col.relativePosition = int.MaxValue;
                    }
                    else
                    {
                        int relativepos = 0;
                        foreach (var item in this.Columns)
                        {
                            if (item.relativePosition != int.MaxValue)
                            {
                                int pos = item.relativePosition + item.Length + 8;   // 8 bytes to record the fieldmhash + len. 
                                if (pos > relativepos)
                                {
                                    relativepos = pos;
                                }
                            }
                        }
                        col.relativePosition = relativepos;
                    }
                }

                if (string.IsNullOrEmpty(col.ControlType))
                {
                    col.ControlType = AssignControlType(col.ClrType);
                }

                this.Columns.Add(col);
                this.Columns = new HashSet<TableColumn>(this.Columns.OrderBy(o => o.relativePosition));

                if (col.IsPrimaryKey)
                {
                    EnsurePrimaryKey(col.Name);
                }
            }
        }

        public static string AssignControlType(Type datatype, object value = null)
        {
            if (datatype == typeof(string))
            {
                if (value != null)
                {
                    var rightvalue = value.ToString();
                    if (rightvalue == null || rightvalue.Length < 256)
                    {
                        return "TextBox";
                    }
                    else
                    {
                        return "TextArea";
                    }
                }
                else
                {
                    return "TextBox";
                }
            }
            else if (datatype == typeof(Byte) || datatype == typeof(Int16) || datatype == typeof(Int32) || datatype == typeof(Int64) || datatype == typeof(decimal) || datatype == typeof(double) || datatype == typeof(float))
            {
                return "Number";
            }
            else if (datatype == typeof(bool))
            {
                return "Boolean";
            }

            else if (datatype == typeof(DateTime))
            {
                return "DateTime";
            }
            return "TextBox";
        }

        public void AppendColumn(string FieldOrPropertyName, Type DataType, int length)
        {
            lock (_locker)
            {
                var exists = this.Columns.FirstOrDefault(o => IsSameValue(o.Name, FieldOrPropertyName));
                if (exists != null)
                {
                    return;
                }
                bool isComplex = SettingHelper.IsComplexType(DataType);

                TableColumn newcol = new TableColumn();
                newcol.DataType = DataType.FullName;
                newcol.Name = FieldOrPropertyName;
                newcol.Length = length;
                if (isComplex)
                {
                    newcol.isComplex = true;
                }
                AddColumn(newcol);
            }
        }

        public void AppendColumn<TValue>(Expression<Func<TValue, object>> expression, int length = 0)
        {
            string fieldname = Helper.ExpressionHelper.GetFieldName<TValue>(expression);
            if (!string.IsNullOrEmpty(fieldname))
            {
                var type = Helper.TypeHelper.GetFieldType(typeof(TValue), fieldname);
                if (type != null)
                {
                    this.AppendColumn(fieldname, type, length);
                }
            }
        }
    }

    public class TableColumn : IEquatable<TableColumn>
    {
        public string Name { get; set; }

        private string _datatype;
        public string DataType
        {
            get { return _datatype; }
            set
            {
                _datatype = value;
                _clrtype = null;
            }
        }

        // int.maxvalue == varies length. 
        public int Length { get; set; }

        public bool isComplex { get; set; }

        // The position that this will be stored in the block data file. 
        // Int.maxvalue means dynamic.. can not be searched....
        // When set to INT.Max. it is only for the sorting. 
        public int relativePosition { get; set; }

        private Type _clrtype;
        [CustomAttributes.KoobooIgnore]
        public Type ClrType
        {
            get
            {
                if (_clrtype == null)
                {
                    _clrtype = Helper.TypeHelper.GetType(this.DataType);
                }
                return _clrtype;
            }
        }

        public bool IsIncremental { get; set; }

        public long Seed { get; set; }

        public int Increment { get; set; }

        public bool IsIndex { get; set; }

        // when this is set to an value non defined as "_id", it means the _id field will be hash from this value. 
        // Then there will be at least two index instantly... Fine... 
        public bool IsPrimaryKey { get; set; }

        public bool IsUnique { get; set; }

        public bool IsSystem { get; set; }

        public string ControlType { get; set; }

        public string Setting { get; set; }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }

        public bool Equals(TableColumn other)
        {
            return this.Name == other.Name;
        }

    }

}
