using System.Linq.Expressions;

namespace DotQuery;

public class WhereBuilder<T> : IQueryBuilder
{
    private readonly FromBuilder<T> _fromBuilder;
    private readonly Expression<Func<T, bool>> _predicate;

    public WhereBuilder(FromBuilder<T> fromBuilder, Expression<Func<T, bool>> predicate)
    {
        _fromBuilder = fromBuilder;
        _predicate = predicate;
    }

    public SelectBuilder<T, TResult> Select<TResult>(Expression<Func<T, TResult>> selector)
    {
        return new SelectBuilder<T, TResult>(_fromBuilder, this, selector);
    }

    public SqlFormattableString Build()
    {
        var visitor = new TSqlExpressionVisitor();
        visitor.Visit(_predicate.Body);

        return new SqlFormattableStringBuilder()
            .AppendRawLine("select *")
            .AppendRaw("from ")
            .AppendLine(_fromBuilder.FromStatement)
            .AppendRaw("where ")
            .Append(visitor.Build())
            .Build();
    }
}