//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Kooboo.IndexedDB.Query
{
    /// <summary>
    /// Use the filter to select records. 
    /// NOTE: only && operator is supported now, || operator is not supported for the time being.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class WhereFilter<TKey, TValue>
    {
        ObjectStore<TKey, TValue> store;

        private string OrderByFieldName;
        private int skip;

        public Filter<TKey, TValue> filter;

        public WhereFilter(ObjectStore<TKey, TValue> store, Expression<Predicate<TValue>> predicate)
        {
            this.store = store;
            this.skip = 0;
            filter = store.Filter;
            LambdaExpression lambda = predicate as LambdaExpression;
            ParseLambda(lambda);
        }


        public WhereFilter(ObjectStore<TKey, TValue> store)
        {
            this.store = store;
            this.skip = 0;
            filter = store.Filter;
        }


        public WhereFilter<TKey, TValue> Where(Expression<Predicate<TValue>> predicate)
        {
            LambdaExpression lambda = predicate as LambdaExpression;
            ParseLambda(lambda);
            return this;
        }

        public WhereFilter<TKey, TValue> WhereIn(string FieldOrPropertyName, List<object> Values)
        {
            this.filter.WhereIn(FieldOrPropertyName, Values);
            return this;
        }

        public WhereFilter<TKey, TValue> WhereIn<T>(string FieldOrPropertyName, List<T> Values)
        {
            this.filter.WhereIn<T>(FieldOrPropertyName, Values);
            return this;
        }

        public WhereFilter<TKey, TValue> WhereIn<T>(Expression<Func<TValue, object>> FieldExpression, List<T> Values)
        {
            string fieldname = Helper.ExpressionHelper.GetFieldName<TValue>(FieldExpression);

            if (!string.IsNullOrEmpty(fieldname))
            {
                this.filter.WhereIn<T>(fieldname, Values);
            }

            return this;
        }


        public bool Exists()
        {
            return this.filter.Exists();
        }


        /// <summary>
        /// Order by the primary key.
        /// </summary>
        public WhereFilter<TKey, TValue> OrderByAscending()
        {
            this.filter.OrderByAscending();
            return this;
        }

        /// <summary>
        /// Order by a field or property. This field should have an index on it. 
        /// Order by a non-indexed field will have very bad performance. 
        /// </summary>
        public WhereFilter<TKey, TValue> OrderByAscending(string FieldOrPropertyName)
        {
            this.OrderByFieldName = FieldOrPropertyName;

            this.filter.OrderByAscending(FieldOrPropertyName);
            return this;
        }


        public WhereFilter<TKey, TValue> OrderByAscending(Expression<Func<TValue, object>> expression)
        {
            string fieldname = Helper.ExpressionHelper.GetFieldName<TValue>(expression);

            if (!string.IsNullOrEmpty(fieldname))
            {
                return OrderByAscending(fieldname);
            }

            return this;
        }


        /// <summary>
        /// Order by descending based on the primary key.
        /// </summary>
        public WhereFilter<TKey, TValue> OrderByDescending()
        {
            this.filter.OrderByDescending();
            return this;
        }

        /// <summary>
        /// Order by descending on a field or property. This field should have an index on it. 
        /// </summary>
        public WhereFilter<TKey, TValue> OrderByDescending(string FieldOrPropertyName)
        {
            this.OrderByFieldName = FieldOrPropertyName;
            this.filter.OrderByDescending(FieldOrPropertyName);
            return this;
        }


        public WhereFilter<TKey, TValue> OrderByDescending(Expression<Func<TValue, object>> expression)
        {
            string fieldname = Helper.ExpressionHelper.GetFieldName<TValue>(expression);

            if (!string.IsNullOrEmpty(fieldname))
            {
                return OrderByDescending(fieldname);
            }

            return this;
        }


        /// <summary>
        /// use column data to fill in the return TValue object. 
        /// The TValue object must have a parameterless constructor. 
        /// </summary>
        public WhereFilter<TKey, TValue> UseColumnData()
        {
            this.filter.UseColumnData();
            return this;
        }

        public WhereFilter<TKey, TValue> Skip(int count)
        {
            this.skip = count;
            this.filter.Skip(count);
            return this;
        }

        public TValue FirstOrDefault()
        {
            return this.filter.FirstOrDefault();
        }


        public List<TValue> SelectAll()
        {
            return Take(99999);
        }


        public int Count()
        {
            return this.filter.Count();
        }

        public List<TValue> Take(int count)
        {
            return filter.Take(count);
        }

        private void ParseLambda(LambdaExpression lambda)
        {
            ParseExpression(lambda.Body);
        }

        private void ParseAndAlso(System.Linq.Expressions.Expression expression)
        {
            BinaryExpression binary = expression as BinaryExpression;

            ParseExpression(binary.Left);
            ParseExpression(binary.Right);
        }

        private void ParseExpression(System.Linq.Expressions.Expression expression)
        {
            if (expression.NodeType == ExpressionType.Equal)
            {
                ParseComparer(expression, Comparer.EqualTo);
            }
            else if (expression.NodeType == ExpressionType.GreaterThan)
            {
                ParseComparer(expression, Comparer.GreaterThan);
            }
            else if (expression.NodeType == ExpressionType.NotEqual)
            {
                ParseComparer(expression, Comparer.NotEqualTo);
            }

            else if (expression.NodeType == ExpressionType.LessThan)
            {
                ParseComparer(expression, Comparer.LessThan);
            }
            else if (expression.NodeType == ExpressionType.LessThanOrEqual)
            {
                ParseComparer(expression, Comparer.LessThanOrEqual);
            }
            else if (expression.NodeType == ExpressionType.GreaterThanOrEqual)
            {
                ParseComparer(expression, Comparer.GreaterThanOrEqual);
            }
            else if (expression.NodeType == ExpressionType.AndAlso)
            {
                ParseAndAlso(expression);
            }
            else if (expression.NodeType == ExpressionType.MemberAccess)
            {
                ParseMemberAccess(expression);
            }
            else if (expression.NodeType == ExpressionType.Not)
            {
                ParseNotExpression(expression);
            }
            else if (expression.NodeType == ExpressionType.Call)
            {
                ParseCallExpresion(expression);
            }
            else
            {
                var msg = "expression not supported " + expression.NodeType.ToString();
                throw new Exception(msg);
            }
        }

        private void ParseMemberAccess(System.Linq.Expressions.Expression expression)
        {
            MemberExpression member = expression as MemberExpression;
            string name = member.Member.Name;
            this.filter.WhereEqual(name, true);
        }

        private void ParseCallExpresion(System.Linq.Expressions.Expression expression)
        {
            MethodCallExpression call = expression as MethodCallExpression;
            this.filter.MethodCall(call);
        }

        private void ParseNotExpression(System.Linq.Expressions.Expression expression)
        {
            UnaryExpression unary = expression as UnaryExpression;
            if (unary.Operand.NodeType == ExpressionType.MemberAccess)
            {
                MemberExpression member = unary.Operand as MemberExpression;
                string name = member.Member.Name;

                this.filter.WhereEqual(name, false);
            }
            else
            {
                throw new Exception("more complicate condition is not supported yet");
            }
        }

        private void ParseComparer(System.Linq.Expressions.Expression expression, Comparer compare)
        {
            BinaryExpression binary = expression as BinaryExpression;
            string name = string.Empty;
            if (binary.Left.NodeType == ExpressionType.MemberAccess)
            {
                MemberExpression member = binary.Left as MemberExpression;
                name = member.Member.Name;
            }
            else if (binary.Left.NodeType == ExpressionType.Convert)
            {
                UnaryExpression unary = binary.Left as UnaryExpression;
                MemberExpression member = unary.Operand as MemberExpression;
                name = member.Member.Name;
            }

            object constatvalue;
            if (binary.Right.NodeType == ExpressionType.Constant)
            {
                ConstantExpression value = binary.Right as ConstantExpression;
                constatvalue = value.Value;
            }
            else if (binary.Right.NodeType == ExpressionType.MemberAccess)
            {
                constatvalue = System.Linq.Expressions.Expression
                    .Lambda<Func<object>>(System.Linq.Expressions.Expression.Convert(binary.Right, typeof(object)))
                    .Compile().Invoke();
            }
            else if (binary.Right.NodeType == ExpressionType.Convert)
            {
                UnaryExpression unary = binary.Right as UnaryExpression;
                MemberExpression member = unary.Operand as MemberExpression;

                constatvalue = System.Linq.Expressions.Expression
                    .Lambda<Func<object>>(System.Linq.Expressions.Expression.Convert(member, typeof(object))).Compile()
                    .Invoke();
            }
            else
            {
                throw new Exception("operation not supported yet, please report " + binary.Right.NodeType.ToString());
            }


            this.filter.Where(name, compare, constatvalue);
        }


        private object GetExpressionValue(System.Linq.Expressions.Expression member)
        {
            var objectMember = System.Linq.Expressions.Expression.Convert(member, typeof(object));

            var getterLambda = System.Linq.Expressions.Expression.Lambda<Func<object>>(objectMember);

            var getter = getterLambda.Compile();

            return getter();
        }


        private object GetValue(MemberExpression member)
        {
            var objectMember = System.Linq.Expressions.Expression.Convert(member, typeof(object));

            var getterLambda = System.Linq.Expressions.Expression.Lambda<Func<object>>(objectMember);

            var getter = getterLambda.Compile();

            return getter();
        }
    }
}