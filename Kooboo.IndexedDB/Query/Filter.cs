//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using Kooboo.IndexedDB.ByteConverter;
using Kooboo.IndexedDB.Indexs;
using Kooboo.IndexedDB.Btree;
using Kooboo.IndexedDB.Columns;
using System.Linq.Expressions;
using Kooboo.IndexedDB.Serializer.Simple;

namespace Kooboo.IndexedDB.Query
{
    /// <summary>
    /// Where condition based query fitler to search database quickly.
    /// For better syntax, you can use Select. store.Select. 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class Filter<TKey, TValue>
    {
        public int SkipCount;
        public bool Ascending;
        public string OrderByFieldName;
        internal bool OrderByPrimaryKey;
        
        /// <summary>
        /// Use the column data to generate object only, but not read entire object from disk. 
        /// This is used for faster performance in senarios like MailMessage contains a raw body that is not needed most of the time. 
        /// </summary>
        private bool ColumnDataOnly;
        
        internal ObjectStore<TKey, TValue> store;

        public List<FilterItem> items = new List<FilterItem>();

        public Dictionary<string, List<byte[]>> InItems = new Dictionary<string, List<byte[]>>();

        public List<MethodCallExpression> calls = new List<MethodCallExpression>(); 

        public Filter(ObjectStore<TKey, TValue> store)
        {
            this.store = store;
            this.SkipCount = 0;
            this.OrderByPrimaryKey = false;
            this.Ascending = true;
            this.ColumnDataOnly = false;
        }

        public Filter<TKey, TValue> Where(string FieldOrPropertyName, Comparer comparer, object CompareValue)
        {
            FilterItem item = new FilterItem();

            // check to see if there is in the column. 
            IColumn<TValue> column = this.store.GetColumn(FieldOrPropertyName);

            if (column != null)
            {
                item.FieldType = column.DataType;
                item.Length = column.Length;
            }
            else
            {
                IIndex<TValue> index = this.store.Indexes.getIndex(FieldOrPropertyName);

                if (index != null)
                {
                    item.FieldType = index.keyType;
                    item.Length = index.Length;
                }
                else
                {
                    if (FieldOrPropertyName.IsSameValue(this.store.StoreSetting.PrimaryKey))
                    {
                        var keytype = Helper.TypeHelper.GetFieldType(this.store.StoreSetting.ValueType, this.store.StoreSetting.PrimaryKey);

                        item.FieldType = keytype;
                        item.Length = this.store.StoreSetting.PrimaryKeyLen; 
                    }
                }

            }

            if (item.FieldType == null || item.Length < 1)
            {
                throw new Exception("only index or column are allowed in the where condition, consider adding the field into columns in order to search");
            }

            item.Compare = comparer;

            item.FieldOrProperty = FieldOrPropertyName;

            byte[] bytevalue = getByteValue(item.FieldType, CompareValue);

            if (bytevalue != null)
            {
                item.Value = bytevalue;

                //bool and guid, there is not > < =, only equal or not equal. 
                if (item.FieldType == typeof(bool) || item.FieldType == typeof(Guid))
                {
                    if (item.Compare != Comparer.EqualTo && item.Compare != Comparer.NotEqualTo)
                    {
                        item.Compare = Comparer.EqualTo;
                    }
                }

                //date time, must have specify comare till min, second, millsecond, etc. 
                if (item.FieldType == typeof(DateTime))
                {
                    if (item.TimeScope == default(DateTimeScope))
                    {
                        item.TimeScope = DateTimeScope.day;
                    }
                }

                this.items.Add(item);
            }

            return this;
        }

        public Filter<TKey, TValue> WhereEqual(string FieldOrPropertyName, bool Value)
        {
            FilterItem item = new FilterItem();

            item.Compare = Comparer.EqualTo;

            item.FieldOrProperty = FieldOrPropertyName;
            item.FieldType = typeof(bool);
            item.Length = 1;

            byte[] valuebyte = new byte[1];
            if (Value)
            {
                valuebyte[0] = 1;
            }
            else
            {
                valuebyte[0] = 0;
            }
            item.Value = valuebyte;

            this.items.Add(item);

            return this;
        }

        public Filter<TKey, TValue> WhereIn(string FieldOrPropertyName, List<object> Values)
        { 
            IColumn<TValue> column = this.store.GetColumn(FieldOrPropertyName);

            if (column == null)
            {
                throw new Exception("Only Fields defined in Column are allowed to use In query"); 
            }

            List<byte[]> invalues = new List<byte[]>(); 

            foreach (var item in Values)
            {
                byte[] value = getByteValue(column.DataType, item);
                invalues.Add(value); 
            }

            this.InItems[FieldOrPropertyName] = invalues; 

            return this;
        }
         
        public Filter<TKey, TValue> MethodCall(MethodCallExpression call)
        {
            this.calls.Add(call); 
            return this; 
        }

        public Filter<TKey, TValue> WhereIn<Type>(string FieldOrPropertyName, List<Type> Values)
        {
            IColumn<TValue> column = this.store.GetColumn(FieldOrPropertyName);

            if (column == null)
            {
                throw new Exception("Only Fields defined in Column are allowed to use In query");
            }

            List<byte[]> invalues = new List<byte[]>();

            foreach (var item in Values)
            {
                byte[] value = getByteValue(column.DataType, item);
                invalues.Add(value);
            }

            this.InItems[FieldOrPropertyName] = invalues;

            return this;
        }

        public Filter<TKey, TValue> WhereIn<T>(Expression<Func<TValue, object>> FieldExpression, List<T> Values)
        {
            string fieldname = Helper.ExpressionHelper.GetFieldName<TValue>(FieldExpression); 

            if (!string.IsNullOrEmpty(fieldname))
            {
                this.WhereIn<T>(fieldname, Values);
            }
            return this;
        }

        
        public Filter<TKey, TValue> WhereEqual(string FieldOrPropertyName, Guid Value)
        {
            FilterItem item = new FilterItem();

            item.Compare = Comparer.EqualTo;

            item.FieldOrProperty = FieldOrPropertyName;
            item.FieldType = typeof(Guid);
            IByteConverter<Guid> x = ObjectContainer.GetConverter<Guid>();
            item.Value = x.ToByte(Value);

            item.Length = 16;

            this.items.Add(item);

            return this;
        }

        public Filter<TKey, TValue> WhereEqual(string FieldOrPropertyName, object Value)
        {
            return Where(FieldOrPropertyName, Comparer.EqualTo, Value);
        }

        public Filter<TKey, TValue> Where(string FieldOrPropertyName, Comparer comparer, DateTime CompareValue, DateTimeScope scope)
        {
            FilterItem item = new FilterItem();

            item.Compare = comparer;

            item.TimeScope = scope;

            item.FieldOrProperty = FieldOrPropertyName;
            item.FieldType = typeof(DateTime);

            item.Length = 8;

            IByteConverter<DateTime> x = ObjectContainer.GetConverter<DateTime>();

            item.Value = x.ToByte(CompareValue);

            this.items.Add(item);

            return this;
        }

        /// <summary>
        /// Filter based on the object primary key.
        /// </summary>
        /// <param name="FieldOrPropertyName"></param>
        /// <param name="comparer"></param>
        /// <param name="CompareValue"></param>
        /// <returns></returns>
        public Filter<TKey, TValue> WhereKey(Comparer comparer, object CompareValue)
        {
            FilterItem item = new FilterItem();

            item.FieldType = typeof(TKey); 
            item.Length = this.store.StoreSetting.PrimaryKeyLen;

            item.Compare = comparer;

            item.FieldOrProperty = this.store.StoreSetting.PrimaryKey;

            byte[] bytevalue = this.getByteValue(typeof(TKey), CompareValue);
             
            if (bytevalue != null)
            {
                item.Value = bytevalue; 
                this.items.Add(item);
            }

            return this;
        }

        private byte[] getByteValue(Type datatype, object objectvalue)
        {
            if (datatype == typeof(Int32))
            {
                int value = Convert.ToInt32(objectvalue);
                IByteConverter<Int32> x = ObjectContainer.GetConverter<Int32>();
                return x.ToByte(value);
            }
            else if (datatype == typeof(Int16))
            {  
                Int16 value = Convert.ToInt16(objectvalue);
                IByteConverter<Int16> x = ObjectContainer.GetConverter<Int16>();
                return x.ToByte(value);
            }
            else if (datatype == typeof(string))
            {
                string value = Convert.ToString(objectvalue);
                IByteConverter<string> x = ObjectContainer.GetConverter<string>();

                return x.ToByte(value);
            }
            else if (datatype == typeof(Int64))
            {
                Int64 value = Convert.ToInt64(objectvalue);
                IByteConverter<Int64> x = ObjectContainer.GetConverter<Int64>();
                return x.ToByte(value);
            }
            else if (datatype == typeof(float))
            {
                float value = Convert.ToSingle(objectvalue);
                IByteConverter<float> x = ObjectContainer.GetConverter<float>();
                return x.ToByte(value);
            }
            else if (datatype == typeof(double))
            {
                double value = Convert.ToDouble(objectvalue);
                IByteConverter<double> x = ObjectContainer.GetConverter<double>();
                return x.ToByte(value);
            }
            else if (datatype == typeof(decimal))
            {
                double value = Convert.ToDouble(objectvalue);
                IByteConverter<double> x = ObjectContainer.GetConverter<double>();
                return x.ToByte(value);
            }

            else if (datatype == typeof(Guid))
            {
                Guid value = Guid.Parse(Convert.ToString(objectvalue));

                if (value == null)
                {
                    return null;
                }
                else
                {
                    IByteConverter<Guid> x = ObjectContainer.GetConverter<Guid>();
                    return x.ToByte(value);
                }
            }

            else if (datatype == typeof(bool))
            {
                bool value = Convert.ToBoolean(objectvalue);
                 
                byte[] valuebyte = new byte[1];
                if (value)
                {
                    valuebyte[0] = 1;
                }
                else
                {
                    valuebyte[0] = 0;
                }

                return valuebyte;
            }

            else if (datatype == typeof(DateTime))
            {
                DateTime datevalue = (DateTime)objectvalue; 
                 
                IByteConverter<DateTime> x = ObjectContainer.GetConverter<DateTime>();
                return x.ToByte(datevalue); 
            }

            else if (datatype == typeof(byte))
            {
                byte[] valuebyte = new byte[1];
                valuebyte[0] = Convert.ToByte(objectvalue);

                return valuebyte;
            }

            else if (datatype.IsEnum)
            {
                int intvalue = (int)objectvalue;
                IByteConverter<Int32> x = ObjectContainer.GetConverter<Int32>();
                return x.ToByte(intvalue);
            } 
            else
            { 
                throw new Exception("data type not supported for filter");
            }

        }

        /// <summary>
        /// Order by the primary key.
        /// </summary>
        public Filter<TKey, TValue> OrderByAscending()
        {
            this.Ascending = true;
            this.OrderByPrimaryKey = true;
            this.OrderByFieldName = string.Empty;
            return this;
        }

        /// <summary>
        /// Order by a field or property. This field should have an index on it. 
        /// Order by a non-indexed field will have very bad performance. 
        /// </summary>
        public Filter<TKey, TValue> OrderByAscending(string FieldOrPropertyName)
        {
            if (FieldOrPropertyName.IsSameValue(this.store.StoreSetting.PrimaryKey))
            {
                return this.OrderByAscending();
            }
            else
            {
                this.Ascending = true;
                this.OrderByFieldName = FieldOrPropertyName;
                this.OrderByPrimaryKey = false;
                return this;
            }
        }

        /// <summary>
        /// Order by descending based on the primary key.
        /// </summary>
        public Filter<TKey, TValue> OrderByDescending()
        {
            this.Ascending = false;
            this.OrderByPrimaryKey = true;
            this.OrderByFieldName = string.Empty;
            return this;
        }

        /// <summary>
        /// Order by descending on a field or property. This field should have an index on it. 
        /// </summary>
        public Filter<TKey, TValue> OrderByDescending(string FieldOrPropertyName)
        {
            if (FieldOrPropertyName.IsSameValue(this.store.StoreSetting.PrimaryKey))
            {
                return this.OrderByDescending();
            }
            else
            {
                this.Ascending = false;
                this.OrderByFieldName = FieldOrPropertyName;
                this.OrderByPrimaryKey = false;
                return this;
            }
        }

        /// <summary>
        /// use column data to fill in the return TValue object. 
        /// The TValue object must have a parameterless constructor. 
        /// </summary>
        public Filter<TKey, TValue> UseColumnData()
        {
            this.ColumnDataOnly = true;
            return this;
        }


        public Filter<TKey, TValue> Skip(int count)
        {
            this.SkipCount = count;
            return this;
        }

        public TValue FirstOrDefault()
        {
            List<TValue> x = Take(1);

            if (x != null && x.Count() > 0)
            {
                return x[0];
            }
            else
            {
                return default(TValue);
            }
        }

        /// <summary>
        /// check whether the filter condition match any record or not. 
        /// </summary>
        /// <returns></returns>
        public bool Exists()
        {
            List<TValue> values = Take(1);

            if (values == null || values.Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        public List<TValue> Take(int count)
        {
            List<TValue> listvalue = new List<TValue>();

            List<long> list = GetList(count);

            if (this.ColumnDataOnly)
            {
                foreach (var item in list)
                {
                    TValue record = this.store.getValueFromColumns(item);
                    listvalue.Add(record);
                }
            }
            else
            {
                foreach (var item in list)
                {
                    TValue record = this.store.getValue(item);
                    listvalue.Add(record);
                }
            }

            return listvalue;
        }


        internal List<long> GetList(int count)
        {
            lock (this.store._Locker)
            {
                int skipped = 0;
                int taken = 0;

                ExecutionPlan executionplan = FilterParser<TKey, TValue>.GetExecutionPlan(this);

                List<List<long>> rangelist = new List<List<long>>();

                List<long> returnlist = new List<long>();

                foreach (var item in executionplan.indexRanges)
                {

                    /// check if the primary key is included. 

                    List<long> blockpositionList = new List<long>();

                    ItemCollection collection = this.store.Indexes.getIndex(item.Key).GetCollection(item.Value.lower, item.Value.upper, item.Value.lowerOpen, item.Value.upperOpen, this.Ascending);

                    foreach (Int64 position in collection)
                    {
                        blockpositionList.Add(position);
                    }

                    rangelist.Add(blockpositionList);
                }

                bool itemMatch = true;

                foreach (Int64 item in executionplan.startCollection)
                {
                    /// check matches. 
                    itemMatch = true;
                    foreach (List<long> rangeitem in rangelist)
                    {
                        if (!rangeitem.Contains(item))
                        {
                            itemMatch = false;
                            break;
                        }
                    }

                    if (!itemMatch)
                    {
                        continue;
                    }

                    /// check column matchs. 
                    foreach (ColumnScan plan in executionplan.scanColumns)
                    {
                        byte[] columnbytes = this.store.getColumnsBytes(item, plan.relativeStartPosition, plan.length);

                        if (!plan.Evaluator.isMatch(columnbytes))
                        {
                            itemMatch = false;
                            break;
                        }

                    }

                    if (!itemMatch)
                    {
                        continue;
                    }

                    if (skipped < this.SkipCount)
                    {
                        skipped += 1;
                        continue;
                    }

                    returnlist.Add(item);

                    taken += 1;

                    if (taken >= count)
                    {
                        return returnlist;
                    }
                }

                executionplan = null;

                return returnlist;
            }
        }


        /// <summary>
        /// select all records, default limit to max 500 records return.
        /// to return more than 500 records, consider using Take(int) with a bigger enough count int.
        /// </summary>
        /// <returns></returns>
        public List<TValue> SelectAll()
        {
            ///default limits to 5000 records. 
            return Take(5000);
        }



        public int Count()
        {
            lock (this.store._Locker)
            {

                int skipped = 0;
                int taken = 0;

                ExecutionPlan executionplan = FilterParser<TKey, TValue>.GetExecutionPlan(this);

                List<List<long>> rangelist = new List<List<long>>();
                List<TValue> returnlist = new List<TValue>();

                foreach (var item in executionplan.indexRanges)
                {

                    /// check if the primary key is included. 

                    List<long> blockpositionList = new List<long>();

                    ItemCollection collection = this.store.Indexes.getIndex(item.Key).GetCollection(item.Value.lower, item.Value.upper, item.Value.lowerOpen, item.Value.upperOpen, this.Ascending);

                    foreach (Int64 position in collection)
                    {
                        blockpositionList.Add(position);
                    }

                    rangelist.Add(blockpositionList);
                }

                bool itemMatch = true;

                foreach (Int64 item in executionplan.startCollection)
                {
                    /// check matches. 
                    itemMatch = true;
                    foreach (List<long> rangeitem in rangelist)
                    {
                        if (!rangeitem.Contains(item))
                        {
                            itemMatch = false;
                            break;
                        }
                    }

                    if (!itemMatch)
                    {
                        continue;
                    }


                    /// check column matchs. 
                    foreach (ColumnScan plan in executionplan.scanColumns)
                    {
                        byte[] columnbytes = this.store.getColumnsBytes(item, plan.relativeStartPosition, plan.length);

                        if (!plan.Evaluator.isMatch(columnbytes))
                        {
                            itemMatch = false;
                            break;
                        }

                    }

                    if (!itemMatch)
                    {
                        continue;
                    }

                    /// pass all tests. 

                    if (skipped < this.SkipCount)
                    {
                        skipped += 1;
                        continue;
                    }


                    taken += 1;

                }

                executionplan = null;

                return taken;
            }
        }


    }
}
