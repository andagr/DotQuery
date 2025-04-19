using System.Runtime.CompilerServices;
using System.Text;
// ReSharper disable UnusedParameter.Local
// ReSharper disable UnusedMember.Global

namespace DotQuery;

[InterpolatedStringHandler]
internal readonly struct SqlFormattableStringHandler
{
    private readonly StringBuilder _format;
    private readonly List<object?> _arguments = [];

    public SqlFormattableStringHandler(int literalLength, int formattedCount) => _format = new StringBuilder();

    public string Format => _format.ToString();
    public object?[] Arguments => _arguments.ToArray();

    public void AppendLiteral(string s)
    {
        _format.Append(s);
    }

    public void AppendFormatted(object? value, int alignment = 0, string? format = null)
    {
        _format.Append('{').Append(_arguments.Count);
        if (alignment != 0)
        {
            _format.Append(',');
            _format.Append(alignment);
        }
        if (format is not null)
        {
            _format.Append(':').Append(format);
        }
        _format.Append('}');
        _arguments.Add(value);
    }
}