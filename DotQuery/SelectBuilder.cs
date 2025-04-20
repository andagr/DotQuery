using System.Linq.Expressions;

namespace DotQuery;

public class SelectBuilder<T, TSelection> : IQueryBuilder
{
    private readonly FromBuilder<T> _fromBuilder;
    private readonly WhereBuilder<T>? _whereBuilder;
    private readonly Expression<Func<T, TSelection>> _selector;

    public SelectBuilder(
        FromBuilder<T> fromBuilder,
        WhereBuilder<T>? whereBuilder,
        Expression<Func<T, TSelection>> selector)
    {
        _fromBuilder = fromBuilder;
        _whereBuilder = whereBuilder;
        _selector = selector;
    }

    public SqlFormattableString Build()
    {
        var projectionVisitor = new ProjectionExpressionVisitor();
        projectionVisitor.Visit(_selector.Body);

        var builder = new SqlFormattableStringBuilder()
            .AppendRaw("select ").AppendLine(projectionVisitor.Build())
            .AppendRaw("from ").AppendLine(_fromBuilder.FromStatement);

        if (_whereBuilder is not null)
        {
            var whereVisitor = new PredicateExpressionVisitor();
            whereVisitor.Visit(_whereBuilder.Predicate.Body);
            builder.AppendRaw("where ").Append(whereVisitor.Build());
        }

        return builder.Build();
    }
}