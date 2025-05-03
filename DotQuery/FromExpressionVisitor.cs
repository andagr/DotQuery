using System.Linq.Expressions;

namespace DotQuery;

internal class FromExpressionVisitor : ExpressionVisitor
{
    public string FromStatement { get; private set; } = "";

    public void HandleFrom(MethodCallExpression node)
    {
        var tableName = node.Method.GetGenericArguments().Single().Name;
        var alias = tableName.ToLowerInvariant().First();
        FromStatement = $"[{tableName}] as [{alias}]";
    }
}
