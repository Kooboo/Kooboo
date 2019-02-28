//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Render.RenderTasks.Tal
{
    public class ConditionContains : IConditionEvaluator
    {
        public string ConditionOperator
        {
            get
            {
                return "contains";
            }
        }

        public string LeftExpression
        {
            get; set;
        }

        public string RightValue
        {
            get; set;
        }

        public bool Check(Kooboo.Data.Context.DataContext context)
        {
            var value = context.GetValue(LeftExpression);
            if (value == null)
            {
                return false;
            }
            var stringvalue = value.ToString();

            return stringvalue.Contains(this.RightValue);
        }
    }
    
    public class ConditionStartWith : IConditionEvaluator
    {
        public string ConditionOperator
        {
            get
            {
                return "startwith";
            }
        }

        public string LeftExpression
        {
            get; set;
        }

        public string RightValue
        {
            get; set;
        }

        public bool Check(Kooboo.Data.Context.DataContext context)
        {
            var value = context.GetValue(LeftExpression);
            if (value == null)
            {
                return false;
            }
            var stringvalue = value.ToString();

            return stringvalue.StartsWith(this.RightValue);
        }
    }

    public class ConditionGreaterThan : IConditionEvaluator
    {
        public string ConditionOperator
        {
            get
            {
                return ">";
            }
        }

        public string LeftExpression
        {
            get; set;
        }

        public string RightValue
        {
            get; set;
        }

        public bool Check(Kooboo.Data.Context.DataContext context)
        {
            var value = context.GetValue(this.LeftExpression);
            if (value == null)
            {
                return false;
            }

            var valuetype = value.GetType();

            if (valuetype == typeof(Int32) || valuetype == typeof(Int16) || valuetype == typeof(Int64) || valuetype == typeof(byte))
            {
                var intvalue = (int)value;

                int comparevalue;

                if (int.TryParse(this.RightValue, out comparevalue))
                {
                    return intvalue > comparevalue;
                }
                else
                {
                    return false;
                }

            }


            return false;
        }
    }

    public class ConditionGreaterThanOrEqual : IConditionEvaluator
    {
        public string ConditionOperator
        {
            get
            {
                return ">=";
            }
        }

        public string LeftExpression
        {
            get; set;
        }

        public string RightValue
        {
            get; set;
        }

        public bool Check(Kooboo.Data.Context.DataContext context)
        {
            var value = context.GetValue(this.LeftExpression);
            if (value == null)
            {
                return false;
            }

            var valuetype = value.GetType();

            if (valuetype == typeof(Int32) || valuetype == typeof(Int16) || valuetype == typeof(Int64) || valuetype == typeof(byte))
            {
                var intvalue = (int)value;

                int comparevalue;

                if (int.TryParse(this.RightValue, out comparevalue))
                {
                    return intvalue >= comparevalue;
                }
                else
                {
                    return false;
                }

            }


            return false;
        }
    }
     
    public class ConditionLessThan : IConditionEvaluator
    {
        public string ConditionOperator
        {
            get
            {
                return "<";
            }
        }

        public string LeftExpression
        {
            get; set;
        }

        public string RightValue
        {
            get; set;
        }

        public bool Check(Kooboo.Data.Context.DataContext context)
        {
            var value = context.GetValue(this.LeftExpression);
            if (value == null)
            {
                return false;
            }

            var valuetype = value.GetType();

            if (valuetype == typeof(Int32) || valuetype == typeof(Int16) || valuetype == typeof(Int64) || valuetype == typeof(byte))
            {
                var intvalue = (int)value;

                int comparevalue;

                if (int.TryParse(this.RightValue, out comparevalue))
                {
                    return intvalue < comparevalue;
                }
                else
                {
                    return false;
                }

            }


            return false;
        }
    }

    public class ConditionLessThanOrEqual : IConditionEvaluator
    {
        public string ConditionOperator
        {
            get
            {
                return "<=";
            }
        }

        public string LeftExpression
        {
            get; set;
        }

        public string RightValue
        {
            get; set;
        }

        public bool Check(Kooboo.Data.Context.DataContext context)
        {
            var value = context.GetValue(this.LeftExpression);
            if (value == null)
            {
                return false;
            }

            var valuetype = value.GetType();

            if (valuetype == typeof(Int32) || valuetype == typeof(Int16) || valuetype == typeof(Int64) || valuetype == typeof(byte))
            {
                var intvalue = (int)value;

                int comparevalue;

                if (int.TryParse(this.RightValue, out comparevalue))
                {
                    return intvalue <= comparevalue;
                }
                else
                {
                    return false;
                }

            }


            return false;
        }
    }

    public class ConditionEqual : IConditionEvaluator
    {
        public string ConditionOperator
        {
            get
            {
                return "=";
            }
        }

        public string LeftExpression
        {
            get; set;
        }

        private string _rightvalue;
        public string RightValue
        {
            get { return _rightvalue; }
            set
            {
                _rightvalue = value;
                if (!string.IsNullOrEmpty(_rightvalue))
                {
                    _rightvalue = _rightvalue.ToLower();
                }
            }
        }

        public bool Check(Kooboo.Data.Context.DataContext context)
        {
            var value = context.GetValue(this.LeftExpression);
            if (value == null)
            {
                return false;
            }

            string stringvalue = value.ToString().ToLower();

            if (stringvalue == this.RightValue)
            {
                return true;
            }

            if (value.GetType() == typeof(bool))
            {
                bool boolrightvalue = false;

                if (string.IsNullOrWhiteSpace(this.RightValue) || this.RightValue.ToLower() == "true" || this.RightValue == "1" || this.RightValue.ToLower() == "yes" || this.RightValue.ToLower() == "ok")
                {
                    boolrightvalue = true;
                }

                var boolvalue = (bool)value;

                if (boolvalue == boolrightvalue)
                {
                    return true;
                }
            }

            return false;
        }
    }

    public class ConditionNotEqual : IConditionEvaluator
    {
        public string ConditionOperator
        {
            get
            {
                return "!=";
            }
        }

        public string LeftExpression
        {
            get; set;
        }

        private string _rightvalue;
        public string RightValue
        {
            get { return _rightvalue; }
            set
            {
                _rightvalue = value;
                if (!string.IsNullOrEmpty(_rightvalue))
                {
                    _rightvalue = _rightvalue.ToLower();
                }
            }
        }

        public bool Check(Kooboo.Data.Context.DataContext context)
        {
            var value = context.GetValue(this.LeftExpression);
            if (value == null)
            {
                return false;
            }

            string stringvalue = value.ToString().ToLower();

            if (stringvalue == this.RightValue)
            {
                return false;
            }

            if (value.GetType() == typeof(bool))
            {
                bool boolrightvalue = false;

                if (string.IsNullOrWhiteSpace(this.RightValue) || this.RightValue.ToLower() == "true" || this.RightValue == "1" || this.RightValue.ToLower() == "yes" || this.RightValue.ToLower() == "ok")
                {
                    boolrightvalue = true;
                }

                var boolvalue = (bool)value;

                if (boolvalue == boolrightvalue)
                {
                    return false;
                }
            }

            return true;

        }
    }
}
