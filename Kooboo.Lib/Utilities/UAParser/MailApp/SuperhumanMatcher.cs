using System.Collections.Generic;

namespace Kooboo.Lib.Utilities.UAParser.MailApp
{
    public class SuperhumanMatcher : BaseMailAppMatcher
    {
        public SuperhumanMatcher()
        {
        }

        public override string MailAppName => "Superhuman";

        public override List<string> SpecialIdentification => new List<string>()
        {
            "Superhuman"
        };
    }
}

