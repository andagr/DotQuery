using System.Globalization;

namespace DotQuery;

public sealed class SqlFormattableString : FormattableString
{
    private readonly object?[] _arguments;

    internal SqlFormattableString(string format, object?[] arguments)
    {
        Format = format;
        _arguments = arguments;
    }

    public override string Format { get; }

    public override object?[] GetArguments() => _arguments;
    public override int ArgumentCount => _arguments.Length;
    public override object? GetArgument(int index) => _arguments[index];

    public override string ToString(IFormatProvider? formatProvider) =>
        throw new NotSupportedException(
            $"{nameof(ToString)} creates a raw, unparameterized, SQL statement, which could " +
            $"be unsafe with regards to SQL injection. Use {nameof(ToRawString)} instead.");

    /// <summary>
    /// Dangerously converts the parameterized SQL statement to a raw string. This should only
    /// be used when you are sure that the SQL statement is safe from SQL injection.
    /// </summary>
    public string ToRawString(IFormatProvider? formatProvider = null)
    {
        return string.Format(formatProvider ?? CultureInfo.CurrentCulture, Format, _arguments);
    }
}