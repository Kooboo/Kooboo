//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Kooboo.IndexedDB.Condition.ColumnScan;
using Kooboo.IndexedDB.IndexRange;
using Kooboo.IndexedDB.Query;

namespace Kooboo.IndexedDB.Dynamic
{
    public class QueryPraser
    {
        /// <summary>
        /// prase the filter collection and get an execution plan. 
        /// </summary>
        /// <returns></returns>
        public static ExecutionPlan GetExecutionPlan(Query query)
        {
            var executionPlan = new ExecutionPlan();

            ITableIndex startIndex = null;

            if (!string.IsNullOrEmpty(query.OrderByFieldName))
            {
                startIndex = query.table.Indexs.Find(o => o.FieldName == query.OrderByFieldName);
                executionPlan.RequireOrderBy = startIndex == null;
                if (!executionPlan.RequireOrderBy)
                {
                    var comparer = ObjectContainer.getComparer(startIndex.keyType, startIndex.Length);
                    var ranges = query.Node?.GetRanges(query.OrderByFieldName, comparer);

                    if (ranges != null)
                    {
                        if (!query.Ascending) ranges.Reverse();

                        executionPlan.StartCollection = ranges.SelectMany(r => startIndex.GetCollection(
                            r.lower,
                            r.upper,
                            r.lowerOpen,
                            r.upperOpen,
                            query.Ascending)
                        );
                    }
                }
            }

            // find other where fields......  
            startIndex ??= query.table.Indexs.Find(o => o.IsSystem);
            executionPlan.StartCollection ??= startIndex.AllItems(query.Ascending);

            // check all index fields that has been used in the filter. 
            foreach (var item in query.table.Indexs)
            {
                if (item.FieldName != startIndex.FieldName)
                {
                    var comparer = ObjectContainer.getComparer(item.keyType, item.Length);
                    var ranges = query.Node?.GetRanges(item.FieldName, comparer);

                    if (ranges != null)
                    {
                        executionPlan.IndexRanges.Add(item.FieldName, ranges);
                    }
                }
            }

            var indexNames = query.table.Indexs.Select(s => s.FieldName).ToArray();
            var columnNames = query.table.ObjectConverter.Fields.Select(s => s.FieldName).ToArray();
            var notInColumnIndexes = indexNames.Except(columnNames).ToArray();

            // now the left columns.. 
            executionPlan.ColumnScanner = Node.FromExpression(
                name => query.table.ObjectConverter.Fields.Find(o => o.FieldName == name),
                query.Node,
                notInColumnIndexes);

            // the left column query.
            foreach (var item in query.InItems)
            {
                var column = query.table.ObjectConverter.Fields.Find(o => o.FieldName == item.Key);

                if (column != null)
                {
                    var filterNode = new FilterNode
                    {
                        ColumnName = column.FieldName,
                        RelativeStartPosition = column.RelativePosition,
                        Length = column.Length,
                        Evaluator = ColumnInEvaluator.GetInEvaluator(column.DataType, item.Value, column.Length)
                    };

                    executionPlan.ColumnScanner = Node.And(executionPlan.ColumnScanner, filterNode);
                }
                else
                {
                    throw new Exception("filter field must be a column with fixed length");
                }
            }

            /// for the methods calls. 
            foreach (var item in query.calls)
            {
                MemberExpression memberaccess = null;
                foreach (var xitem in item.Arguments)
                {
                    if (xitem.NodeType == ExpressionType.MemberAccess)
                    {
                        memberaccess = xitem as MemberExpression;
                    }
                }

                if (memberaccess == null)
                {
                    throw new Exception("Method call require use one of the Fields or Property as parameters");
                }

                string fieldname = memberaccess.Member.Name;


                var column = query.table.ObjectConverter.Fields.Find(o => o.FieldName == fieldname);

                if (column != null)
                {
                    var filterNode = new FilterNode
                    {
                        ColumnName = column.FieldName,
                        RelativeStartPosition = column.RelativePosition,
                        Length = column.Length,
                        Evaluator = ColumnMethodCallEvaluator.GetMethodEvaluator(column.DataType, column.Length, item)
                    };

                    executionPlan.ColumnScanner = Node.And(executionPlan.ColumnScanner, filterNode);
                }
                else
                {
                    throw new Exception(
                        "methed call parameter must be a column, add the field to colomn creating creating the store, otherwise use the fullscan option");
                }
            }

            return executionPlan;
        }

        public static List<ConditionItem> ParseConditoin(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
            {
                return new List<ConditionItem>();
            }

            var scanner = new SyntaxScanner(expression);

            var tokenRet = scanner.ConsumeNext();
            var token = tokenRet?.Value;

            string field = null;
            string compare = null;
            string value = null;
            bool isValueQuoted = false;

            List<ConditionItem> result = new List<ConditionItem>();

            while (token != null)
            {
                if (token == "&&" || token == "&")
                {
                    if (field != null && compare != null)
                    {
                        if (value != null)
                        {
                            ConditionItem item = new ConditionItem();
                            item.Field = field;
                            item.Comparer = GetComparer(compare);
                            item.Value = value;
                            item.IsQuoted = isValueQuoted;
                            result.Add(item);
                        }
                        else
                        {
                            ConditionItem item = new ConditionItem();
                            item.Field = field;
                            item.Comparer = Comparer.EqualTo;
                            item.Value = compare;
                            result.Add(item);
                        }


                        field = null;
                        compare = null;
                        value = null;
                    }

                    field = null;
                    compare = null;
                    value = null;
                }
                else
                {
                    if (field == null)
                    {
                        field = token;
                    }
                    else if (compare == null)
                    {
                        compare = token;
                    }
                    else if (value == null)
                    {
                        value = token;
                        isValueQuoted = tokenRet.IsQuoted;
                    }

                    if (field != null && compare != null && value != null)
                    {
                        ConditionItem item = new ConditionItem();
                        item.Field = field;
                        item.Comparer = GetComparer(compare);
                        item.Value = value;
                        item.IsQuoted = isValueQuoted;
                        result.Add(item);

                        field = null;
                        compare = null;
                        value = null;
                    }
                }

                tokenRet = scanner.ConsumeNext();
                token = tokenRet?.Value;
            }

            if (field != null && compare != null)
            {
                ConditionItem item = new ConditionItem();
                item.Field = field;
                item.Comparer = Comparer.EqualTo;
                item.Value = compare;
                result.Add(item);
            }

            return result;
        }

        private static Kooboo.IndexedDB.Query.Comparer GetComparer(string input)
        {
            if (input == "=" || input == "==" || input == "===")
            {
                return Comparer.EqualTo;
            }
            else if (input == ">")
            {
                return Comparer.GreaterThan;
            }
            else if (input == "<")
            {
                return Comparer.LessThan;
            }
            else if (input == ">=")
            {
                return Comparer.GreaterThanOrEqual;
            }
            else if (input == "<=")
            {
                return Comparer.LessThanOrEqual;
            }
            else if (input == "!=")
            {
                return Comparer.NotEqualTo;
            }
            else if (input.ToLower() == "contains")
            {
                return Comparer.Contains;
            }
            else if (input.ToLower() == "startwith")
            {
                return Comparer.StartWith;
            }

            return Comparer.EqualTo;
        }
    }

    public class ConditionItem
    {
        public string Field { get; set; }

        public Comparer Comparer { get; set; }

        public bool IsQuoted { get; set; }

        public string Value { get; set; }
    }
}