using System.Collections.Generic;

namespace Kooboo.Lib.Utilities.UAParser.MailApp
{
    public class OutlookMatcher : BaseMailAppMatcher
    {
        public OutlookMatcher()
        {
        }

        public override string MailAppName => "Outlook";

        public override List<string> SpecialIdentification => new List<string>()
        {
            "Outlook",
            "owa"
        };
    }
}

