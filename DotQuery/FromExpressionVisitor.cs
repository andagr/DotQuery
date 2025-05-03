using System.Linq.Expressions;

namespace DotQuery;

internal class FromExpressionVisitor : ExpressionVisitor
{
    public string FromTable { get; private set; } = "";

    public void HandleFrom(MethodCallExpression node)
    {
        FromTable = node.Method.GetGenericArguments().Single().Name;
    }
}
