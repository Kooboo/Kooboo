//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;

namespace Kooboo.Lib.Helper
{
    public static class DataTypeHelper
    {
        public static bool IsGuid(string input)
        {
            Guid outid;
            return System.Guid.TryParse(input, out outid);
        }

        public static bool IsInt(string input)
        {
            long outid;
            return long.TryParse(input, out outid);
        }

        public static bool IsJsonType(string json, Type type)
        {
            try
            {
                var x = Lib.Helper.JsonHelper.Deserialize(json, type);
                return true;
            }
            catch (Exception)
            {
            }

            return false;
        }

        public static bool IsBool(string input)
        {
            bool OK;
            return bool.TryParse(input, out OK);
        }

        public static T ConvertType<T>(string value)
        {
            var type = typeof(T);

            if (type == typeof(Guid))
            {
                if (Guid.TryParse(value, out Guid toValue))
                {
                    return (T)(object)toValue;
                }
            }
            else if (type == typeof(bool))
            {
                if (bool.TryParse(value, out bool toValue))
                {
                    return (T)(object)toValue;
                }
            }
            else if (type == typeof(int))
            {
                if (int.TryParse(value, out int toValue))
                {
                    return (T)(object)toValue;
                }
            }
            else if (type == typeof(long))
            {
                if (long.TryParse(value, out long toValue))
                {
                    return (T)(object)toValue;
                }
            }
            else if (type == typeof(short))
            {
                if (short.TryParse(value, out short toValue))
                {
                    return (T)(object)toValue;
                }
            }
            else if (type == typeof(string))
            {
                return (T)(object)value.ToString();
            }
            else if (type == typeof(decimal))
            {
                if (decimal.TryParse(value, out decimal toValue))
                {
                    return (T)(object)toValue;
                }
            }
            else if (type == typeof(float))
            {
                if (float.TryParse(value, out float toValue))
                {
                    return (T)(object)toValue;
                }
            }
            else if (type == typeof(double))
            {
                if (double.TryParse(value, out double toValue))
                {
                    return (T)(object)toValue;
                }
            }
            else if (type.IsClass)
            {
                try
                {
                    return Helper.JsonHelper.Deserialize<T>(value);
                }
                catch (Exception)
                {

                }
            }
            else if (type.IsEnum)
            {
                var result = EnumHelper.GetEnum(type, value);
                if (result != null)
                {
                    return (T)result;
                }
            }

            return default(T);

        }

        public static bool IsNumber(this object value)
        {
            return value is sbyte
                    || value is byte
                    || value is short
                    || value is ushort
                    || value is int
                    || value is uint
                    || value is long
                    || value is ulong
                    || value is float
                    || value is double
                    || value is decimal;
        }
    }

}