using System.Linq.Expressions;

namespace DotQuery;

public class SelectBuilder<T, TSelection> : QueryBuilder
{
    public SelectBuilder(WhereBuilder<T> whereBuilder, Expression<Func<T, TSelection>> selector)
    {
        throw new NotImplementedException();
    }

    public override string BuildSql() => throw new NotImplementedException();
}