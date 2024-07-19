//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Kooboo.IndexedDB.Query
{
    public class ColumnMethodCallEvaluator : ColumnEvaluator
    {
        public Func<byte[], object> GetColumnValue;

        public Func<object, bool> Check;


        public override bool isMatch(byte[] columnbytes)
        {
            object value = GetColumnValue(columnbytes);
            return Check(value);
        }

        public static ColumnMethodCallEvaluator GetMethodEvaluator(Type datatype, int columnLength, MethodCallExpression CallExpression)
        {
            ColumnMethodCallEvaluator evaluator = new ColumnMethodCallEvaluator();
            evaluator.GetColumnValue = Serializer.Simple.ConverterHelper.GetBytesToValue(datatype);

            if (evaluator.GetColumnValue == null)
            {
                throw new Exception(datatype.Name + " type does not supported to be used as a parameter yet.");
            }

            if (CallExpression.Method.ReturnType != typeof(bool))
            {
                throw new Exception("Call method must return a boolean");
            }

            List<System.Linq.Expressions.Expression> arguments = new List<System.Linq.Expressions.Expression>();
            ParameterExpression objectPara = System.Linq.Expressions.Expression.Parameter(typeof(object));
            foreach (var item in CallExpression.Arguments)
            {
                if (item.NodeType == ExpressionType.MemberAccess)
                {
                    var expression = System.Linq.Expressions.Expression.Convert(objectPara, datatype);
                    arguments.Add(expression);
                }
                else
                {
                    arguments.Add(item);
                }
            }

            MethodCallExpression newexpression = System.Linq.Expressions.Expression.Call(CallExpression.Method, arguments.ToArray());

            evaluator.Check = System.Linq.Expressions.Expression.Lambda<Func<object, bool>>(newexpression, objectPara).Compile();

            return evaluator;
        }


    }
}
