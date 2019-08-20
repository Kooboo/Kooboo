//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Definition;
using Kooboo.Sites.DataSources;
using System;
using Kooboo.Data.Helper;
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

            if (filter.FieldValue !=null)
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
                    FilterDefinition filter = new FilterDefinition();

                    filter.FieldName = expression.Substring(0, index).Trim();

                    filter.FieldValue = expression.Substring(index + item.Length).Trim();

                    filter.Comparer = ComparerHelper.GetComparer(item);

                    return filter;
                }
            }
            return null;
        }

        public static bool Check(string FieldValue, Comparer Comparer, string CompareValue, Type ClrType = null)
        {
            if (ClrType == null)
            {
                ClrType = ComparerHelper.DetermineCompareType(FieldValue, CompareValue);
            }

            if (ClrType == null)
            {
                return false;
            }

            if (Comparer == Comparer.EqualTo)
            {
                return Lib.Helper.StringHelper.IsSameValue(FieldValue, CompareValue);
            }
            else if (Comparer == Comparer.NotEqualTo)
            {
                return !Lib.Helper.StringHelper.IsSameValue(FieldValue, CompareValue);
            }

            else if (Comparer == Comparer.Contains)
            {
                return FieldValue.Contains(CompareValue);
            }
            else if (Comparer == Comparer.StartWith)
            {
                return FieldValue.StartsWith(CompareValue);
            }

            else
            {
                try
                {
                    var value = Convert.ChangeType(FieldValue, ClrType);
                    var tovale = Convert.ChangeType(CompareValue, ClrType);

                    if (Comparer == Comparer.GreaterThan)
                    {
                        if (value != null && value is IComparable && tovale is IComparable)
                        {
                            return ((IComparable)value).CompareTo((IComparable)tovale) > 0;
                        }
                    }

                    else if (Comparer == Comparer.GreaterThanOrEqual)
                    {
                        if (value != null && value is IComparable && tovale is IComparable)
                        {
                            return ((IComparable)value).CompareTo((IComparable)tovale) >= 0;
                        }
                    }
                    else if (Comparer == Comparer.LessThan)
                    {
                        if (value != null && value is IComparable && tovale is IComparable)
                        {
                            return ((IComparable)value).CompareTo((IComparable)tovale) < 0;
                        }
                    }
                    else if (Comparer == Comparer.LessThanOrEqual)
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
                    _comparerlist = new List<string>();
                    _comparerlist.Add("===");
                    _comparerlist.Add(">=");
                    _comparerlist.Add("==");
                    _comparerlist.Add("<=");
                    _comparerlist.Add("!=");
                    _comparerlist.Add("<=");
                    _comparerlist.Add("=");
                    _comparerlist.Add(">");
                    _comparerlist.Add("<");
                }
                return _comparerlist;
            }
        }

    }
}
