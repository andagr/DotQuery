using System.Linq.Expressions;

namespace DotQuery;

public class FromBuilder<T> : QueryBuilder
{
    private readonly string _tableName;

    public FromBuilder()
    {
        _tableName = typeof(T).Name; // Could be customized with attributes
    }

    public WhereBuilder<T> Where(Expression<Func<T, bool>> predicate)
    {
        return new WhereBuilder<T>(this, predicate);
    }

    public override string BuildSql()
    {
        return $"SELECT * FROM {_tableName}";
    }
}