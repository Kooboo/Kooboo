using System.Collections.Generic;

namespace Kooboo.Lib.Utilities.UAParser.MailApp
{
    public class QQMailMatcher : BaseMailAppMatcher
    {
        public QQMailMatcher()
        {
        }

        public override string MailAppName => "QQMail";

        public override List<string> SpecialIdentification => new List<string>()
        {
            "QQMail",
            "QQ-mail"
        };
    }
}

