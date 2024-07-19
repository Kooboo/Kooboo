using System.IO;
using System.Text;
using MimeKit;

namespace Kooboo.Mail;

public static class MimeEntityExtensions
{
    public static string GetText(this MimeEntity entity)
    {
        var ms = new MemoryStream();

        entity.WriteTo(new FormatOptions
        {
            NewLineFormat = NewLineFormat.Dos
        }, ms);

        return Encoding.UTF8.GetString(ms.ToArray());
    }


    public static string ToMessageText(this MimeMessage msg)
    {

        MemoryStream memoryStream = new MemoryStream();

        msg.WriteTo(new FormatOptions
        {
            NewLineFormat = NewLineFormat.Dos
        }, memoryStream);

        var result = Encoding.UTF8.GetString(memoryStream.ToArray());
        return result.TrimEnd();
    }
}