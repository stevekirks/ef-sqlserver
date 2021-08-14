using EfSqlServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EfSqlServer.Helpers
{
    public static class ExpressionExtensions
    {
        public static IQueryable<T> WhereAny<T>(this IQueryable<T> query, List<Expression<Func<T, bool>>> orExpressions)
        {
            return query.Where(OrElse(orExpressions));
        }

        private static Expression<Func<T, bool>> OrElse<T>(List<Expression<Func<T, bool>>> orExpressions)
        {
            Expression<Func<T, bool>> combinedExpression = null;
            foreach (var orExpression in orExpressions)
            {
                if (combinedExpression == null)
                {
                    combinedExpression = orExpression;
                }
                else
                {
                    combinedExpression = OrElse(combinedExpression, orExpression);
                }
            }
            return combinedExpression;
        }

        private static Expression<Func<T, bool>> OrElse<T>(
            this Expression<Func<T, bool>> expr1,
            Expression<Func<T, bool>> expr2)
        {
            var parameter = Expression.Parameter(typeof(T));

            var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
            var left = leftVisitor.Visit(expr1.Body);

            var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
            var right = rightVisitor.Visit(expr2.Body);

            return Expression.Lambda<Func<T, bool>>(
                Expression.OrElse(left, right), parameter);
        }

        private class ReplaceExpressionVisitor : ExpressionVisitor
        {
            private readonly Expression _oldValue;
            private readonly Expression _newValue;

            public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
            {
                _oldValue = oldValue;
                _newValue = newValue;
            }

            public override Expression Visit(Expression node)
            {
                if (node == _oldValue)
                    return _newValue;
                return base.Visit(node);
            }
        }
    }
}
