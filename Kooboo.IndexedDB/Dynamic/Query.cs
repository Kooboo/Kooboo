//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Kooboo.IndexedDB.Condition;
using Kooboo.IndexedDB.Condition.Expression;
using Kooboo.IndexedDB.Query;

namespace Kooboo.IndexedDB.Dynamic
{
    public class Query
    {
        public int SkipCount;
        public bool Ascending;

        public string OrderByFieldName { get; set; }

        internal bool OrderByPrimaryKey;

        internal Table table { get; set; }

        public Node Node { get; set; }

        public Dictionary<string, List<byte[]>> InItems = new Dictionary<string, List<byte[]>>();

        public List<MethodCallExpression> calls = new List<MethodCallExpression>();

        public Query(Table table)
        {
            this.table = table;
            SkipCount = 0;
            Ascending = true;
        }

        public Query Where(string FieldOrPropertyName, Comparer comparer, object CompareValue)
        {
            var field = table.ObjectConverter.Fields.Find(o => o.FieldName == FieldOrPropertyName);

            if (field == null)
            {
                string message = $"only column are allowed in the where condition, consider adding the field into columns in order to search, table: {table.Name}, field: {FieldOrPropertyName}\r\ncurrent fields: {string.Join(',', table.ObjectConverter.Fields.Select(s => s.FieldName))}";

                throw new Exception(message);
            }

            var filter = new FilterNode
            {
                Value = new ValueNode(null, false)
                {
                    Type = field.DataType,
                    Length = field.Length
                },
                Comparer = comparer,
                Property = FieldOrPropertyName
            };

            var bytes = field.ToBytes(CompareValue);
            if (bytes == null) return this;
            filter.Value.ValueBytes = bytes;

            //bool and guid, there is not > < =, only equal or not equal. 
            if (filter.Value.Type == typeof(bool) || filter.Value.Type == typeof(Guid))
            {
                if (filter.Comparer != Comparer.EqualTo && filter.Comparer != Comparer.NotEqualTo)
                {
                    filter.Comparer = Comparer.EqualTo;
                }
            }

            //date time, must have specify comare till min, second, millsecond, etc. 
            if (filter.Value.Type == typeof(DateTime))
            {
                if (filter.Value.TimeScope == default)
                {
                    filter.Value.TimeScope = DateTimeScope.day;
                }
            }

            Node = Node.And(Node, filter);
            return this;
        }

        public Query WhereEqual(string FieldOrPropertyName, bool Value)
        {
            var filter = new FilterNode
            {
                Comparer = Comparer.EqualTo,
                Property = FieldOrPropertyName,
                Value = new ValueNode(null, false)
                {
                    Type = typeof(bool),
                    Length = 1,
                    ValueBytes = new byte[] { (byte)(Value ? 1 : 0) }
                }
            };

            Node = Node.And(Node, filter);
            return this;
        }

        public Query WhereIn(string FieldOrPropertyName, List<object> Values)
        {
            var field = table.ObjectConverter.Fields.Find(o => o.FieldName == FieldOrPropertyName);

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

            InItems[FieldOrPropertyName] = invalues;

            return this;
        }

        private Query MethodCall(MethodCallExpression call)
        {
            calls.Add(call);
            return this;
        }

        public Query WhereIn<Type>(string FieldOrPropertyName, List<Type> Values)
        {
            var field = table.ObjectConverter.Fields.Find(o => o.FieldName == FieldOrPropertyName);

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

            InItems[FieldOrPropertyName] = invalues;

            return this;
        }

        public Query WhereIn<TValue, T>(Expression<Func<TValue, object>> FieldExpression, List<T> Values)
        {
            string fieldname = Helper.ExpressionHelper.GetFieldName<TValue>(FieldExpression);

            if (!string.IsNullOrEmpty(fieldname))
            {
                WhereIn<T>(fieldname, Values);
            }

            return this;
        }


