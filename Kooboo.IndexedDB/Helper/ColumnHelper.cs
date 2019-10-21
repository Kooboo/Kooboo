//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.IndexedDB.Columns;
using System;
using System.Linq.Expressions;

namespace Kooboo.IndexedDB.Helper
{
    public static class ColumnHelper
    {
        public static IColumn<T> GetColumn<T>(string fieldName, Type fieldType, int len = 0)
        {
            if (fieldType == null)
            {
                throw new Exception("Field Type is required");
            }
            if (fieldType == typeof(string))
            {
                return new ColumnBase<T, string>(fieldName, len);
            }
            else if (fieldType == typeof(Int32))
            {
                return new ColumnBase<T, int>(fieldName);
            }
            else if (fieldType == typeof(Int64))
            {
                return new ColumnBase<T, Int64>(fieldName);
            }
            else if (fieldType == typeof(Int16))
            {
                return new ColumnBase<T, Int16>(fieldName);
            }
            else if (fieldType == typeof(decimal))
            {
                return new ColumnBase<T, decimal>(fieldName);
            }
            else if (fieldType == typeof(double))
            {
                return new ColumnBase<T, double>(fieldName);
            }
            else if (fieldType == typeof(float))
            {
                return new ColumnBase<T, float>(fieldName);
            }
            else if (fieldType == typeof(DateTime))
            {
                return new DateTimeColumn<T>(fieldName);
            }
            else if (fieldType == typeof(Guid))
            {
                return new ColumnBase<T, Guid>(fieldName);
            }
            else if (fieldType == typeof(byte))
            {
                return new ColumnBase<T, byte>(fieldName);
            }
            else if (fieldType == typeof(bool))
            {
                return new ColumnBase<T, bool>(fieldName);
            }
            else if (fieldType.IsEnum)
            {
                return new EnumColumn<T>(fieldName, fieldType);
            }
            else
            {
                throw new Exception(fieldType.ToString() + " data type not supported");
            }
        }

        public static IColumn<T> GetColumn<T>(Expression<Func<T, object>> expression, int len = 0)
        {
            string fieldname = string.Empty;
            Type type;

            if (expression.Body is MemberExpression exp1)
            {
                fieldname = exp1.Member.Name;
                type = GetType(exp1);
            }
            else if (expression.Body is UnaryExpression)
            {
                var exp = (MemberExpression)((UnaryExpression)expression.Body).Operand;
                fieldname = exp.Member.Name;
                type = GetType(exp);
            }
            else
            {
                throw new ArgumentException("Expression must represent field or property access.");
            }

            return GetColumn<T>(fieldname, type, len);
        }

        private static Type GetType(MemberExpression memberExp)
        {
            var x = memberExp.Member as System.Reflection.MemberInfo;

            if (x.MemberType == System.Reflection.MemberTypes.Property)
            {
                var prop = x as System.Reflection.PropertyInfo;
                return prop?.PropertyType;
            }
            else if (x.MemberType == System.Reflection.MemberTypes.Field)
            {
                var field = x as System.Reflection.FieldInfo;
                return field?.FieldType;
            }
            return null;
        }
    }
}