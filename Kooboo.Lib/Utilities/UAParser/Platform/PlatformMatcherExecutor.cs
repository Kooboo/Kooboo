using System.Linq;
namespace Kooboo.Lib.Utilities.UAParser.Platform
{
    public class PlatformMatcherExecutor
    {
        private const string Others = "Others";
        private readonly IPlatformMatcher[] _platformMatchers;

        public PlatformMatcherExecutor()
        {
            _platformMatchers = new IPlatformMatcher[]
            {
                new TabletPlatformMatcher(),
                new MobilePlatformMatcher(),
                new DesktopPlatformMatcher()
            }.OrderBy(o => o.Preference).ToArray();
        }

        public string Match(string userAgentString)
        {
            foreach (var item in _platformMatchers)
            {
                if (item.Match(userAgentString))
                    return item.PlatformName;
            }

            return Others;
        }
    }
}

