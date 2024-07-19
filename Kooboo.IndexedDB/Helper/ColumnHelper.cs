//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Linq.Expressions;
using Kooboo.IndexedDB.Columns;

namespace Kooboo.IndexedDB.Helper
{
    public static class ColumnHelper
    {
        public static IColumn<T> GetColumn<T>(string FieldName, Type FieldType, int len = 0)
        {
            if (FieldType == null)
            {
                throw new Exception("Field Type is required");
            }
            if (FieldType == typeof(string))
            {
                return new ColumnBase<T, string>(FieldName, len);
            }
            else if (FieldType == typeof(Int32))
            {
                return new ColumnBase<T, int>(FieldName);
            }
            else if (FieldType == typeof(Int64))
            {
                return new ColumnBase<T, Int64>(FieldName);
            }
            else if (FieldType == typeof(Int16))
            {
                return new ColumnBase<T, Int16>(FieldName);
            }
            else if (FieldType == typeof(decimal))
            {
                return new ColumnBase<T, decimal>(FieldName);
            }
            else if (FieldType == typeof(double))
            {
                return new ColumnBase<T, double>(FieldName);
            }
            else if (FieldType == typeof(float))
            {
                return new ColumnBase<T, float>(FieldName);
            }
            else if (FieldType == typeof(DateTime))
            {
                return new DateTimeColumn<T>(FieldName);
            }
            else if (FieldType == typeof(Guid))
            {
                return new ColumnBase<T, Guid>(FieldName);
            }
            else if (FieldType == typeof(byte))
            {
                return new ColumnBase<T, byte>(FieldName);
            }
            else if (FieldType == typeof(bool))
            {
                return new ColumnBase<T, bool>(FieldName);
            }
            else if (FieldType.IsEnum)
            {
                return new EnumColumn<T>(FieldName, FieldType);
            }
            else
            {
                throw new Exception(FieldType.ToString() + " data type not supported");
            }

        }

        public static IColumn<T> GetColumn<T>(Expression<Func<T, object>> expression, int len = 0)
        {
            string fieldname = string.Empty;
            Type type;

            if (expression.Body is MemberExpression)
            {
                var exp = (MemberExpression)expression.Body;
                fieldname = exp.Member.Name;
                type = GetType(exp);
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
                return prop.PropertyType;
            }
            else if (x.MemberType == System.Reflection.MemberTypes.Field)
            {
                var field = x as System.Reflection.FieldInfo;
                return field.FieldType;
            }
            return null;
        }

    }
}
