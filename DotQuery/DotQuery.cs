namespace DotQuery;

public static class DotQuery
{
    public static FromBuilder<T> From<T>() => new();
}