using System;
using System.Collections.Generic;

namespace Kooboo.ApiMarket;

public class ResultBase
{
    private readonly List<string> _headers = new();
    public string Body { get; }
    public IEnumerable<string> Headers => _headers;

    public ResultBase(string content)
    {
        while (true)
        {
            var line = CutSpan(content, out content);

            if (!string.IsNullOrEmpty(line))
            {
                _headers.Add(line);
            }
            else if (line == string.Empty)
            {
                Body = content;
                break;
            }
            else
            {
                break;
            }
        }
    }

    internal static string? CutSpan(string content, out string surplus, string separator = "\r\n")
    {
        var index = content.IndexOf(separator, StringComparison.Ordinal);

        if (index < 0)
        {
            surplus = content;
            return null;
        }

        surplus = content[(index + separator.Length)..];
        var line = content[..index];
        return line;
    }
}