using System.Runtime.CompilerServices;
using System.Text;

namespace DotQuery;

internal class SqlFormattableStringBuilder
{
    private readonly StringBuilder _format = new();
    private readonly List<object?> _arguments = [];

    public SqlFormattableStringBuilder Append(SqlFormattableString formattableString)
    {
        var bumpedPosArgs = formattableString.GetArguments()
            .Select((_, i) => $"{{{i + _arguments.Count}}}")
            .Cast<object>()
            .ToArray();
        _format.Append(string.Format(formattableString.Format, bumpedPosArgs));
        _arguments.AddRange(formattableString.GetArguments());
        return this;
    }

    public SqlFormattableStringBuilder AppendLine(SqlFormattableString formattableString)
    {
        Append(formattableString);
        _format.AppendLine();
        return this;
    }

    public SqlFormattableStringBuilder Append([InterpolatedStringHandlerArgument] SqlFormattableStringHandler handler) =>
        Append(new SqlFormattableString(handler.Format, handler.Arguments));

    public SqlFormattableStringBuilder AppendRaw(string value)
    {
        _format.Append(value);
        return this;
    }

    public SqlFormattableStringBuilder AppendRawLine(string? value = null)
    {
        _format.AppendLine(value);
        return this;
    }

    public SqlFormattableString Build()
    {
        var sql = _format.ToString();
        return new SqlFormattableString(sql, _arguments.ToArray());
    }
}