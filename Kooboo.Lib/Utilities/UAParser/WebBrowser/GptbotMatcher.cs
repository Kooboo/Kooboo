using System.Collections.Generic;

namespace Kooboo.Lib.Utilities.UAParser.WebBrowser
{
    public class GptbotMatcher : BaseWebBrowserMatcher
    {
        public override string BrowserName => "GPT Bot";

        public override List<string> SpecialIdentification => new List<string>()
        {
            "GPTBot",
            "openai.com/gptbot"
        };
    }
}