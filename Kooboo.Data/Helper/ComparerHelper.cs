//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Definition;
using System;

namespace Kooboo.Data.Helper
{
    public class ComparerHelper
    {
        public static Comparer GetComparer(string strOperator)
        {
            if (string.IsNullOrEmpty(strOperator))
            {
                return Comparer.EqualTo;
            }

            strOperator = strOperator.ToLower().Trim();

            switch (strOperator)
            {
                case "=":
                case "==":
                case "equalto":
                    return Comparer.EqualTo;
                case "!=":
                case "notequalto":
                    return Comparer.NotEqualTo;
                case ">":
                case "greaterthan":
                    return Comparer.GreaterThan;
                case ">=":
                case "greaterthanorequal":
                    return Comparer.GreaterThanOrEqual;
                case "<":
                case "lessthan":
                    return Comparer.LessThan;
                case "<=":
                case "lessthanorequal":
                    return Comparer.LessThanOrEqual;
                case "contains":
                    return Comparer.Contains;
                case "startwith":
                    return Comparer.StartWith;
                default:
                    return Comparer.EqualTo;
            }
        }

        public static Type DetermineCompareType(string x, string y)
        {
            if (string.IsNullOrEmpty(x) || string.IsNullOrEmpty(y))
            {
                return typeof(string);
            }

            DateTime datex; DateTime datey;

            if (DateTime.TryParse(x, out datex) && DateTime.TryParse(y, out datey))
            {
                return typeof(DateTime);
            }

            decimal decx; decimal decy;

            if (decimal.TryParse(x, out decx) && decimal.TryParse(y, out decy))
            {
                return typeof(decimal);
            }

            long longx; long longy;

            if (long.TryParse(x, out longx) && long.TryParse(y, out longy))
            {
                return typeof(long);
            }

            bool boolx; bool booly;

            if (bool.TryParse(x, out boolx) && bool.TryParse(y, out booly))
            {
                return typeof(bool);
            }

            return typeof(string);
        }

        public static string GetSymbol(Comparer comparer)
        {
            switch (comparer)
            {
                case Comparer.EqualTo:
                    return "==";

                case Comparer.GreaterThan:
                    return ">";

                case Comparer.GreaterThanOrEqual:
                    return ">=";

                case Comparer.LessThan:
                    return "<";

                case Comparer.LessThanOrEqual:
                    return "<=";

                case Comparer.NotEqualTo:
                    return "!=";

                case Comparer.StartWith:
                    return "startWith";

                case Comparer.Contains:
                    return "Contains";

                default:
                    throw new NotSupportedException();
            }
        }
    }
}