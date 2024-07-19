namespace Kooboo.Lib.Utilities.UAParser.MailApp
{
    public class MailAppExecutor
    {
        private const string Others = "Others";
        private readonly IMailAppMatcher[] _mailAppMatchers;

        public MailAppExecutor()
        {
            _mailAppMatchers = new IMailAppMatcher[]
            {
                new AirMailMatcher(),
                new AppleMailMatcher(),
                new GmailMatcher(),
                new WindowsLiveMailMatcher(),
                new OutlookMatcher(),
                new PostboxMatcher(),
                new SuperhumanMatcher(),
                new ThunderBirdMatcher(),
                new YahooMailMatcher(),
                new QQMailMatcher()
            };
        }

        public ApplicationInfo Match(string userAgentString)
        {
            foreach (var item in _mailAppMatchers)
            {
                if (item.TryMatch(userAgentString, out var result))
                    return result;
            }

            return null;
        }
    }
}

