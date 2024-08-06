using System;
using System.Collections.Generic;

namespace Kooboo.Lib.Utilities.UAParser
{
    public static class BotIdentifier
    {
        private static readonly string[] KeysWords = { "bot", "spider", "crawl", "lighthouse", "curl", "python", "wget", "java" };


        private static char[] separators = "()| ,;/\\".ToCharArray();

        private static readonly Dictionary<string, string> knownBots = new Dictionary<string, string>
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
        { "Screaming Frog", "Screaming Frog" },
        {"Mediapartners-Google", "Google MediaPartner" }


    };

        // Method to identify bot from user-agent
        public static string GetBotName(string userAgent)
        {
            if (string.IsNullOrEmpty(userAgent))
            {
                return null;
            }

            foreach (var bot in knownBots)
            {
                if (userAgent.IndexOf(bot.Key, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return bot.Value;
                }
            }

            userAgent = userAgent.ToLower();

            var index = userAgent.IndexOf("http://");

            if (index == -1)
            {
                index = userAgent.IndexOf("https://");
            }

            if (index > -1)
            {
                var nextMark = userAgent.IndexOf('/', index + 8);
                var nextMark2 = userAgent.IndexOf(')', index + 8);
                if (nextMark2 > -1 && nextMark2 < nextMark)
                {
                    nextMark = nextMark2;
                }

                if (nextMark > -1 && nextMark > index)
                {
                    var url = userAgent.Substring(index, nextMark - index);

                    if (url != null)
                    {
                        var host = Kooboo.Lib.Helper.UrlHelper.GetHost(url);

                        var root = Kooboo.Lib.Domain.DomainService.Parse(host);
                        if (root != null)
                        {
                            host = root.Root;
                        }

                        var lastIndex = host.LastIndexOf('.');
                        return lastIndex > -1 ? host.Substring(0, lastIndex) : host;
                    }
                }
            }

            foreach (var key in KeysWords)
            {

                if (userAgent.IndexOf(key) >= 0)
                {
                    var parts = userAgent.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                    if (parts != null)
                    {
                        foreach (var part in parts)
                        {
                            if (part.Contains(key))
                            {
                                return part;
                            }
                        }
                    }

                }
            }

            // return what is inside (); 
            return null;
        }

        public static bool IsBot(string UserAgent)
        {
            if (string.IsNullOrEmpty(UserAgent))
            {
                return false;
            }

            foreach (var bot in knownBots)
            {
                if (UserAgent.IndexOf(bot.Key, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }
            }

            if (UserAgent.Contains("http://") || UserAgent.Contains("https://"))
            {
                return true;
            }

            foreach (var key in KeysWords)
            {
                //match keyword, and contains http or @

                var index = UserAgent.IndexOf(key, StringComparison.OrdinalIgnoreCase);

                if (index >= 0)
                {
                    // contains an email address. 
                    if (UserAgent.Contains("@"))
                    {
                        //TODO: improve and check it as an email. 
                        return true;
                    }

                    //if before or after is Ascii.
                    if (index > 0)
                    {
                        var before = UserAgent[index - 1];
                        if (char.IsAscii(before) || before == '-' || before == '_')
                        {
                            return true;
                        }
                    }

                    if (index + key.Length < UserAgent.Length)
                    {
                        var after = UserAgent[index + key.Length];

                        if (char.IsAscii(after) || after == '-' || after == '_')
                        {
                            return true;
                        }
                    }


                }
            }

            return false;
        }


    }
}
