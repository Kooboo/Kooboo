//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.IndexedDB.Indexs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

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
            ExecutionPlan executionplan = new ExecutionPlan();

            //first check order by field. 
            if (filter.OrderByPrimaryKey)
            {
                // does not support range with primary key yet, will be supported later.
                Range<byte[]> primaryrange = getRange(filter.store.StoreSetting.PrimaryKey, filter.items);

                if (primaryrange != null)
                {
                    executionplan.startCollection = filter.store.primaryIndex.getCollection(primaryrange.lower, primaryrange.upper, primaryrange.lowerOpen, primaryrange.upperOpen, filter.Ascending);
                }
                else
                {
                    executionplan.startCollection = filter.store.primaryIndex.allItemCollection(filter.Ascending);
                }

               // executionplan.OrderBySettled = true;
                executionplan.hasStartCollection = true;
            }
            else
            {
                if (!string.IsNullOrEmpty(filter.OrderByFieldName))
                {
                    if (filter.store.Indexes.HasIndex(filter.OrderByFieldName))
                    {
                        Range<byte[]> range = getRange(filter.OrderByFieldName, filter.items);

                        if (range != null)
                        {
                            executionplan.startCollection = filter.store.Indexes.getIndex(filter.OrderByFieldName).GetCollection(range.lower, range.upper, range.lowerOpen, range.upperOpen, filter.Ascending);
                        }
                        else
                        {
                            executionplan.startCollection = filter.store.Indexes.getIndex(filter.OrderByFieldName).AllItems(filter.Ascending);
                        }

                       // executionplan.OrderBySettled = true;
                        executionplan.hasStartCollection = true;
                    }
                }
            }

            // check the primary key index. 
            Range<byte[]> primarykeyrange = getRange(filter.store.StoreSetting.PrimaryKey, filter.items);

            if (primarykeyrange != null)
            {
                executionplan.startCollection = filter.store.primaryIndex.getCollection(primarykeyrange.lower, primarykeyrange.upper, primarykeyrange.lowerOpen, primarykeyrange.upperOpen, filter.Ascending);
                executionplan.hasStartCollection = true;
            }

            // check all index fields that has been used in the filter. 
            foreach (var item in filter.store.Indexes.items)
            {
                Range<byte[]> indexrange = getRange(item.FieldName, filter.items);
                if (indexrange != null)
                {
                    executionplan.indexRanges.Add(item.FieldName, indexrange);
                }
            }

            // now parse columns. All query where condition item must be in columns, otherwise this will be a problem. 
            foreach (var item in filter.items)
            {
                Columns.IColumn<TValue> column;
                column = filter.store.GetColumn(item.FieldOrProperty);
                if (column != null)
                {
                    ColumnScan colplan = new ColumnScan();

                    colplan.ColumnName = column.FieldName;
                    colplan.relativeStartPosition = column.relativePosition;
                    colplan.length = column.Length;
                    colplan.Evaluator = ColumnEvaluator.GetEvaluator(column.DataType, item.Compare, item.Value, column.Length);

                    executionplan.scanColumns.Add(colplan); 
                }
                else
                {
                    throw new Exception("filter field must be index or column, add them to colomn or index when creating the store, otherwise use the fullscan option");
                }
            }

            foreach (var item in filter.InItems)
            {
                Columns.IColumn<TValue> column;
                column = filter.store.GetColumn(item.Key);
                if (column != null)
                {
                    ColumnScan colplan = new ColumnScan();

                    colplan.ColumnName = column.FieldName;
                    colplan.relativeStartPosition = column.relativePosition;
                    colplan.length = column.Length;
                    colplan.Evaluator = ColumnInEvaluator.GetInEvaluator(column.DataType, item.Value, column.Length); 

                    executionplan.scanColumns.Add(colplan); 
                }
                else
                {
                    throw new Exception("filter field must be index or column, add them to colomn or index when creating the store, otherwise use the fullscan option");
                }
            }

            /// for the methods calls. 
            foreach (var item in filter.calls)
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

                Columns.IColumn<TValue> column;
                column = filter.store.GetColumn(fieldname);
                if (column != null)
                {
                    ColumnScan colplan = new ColumnScan();

                    colplan.ColumnName = column.FieldName;
                    colplan.relativeStartPosition = column.relativePosition;
                    colplan.length = column.Length;

                    colplan.Evaluator = ColumnMethodCallEvaluator.GetMethodEvaluator(column.DataType, column.Length, item);

                    executionplan.scanColumns.Add(colplan);
                }
                else
                {
                    throw new Exception("methed call parameter must be a column, add the field to colomn creating creating the store, otherwise use the fullscan option");
                }
            } 
            /// verify the plan. or optimize it.
            if (!executionplan.hasStartCollection)
            {
                //make one of the range. pick any one now. should be pick by the optimizer.  
                foreach (var item in executionplan.indexRanges)
                {
                    IIndex<TValue> index = filter.store.Indexes.getIndex(item.Key);
                    if (index != null)
                    {
                        executionplan.startCollection = index.GetCollection(item.Value.lower, item.Value.upper, item.Value.lowerOpen, item.Value.upperOpen, filter.Ascending);
                        executionplan.hasStartCollection = true; 
                        executionplan.indexRanges.Remove(item.Key);
                        break;
                    }
                }
                if (!executionplan.hasStartCollection)
                {
                    executionplan.startCollection = filter.store.primaryIndex.allItemCollection(filter.Ascending);
                    executionplan.hasStartCollection = true;
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


    }
}
