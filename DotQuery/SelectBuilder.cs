using System.Linq.Expressions;

namespace DotQuery;

public class SelectBuilder<T, TSelection> : IQueryBuilder
{
    private readonly FromBuilder<T> _fromBuilder;
    private readonly WhereBuilder<T> _whereBuilder;
    private readonly Expression<Func<T, TSelection>> _selector;

    public SelectBuilder(
        FromBuilder<T> fromBuilder,
        WhereBuilder<T> whereBuilder,
        Expression<Func<T, TSelection>> selector)
    {
        _fromBuilder = fromBuilder;
        _whereBuilder = whereBuilder;
        _selector = selector;
    }

    public SqlFormattableString Build()
    {
        var projectionVisitor = new TSqlExpressionVisitor();
        projectionVisitor.Visit(_selector.Body);

        var whereVisitor = new TSqlExpressionVisitor();
        whereVisitor.Visit(_whereBuilder.Predicate.Body);

        return new SqlFormattableStringBuilder()
            .AppendRaw("select ").AppendLine(projectionVisitor.Build())
            .AppendRaw("from ").AppendLine(_fromBuilder.FromStatement)
            .AppendRaw("where ").Append(whereVisitor.Build())
            .Build();
    }
}