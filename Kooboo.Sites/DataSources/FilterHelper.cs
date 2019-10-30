//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Definition;
using Kooboo.Data.Helper;
using System;
using System.Collections.Generic;

namespace Kooboo.Sites.DataSources
{
    public class FilterHelper
    {
        public static FilterDefinition GetFilter(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
            {
                return null;
            }
            expression = expression.Replace("  ", " ");
            expression = expression.Trim();

            var trysymbol = GetFilterBySymbol(expression);
            if (trysymbol != null)
            {
                return trysymbol;
            }

            FilterDefinition filter = new FilterDefinition();
            int index = expression.IndexOf(" ");

            if (index == -1)
            {
                filter.FieldName = expression;
                filter.Comparer = Comparer.EqualTo;
                filter.FieldValue = "true";
            }
            else
            {
                filter.FieldName = expression.Substring(0, index);

                var nextindex = expression.IndexOf(" ", index + 1);

                if (nextindex == -1)
                {
                    string compare = expression.Substring(index).Trim();
                    filter.Comparer = ComparerHelper.GetComparer(compare);
                }
                else
                {
                    string compare = expression.Substring(index, nextindex - index);
                    filter.Comparer = ComparerHelper.GetComparer(compare);

                    filter.FieldValue = expression.Substring(nextindex).Trim();
                }
            }

            return filter;
        }

        public static void CheckValueType(FilterDefinition filter)
        {
            if (filter.FieldName != null)
            {
                if (filter.FieldName.StartsWith("'") && filter.FieldName.EndsWith("'"))
                {
                    filter.FieldName = filter.FieldName.Trim('\'');
                    filter.IsNameValueType = true;
                }
                else if (filter.FieldName.StartsWith("\"") && filter.FieldName.EndsWith("\""))
                {
                    filter.FieldName = filter.FieldName.Trim('"');
                    filter.IsNameValueType = true;
                }
            }

            if (filter.FieldValue != null)
            {
                if (filter.FieldValue.StartsWith("'") && filter.FieldValue.EndsWith("'"))
                {
                    filter.FieldValue = filter.FieldValue.Trim('\'');
                    filter.IsValueValueType = true;
                }
                else if (filter.FieldValue.StartsWith("\"") && filter.FieldValue.EndsWith("\""))
                {
                    filter.FieldValue = filter.FieldValue.Trim('"');
                    filter.IsValueValueType = true;
                }
            }
        }

        public static FilterDefinition GetFilterBySymbol(string expression)
        {
            foreach (var item in ComparerList)
            {
                int index = expression.IndexOf(item);
                if (index > -1)
                {
                    FilterDefinition filter = new FilterDefinition
                    {
                        FieldName = expression.Substring(0, index).Trim(),
                        FieldValue = expression.Substring(index + item.Length).Trim(),
                        Comparer = ComparerHelper.GetComparer(item)
                    };




                    return filter;
                }
            }
            return null;
        }

        public static bool Check(string fieldValue, Comparer comparer, string compareValue, Type clrType = null)
        {
            if (clrType == null)
            {
                clrType = ComparerHelper.DetermineCompareType(fieldValue, compareValue);
            }

            if (clrType == null)
            {
                return false;
            }

            switch (comparer)
            {
                case Comparer.EqualTo:
                    return Lib.Helper.StringHelper.IsSameValue(fieldValue, compareValue);
                case Comparer.NotEqualTo:
                    return !Lib.Helper.StringHelper.IsSameValue(fieldValue, compareValue);
                case Comparer.Contains:
                    return fieldValue.Contains(compareValue);
                case Comparer.StartWith:
                    return fieldValue.StartsWith(compareValue);
                default:
                    try
                    {
                        var value = Convert.ChangeType(fieldValue, clrType);
                        var tovale = Convert.ChangeType(compareValue, clrType);

                        if (comparer == Comparer.GreaterThan)
                        {
                            if (value != null && value is IComparable && tovale is IComparable)
                            {
                                return ((IComparable)value).CompareTo((IComparable)tovale) > 0;
                            }
                        }
                        else if (comparer == Comparer.GreaterThanOrEqual)
                        {
                            if (value != null && value is IComparable && tovale is IComparable)
                            {
                                return ((IComparable)value).CompareTo((IComparable)tovale) >= 0;
                            }
                        }
                        else if (comparer == Comparer.LessThan)
                        {
                            if (value != null && value is IComparable && tovale is IComparable)
                            {
                                return ((IComparable)value).CompareTo((IComparable)tovale) < 0;
                            }
                        }
                        else if (comparer == Comparer.LessThanOrEqual)
                        {
                            if (value != null && value is IComparable && tovale is IComparable)
                            {
                                return ((IComparable)value).CompareTo((IComparable)tovale) <= 0;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        return false;
                    }

                    break;
            }
            return true;
        }

        private static List<string> _comparerlist;

        private static List<string> ComparerList
        {
            get
            {
                if (_comparerlist == null)
                {
                    _comparerlist = new List<string>
                    {
                        "===",
                        ">=",
                        "==",
                        "<=",
                        "!=",
                        "<=",
                        "=",
                        ">",
                        "<"
                    };
                }
                return _comparerlist;
            }
        }
    }
}