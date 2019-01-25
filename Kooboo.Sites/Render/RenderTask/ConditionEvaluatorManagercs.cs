//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Render
{
  public  class ConditionEvaluatorManagercs
    {
        private static List<string> _Operators; 
        public static List<string> Operators
        {
            get
            {
                if (_Operators == null)
                {
                    _Operators = new List<string>();
                    _Operators.Add(">");
                    _Operators.Add(">=");
                    _Operators.Add("<");
                    _Operators.Add("<=");
                    _Operators.Add("=");
                    _Operators.Add("contains");
                    _Operators.Add("startwith"); 

                }

                return _Operators; 

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

            if (three.Length ==1)
            {
                var evalutor = new ConditionEqual();
                evalutor.LeftExpression = three[0];
                evalutor.RightValue = "true";
                return evalutor; 
            }
            else if (three.Length ==3)
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

        private static IConditionEvaluator GetNewEvaluator(string MatchOperator)
        {
            if (string.IsNullOrEmpty(MatchOperator))
            {
                return null; 
            }

            MatchOperator = MatchOperator.ToLower(); 

            switch (MatchOperator)
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
