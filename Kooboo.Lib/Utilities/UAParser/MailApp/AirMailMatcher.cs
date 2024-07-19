using System.Collections.Generic;

namespace Kooboo.Lib.Utilities.UAParser.MailApp
{
    public class AirMailMatcher : BaseMailAppMatcher
    {
        public AirMailMatcher()
        {
        }

        public override string MailAppName => "AirMail";

        public override List<string> SpecialIdentification => new List<string>()
        {
            "Airmail"
        };
    }
}

