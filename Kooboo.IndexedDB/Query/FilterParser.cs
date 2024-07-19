//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Kooboo.IndexedDB.Condition.ColumnScan;
using Kooboo.IndexedDB.IndexRange;

namespace Kooboo.IndexedDB.Query
{
    public class FilterParser<TKey, TValue>
    {
        /// <summary>
        /// prase the filter collection and get an execution plan. 
        /// </summary>
        /// <returns></returns>
        public static ExecutionPlan GetExecutionPlan(Filter<TKey, TValue> filter)
        {
            var executionPlan = new ExecutionPlan();

            //first check order by field. 
            if (filter.OrderByPrimaryKey)
            {
                // does not support range with primary key yet, will be supported later.
                var ranges = filter.Node?.GetRanges(filter.store.StoreSetting.PrimaryKey,
                    filter.store.primaryIndex.Tree.comparer);

                if (ranges != null)
                {
                    if (!filter.Ascending) ranges.Reverse();

                    executionPlan.StartCollection = ranges.SelectMany(s => filter.store.primaryIndex.getCollection(
                        s.lower,
                        s.upper,
                        s.lowerOpen,
                        s.upperOpen,
                        filter.Ascending)
                    );
                }


                executionPlan.StartCollection ??= filter.store.primaryIndex.allItemCollection(filter.Ascending);

                // executionplan.OrderBySettled = true;
                executionPlan.HasStartCollection = true;
            }
            else
            {
                if (!string.IsNullOrEmpty(filter.OrderByFieldName))
                {
                    var index = filter.store.Indexes.getIndex(filter.OrderByFieldName);
                    if (index != null)
                    {
                        var comparer = ObjectContainer.getComparer(index.keyType, index.Length);
                        var ranges = filter.Node?.GetRanges(filter.OrderByFieldName, comparer);

                        if (ranges != null)
                        {
                            if (!filter.Ascending) ranges.Reverse();

                            executionPlan.StartCollection = ranges.SelectMany(s => filter.store.Indexes
                                .getIndex(filter.OrderByFieldName)
                                .GetCollection(
                                    s.lower,
                                    s.upper,
                                    s.lowerOpen,
                                    s.upperOpen,
                                    filter.Ascending)
                            );
                        }
                        else
                        {
                            executionPlan.StartCollection = filter.store.Indexes.getIndex(filter.OrderByFieldName)
                                .AllItems(filter.Ascending);
                        }

                        executionPlan.HasStartCollection = true;
                    }
                }
            }

            // check the primary key index. 
            var list = filter.Node?.GetRanges(filter.store.StoreSetting.PrimaryKey,
                filter.store.primaryIndex.Tree.comparer);

            if (list != null)
            {
                executionPlan.StartCollection = list.SelectMany(s => filter.store.primaryIndex.getCollection(
                    s.lower,
                    s.upper,
                    s.lowerOpen,
                    s.upperOpen,
                    filter.Ascending)
                );
                executionPlan.HasStartCollection = true;
            }

            // check all index fields that has been used in the filter. 
            foreach (var item in filter.store.Indexes.items)
            {
                var comparer = ObjectContainer.getComparer(item.keyType, item.Length);
                var ranges = filter.Node?.GetRanges(item.FieldName, comparer);
                if (ranges != null)
                {
                    executionPlan.IndexRanges.Add(item.FieldName, ranges);
                }
            }

            var indexNames = new List<string>();
            indexNames.AddRange(filter.store.Indexes.items.Select(s => s.FieldName));
            if (filter.store.primaryIndex != null) indexNames.Add(filter.store.primaryIndex.fieldname);
            var columnNames = filter.store.SettingColumns.Select(s => s.Key).ToArray();
            var notInColumnIndexes = indexNames.Except(columnNames).ToArray();

            // now parse columns. All query where condition item must be in columns, otherwise this will be a problem. 
            executionPlan.ColumnScanner =
                Node.FromExpression(name => filter.store.GetColumn(name), filter.Node, notInColumnIndexes);

            foreach (var item in filter.InItems)
            {
                var column = filter.store.GetColumn(item.Key);

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
                    throw new Exception(
                        "filter field must be index or column, add them to colomn or index when creating the store, otherwise use the fullscan option");
                }
            }

            /// for the methods calls. 
            foreach (var item in filter.calls)
            {
                MemberExpression memberAccess = null;
                foreach (var xItem in item.Arguments)
                {
                    if (xItem.NodeType == ExpressionType.MemberAccess)
                    {
                        memberAccess = xItem as MemberExpression;
                    }
                }

                if (memberAccess == null)
                {
                    throw new Exception("Method call require use one of the Fields or Property as parameters");
                }

                string fieldname = memberAccess.Member.Name;

                var column = filter.store.GetColumn(fieldname);

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

            /// verify the plan. or optimize it.
            if (!executionPlan.HasStartCollection)
            {
                //make one of the range. pick any one now. should be pick by the optimizer.  
                foreach (var item in executionPlan.IndexRanges)
                {
                    var index = filter.store.Indexes.getIndex(item.Key);
                    if (index == null) continue;

                    executionPlan.StartCollection = item.Value.SelectMany(s => index.GetCollection(
                        s.lower,
                        s.upper,
                        s.lowerOpen,
                        s.upperOpen,
                        filter.Ascending)
                    );

                    executionPlan.HasStartCollection = true;
                    executionPlan.IndexRanges.Remove(item.Key);
                    break;
                }

                if (!executionPlan.HasStartCollection)
                {
                    executionPlan.StartCollection = filter.store.primaryIndex.allItemCollection(filter.Ascending);
                    executionPlan.HasStartCollection = true;
                }
            }

            return executionPlan;
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

            List<int> removedItem = new List<int>();

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
                            removedItem.Add(i);
                            break;
                        case Comparer.GreaterThan:
                            range.lower = items[i].Value;
                            range.lowerOpen = true;
                            removedItem.Add(i);
                            break;
                        case Comparer.GreaterThanOrEqual:
                            range.lower = items[i].Value;
                            range.lowerOpen = false;
                            removedItem.Add(i);
                            break;
                        case Comparer.LessThan:
                            range.upper = items[i].Value;
                            range.upperOpen = true;
                            removedItem.Add(i);
                            break;
                        case Comparer.LessThanOrEqual:
                            range.upper = items[i].Value;
                            range.upperOpen = false;
                            removedItem.Add(i);
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

            bool hasMatch = false;
            foreach (int item in removedItem.OrderByDescending(o => o))
            {
                hasMatch = true;
                items.RemoveAt(item);
            }

            if (hasMatch)
            {
                return range;
            }
            else
            {
                return null;
            }
        }
    }
}