        public Query WhereEqual(string FieldOrPropertyName, Guid Value)
        {
            var filter = new FilterNode
            {
                Comparer = Comparer.EqualTo,
                Property = FieldOrPropertyName,
                Value = new ValueNode(null, false)
                {
                    Type = typeof(Guid),
                    Length = 16,
                    ValueBytes = ObjectContainer.GetConverter<Guid>().ToByte(Value)
                }
            };

            Node = Node.And(Node, filter);
            return this;
        }

        public Query WhereEqual(string FieldOrPropertyName, object Value)
        {
            return Where(FieldOrPropertyName, Comparer.EqualTo, Value);
        }

        public Query Where(string FieldOrPropertyName, Comparer comparer, DateTime CompareValue, DateTimeScope scope)
        {
            var filter = new FilterNode
            {
                Comparer = comparer,
                Property = FieldOrPropertyName,
                Value = new ValueNode(null, false)
                {
                    TimeScope = scope,
                    Type = typeof(DateTime),
                    Length = 8,
                    Value = ObjectContainer.GetConverter<DateTime>().ToByte(CompareValue)
                }
            };

            Node = Node.And(Node, filter);
            return this;
        }


        private string _primarykey;

        private string PrimaryKey
        {
            get
            {
                if (_primarykey == null)
                {
                    var key = table.Setting.Columns.FirstOrDefault(o => o.IsPrimaryKey);
                    _primarykey = key.Name;
                }

                return _primarykey;
            }
        }

        public Query OrderByAscending()
        {
            Ascending = true;
            OrderByFieldName = PrimaryKey;
            return this;
        }

        /// <summary>
        /// Order by a field or property. This field should have an index on it. 
        /// Order by a non-indexed field will have very bad performance. 
        /// </summary>
        public Query OrderByAscending(string FieldOrPropertyName)
        {
            Ascending = true;
            OrderByFieldName = FieldOrPropertyName;
            return this;
        }

        public Query OrderByDescending()
        {
            Ascending = false;
            OrderByFieldName = PrimaryKey;
            return this;
        }

        public Query OrderByDescending(string FieldOrPropertyName)
        {
            Ascending = false;
            OrderByFieldName = FieldOrPropertyName;
            return this;
        }

        public Query Skip(int count)
        {
            SkipCount = count;
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
            var expression = Parser.Parse(searchtext);
            return FindAll(expression);
        }

        public List<IDictionary<string, object>> FindAll(Node node)
        {
            ValueNodeHandle(node);
            Node = node;
            return Take(5000);
        }

        public List<T> FindAll<T>(string searchtext)
        {
            Node = ParserFilter(searchtext);
            return Take<T>(5000);
        }

        public IDictionary<string, object> Find(string searchtext)
        {
            var expression = Parser.Parse(searchtext);
            return Find(expression);
        }

        public IDictionary<string, object> Find(Node node)
        {
            ValueNodeHandle(node);
            Node = node;
            return FirstOrDefault();
        }

        public T Find<T>(string searchtext)
        {
            Node = ParserFilter(searchtext);
            return FirstOrDefault<T>();
        }

        public Node ParserFilter(string conditiontext)
        {
            var expression = Parser.Parse(conditiontext);
            if (expression == null) return null;
            ValueNodeHandle(expression);
            return expression;
        }

        public void ValueNodeHandle(Node expression)
        {
            var filterExpressions = expression.GetNodes().Where(w => w is FilterNode);

            foreach (FilterNode item in filterExpressions)
            {
                var col = table.ObjectConverter.Fields.Find(o => o.FieldName == item.Property);
                if (col == null || item.Value == null) continue;

                if (!col.DataType.IsValueType && col.DataType != typeof(string))
                {
                    throw new Exception("only value type column are allowed to use on search condition");
                }

                var value = Convert.ChangeType(item.Value.Value, col.DataType);
                item.Value.ValueBytes = col.ToBytes(value);
                item.Value.Type = col.DataType;
                item.Value.Length = col.Length;
            }
        }

