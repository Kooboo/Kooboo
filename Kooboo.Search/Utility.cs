//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Text;
using System.Text.RegularExpressions;
using Kooboo.Dom;

namespace Kooboo.Search
{
    public static class Utility
    {
        public static string RemoveHtml(string html, bool excludeScriptAndStyle = false)
        {
            html = System.Net.WebUtility.HtmlDecode(html);

            if (excludeScriptAndStyle && html != default)
            {
                html = Regex.Replace(html, "<script[^>]*>[\\s\\S]*?</script>", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                html = Regex.Replace(html, "<style[^>]*>[\\s\\S]*?</style>", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            }

            TreeConstruction tree = new TreeConstruction();
            var tokenizer = new Tokenizer(html, tree);
            var token = tokenizer.ReadNextToken();
            StringBuilder sb = new StringBuilder();
            bool lastIsSpace = false;
            while (token != null && token.type != enumHtmlTokenType.EOF)
            {
                if (token.type == enumHtmlTokenType.Character)
                {
                    var data = Lib.Helper.StringHelper.TrimSpace(token.data);
                    if (string.IsNullOrEmpty(data))
                    {
                        if (!lastIsSpace)
                        {
                            sb.Append(" ");
                            lastIsSpace = true;
                        }
                    }
                    else
                    {
                        sb.Append(data);
                        if (lastIsSpace)
                        {
                            lastIsSpace = false;
                        }
                    }
                }
                else if (token.type == enumHtmlTokenType.StartTag || token.type == enumHtmlTokenType.EndTag)
                {
                    if (!lastIsSpace)
                    {
                        sb.Append(" ");
                        lastIsSpace = true;
                    }

                }
                if (tree.stop)
                {
                    break;
                }

                if (tree._nexttoken != null)
                {
                    token = tree._nexttoken;
                    tree._nexttoken = null;
                }
                else
                {
                    token = tokenizer.ReadNextToken();
                }
            }

            return sb.ToString();
        }
    }
}
