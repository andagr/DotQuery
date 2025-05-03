using System.Linq.Expressions;

namespace DotQuery;

internal class SqlExpressionVisitor : ExpressionVisitor
{
    private readonly SelectExpressionVisitor _selectExpressionVisitor = new();
    private readonly FromExpressionVisitor _fromExpressionVisitor = new();
    private readonly WhereExpressionVisitor _whereExpressionVisitor = new();

    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        if (node.Method.ReturnType.GetGenericTypeDefinition() == typeof(FromBuilder<>))
        {
            _fromExpressionVisitor.HandleFrom(node);
        }
        else if (node.Method.ReturnType.GetGenericTypeDefinition() == typeof(WhereBuilder<>))
        {
            _whereExpressionVisitor.HandleWhere(node);
        }
        else if (node.Method.ReturnType.GetGenericTypeDefinition() == typeof(SelectBuilder<,>))
        {
            _selectExpressionVisitor.HandleSelect(node);
        }

        return node.Object is not null ? Visit(node.Object) : node;
    }

    public SqlFormattableString Build()
    {
        var sqlBuilder = new SqlFormattableStringBuilder();
        var projections = _selectExpressionVisitor.Build();
        var fromTable = _fromExpressionVisitor.FromTable;
        var predicates = _whereExpressionVisitor.Build();

        sqlBuilder.AppendRaw("select ");
        if (string.IsNullOrEmpty(projections.Format))
        {
            sqlBuilder.AppendRawLine("*");
        }
        else
        {
            sqlBuilder.AppendLine(projections);
        }

        sqlBuilder.AppendRaw("from ").AppendRaw(fromTable);

        if (!string.IsNullOrEmpty(predicates.Format))
        {
            sqlBuilder
                .AppendRawLine()
                .AppendRaw("where ").Append(predicates);
        }

        return sqlBuilder.Build();
    }
}