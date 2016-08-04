using System;
using System.Linq.Expressions;

namespace Liquid.Expressions
{
    public static class DynamicExtensions
    {
        public static string LambdaToString<T>(this Expression<Func<T, bool>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            return expression.UpdateLocalVariables()
                             .ToString()
                             .UpdateOperators();
        }

        public static Expression<Func<T, bool>> StringToLambda<T>(this string expression)
        {
            if (string.IsNullOrEmpty(expression) || expression.Length < 3)
            {
                throw new ArgumentNullException("expression");
            }

            Expression<Func<T, bool>> exp;

            expression = expression.UpdateOperators();

            if (expression.Contains("=>"))
            {
                string param = expression.Substring(0, expression.IndexOf("=>")).Trim();
                string lambda = expression.Substring(expression.IndexOf("=>") + 2).Trim();
                ParameterExpression pex = Expression.Parameter(typeof(T), param);
                exp = (Expression<Func<T, bool>>)System.Linq.Dynamic.DynamicExpression.ParseLambda(new[] { pex }, typeof(bool), lambda);
            }
            else {
                exp = System.Linq.Dynamic.DynamicExpression.ParseLambda<T, bool>(expression, typeof(T));
            }

            return exp;
        }

        internal static string UpdateOperators(this string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
            {
                throw new ArgumentNullException("expression");
            }

            return expression.Replace("AndAlso", "&&")
                             .Replace("OrElse", "||")
                             .Replace(",", ".");
        }

        public static Expression<Func<T, bool>> UpdateLocalVariables<T>(this Expression<Func<T, bool>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            return (Expression<Func<T, bool>>)new LocalVariableUpdater().Visit(expression);
        }
    }
}