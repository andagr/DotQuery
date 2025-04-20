using System.Linq.Expressions;

namespace DotQuery;

// ReSharper disable once InconsistentNaming
internal class ProjectionExpressionVisitor : ExpressionVisitor
{
    private readonly SqlFormattableStringBuilder _builder = new();
    private int _projectionCount;
    private Dictionary<string, string> _aliases = new();

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

    public SqlFormattableString Build() => _builder.Build();
}