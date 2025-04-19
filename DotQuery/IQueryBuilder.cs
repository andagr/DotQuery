namespace DotQuery;

public interface IQueryBuilder
{
    SqlFormattableString Build();
}