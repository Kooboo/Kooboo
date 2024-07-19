//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Text.RegularExpressions;

namespace Kooboo.Lib.Utilities
{
    public class HtmlUtility
    {
        public static string RemoveHtmlTags(string html)
        {
            return Regex.Replace(html, "<[^>]*>", " ");
        }

        public static string RemoveScripts(string html)
        {
            return Regex.Replace(html, "<script[^>]*>[^<]*</script>", String.Empty);
        }

        public static string RemoveScriptsAndStyleBlock(string html)
        {
            return Regex.Replace(html, "(<style[^>]*>[^<]*</style>)|(<script[^>]*>[^<]*</script>)", String.Empty);
        }

        public static string RemoveComments(string html)
        {
            return Regex.Replace(html, "<!--.*?-->", String.Empty);
        }

        public static string ConvertToText(string html)
        {
            html = RemoveComments(html);
            html = RemoveScriptsAndStyleBlock(html);
            html = RemoveHtmlTags(html);
            html = System.Web.HttpUtility.HtmlDecode(html);
            return html;
        }

        public static string ConvertToHtml(string text)
        {
            return Regex.Replace(System.Web.HttpUtility.HtmlEncode(text), "\r?\n", "<br />");
        }

        public static string CompressSpace(string text)
        {
            return Regex.Replace(text, @"\s+", " ");
        }

        public static bool HasHtmlOrBodyTag(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            int htmlindex = input.IndexOf("<html", StringComparison.OrdinalIgnoreCase);

            if (htmlindex >= 0)
            {
                return true;
            }

            int bodyindex = input.IndexOf("<body", StringComparison.OrdinalIgnoreCase);

            if (bodyindex >= 0)
            {
                return true;
            }

            htmlindex = input.IndexOf("</html>", StringComparison.OrdinalIgnoreCase);

            if (htmlindex >= 0)
            {
                return true;
            }

            bodyindex = input.IndexOf("</body>", StringComparison.OrdinalIgnoreCase);

            if (bodyindex >= 0)
            {
                return true;
            }

            return false;


        }


    }
}
