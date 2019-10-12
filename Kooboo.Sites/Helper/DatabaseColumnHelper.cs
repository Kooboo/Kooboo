using System;

namespace Kooboo.Sites.Helper
{
    public static class DatabaseColumnHelper
    {
        public static string ToFrontEndDataType(Type clrType)
        {
            // string, number, datetime, bool, undefined
            if (clrType == typeof(Int16) || clrType == typeof(Int32) || clrType == typeof(Int64) || clrType == typeof(byte))
            {
                return "number";
            }
            else if (clrType == typeof(decimal) || clrType == typeof(double) || clrType == typeof(float))
            {
                return "number";
            }
            else if (clrType == typeof(DateTime))
            {
                return "datetime";
            }
            else if (clrType == typeof(bool))
            {
                return "bool";
            }
            else if (clrType == typeof(string) || clrType == typeof(Guid))
            {
                return "string";
            }
            return "string";
        }

        public static Type ToClrType(string type)
        {
            string lower = type.ToLower();

            if (lower == "datetime")
            {
                return typeof(System.DateTime);
            }
            else if (lower == "number")
            {
                return typeof(System.Double);
            }
            else if (lower == "bool")
            {
                return typeof(bool);
            }
            else if (lower == "string")
            {
                return typeof(string);
            }
            return typeof(string);
        }
    }
}