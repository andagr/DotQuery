// ReSharper disable UnusedParameter.Global
namespace DotQuery;

public static class Operators
{
    public static bool In<T>(this T property, T value1, params T[] values)
    {
        throw new NotSupportedException("This method should only be used in DotQuery expressions");
    }

    public static bool In<T>(this T property, T[] values)
    {
        throw new NotSupportedException("This method should only be used in DotQuery expressions");
    }

    public static bool Exists(IQueryBuilder query)
    {
        throw new NotSupportedException("This method should only be used in DotQuery expressions");
    }
}