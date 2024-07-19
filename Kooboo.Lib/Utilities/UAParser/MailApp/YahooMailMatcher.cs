using System.Collections.Generic;

namespace Kooboo.Lib.Utilities.UAParser.MailApp
{
    public class YahooMailMatcher : BaseMailAppMatcher
    {
        public YahooMailMatcher()
        {
        }

        public override string MailAppName => "Yahoo";

        public override List<string> SpecialIdentification => new List<string>()
        {
            "YAHOOMAIL",
            "YAHOO-MAIL",
            "YahooMailProxy;",
            "https://help.yahoo.com/kb/yahoo-mail-proxy-SLN28749.html",
            "YahooMailProxy; https://help.yahoo.com/kb/yahoo-mail-proxy-SLN28749.html"
        };
    }
}

