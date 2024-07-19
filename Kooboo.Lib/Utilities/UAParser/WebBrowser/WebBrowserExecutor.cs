namespace Kooboo.Lib.Utilities.UAParser.WebBrowser
{
    public class WebBrowserExecutor
    {
        private const string Others = "Others";
        private readonly IWebBrowserMatcher[] _webBrowserMatchers;

        public WebBrowserExecutor()
        {
            _webBrowserMatchers = new IWebBrowserMatcher[]
            {
                //bot
                new BaiduSpiderMatcher(),
                new BingSpiderMatcher(),
                new GoogleBotMatcher(),
                new ChatgptMatcher(),
                new GptbotMatcher(),
                new YandexMatcher(),

                //web browser
                new WebBrowser360SEMatcher(),
                new MicrosoftEdgeMatcher(),
                new FirefoxBrowserMatcher(),
                new ChromeBrowserMatcher(),
                new SafariWebBrowserMatcher()
            };
        }

        public ApplicationInfo Match(string userAgentString)
        {
            foreach (var item in _webBrowserMatchers)
            {
                if (item.TryMatch(userAgentString, out var result))
                    return result;
            }

            return null;
        }
    }
}