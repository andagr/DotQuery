using System.Linq.Expressions;

namespace DotQuery;

// ReSharper disable once InconsistentNaming
internal class ProjectionExpressionVisitor : ExpressionVisitor
{
    private readonly SqlFormattableStringBuilder _builder = new();

    protected override Expression VisitMember(MemberExpression node)
    {
        _builder.AppendRaw($"{node.Member.Name}");
        return node;
    }

    public SqlFormattableString Build() => _builder.Build();
}