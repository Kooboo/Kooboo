//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Definition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Helper
{
    public class ComparerHelper
    {
        public static Comparer GetComparer(string StrOperator)
        {
            if (string.IsNullOrEmpty(StrOperator))
            {
                return Comparer.EqualTo;
            }

            StrOperator = StrOperator.ToLower().Trim();

            if (StrOperator == "=" || StrOperator == "==" || StrOperator == "equalto")
            {
                return Comparer.EqualTo;
            }
            else if (StrOperator == "!=" || StrOperator == "notequalto")
            {
                return Comparer.NotEqualTo;
            }

            else if (StrOperator == ">" || StrOperator == "greaterthan")
            {
                return Comparer.GreaterThan;
            }
            else if (StrOperator == ">=" || StrOperator == "greaterthanorequal")
            {
                return Comparer.GreaterThanOrEqual;
            }
            else if (StrOperator == "<" || StrOperator == "lessthan")
            {
                return Comparer.LessThan;
            }
            else if (StrOperator == "<=" || StrOperator == "lessthanorequal")
            {
                return Comparer.LessThanOrEqual;
            }
            else if (StrOperator == "contains")
            {
                return Comparer.Contains;
            }
            else if (StrOperator == "startwith")
            {
                return Comparer.StartWith;
            }

            return Comparer.EqualTo;

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

            if (long.TryParse(x,out longx) && long.TryParse(y, out longy))
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
