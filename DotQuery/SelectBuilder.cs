using System.Linq.Expressions;

namespace DotQuery;

public class SelectBuilder<T, TSelection> : IQueryBuilder
{
    public SelectBuilder(WhereBuilder<T> whereBuilder, Expression<Func<T, TSelection>> selector)
    {
        throw new NotImplementedException();
    }

    public SqlFormattableString Build() => throw new NotImplementedException();
}