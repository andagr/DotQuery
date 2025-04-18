using System.Linq.Expressions;

namespace DotQuery;

public class WhereBuilder<T> : QueryBuilder
{
    private readonly FromBuilder<T> _fromBuilder;
    private readonly Expression<Func<T, bool>> _predicate;
    private ExpressionVisitor _visitor;

    public WhereBuilder(FromBuilder<T> fromBuilder, Expression<Func<T, bool>> predicate)
    {
        _fromBuilder = fromBuilder;
        _predicate = predicate;
        _visitor = new TSqlExpressionVisitor(Parameters);
    }

    public SelectBuilder<T, TResult> Select<TResult>(Expression<Func<T, TResult>> selector)
    {
        return new SelectBuilder<T, TResult>(this, selector);
    }

    public override string BuildSql()
    {
        string baseSql = _fromBuilder.BuildSql();
        string whereClause = TranslateWhereExpression(_predicate);
        return $"{baseSql} WHERE {whereClause}";
    }

    private string TranslateWhereExpression(Expression<Func<T, bool>> predicate)
    {
        // This is where you'd parse the expression tree and convert to SQL
        // Using your visitor pattern
        return _visitor.Visit(predicate.Body).ToString();
    }
}