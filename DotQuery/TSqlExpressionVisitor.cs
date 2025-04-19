using System.Linq.Expressions;

namespace DotQuery;

// ReSharper disable once InconsistentNaming
internal class TSqlExpressionVisitor : ExpressionVisitor
{
    private readonly SqlFormattableStringBuilder _builder = new();

    protected override Expression VisitBinary(BinaryExpression node)
    {
        Visit(node.Left);

        switch (node.NodeType)
        {
            case ExpressionType.Equal:
                _builder.AppendRaw(" = ");
                break;
            case ExpressionType.NotEqual:
                _builder.AppendRaw(" <> ");
                break;
            default:
                throw new NotSupportedException($"Unsupported binary operator: {node.NodeType}");
        }

        Visit(node.Right);
        return node;
    }

    protected override Expression VisitMember(MemberExpression node)
    {
        _builder.AppendRaw($"{node.Member.Name}");
        return node;
    }

    protected override Expression VisitConstant(ConstantExpression node)
    {
        _builder.Append($"{node.Value}");
        return node;
    }

    public SqlFormattableString Build() => _builder.Build();
}