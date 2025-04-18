using System.Linq.Expressions;
using System.Text;

namespace DotQuery;

public class TSqlExpressionVisitor : ExpressionVisitor
{
    private readonly List<object> _parameters;
    private StringBuilder _sql = new StringBuilder();

    public TSqlExpressionVisitor(List<object> parameters)
    {
        _parameters = parameters;
    }

    protected override Expression VisitBinary(BinaryExpression node)
    {
        Visit(node.Left);

        switch (node.NodeType)
        {
            case ExpressionType.Equal:
                _sql.Append(" = ");
                break;
            case ExpressionType.NotEqual:
                _sql.Append(" <> ");
                break;
            // Handle other operators
        }

        Visit(node.Right);
        return node;
    }

    protected override Expression VisitMember(MemberExpression node)
    {
        // Handle property access, converting to column names
        _sql.Append(GetColumnName(node));
        return node;
    }

    protected override Expression VisitConstant(ConstantExpression node)
    {
        // Create a parameter placeholder and add the value
        _parameters.Add(node.Value);
        _sql.Append($"{{{_parameters.Count - 1}}}");
        return node;
    }

    public override string ToString() => _sql.ToString();
}