using System.Linq.Expressions;

namespace DotQuery;

public class FromBuilder<T> : IQueryBuilder
{
    internal SqlFormattableString FromStatement { get; } = new(typeof(T).Name, []);

    public WhereBuilder<T> Where(Expression<Func<T, bool>> predicate) => new(this, predicate);

    public SelectBuilder<T, TResult> Select<TResult>(Expression<Func<T, TResult>> selector) =>
        new(this, null, selector);

    public SqlFormattableString Build() =>
        new SqlFormattableStringBuilder()
            .AppendRawLine("select *")
            .AppendRaw("from ").Append(FromStatement)
            .Build();
}