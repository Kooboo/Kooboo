using System.Text.RegularExpressions;
using Kooboo.Mail.Multipart;

namespace Kooboo.Web.Api.Implementation.Mails
{
    public static class EmailHelper
    {
        //incase there are old html files.
        public static string ReplaceOldMsg(string originalHtml, long CurrentMsgId)
        {
            //BodyComposer.InlineImagePrefix+ ""

            var oldfile1 = BodyComposer.InlineImageMessageFilePrefix + "\\d*/";

            var oldfile2 = BodyComposer.InlineImagePrefix + "\\d*/";

            var replace1 = BodyComposer.InlineImageMessageFilePrefix + CurrentMsgId.ToString() + "/";

            var replace2 = BodyComposer.InlineImagePrefix + CurrentMsgId.ToString() + "/";

            originalHtml = Regex.Replace(originalHtml, oldfile1, replace1);

            originalHtml = Regex.Replace(originalHtml, oldfile2, replace2);

            return originalHtml;

        }


    }
}
