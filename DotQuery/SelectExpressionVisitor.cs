using System.Linq.Expressions;

namespace DotQuery;

internal class SelectExpressionVisitor : ExpressionVisitor
{
    private int _projectionCount;
    private readonly Dictionary<string, string> _aliases = new();

    private readonly SqlFormattableStringBuilder _builder = new();

    public SqlFormattableString Build() => _builder.Build();

    public void VisitSelectMethod(MethodCallExpression node)
    {
        foreach(var argument in node.Arguments)
        {
            Visit(argument);
        }
    }

    protected override Expression VisitConstant(ConstantExpression node)
    {
        if (node.Value is LambdaExpression lambda)
        {
            // Visit the body of the lambda expression
            Visit(lambda.Body);

            // Optionally, visit the parameters of the lambda
            foreach (var parameter in lambda.Parameters)
            {
                Visit(parameter);
            }
        }

        return node;
    }

    protected override Expression VisitNew(NewExpression node)
    {
        var ctorParams = (node.Constructor?.GetParameters() ?? [])
            .Select((p, index) => (index, name: p.Name))
            .Where(p => p.name is not null)
            .Select(p => (p.index, name: p.name!))
            .ToDictionary(x => x.index, x => x.name);

        foreach(var (index, argument) in node.Arguments.OfType<MemberExpression>().Select((arg, i) => (i, arg.Member.Name)))
        {
            if (ctorParams.TryGetValue(index, out var value))
            {
                _aliases[argument] = value;
            }
        }

        return base.VisitNew(node);
    }

    protected override Expression VisitMember(MemberExpression node)
    {
        if (_projectionCount > 0)
        {
            _builder.AppendRaw(", ");
        }
        _builder.AppendRaw($"{node.Member.Name}");
        if (_aliases.TryGetValue(node.Member.Name, out var alias) && alias != node.Member.Name)
        {
            _builder.AppendRaw($" as {alias}");
        }
        _projectionCount++;

        return node;
    }
}