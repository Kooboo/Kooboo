//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.FrontEvent
{
   public static  class ConditionManager
    { 
        public static bool Evaluate(IFrontEvent TheEvent, Condition condition)
        {
            var value =   Lib.Reflection.Dynamic.GetObjectMember(TheEvent, condition.Left);  
            if (value == null)
            {
                return false;
            }
            var type = value.GetType();
            string Operator = condition.Operator.ToLower();

            if (Operator == ">" || Operator == ">=" || Operator == "<" || Operator == "<=")
            {
                if (type == typeof(Int32) || type == typeof(byte) || type == typeof(Int16) || type == typeof(Int64))
                {
                    long leftvalue = Convert.ToInt64(value);
                    long rightvalue;

                    if (long.TryParse(condition.Right, out rightvalue))
                    {
                        if (Operator == ">")
                        {
                            return leftvalue > rightvalue;
                        }
                        else if (Operator == ">=")
                        {
                            return leftvalue >= rightvalue;
                        }
                        else if (Operator == "<")
                        {
                            return leftvalue < rightvalue;
                        }
                        else if (Operator == "<=")
                        {
                            return leftvalue <= rightvalue;
                        }
                    }

                }
                else if (type == typeof(decimal) || type == typeof(float) || type == typeof(double))
                {
                    decimal leftvalue = Convert.ToDecimal(value);
                    decimal rightvalue;
                    if (decimal.TryParse(condition.Right, out rightvalue))
                    {
                        if (Operator == ">")
                        {
                            return leftvalue > rightvalue;
                        }
                        else if (Operator == ">=")
                        {
                            return leftvalue >= rightvalue;
                        }
                        else if (Operator == "<")
                        {
                            return leftvalue < rightvalue;
                        }
                        else if (Operator == "<=")
                        {
                            return leftvalue <= rightvalue;
                        }
                    }
                }

            }
            else if (Operator == "=" || Operator == "==" || Operator == "!=")
            {
                if (type == typeof(Guid))
                {
                    Guid leftvalue = (Guid)value;
                    Guid rightvalue;
                    if (Guid.TryParse(condition.Right, out rightvalue))
                    {
                        if (Operator == "=" || Operator == "==")
                        {
                            return leftvalue == rightvalue;
                        }
                        else if (Operator == "!=")
                        {
                            return leftvalue != rightvalue;
                        }
                    }
                }

                else if (type == typeof(bool))
                {
                    bool leftvalue = (bool)value;
                    bool rightvalue;
                    if (bool.TryParse(condition.Right, out rightvalue))
                    {
                        if (Operator == "=" || Operator == "==")
                        {
                            return leftvalue == rightvalue;
                        }
                        else if (Operator == "!=")
                        {
                            return leftvalue != rightvalue;
                        }
                    }


                }

                else
                {
                    string leftvalue = value.ToString();
                    if (Operator == "=" || Operator == "==")
                    {
                        return leftvalue == condition.Right;
                    }
                    else if (Operator == "!=")
                    {
                        return leftvalue != condition.Right;
                    }
                }
            }

            else
            {

                //Operators.Add("Contains");
                //Operators.Add("NotContains");
                //Operators.Add("Startwith");
                //Operators.Add("NotStartwith");

                string leftvalue = value.ToString();

                if (Operator == "contains")
                {
                    return leftvalue.Contains(condition.Right);
                }
                else if (Operator == "startwith")
                {
                    return leftvalue.StartsWith(condition.Right);
                }
                else if (Operator == "notcontains")
                {
                    return !leftvalue.Contains(condition.Right);
                }
                else if (Operator == "notstartwith")
                {
                    return !leftvalue.StartsWith(condition.Right);
                }
            }
            return false;
        }

        public static List<string> GetOperators(Type datatype)
        {
            List<string> Operators = new List<string>();

            if (datatype == typeof(Int32) || datatype == typeof(Int16) || datatype == typeof(Int64) || datatype == typeof(Decimal) || datatype == typeof(float) || datatype == typeof(DateTime) || datatype == typeof(byte) || datatype == typeof(double))
            {
                Operators.Add(">");
                Operators.Add(">=");
                Operators.Add("<=");
                Operators.Add("<");
                Operators.Add("=");
                Operators.Add("!=");
            }
            else if (datatype == typeof(Guid) || datatype == typeof(bool))
            {
                Operators.Add("=");
                Operators.Add("!=");
            }
            else if (datatype == typeof(string))

            {
                Operators.Add("=");
                Operators.Add("!=");
                Operators.Add("Contains");
                Operators.Add("NotContains");
                Operators.Add("Startwith");
                Operators.Add("NotStartwith");
            }

            return Operators;
        }
    }
}
