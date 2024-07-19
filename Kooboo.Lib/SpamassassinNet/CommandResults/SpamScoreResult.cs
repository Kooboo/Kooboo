using System;
using System.Linq;

namespace Kooboo.ApiMarket;

public class SpamScoreResult : BasicResult
{
    public SpamScoreResult(string content) : base(content)
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
            return bool.Parse(spam);
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

    public float CriticalScore
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Score) || !Score.Contains("/")) return 0;
            var value = Score.Split("/", StringSplitOptions.RemoveEmptyEntries).Last();
            return float.Parse(value);
        }
    }

    public float CurrentScore
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Score) || !Score.Contains("/")) return -1;
            var value = Score.Split("/", StringSplitOptions.RemoveEmptyEntries).First();
            return float.Parse(value);
        }
    }
}