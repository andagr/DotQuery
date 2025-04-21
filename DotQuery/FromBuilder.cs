using System.Linq.Expressions;

namespace DotQuery;

public class FromBuilder<T> : IQueryBuilder
{
    internal Expression SqlExpression { get; }

    internal FromBuilder()
    {
        SqlExpression = Expression.Call(
            typeof(DotQuery),
            nameof(DotQuery.From),
            [typeof(T)]);
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