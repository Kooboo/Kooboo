//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Dom;
using System.Text;

namespace Kooboo.Search
{
    public static class Utility
    {
        public static string RemoveHtml(string html)
        {
            html = System.Net.WebUtility.HtmlDecode(html); 
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
