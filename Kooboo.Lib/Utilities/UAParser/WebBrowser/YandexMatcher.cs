using System.Collections.Generic;

namespace Kooboo.Lib.Utilities.UAParser.WebBrowser
{
    public class YandexMatcher : BaseWebBrowserMatcher
    {
        public override string BrowserName => "Yandex Box";

        public override List<string> SpecialIdentification => new List<string>()
        {
            "YaDirectFetcher",
            "YandexVerticals",
            "YandexBot",
            "YandexTracker",
            "YandexPagechecker",
            "YandexAccessibilityBot",
            "yandex.com/bots"
        };
    }
}