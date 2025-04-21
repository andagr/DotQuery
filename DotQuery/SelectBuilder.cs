using System.Linq.Expressions;

namespace DotQuery;

public class SelectBuilder<T, TSelection> : IQueryBuilder
{
    internal Expression SqlExpression { get; }

    internal SelectBuilder(
        FromBuilder<T> fromBuilder,
        Expression<Func<T, TSelection>> selector)
    {
        SqlExpression = Expression.Call(
            fromBuilder.SqlExpression,
            typeof(FromBuilder<T>)
                .GetMethod(nameof(FromBuilder<T>.Select))!
                .MakeGenericMethod(typeof(TSelection)),
            Expression.Constant(selector));
    }

    internal SelectBuilder(
        WhereBuilder<T> whereBuilder,
        Expression<Func<T, TSelection>> selector)
    {
        SqlExpression = Expression.Call(
            whereBuilder.SqlExpression,
            typeof(WhereBuilder<T>)
                .GetMethod(nameof(WhereBuilder<T>.Select))!
                .MakeGenericMethod(typeof(TSelection)),
            Expression.Constant(selector));
    }

    public SqlFormattableString Build()
    {
        var visitor = new SqlExpressionVisitor();
        visitor.Visit(SqlExpression);
        return visitor.Build();
    }
}