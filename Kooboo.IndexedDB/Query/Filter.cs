//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Kooboo.IndexedDB.ByteConverter;
using Kooboo.IndexedDB.Columns;
using Kooboo.IndexedDB.Condition.Expression;

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
        private bool _columnDataOnly;

        internal readonly ObjectStore<TKey, TValue> store;

        public Node Node { get; private set; }

        public readonly Dictionary<string, List<byte[]>> InItems = new();

        public readonly List<MethodCallExpression> calls = new();

        public Filter(ObjectStore<TKey, TValue> store)
        {
            this.store = store;
            SkipCount = 0;
            OrderByPrimaryKey = false;
            Ascending = true;
            _columnDataOnly = false;
        }

        public Filter<TKey, TValue> Where(string fieldOrPropertyName, Comparer comparer, object compareValue)
        {
            var item = new FilterNode
            {
                Value = new ValueNode(null, false),
                Comparer = comparer,
                Property = fieldOrPropertyName
            };

            // check to see if there is in the column. 
            var column = store.GetColumn(fieldOrPropertyName);

            if (column != null)
            {
                item.Value.Type = column.DataType;
                item.Value.Length = column.Length;
            }
            else
            {
                var index = store.Indexes.getIndex(fieldOrPropertyName);

                if (index != null)
                {
                    item.Value.Type = index.keyType;
                    item.Value.Length = index.Length;
                }
                else
                {
                    if (fieldOrPropertyName.IsSameValue(store.StoreSetting.PrimaryKey))
                    {
                        var keyType = Helper.TypeHelper.GetFieldType(store.StoreSetting.ValueType,
                            store.StoreSetting.PrimaryKey);

                        item.Value.Type = keyType;
                        item.Value.Length = store.StoreSetting.PrimaryKeyLen;
                    }
                }
            }

            if (item.Value.Type == null || item.Value.Length < 1)
            {
                throw new Exception(
                    "only index or column are allowed in the where condition, consider adding the field into columns in order to search");
            }

            var byteValue = getByteValue(item.Value.Type, compareValue);

            if (byteValue == null) return this;
            item.Value.ValueBytes = byteValue;

            //bool and guid, there is not > < =, only equal or not equal. 
            if (item.Value.Type == typeof(bool) || item.Value.Type == typeof(Guid))
            {
                if (item.Comparer != Comparer.EqualTo && item.Comparer != Comparer.NotEqualTo)
                {
                    item.Comparer = Comparer.EqualTo;
                }
            }

            //date time, must have specify comare till min, second, millsecond, etc. 
            if (item.Value.Type == typeof(DateTime))
            {
                if (item.Value.TimeScope == default(DateTimeScope))
                {
                    item.Value.TimeScope = DateTimeScope.day;
                }
            }

            Node = Node.And(Node, item);

            return this;
        }

        public Filter<TKey, TValue> WhereEqual(string name, bool value)
        {
            var filter = new FilterNode
            {
                Comparer = Comparer.EqualTo,
                Property = name,
                Value = new ValueNode(null, false)
                {
                    Type = typeof(bool),
                    Length = 1,
                    ValueBytes = new[] { (byte)(value ? 1 : 0) }
                }
            };

            Node = Node.And(Node, filter);
            return this;
        }

        public Filter<TKey, TValue> WhereIn(string fieldOrPropertyName, List<object> values)
        {
            var column = store.GetColumn(fieldOrPropertyName);

            if (column == null)
            {
                throw new Exception("Only Fields defined in Column are allowed to use In query");
            }

            var invalues = new List<byte[]>();

            foreach (var item in values)
            {
                byte[] value = getByteValue(column.DataType, item);
                invalues.Add(value);
            }

            InItems[fieldOrPropertyName] = invalues;

            return this;
        }

        public Filter<TKey, TValue> MethodCall(MethodCallExpression call)
        {
            calls.Add(call);
            return this;
        }

        public Filter<TKey, TValue> WhereIn<Type>(string FieldOrPropertyName, List<Type> Values)
        {
            IColumn<TValue> column = store.GetColumn(FieldOrPropertyName);

            if (column == null)
            {
                throw new Exception("Only Fields defined in Column are allowed to use In query");
            }

            List<byte[]> inValues = new List<byte[]>();

            foreach (var item in Values)
            {
                byte[] value = getByteValue(column.DataType, item);
                inValues.Add(value);
            }

            InItems[FieldOrPropertyName] = inValues;

            return this;
        }

        public Filter<TKey, TValue> WhereIn<T>(Expression<Func<TValue, object>> fieldExpression, List<T> values)
        {
            string fieldName = Helper.ExpressionHelper.GetFieldName<TValue>(fieldExpression);

            if (!string.IsNullOrEmpty(fieldName))
            {
                WhereIn<T>(fieldName, values);
            }

            return this;
        }


        public Filter<TKey, TValue> WhereEqual(string name, Guid value)
        {
            var filter = new FilterNode
            {
                Comparer = Comparer.EqualTo,
                Property = name,
                Value = new ValueNode(null, false)
                {
                    Type = typeof(Guid),
                    Length = 16,
                    ValueBytes = ObjectContainer.GetConverter<Guid>().ToByte(value)
                }
            };

            Node = Node.And(Node, filter);
            return this;
        }

        public Filter<TKey, TValue> WhereEqual(string fieldOrPropertyName, object value)
        {
            return Where(fieldOrPropertyName, Comparer.EqualTo, value);
        }

        public Filter<TKey, TValue> Where(string fieldOrPropertyName, Comparer comparer, DateTime compareValue,
            DateTimeScope scope)
        {
            var filter = new FilterNode
            {
                Comparer = comparer,
                Property = fieldOrPropertyName,
                Value = new ValueNode(null, false)
                {
                    TimeScope = scope,
                    Type = typeof(DateTime),
                    Length = 8,
                    ValueBytes = ObjectContainer.GetConverter<DateTime>().ToByte(compareValue)
                }
            };

            Node = Node.And(Node, filter);
            return this;
        }

        /// <summary>
        /// Filter based on the object primary key.
        /// </summary>
        /// <param name="FieldOrPropertyName"></param>
        /// <param name="comparer"></param>
        /// <param name="compareValue"></param>
        /// <returns></returns>
        public Filter<TKey, TValue> WhereKey(Comparer comparer, object compareValue)
        {
            var bytes = getByteValue(typeof(TKey), compareValue);
            if (bytes == null) return this;

            var filter = new FilterNode
            {
                Comparer = comparer,
                Property = store.StoreSetting.PrimaryKey,
                Value = new ValueNode(null, false)
                {
                    Type = typeof(TKey),
                    Length = store.StoreSetting.PrimaryKeyLen,
                    ValueBytes = bytes
                }
            };

            Node = Node.And(Node, filter);
            return this;
        }

        private byte[] getByteValue(Type datatype, object objectValue)
        {
            if (datatype == typeof(Int32))
            {
                int value = Convert.ToInt32(objectValue);
                IByteConverter<Int32> x = ObjectContainer.GetConverter<Int32>();
                return x.ToByte(value);
            }
            else if (datatype == typeof(Int16))
            {
                Int16 value = Convert.ToInt16(objectValue);
                IByteConverter<Int16> x = ObjectContainer.GetConverter<Int16>();
                return x.ToByte(value);
            }
            else if (datatype == typeof(string))
            {
                string value = Convert.ToString(objectValue);
                IByteConverter<string> x = ObjectContainer.GetConverter<string>();

                return x.ToByte(value);
            }
            else if (datatype == typeof(Int64))
            {
                Int64 value = Convert.ToInt64(objectValue);
                IByteConverter<Int64> x = ObjectContainer.GetConverter<Int64>();
                return x.ToByte(value);
            }
            else if (datatype == typeof(float))
            {
                float value = Convert.ToSingle(objectValue);
                IByteConverter<float> x = ObjectContainer.GetConverter<float>();
                return x.ToByte(value);
            }
            else if (datatype == typeof(double))
            {
                double value = Convert.ToDouble(objectValue);
                IByteConverter<double> x = ObjectContainer.GetConverter<double>();
                return x.ToByte(value);
            }
            else if (datatype == typeof(decimal))
            {
                double value = Convert.ToDouble(objectValue);
                IByteConverter<double> x = ObjectContainer.GetConverter<double>();
                return x.ToByte(value);
            }

            else if (datatype == typeof(Guid))
            {
                Guid value = Guid.Parse(Convert.ToString(objectValue));

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
                bool value = Convert.ToBoolean(objectValue);

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
                DateTime datevalue = (DateTime)objectValue;

                IByteConverter<DateTime> x = ObjectContainer.GetConverter<DateTime>();
                return x.ToByte(datevalue);
            }

            else if (datatype == typeof(byte))
            {
                byte[] valuebyte = new byte[1];
                valuebyte[0] = Convert.ToByte(objectValue);

                return valuebyte;
            }

            else if (datatype.IsEnum)
            {
                int intvalue = (int)objectValue;
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
            Ascending = true;
            OrderByPrimaryKey = true;
            OrderByFieldName = string.Empty;
            return this;
        }

        /// <summary>
        /// Order by a field or property. This field should have an index on it. 
        /// Order by a non-indexed field will have very bad performance. 
        /// </summary>
        public Filter<TKey, TValue> OrderByAscending(string FieldOrPropertyName)
        {
            if (FieldOrPropertyName.IsSameValue(store.StoreSetting.PrimaryKey))
            {
                return OrderByAscending();
            }
            else
            {
                Ascending = true;
                OrderByFieldName = FieldOrPropertyName;
                OrderByPrimaryKey = false;
                return this;
            }
        }

        /// <summary>
        /// Order by descending based on the primary key.
        /// </summary>
        public Filter<TKey, TValue> OrderByDescending()
        {
            Ascending = false;
            OrderByPrimaryKey = true;
            OrderByFieldName = string.Empty;
            return this;
        }

        /// <summary>
        /// Order by descending on a field or property. This field should have an index on it. 
        /// </summary>
        public Filter<TKey, TValue> OrderByDescending(string FieldOrPropertyName)
        {
            if (FieldOrPropertyName.IsSameValue(store.StoreSetting.PrimaryKey))
            {
                return OrderByDescending();
            }
            else
            {
                Ascending = false;
                OrderByFieldName = FieldOrPropertyName;
                OrderByPrimaryKey = false;
                return this;
            }
        }

        /// <summary>
        /// use column data to fill in the return TValue object. 
        /// The TValue object must have a parameterless constructor. 
        /// </summary>
        public Filter<TKey, TValue> UseColumnData()
        {
            _columnDataOnly = true;
            return this;
        }


        public Filter<TKey, TValue> Skip(int count)
        {
            SkipCount = count;
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

            if (_columnDataOnly)
            {
                foreach (var item in list)
                {
                    TValue record = store.getValueFromColumns(item);
                    listvalue.Add(record);
                }
            }
            else
            {
                foreach (var item in list)
                {
                    TValue record = store.getValue(item);
                    listvalue.Add(record);
                }
            }

            return listvalue;
        }


        internal List<long> GetList(int count)
        {
            lock (store._Locker)
            {
                int skipped = 0;
                int taken = 0;

                ExecutionPlan executionplan = FilterParser<TKey, TValue>.GetExecutionPlan(this);

                List<List<long>> rangelist = new List<List<long>>();

                List<long> returnlist = new List<long>();

                foreach (var item in executionplan.IndexRanges)
                {
                    /// check if the primary key is included. 
                    var collection = item.Value.SelectMany(s => store.Indexes.getIndex(item.Key).GetCollection(
                        s.lower,
                        s.upper,
                        s.lowerOpen,
                        s.upperOpen,
                        Ascending)
                    );

                    rangelist.Add(collection.ToList());
                }

                bool itemMatch = true;

                foreach (Int64 item in executionplan.StartCollection)
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
                    if (executionplan.ColumnScanner != null &&
                        !executionplan.ColumnScanner.Match(item, store.getColumnsBytes))
                    {
                        continue;
                    }

                    if (skipped < SkipCount)
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
            return Take(99999);
        }


        public int Count()
        {
            lock (store._Locker)
            {
                int skipped = 0;
                int taken = 0;

                var executionPlan = FilterParser<TKey, TValue>.GetExecutionPlan(this);
                var list = new List<List<long>>();

                foreach (var item in executionPlan.IndexRanges)
                {
                    /// check if the primary key is included. 
                    var collection = item.Value.SelectMany(s => store.Indexes.getIndex(item.Key).GetCollection(
                        s.lower,
                        s.upper,
                        s.lowerOpen,
                        s.upperOpen,
                        Ascending)
                    );

                    list.Add(collection.ToList());
                }

                bool itemMatch = true;

                foreach (Int64 item in executionPlan.StartCollection)
                {
                    /// check matches. 
                    itemMatch = true;
                    foreach (List<long> rangeitem in list)
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
                    if (executionPlan.ColumnScanner != null &&
                        !executionPlan.ColumnScanner.Match(item, store.getColumnsBytes))
                    {
                        continue;
                    }

                    /// pass all tests. 

                    if (skipped < SkipCount)
                    {
                        skipped += 1;
                        continue;
                    }


                    taken += 1;
                }

                executionPlan = null;

                return taken;
            }
        }
    }
}