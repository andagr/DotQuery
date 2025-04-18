using System.Runtime.CompilerServices;

namespace DotQuery;

public abstract class QueryBuilder
{
    protected List<object> Parameters { get; } = [];

    public abstract string BuildSql();

    public FormattableString Build()
    {
        string sql = BuildSql();
        return FormattableStringFactory.Create(sql, Parameters.ToArray());
    }
}