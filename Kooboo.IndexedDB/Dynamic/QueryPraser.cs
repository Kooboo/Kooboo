//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.IndexedDB.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

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
            ExecutionPlan executionplan = new ExecutionPlan();

            ITableIndex startindex = null;
            if (!string.IsNullOrEmpty(query.OrderByFieldName))
            {
                startindex = query.table.Indexs.Find(o => o.FieldName == query.OrderByFieldName); 
                if (startindex == null)
                {
                    executionplan.RequireOrderBy = true; 
                }
            }

            // find other where fields......  
            if (startindex == null)
            {
                startindex = query.table.Indexs.Find(o => o.IsSystem);
            }

            if (!string.IsNullOrEmpty(query.OrderByFieldName) && !executionplan.RequireOrderBy)
            {
                Range<byte[]> range = getRange(query.OrderByFieldName, query.items);
                if (range != null)
                {
                    executionplan.startCollection = startindex.GetCollection(range.lower, range.upper, range.lowerOpen, range.upperOpen, query.Ascending);
                }
                else
                {
                    executionplan.startCollection = startindex.AllItems(query.Ascending);
                }
            }
            else
            {
                executionplan.startCollection = startindex.AllItems(query.Ascending);
            }

            // check all index fields that has been used in the filter. 
            foreach (var item in query.table.Indexs)
            {
                if (item.FieldName != startindex.FieldName)
                {
                    Range<byte[]> indexrange = getRange(item.FieldName, query.items);
                    if (indexrange != null)
                    {
                        executionplan.indexRanges.Add(item.FieldName, indexrange);
                    }
                }
            }

            // now the left columns.. 
            foreach (var item in query.items)
            {
                var column = query.table.ObjectConverter.Fields.Find(o => o.FieldName == item.FieldOrProperty);

                if (column != null)
                {
                    ColumnScan colplan = new ColumnScan();

                    colplan.ColumnName = column.FieldName;
                    colplan.relativeStartPosition = column.RelativePosition;
                    colplan.length = column.Length;
                    colplan.Evaluator = ColumnEvaluator.GetEvaluator(column.ClrType, item.Compare, item.Value, column.Length);

                    executionplan.scanColumns.Add(colplan);
                }
                else
                {
                    throw new Exception("filter field must be  column with fixed len");
                }
            }

            // the left column query.
            foreach (var item in query.InItems)
            {
                var column = query.table.ObjectConverter.Fields.Find(o => o.FieldName == item.Key);

                if (column != null)
                {
                    ColumnScan colplan = new ColumnScan();

                    colplan.ColumnName = column.FieldName;
                    colplan.relativeStartPosition = column.RelativePosition;
                    colplan.length = column.Length;
                    colplan.Evaluator = ColumnInEvaluator.GetInEvaluator(column.ClrType, item.Value, column.Length);

                    executionplan.scanColumns.Add(colplan);
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
                    ColumnScan colplan = new ColumnScan();

                    colplan.ColumnName = column.FieldName;
                    colplan.relativeStartPosition = column.RelativePosition;
                    colplan.length = column.Length;

                    colplan.Evaluator = ColumnMethodCallEvaluator.GetMethodEvaluator(column.ClrType, column.Length, item);

                    executionplan.scanColumns.Add(colplan);
                }
                else
                {
                    throw new Exception("methed call parameter must be a column, add the field to colomn creating creating the store, otherwise use the fullscan option");
                }
            }

            return executionplan;
        }

        /// <summary>
        /// get the range query collection of index fields.for looping.  this can be OrderBy fields or fields that has more sparnse. this only works for index fields. 
        /// after get, the related field or property item will be removed from the item collection.
        /// </summary>
        /// <param name="FieldOrPropertyName"></param>
        /// <returns></returns>
        private static Range<byte[]> getRange(string FieldOrPropertyName, List<FilterItem> items)
        {
            if (string.IsNullOrWhiteSpace(FieldOrPropertyName))
            {
                return null;
            }

            FieldOrPropertyName = FieldOrPropertyName.ToLower();

            Range<byte[]> range = new Range<byte[]>();

            List<int> removeditem = new List<int>();

            for (int i = 0; i < items.Count; i++)
            {

                if (items[i].FieldOrProperty.ToLower() == FieldOrPropertyName)
                {
                    switch (items[i].Compare)
                    {
                        case Comparer.EqualTo:
                            // for equal to. 
                            range.upper = items[i].Value;
                            range.upperOpen = false;
                            range.lower = items[i].Value;
                            range.lowerOpen = false;
                            removeditem.Add(i);
                            break;
                        case Comparer.GreaterThan:
                            range.lower = items[i].Value;
                            range.lowerOpen = true;
                            removeditem.Add(i);
                            break;
                        case Comparer.GreaterThanOrEqual:
                            range.lower = items[i].Value;
                            range.lowerOpen = false;
                            removeditem.Add(i);
                            break;
                        case Comparer.LessThan:
                            range.upper = items[i].Value;
                            range.upperOpen = true;
                            removeditem.Add(i);
                            break;
                        case Comparer.LessThanOrEqual:
                            range.upper = items[i].Value;
                            range.upperOpen = false;
                            removeditem.Add(i);
                            break;
                        case Comparer.NotEqualTo:
                            //does not do anything. 
                            break;
                        case Comparer.StartWith:
                            // does not do anything for startwith or contains. 

                            break;
                        case Comparer.Contains:
                            break;
                        default:
                            break;
                    }

                }

            }

            bool hasmatch = false;
            foreach (int item in removeditem.OrderByDescending(o => o))
            {
                hasmatch = true;
                items.RemoveAt(item);
            }

            if (hasmatch)
            {
                return range;
            }
            else
            {
                return null;
            }
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
            bool isValueString = false;

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
                            item.IsString = isValueString;
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
                        isValueString = tokenRet.IsString;
                    }

                    if (field != null && compare != null && value != null)
                    {
                        ConditionItem item = new ConditionItem();
                        item.Field = field;
                        item.Comparer = GetComparer(compare);
                        item.Value = value;
                        item.IsString = isValueString;
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

        public bool IsString { get; set; }

        public string Value { get; set; }
    }

}
