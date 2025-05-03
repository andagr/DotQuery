using System.Linq.Expressions;
using System.Reflection;

namespace DotQuery;

internal class WhereExpressionVisitor : ExpressionVisitor
{
    private static readonly Dictionary<ExpressionType, string> BinaryOperators = new()
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

    private readonly SqlFormattableStringBuilder _builder = new();
    private readonly List<LambdaExpression> _predicates = [];

    public SqlFormattableString Build()
    {
        if (_predicates.Any())
        {
            var andPredicate = _predicates
                .AsEnumerable()
                // Order is reversed, as evaluation of statements starts from the end, to preserve
                // the same order in the built query as in the order of the where statements.
                .Reverse()
                .Aggregate((p, c) =>
                    Expression.Lambda(
                        Expression.AndAlso(p.Body, c.Body),
                        p.Parameters));
            Visit(andPredicate);
        }

        return _builder.Build();
    }

    public void HandleWhere(MethodCallExpression node)
    {
        if (node.Arguments[0] is ConstantExpression { Value: LambdaExpression lambda })
        {
            _predicates.Add(lambda);
        }
    }

    protected override Expression VisitConstant(ConstantExpression node)
    {
        if (node.Value is LambdaExpression lambda)
        {
            Visit(lambda.Body);
            foreach (var parameter in lambda.Parameters)
            {
                Visit(parameter);
            }
        }
        else
        {
            _builder.Append($"{node.Value}");
        }

        return node;
    }

    protected override Expression VisitBinary(BinaryExpression node)
    {
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

        if (BinaryOperators.TryGetValue(node.NodeType, out var op))
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

    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        if (node.Method.DeclaringType == typeof(Operators))
        {
            if (node.Method.Name == nameof(Operators.In))
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
            else if (node.Method.Name == nameof(Operators.Exists))
            {
                _builder.AppendRaw("exists (");
                var sqlVisitor = new SqlExpressionVisitor();
                sqlVisitor.Visit(node.Arguments[0]);
                var sql = sqlVisitor.Build();
                _builder.Append(sql);
                _builder.AppendRaw(")");
            }
        }

        return node;
    }

    protected override Expression VisitMember(MemberExpression node)
    {
        if (node is { Expression: ConstantExpression constantExpression, Member: FieldInfo fieldInfo })
        {
            // Handle closure: Get the value of the captured variable
            var capturedValueObject = fieldInfo.GetValue(constantExpression.Value);
            if (capturedValueObject is IEnumerable<object> capturedValues)
            {
                var capturedValuesArray = capturedValues.ToArray();
                for (var i = 0; i < capturedValuesArray.Length; i++)
                {
                    _builder.Append($"{capturedValuesArray[i]}");

                    if (i < capturedValuesArray.Length - 1)
                    {
                        _builder.AppendRaw(", ");
                    }
                }
            }
            else
            {
                _builder.Append($"{capturedValueObject}");
            }
        }
        else
        {
            // Handle regular member access
            _builder.AppendRaw($"{node.Member.Name}");
        }

        return node;
    }
}