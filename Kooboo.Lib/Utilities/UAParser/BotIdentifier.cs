using System;
using System.Collections.Generic;

namespace Kooboo.Lib.Utilities.UAParser
{
    public static class BotIdentifier
    {


        private static readonly Dictionary<string, string> BotKeywords = new Dictionary<string, string>
    {
        { "Googlebot", "Google" },
            {"GoogleOther", "GoogleOther" },
        { "Bingbot", "Bing" },
        { "Slurp", "Yahoo" },
        { "DuckDuckBot", "DuckDuckGo" },
        { "Baiduspider", "Baidu" },
        { "YandexBot", "Yandex" },
        { "Sogou", "Sogou" },
        { "Exabot", "Exalead" },
        { "facebookexternalhit", "Facebook" },
        { "facebot", "Facebook" },
        { "ia_archiver", "Alexa" },
        { "Twitterbot", "Twitter" },
        { "LinkedInBot", "LinkedIn" },
        { "Applebot", "Apple" },
        { "Pingdom", "Pingdom" },
        { "MJ12bot", "Majestic" },
        { "AhrefsBot", "Ahrefs" },
        { "SEMrushBot", "SEMrush" },
        { "DotBot", "Moz" },
        { "Screaming Frog", "Screaming Frog" }
    };

        // Method to identify bot from user-agent
        public static string IdentifyBot(string userAgent)
        {
            if (string.IsNullOrEmpty(userAgent))
            {
                return null;
            }

            foreach (var bot in BotKeywords)
            {
                if (userAgent.IndexOf(bot.Key, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return bot.Value;
                }
            }
            return null;
        }

        public static bool IsBot(string UserAgent)
        {
            if (string.IsNullOrEmpty(UserAgent))
            {
                return false;
            }

            foreach (var bot in BotKeywords)
            {
                if (UserAgent.IndexOf(bot.Key, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }
            }
            return false;
        }


    }
}