        public List<IDictionary<string, object>> Take(int count)
        {
            ExecutionPlan executionplan = QueryPraser.GetExecutionPlan(this);

            List<long> list = GetList(executionplan, count);

            List<IDictionary<string, object>> listvalue = new List<IDictionary<string, object>>();
            foreach (var item in list)
            {
                var record = table._getvalue(item);
                listvalue.Add(record);
            }

            if (!executionplan.RequireOrderBy)
            {
                return listvalue;
            }
            else
            {
                if (!string.IsNullOrEmpty(OrderByFieldName) &&
                    table.Setting.Columns.Any(o => o.Name == OrderByFieldName))
                {
                    var col = table.Setting.Columns.First(o => o.Name == OrderByFieldName);

                    if (col != null)
                    {
                        if (Ascending)
                        {
                            return listvalue.OrderBy(o => GetValue(o, OrderByFieldName, col.ClrType))
                                .Skip(SkipCount).Take(count).ToList();
                        }
                        else
                        {
                            return listvalue.OrderByDescending(o => GetValue(o, OrderByFieldName, col.ClrType))
                                .Skip(SkipCount).Take(count).ToList();
                        }
                    }
                }

                return listvalue.Skip(SkipCount).Take(count).ToList();
            }
        }

        public List<T> Take<T>(int count)
        {
            List<T> result = new List<T>();

            var dictvalues = Take(count);

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
                return IndexHelper.DefaultValue(datatype);
            }

            else
            {
                return Accessor.ChangeType(value, datatype);
            }
        }


        internal List<long> GetList(int count)
        {
            lock (table._Locker)
            {
                int skipped = 0;
                int taken = 0;

                ExecutionPlan executionplan = QueryPraser.GetExecutionPlan(this);

                List<List<long>> rangelist = new List<List<long>>();

                List<long> returnlist = new List<long>();

                foreach (var item in executionplan.IndexRanges)
                {
                    /// check if the primary key is included.  
                    var index = table.Indexs.Find(o => o.FieldName == item.Key);

                    var collection = item.Value.SelectMany(s => index.GetCollection(
                        s.lower,
                        s.upper,
                        s.lowerOpen,
                        s.upperOpen,
                        Ascending)
                    ).ToList();

                    rangelist.Add(collection);
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
                        !executionplan.ColumnScanner.Match(item, table.BlockFile.GetCol))
                    {
                        continue;
                    }

                    if (executionplan.RequireOrderBy)
                    {
                    }
                    else
                    {
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
                }

                executionplan = null;

                return returnlist;
            }
        }

        internal List<long> GetList(ExecutionPlan executionplan, int count)
        {
            lock (table._Locker)
            {
                int skipped = 0;
                int taken = 0;

                List<List<long>> rangelist = new List<List<long>>();

                List<long> returnlist = new List<long>();

                foreach (var item in executionplan.IndexRanges)
                {
                    /// check if the primary key is included.  
                    var index = table.Indexs.Find(o => o.FieldName == item.Key);
                    var collection = item.Value.SelectMany(s => index.GetCollection(
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
                        !executionplan.ColumnScanner.Match(item, table.BlockFile.GetCol))
                    {
                        continue;
                    }


                    if (executionplan.RequireOrderBy)
                    {
                        returnlist.Add(item);
                    }
                    else
                    {
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
            lock (table._Locker)
            {
                int skipped = 0;
                int taken = 0;

                ExecutionPlan executionplan = QueryPraser.GetExecutionPlan(this);

                List<List<long>> rangelist = new List<List<long>>();

                foreach (var item in executionplan.IndexRanges)
                {
                    var index = table.Indexs.Find(o => o.FieldName == item.Key);

                    var collection = item.Value.SelectMany(s => index.GetCollection(
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
                        !executionplan.ColumnScanner.Match(item, table.BlockFile.GetCol))
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

                executionplan = null;
                return taken;
            }
        }
    }
}