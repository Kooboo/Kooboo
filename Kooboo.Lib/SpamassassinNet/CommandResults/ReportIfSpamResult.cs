using System.Linq;

namespace Kooboo.ApiMarket;

public class ReportIfSpamResult : BasicResult
{
    public ReportIfSpamResult(string content) : base(content)
    {
    }

    public bool? Spam
    {
        get
        {
            var header = Headers.FirstOrDefault(f => f.StartsWith("Spam:"));
            if (header == null) return null;
            CutSpan(header, out header, ":");
            var spam = CutSpan(header, out header, ";");
            return spam?.Trim()?.ToLower() == "yes";
        }
    }

    public string? Score
    {
        get
        {
            var header = Headers.FirstOrDefault(f => f.StartsWith("Spam:"));
            if (header == null) return null;
            CutSpan(header, out header, ";");
            return header;
        }
    }
}