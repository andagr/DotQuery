using System.Linq.Expressions;

namespace DotQuery;

public class FromBuilder<T> : IQueryBuilder
{
    internal SqlFormattableString FromStatement { get; } = new SqlFormattableStringBuilder().AppendRaw(typeof(T).Name).Build();

    public WhereBuilder<T> Where(Expression<Func<T, bool>> predicate) => new(this, predicate);

    public SqlFormattableString Build() =>
        new SqlFormattableStringBuilder()
            .AppendRawLine("select *")
            .AppendRaw("from ")
            .AppendLine(FromStatement)
            .Build();
}