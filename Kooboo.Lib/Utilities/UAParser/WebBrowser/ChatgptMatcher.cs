using System.Collections.Generic;

namespace Kooboo.Lib.Utilities.UAParser.WebBrowser
{
    public class ChatgptMatcher : BaseWebBrowserMatcher
    {
        public override string BrowserName => "ChatGPT";

        public override List<string> SpecialIdentification => new List<string>()
        {
            "ChatGPT",
            "openai.com/bot"
        };
    }
}