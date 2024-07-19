using System.Collections.Generic;

namespace Kooboo.Lib.Utilities.UAParser.MailApp
{
    public class ThunderBirdMatcher : BaseMailAppMatcher
    {
        public ThunderBirdMatcher()
        {
        }

        public override string MailAppName => "Thunderbird";

        public override List<string> SpecialIdentification => new List<string>()
        {
            "thunderbird"
        };
    }
}

