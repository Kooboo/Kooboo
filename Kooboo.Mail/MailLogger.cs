using System.Text;

public static class MailLogger
{
    private static bool enable
    {
        get
        {

#if DEBUG
            {
                return true;
            }
#endif
            return false;
        }
    }

    private static bool skipLargeLog = true;


    public static void WriteLine(object target, string content, string type, bool isRead)
    {
        if (!enable) return;
        var side = isRead ? "C" : "S";
        Kooboo.Data.Log.Instance.EmailDebug.Write($"{target.GetHashCode()} {type} {side} {content}");
    }

    public static void WriteLine(object target, byte[] content, string type, bool isRead)
    {
        if (!enable) return;
        var str = Encoding.UTF8.GetString(content) + $"###buffer length:{content.Length}###";

        if (skipLargeLog && content.Length > 1024 * 100)
        {
            str = "!!Content to large skip write!!";
        }

        WriteLine(target, str, type, isRead);
    }
}