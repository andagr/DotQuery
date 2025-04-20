using System.Linq.Expressions;

namespace DotQuery;

// ReSharper disable once InconsistentNaming
internal class PredicateExpressionVisitor : ExpressionVisitor
{
    private readonly SqlFormattableStringBuilder _builder = new();

    protected override Expression VisitBinary(BinaryExpression node)
    {
        var binaryOperators = new Dictionary<ExpressionType, string>
        {
            [ExpressionType.Equal] = "=",
            [ExpressionType.NotEqual] = "<>",
            [ExpressionType.GreaterThan] = ">",
            [ExpressionType.GreaterThanOrEqual] = ">=",
            [ExpressionType.LessThan] = "<",
            [ExpressionType.LessThanOrEqual] = "<=",
            [ExpressionType.AndAlso] = "and",
            [ExpressionType.OrElse] = "or"
        };

        if (node.Left is BinaryExpression { NodeType: ExpressionType.OrElse })
        {
            _builder.AppendRaw("(");
            Visit(node.Left);
            _builder.AppendRaw(")");
        }
        else
        {
            Visit(node.Left);
        }

        if (binaryOperators.TryGetValue(node.NodeType, out var op))
        {
            _builder.AppendRaw($" {op} ");
        }
        else
        {
            throw new NotSupportedException($"Unsupported binary operator: {node.NodeType}");
        }

        if (node.Right is BinaryExpression { NodeType: ExpressionType.OrElse })
        {
            _builder.AppendRaw("(");
            Visit(node.Right);
            _builder.AppendRaw(")");
        }
        else
        {
            Visit(node.Right);
        }

        return node;
    }

    protected override Expression VisitUnary(UnaryExpression node)
    {
        if (node.NodeType != ExpressionType.Not)
        {
            return node;
        }

        _builder.AppendRaw("not ");
        if (node.Operand is BinaryExpression { NodeType: ExpressionType.OrElse })
        {
            _builder.AppendRaw("(");
            Visit(node.Operand);
            _builder.AppendRaw(")");
        }
        else
        {
            Visit(node.Operand);
        }

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

    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        if (node.Method.DeclaringType == typeof(Operators) &&
            node.Method.Name == nameof(Operators.In))
        {
            Visit(node.Arguments[0]);
            _builder.AppendRaw(" in (");
            for (var i = 1; i < node.Arguments.Count; i++)
            {
                Visit(node.Arguments[i]);
                if (i < node.Arguments.Count - 1)
                {
                    _builder.AppendRaw(", ");
                }
            }

            _builder.AppendRaw(")");
        }

        return node;
    }

    public SqlFormattableString Build() => _builder.Build();
}