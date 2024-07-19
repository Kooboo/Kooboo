//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Linq.Expressions;

namespace Kooboo.IndexedDB.Helper
{
    public static class ExpressionHelper
    {

        public static string GetFieldName<TValue>(Expression<Func<TValue, object>> expression)
        {
            string fieldname = string.Empty;

            if (expression.Body is MemberExpression)
                fieldname = ((MemberExpression)expression.Body).Member.Name;
            else if (expression.Body is UnaryExpression)
                fieldname = ((MemberExpression)((UnaryExpression)expression.Body).Operand).Member.Name;
            else
            {
                throw new ArgumentException("Expression must represent field or property access.");
            }

            return fieldname;
        }

    }
}
