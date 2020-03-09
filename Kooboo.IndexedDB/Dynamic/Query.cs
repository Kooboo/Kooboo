//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.IndexedDB.Btree;
using Kooboo.IndexedDB.ByteConverter;
using Kooboo.IndexedDB.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Kooboo.IndexedDB.Dynamic
{
    public class Query
    {
        public int SkipCount;
        public bool Ascending;

        public string OrderByFieldName { get; set; }

        internal bool OrderByPrimaryKey;

        internal Table table { get; set; }

        public List<FilterItem> items = new List<FilterItem>();

        public Dictionary<string, List<byte[]>> InItems = new Dictionary<string, List<byte[]>>();

        public List<MethodCallExpression> calls = new List<MethodCallExpression>();

        public Query(Table table)
        {
            this.table = table;
            this.SkipCount = 0;
            this.Ascending = true;
        }

        public Query Where(string FieldOrPropertyName, Comparer comparer, object CompareValue)
        {
            FilterItem item = new FilterItem();

            var field = this.table.ObjectConverter.Fields.Find(o => o.FieldName == FieldOrPropertyName);

            if (field != null)
            {
                item.FieldType = field.ClrType;
                item.Length = field.Length;
            }
            else
            {
                string message = "only column are allowed in the where condition, consider adding the field into columns in order to search, table: " + this.table.Name +", field: " + FieldOrPropertyName;
                message += "\r\ncurrent fields:";
                foreach (var f in this.table.ObjectConverter.Fields)
                {
                    message += f.FieldName;
                }

                throw new Exception(message);
            }

            item.Compare = comparer;

            item.FieldOrProperty = FieldOrPropertyName;

            byte[] bytevalue = field.ToBytes(CompareValue);

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

        public Query WhereEqual(string FieldOrPropertyName, bool Value)
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

        public Query WhereIn(string FieldOrPropertyName, List<object> Values)
        {
            var field = this.table.ObjectConverter.Fields.Find(o => o.FieldName == FieldOrPropertyName);

            if (field == null)
            {
                throw new Exception("Only Fields defined in Column are allowed to use In query");
            }

            List<byte[]> invalues = new List<byte[]>();

            foreach (var item in Values)
            {
                byte[] value = field.ToBytes(item);
                invalues.Add(value);
            }

            this.InItems[FieldOrPropertyName] = invalues;

            return this;
        }

        private Query MethodCall(MethodCallExpression call)
        {
            this.calls.Add(call);
            return this;
        }

        public Query WhereIn<Type>(string FieldOrPropertyName, List<Type> Values)
        {
            var field = this.table.ObjectConverter.Fields.Find(o => o.FieldName == FieldOrPropertyName);

            if (field == null)
            {
                throw new Exception("Only Fields defined in Column are allowed to use In query");
            }

            List<byte[]> invalues = new List<byte[]>();

            foreach (var item in Values)
            {
                byte[] value = field.ToBytes(item);
                invalues.Add(value);
            }

            this.InItems[FieldOrPropertyName] = invalues;

            return this;
        }

        public Query WhereIn<TValue, T>(Expression<Func<TValue, object>> FieldExpression, List<T> Values)
        {
            string fieldname = Helper.ExpressionHelper.GetFieldName<TValue>(FieldExpression);

            if (!string.IsNullOrEmpty(fieldname))
            {
                this.WhereIn<T>(fieldname, Values);
            }
            return this;
        }


        public Query WhereEqual(string FieldOrPropertyName, Guid Value)
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

        public Query WhereEqual(string FieldOrPropertyName, object Value)
        {
            return Where(FieldOrPropertyName, Comparer.EqualTo, Value);
        }

        public Query Where(string FieldOrPropertyName, Comparer comparer, DateTime CompareValue, DateTimeScope scope)
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


        private string _primarykey;
        private string PrimaryKey
        {
            get
            {
                if (_primarykey == null)
                {
                    var key = this.table.Setting.Columns.FirstOrDefault(o => o.IsPrimaryKey);
                    _primarykey = key.Name;
                }
                return _primarykey;
            }

        }

        public Query OrderByAscending()
        {
            this.Ascending = true;
            this.OrderByFieldName = this.PrimaryKey;
            return this;
        }

        /// <summary>
        /// Order by a field or property. This field should have an index on it. 
        /// Order by a non-indexed field will have very bad performance. 
        /// </summary>
        public Query OrderByAscending(string FieldOrPropertyName)
        {
            this.Ascending = true;
            this.OrderByFieldName = FieldOrPropertyName;
            return this;
        }

        public Query OrderByDescending()
        {
            this.Ascending = false;
            this.OrderByFieldName = this.PrimaryKey;
            return this;
        }

        public Query OrderByDescending(string FieldOrPropertyName)
        {
            this.Ascending = false;
            this.OrderByFieldName = FieldOrPropertyName;
            return this;

        }

        public Query Skip(int count)
        {
            this.SkipCount = count;
            return this;
        }

        public IDictionary<string, object> FirstOrDefault()
        {
            List<IDictionary<string, object>> x = Take(1);

            if (x != null && x.Count() > 0)
            {
                return x[0];
            }
            else
            {
                return null;
            }
        }

        public T FirstOrDefault<T>()
        {
            var x = Take<T>(1);

            if (x != null && x.Count() > 0)
            {
                return x[0];
            }
            else
            {
                return default(T);
            }
        }

        /// <summary>
        /// check whether the filter condition match any record or not. 
        /// </summary>
        /// <returns></returns>
        public bool Exists()
        {
            var values = Take(1);

            if (values == null || values.Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public List<IDictionary<string, object>> FindAll(string searchtext)
        {
            var filter = ParserFilter(searchtext);
            this.items = filter;
            return Take(5000);
        }

        public List<T> FindAll<T>(string searchtext)
        {
            var filter = ParserFilter(searchtext);
            this.items = filter;
            return Take<T>(5000);
        }

        public IDictionary<string, object> Find(string searchtext)
        {
            var filter = ParserFilter(searchtext);
            this.items = filter;
            return this.FirstOrDefault();
        }

        public T Find<T>(string searchtext)
        {
            var filter = ParserFilter(searchtext);
            this.items = filter;
            return this.FirstOrDefault<T>();
        }

        public List<FilterItem> ParserFilter(string conditiontext)
        {
            var conditions = QueryPraser.ParseConditoin(conditiontext);

            if (conditions == null || conditions.Count() == 0)
            {
                return null;
            };

            List<FilterItem> result = new List<FilterItem>();
            foreach (var item in conditions)
            {
                var col = this.table.ObjectConverter.Fields.Find(o => o.FieldName == item.Field);
                if (col != null)
                {
                    FilterItem filter = new FilterItem() { FieldOrProperty = col.FieldName };
                    filter.Compare = item.Comparer;
                    if (!col.ClrType.IsValueType && col.ClrType != typeof(string))
                    {
                        throw new Exception("only value type column are allowed to use on search condition");
                    }
                    var rightvalue = Convert.ChangeType(item.Value, col.ClrType);

                    // For datetime col, need to have something different. 
                    if (rightvalue != null)
                    {
                        filter.Value = col.ToBytes(rightvalue);
                        result.Add(filter);
                    }
                }
            }
            return result;
        }

        public List<IDictionary<string, object>> Take(int count)
        {
            ExecutionPlan executionplan = QueryPraser.GetExecutionPlan(this);

            List<long> list = GetList(executionplan, count);

            List<IDictionary<string, object>> listvalue = new List<IDictionary<string, object>>();
            foreach (var item in list)
            {
                var record = this.table._getvalue(item);
                listvalue.Add(record);
            }

            if (!executionplan.RequireOrderBy)
            {
                return listvalue;
            }
            else
            {
                if (!string.IsNullOrEmpty(this.OrderByFieldName) && this.table.Setting.Columns.Any(o => o.Name == this.OrderByFieldName))
                {
                    var col = this.table.Setting.Columns.First(o => o.Name == this.OrderByFieldName);

                    if (col != null)
                    {
                        if (this.Ascending)
                        {
                            return listvalue.OrderBy(o => GetValue(o, this.OrderByFieldName, col.ClrType)).Skip(this.SkipCount).Take(count).ToList();
                        }
                        else
                        {
                            return listvalue.OrderByDescending(o => GetValue(o, this.OrderByFieldName, col.ClrType)).Skip(this.SkipCount).Take(count).ToList();
                        }

                    }

                }

                return listvalue.Skip(this.SkipCount).Take(count).ToList();
            }
        }

        public List<T> Take<T>(int count)
        {
            List<T> result = new List<T>();

            var dictvalues = this.Take(count);

            foreach (var dict in dictvalues)
            {
                var returnobj = Activator.CreateInstance<T>();

                var type = typeof(T);
                var cls = Activator.CreateInstance<T>();

                foreach (var item in dict)
                {
                    Accessor.GetSetter(type, item.Key)?.Invoke(cls, item.Value);
                }

                result.Add(cls);
            }
            return result;
        }


        private object GetValue(IDictionary<string, object> obj, string fieldName, Type datatype)
        {
            object value = null;
            if (obj.ContainsKey(fieldName))
            {
                value = obj[fieldName];
            }

            if (value == null)
            {
                return Kooboo.IndexedDB.Dynamic.IndexHelper.DefaultValue(datatype);
            }

            else
            {
                return Dynamic.Accessor.ChangeType(value, datatype);
            }
        }



        internal List<long> GetList(int count)
        {
            lock (this.table._Locker)
            {
                int skipped = 0;
                int taken = 0;

                ExecutionPlan executionplan = QueryPraser.GetExecutionPlan(this);

                List<List<long>> rangelist = new List<List<long>>();

                List<long> returnlist = new List<long>();

                foreach (var item in executionplan.indexRanges)
                {
                    /// check if the primary key is included.  
                    List<long> blockpositionList = new List<long>();

                    var index = this.table.Indexs.Find(o => o.FieldName == item.Key);

                    ItemCollection collection = index.GetCollection(item.Value.lower, item.Value.upper, item.Value.lowerOpen, item.Value.upperOpen, this.Ascending);

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
                        byte[] columnbytes = this.table.BlockFile.GetCol(item, plan.relativeStartPosition, plan.length);

                        if (columnbytes == null || !plan.Evaluator.isMatch(columnbytes))
                        {
                            itemMatch = false;
                            break;
                        }
                    }

                    if (!itemMatch)
                    {
                        continue;
                    }

                    if (executionplan.RequireOrderBy)
                    {


                    }
                    else
                    {

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
                }

                executionplan = null;

                return returnlist;
            }
        }

        internal List<long> GetList(ExecutionPlan executionplan, int count)
        {
            lock (this.table._Locker)
            {
                int skipped = 0;
                int taken = 0;

                List<List<long>> rangelist = new List<List<long>>();

                List<long> returnlist = new List<long>();

                foreach (var item in executionplan.indexRanges)
                {
                    /// check if the primary key is included.  
                    List<long> blockpositionList = new List<long>();

                    var index = this.table.Indexs.Find(o => o.FieldName == item.Key);

                    ItemCollection collection = index.GetCollection(item.Value.lower, item.Value.upper, item.Value.lowerOpen, item.Value.upperOpen, this.Ascending);

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
                        byte[] columnbytes = this.table.BlockFile.GetCol(item, plan.relativeStartPosition, plan.length);

                        if (columnbytes == null || !plan.Evaluator.isMatch(columnbytes))
                        {
                            itemMatch = false;
                            break;
                        }
                    }

                    if (!itemMatch)
                    {
                        continue;
                    }

                    if (executionplan.RequireOrderBy)
                    {

                        returnlist.Add(item);
                    }
                    else
                    {

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
                }

                return returnlist;
            }
        }


        public List<IDictionary<string, object>> SelectAll()
        {
            return Take(5000);
        }

        public List<T> SelectAll<T>()
        {
            return Take<T>(5000);
        }


        public int Count()
        {
            lock (this.table._Locker)
            {

                int skipped = 0;
                int taken = 0;

                ExecutionPlan executionplan = QueryPraser.GetExecutionPlan(this);

                List<List<long>> rangelist = new List<List<long>>();

                foreach (var item in executionplan.indexRanges)
                {
                    List<long> blockpositionList = new List<long>();
                    var index = this.table.Indexs.Find(o => o.FieldName == item.Key);
                    ItemCollection collection = index.GetCollection(item.Value.lower, item.Value.upper, item.Value.lowerOpen, item.Value.upperOpen, this.Ascending);

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
                        byte[] columnbytes = this.table.BlockFile.GetCol(item, plan.relativeStartPosition, plan.length);

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
