using System.Collections.Generic;

namespace Kooboo.Lib.Utilities.UAParser.MailApp
{
    public class AppleMailMatcher : BaseMailAppMatcher
    {
        public AppleMailMatcher()
        {
        }

        public override string MailAppName => "Apple Mail";

        public override List<string> SpecialIdentification => new List<string>()
        {
            "AppleMail",
            "Apple-mail"
        };
    }
}

