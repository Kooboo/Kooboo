namespace Kooboo.Lib.Utilities.UAParser.OS
{
    public class OSMatcherExecutor
    {
        private const string Others = "Others";
        private readonly IOSMatcher[] _osMatchers;

        public OSMatcherExecutor()
        {
            _osMatchers = new IOSMatcher[]
            {new AppleMobileOsMatcher(),
            new IPadOsMatcher(),
            new MacOsMatcher(),
            new AndroidOsMatcher(),
            new WindowsOsMatcher(),
            new LinuxOsMatcher()
            };
        }

        public string Match(string userAgentString)
        {
            foreach (var item in _osMatchers)
            {
                if (item.Match(userAgentString))
                    return item.OSName;
            }
            return Others;
        }
    }
}

