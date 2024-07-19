using System.Linq;

namespace Kooboo.ApiMarket;

public class BasicResult : ResultBase
{
    public int Code
    {
        get
        {
            var header = Headers.FirstOrDefault(f => f.StartsWith("SPAMD/"));
            CutSpan(header, out header, " ");
            var code = CutSpan(header, out header, " ");
            return int.Parse(code);
        }
    }

    public string Status
    {
        get
        {
            var header = Headers.FirstOrDefault(f => f.StartsWith("SPAMD/"));
            CutSpan(header, out header, " ");
            CutSpan(header, out header, " ");
            return header;
        }
    }

    public long ContentLength
    {
        get
        {
            var header = Headers.FirstOrDefault(f => f.StartsWith("Content-length:"));
            if (header == null) return 0;
            CutSpan(header, out header, " ");
            return long.Parse(header);
        }
    }

    public BasicResult(string content) : base(content)
    {
    }
}