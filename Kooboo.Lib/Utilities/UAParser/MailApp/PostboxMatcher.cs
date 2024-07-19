using System.Collections.Generic;

namespace Kooboo.Lib.Utilities.UAParser.MailApp
{
    public class PostboxMatcher : BaseMailAppMatcher
    {
        public PostboxMatcher()
        {
        }

        public override string MailAppName => "Postbox";

        public override List<string> SpecialIdentification => new List<string>()
        {
            "postbox"
        };
    }
}

