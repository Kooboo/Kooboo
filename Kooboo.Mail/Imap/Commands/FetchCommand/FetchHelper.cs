using MimeKit;

namespace Kooboo.Mail.Imap.Commands.FetchCommand
{
    public static class FetchHelper
    {
        public static string GetHeader(string rawBody)
        {
            var index = rawBody.IndexOf("\r\n\r\n");
            var linuxIndex = rawBody.IndexOf("\n\n");
            if (linuxIndex > -1 && index > linuxIndex)
            {
                return rawBody.Substring(0, linuxIndex);
            }

            if (index > -1)
            {
                return rawBody.Substring(0, index);
            }
            return rawBody;
        }

        public static string GetBodyPart(string FullMsg)
        {
            var index = FullMsg.IndexOf("\r\n\r\n");
            var linuxIndex = FullMsg.IndexOf("\n\n");

            if (linuxIndex > -1 && index > linuxIndex)
            {
                return FullMsg.Substring(linuxIndex + 2);
            }

            if (index > -1)
            {
                return FullMsg.Substring(index + 4);
            }

            return FullMsg;
        }

        public static int GetLineNumber(string body)
        {
            if (body == null)
            {
                return 0;
            }
            return body.Split('\n').Length;
        }


        public static int GetLineNumber(byte[] bytes)
        {
            if (bytes != null)
            {
                var text = System.Text.Encoding.UTF8.GetString(bytes);

                return GetLineNumber(text);
            }
            return 0;
        }


        public static byte[] GetPartBytes(MimePart part)
        {
            var PartText = part.GetText();
            var BodyOnly = GetBodyPart(PartText);
            return System.Text.Encoding.UTF8.GetBytes(BodyOnly);
        }





    }
}
