//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System.Collections.Generic;

namespace Kooboo.Sites.Render
{
    public class ConditionEvaluatorManagercs
    {
        private static List<string> _operators;

        public static List<string> Operators
        {
            get
            {
                return _operators ?? (_operators = new List<string>
                {
                    ">",
                    ">=",
                    "<",
                    "<=",
                    "=",
                    "contains",
                    "startwith"
                });
            }
        }

        /// <summary>
        ///  The formate is always leftExpression  [operator] rightvalue.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static IConditionEvaluator GetEvaluator(string expression)
        {
            expression = expression.Replace("  ", " ");

            string[] three = expression.Split(' ');

            if (three.Length == 1)
            {
                var evalutor = new ConditionEqual {LeftExpression = three[0], RightValue = "true"};
                return evalutor;
            }
            else if (three.Length == 3)
            {
                var evalutor = GetNewEvaluator(three[1]);
                if (evalutor != null)
                {
                    evalutor.LeftExpression = three[0];
                    evalutor.RightValue = three[2];
                    return evalutor;
                }
            }
            return null;
        }

        private static IConditionEvaluator GetNewEvaluator(string matchOperator)
        {
            if (string.IsNullOrEmpty(matchOperator))
            {
                return null;
            }

            matchOperator = matchOperator.ToLower();

            switch (matchOperator)
            {
                case ">":
                    {
                        return new ConditionGreaterThan();
                    }
                case ">=":
                    {
                        return new ConditionGreaterThanOrEqual();
                    }
                case "<":
                    {
                        return new ConditionLessThan();
                    }
                case "<=":
                    {
                        return new ConditionLessThanOrEqual();
                    }
                case "contains":
                    {
                        return new ConditionContains();
                    }
                case "startwith":
                    {
                        return new ConditionStartWith();
                    }
                case "=":
                    {
                        return new ConditionEqual();
                    }
                case "==":
                    {
                        return new ConditionEqual();
                    }
                case "!=":
                    {
                        return new ConditionNotEqual();
                    }

                default:
                    break;
            }

            return null;
        }
    }
}