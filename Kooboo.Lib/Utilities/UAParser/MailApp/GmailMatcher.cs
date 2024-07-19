using System.Collections.Generic;

namespace Kooboo.Lib.Utilities.UAParser.MailApp
{
    public class GmailMatcher : BaseMailAppMatcher
    {
        public GmailMatcher()
        {
        }

        public override string MailAppName => "Gmail";

        public override List<string> SpecialIdentification => new List<string>()
        {
            "via ggpht.com GoogleImageProxy",
            "ggpht.com",
            "Gmail"
        };
    }
}

