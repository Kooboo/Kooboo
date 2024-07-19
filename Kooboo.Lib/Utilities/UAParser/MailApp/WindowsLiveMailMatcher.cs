using System.Collections.Generic;

namespace Kooboo.Lib.Utilities.UAParser.MailApp
{
    public class WindowsLiveMailMatcher : BaseMailAppMatcher
    {
        public WindowsLiveMailMatcher()
        {
        }

        public override string MailAppName => "Windows Live Mail";

        public override List<string> SpecialIdentification => new List<string>()
        {
            "Outlook-Express"
        };
    }
}

