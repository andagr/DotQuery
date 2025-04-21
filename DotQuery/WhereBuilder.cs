using System.Linq.Expressions;

namespace DotQuery;

public class WhereBuilder<T> : IQueryBuilder
{
    internal Expression SqlExpression { get; }

    internal WhereBuilder(FromBuilder<T> fromBuilder, Expression<Func<T, bool>> predicate)
    {
        SqlExpression = Expression.Call(
            fromBuilder.SqlExpression,
            typeof(FromBuilder<T>).GetMethod(nameof(FromBuilder<T>.Where))!,
            Expression.Constant(predicate));
    }

    private WhereBuilder(WhereBuilder<T> whereBuilder, Expression<Func<T, bool>> predicate)
    {
        SqlExpression = Expression.Call(
            whereBuilder.SqlExpression,
            typeof(WhereBuilder<T>).GetMethod(nameof(Where))!,
            Expression.Constant(predicate));
    }

    public WhereBuilder<T> Where(Expression<Func<T, bool>> predicate) => new(this, predicate);

    public SelectBuilder<T, TResult> Select<TResult>(Expression<Func<T, TResult>> selector) =>
        new(this, selector);

    public SqlFormattableString Build()
    {
        var visitor = new SqlExpressionVisitor();
        visitor.Visit(SqlExpression);
        return visitor.Build();
    }
}