using System.IO;
using System.Net.Http;

namespace Kooboo.Lib.Extensions;

public static class HttpContentExtensions
{
    public static string ReadAsString(this HttpContent content)
    {
        var stream = content.ReadAsStream();
        stream.Position = 0;
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    public static byte[] ReadAsByteArray(this HttpContent content)
    {
        var stream = content.ReadAsStream();
        stream.Position = 0;
        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        return ms.ToArray();
    }
}
