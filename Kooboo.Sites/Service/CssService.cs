//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Dom;
using Kooboo.Dom.CSS;
using Kooboo.Dom.CSS.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Lib;
using Kooboo.Sites.Models;

namespace Kooboo.Sites.Service
{
    public static class CssService
    {  
        public static List<UrlInfo> GetUrlInfos(string CssText)
        {
            List<UrlInfo> urllist = new List<UrlInfo>();

            if (string.IsNullOrEmpty(CssText))
            {
                return urllist;
            }

            Kooboo.Dom.CSS.Tokenizer toknizer = new Kooboo.Dom.CSS.Tokenizer(CssText);

            while (true)
            {
                cssToken token = toknizer.ConsumeNextToken();

                if (token == null || token.Type == enumTokenType.EOF)
                {
                    break;
                }
                else
                {
                    if (token.Type == enumTokenType.url)
                    {
                        url_token urltoken = token as url_token;
                        string resourceurl = urltoken.value;
                        if (!string.IsNullOrEmpty(resourceurl))
                        {
                            urllist.Add(new UrlInfo() { StartIndex = urltoken.startIndex, EndIndex = urltoken.endIndex, RawUrl = resourceurl, isUrlToken = true });
                        }
                    }
                    else if (token.Type == enumTokenType.at_keyword)
                    {
                        at_keyword_token keywordtoken = token as at_keyword_token;
                        if (keywordtoken.value.ToLower() == "import")
                        {
                            cssToken nexttoken = toknizer.ConsumeNextToken();
                            while (nexttoken.Type == enumTokenType.whitespace)
                            {
                                nexttoken = toknizer.ConsumeNextToken();
                            }

                            if (nexttoken.Type == enumTokenType.String)
                            {
                                string_token stringtoken = nexttoken as string_token;

                                if (!string.IsNullOrEmpty(stringtoken.value))
                                {
                                    urllist.Add(new UrlInfo() { StartIndex = stringtoken.startIndex, EndIndex = stringtoken.endIndex, RawUrl = stringtoken.value, isImportRule = true });
                                }
                            }
                            else if (nexttoken.Type == enumTokenType.url)
                            {
                                url_token urltoken = nexttoken as url_token;
                                string resourceurl = urltoken.value;
                                if (!string.IsNullOrEmpty(resourceurl))
                                {
                                    urllist.Add(new UrlInfo() { StartIndex = urltoken.startIndex, EndIndex = urltoken.endIndex, RawUrl = resourceurl, isUrlToken = true, isImportRule = true });
                                }

                            }

                            else if (nexttoken.Type == enumTokenType.function)
                            {

                                function_token functoken = nexttoken as function_token;

                                string functionvalue = functoken.Value;
                                if (functionvalue.ToLower() == "url")
                                {
                                    nexttoken = toknizer.ConsumeNextToken();
                                    while (nexttoken.Type == enumTokenType.whitespace)
                                    {
                                        nexttoken = toknizer.ConsumeNextToken();
                                    }
                                    if (nexttoken == null || nexttoken.Type == enumTokenType.EOF)
                                    {
                                        break;
                                    }

                                    if (nexttoken.Type == enumTokenType.String)
                                    {
                                        string_token stringtoken = nexttoken as string_token;

                                        if (!string.IsNullOrEmpty(stringtoken.value))
                                        {
                                            urllist.Add(new UrlInfo() { StartIndex = stringtoken.startIndex, EndIndex = stringtoken.endIndex, RawUrl = stringtoken.value, isImportRule = true });
                                        }
                                    }
                                    else if (nexttoken.Type == enumTokenType.url)
                                    {
                                        url_token urltoken = nexttoken as url_token;
                                        string resourceurl = urltoken.value;
                                        if (!string.IsNullOrEmpty(resourceurl))
                                        {
                                            urllist.Add(new UrlInfo() { StartIndex = urltoken.startIndex, EndIndex = urltoken.endIndex, RawUrl = resourceurl, isUrlToken = true, isImportRule = true });
                                        }

                                    }
                                }

                            } 


                        }
                    }

                    else if (token.Type == enumTokenType.function)
                    {

                        function_token functoken = token as function_token;

                        string functionvalue = functoken.Value;
                        if (functionvalue.ToLower() == "url")
                        {
                            cssToken nexttoken = toknizer.ConsumeNextToken();
                            while (nexttoken.Type == enumTokenType.whitespace)
                            {
                                nexttoken = toknizer.ConsumeNextToken();
                            }
                            if (nexttoken == null || nexttoken.Type == enumTokenType.EOF)
                            {
                                break;
                            }

                            if (nexttoken.Type == enumTokenType.String)
                            {
                                string_token stringtoken = nexttoken as string_token;

                                if (!string.IsNullOrEmpty(stringtoken.value))
                                {
                                    urllist.Add(new UrlInfo() { StartIndex = stringtoken.startIndex, EndIndex = stringtoken.endIndex, RawUrl = stringtoken.value });
                                }

                            }
                        }

                    }

                }

            }

            return cleanSpecialMark(urllist);
        }

        private static List<UrlInfo> cleanSpecialMark(List<UrlInfo> input)
        {
            List<UrlInfo> result = new List<UrlInfo>();

            foreach (var item in input)
            {
                if (!item.RawUrl.StartsWith("#") && !item.RawUrl.Contains("/#"))
                {
                    result.Add(item); 
                }
            }
            return result; 
        }
           
        // Get the original CssRule from style sheet based on the converted cmscssrule.  
        public static CSSRule GetOrginalCssRule(List<CssConvertResult> ConvertResult, CmsCssRule foundCmsCssRule)
        {
            var find=  ConvertResult.Find(o => o.RuleId == foundCmsCssRule.Id);
            return find != null ? find.CssRule : null; 
        }

       public static CSSStyleDeclaration ParseStyleDeclaration(CmsCssRule rule)
        {
            //current.Declarations = Kooboo.Dom.CSS.CSSSerializer.deserializeDeclarationBlock(cmscssrule.RuleText);
         
            var cssrule = Dom.CSSParser.ParseOneCssRule(rule.CssText); 
            if (cssrule !=null)
            {
                var stylerule = cssrule as CSSStyleRule; 
                if (stylerule !=null)
                {
                    return stylerule.style; 
                }
            }
            return null; 
        }
         
       public static string SerializeCmsCssDeclaration(List<CmsCssDeclaration> list)
        {
           string text = string.Empty;

            foreach (var item in list)
            {
                text += item.PropertyName + ": " + item.Value;
                if (item.Important)
                {
                    text += " !important";
                }
                text += ";"; 
            }
            return text; 
        }
   
        public static List<CmsCssDeclaration> ParseDeclarationBlock(string cssBlockText, Guid CmsCssRuleId)
        {
            //from W3.org
            //To serialize a CSS declaration with property name property, value value and optionally an important flag set, follow these steps:
            //Let s be the empty string.
            //Append property to s.
            //Append ": " (U+003A U+0020) to s.
            //Append value to s.
            //If the important flag is set, append " !important" (U+0020 U+0021 U+0069 U+006D U+0070 U+006F U+0072 U+0074 U+0061 U+006E U+0074) to s.
            //Append ";" (U+003B) to s.
            //Return s.

            List<CmsCssDeclaration> declarations = new List<CmsCssDeclaration>();

            // remvove the {} if it contains.
            int startMark = cssBlockText.IndexOf("{");

            if (startMark > 0)
            {
                int endMark = cssBlockText.IndexOf("}");

                if (endMark > 0)
                {
                    cssBlockText = cssBlockText.Substring(startMark + 1, endMark - startMark - 1);
                }
            }

            string[] keyvaluepair = cssBlockText.Split(';');
            string key = string.Empty;
            string value = string.Empty;
            int seperatorIndex = 0;
            bool important = false;

            foreach (var item in keyvaluepair)
            {
                seperatorIndex = item.IndexOf(":");
                if (seperatorIndex > 0)
                {
                    key = item.Substring(0, seperatorIndex).Trim();
                    value = item.Substring(seperatorIndex + 1).Trim();

                    if (value.Contains("!important"))
                    {
                        important = true;
                        value = value.Replace("!important", "").Trim();
                    }
                    else
                    {
                        important = false;
                    }

                    if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                    {
                        declarations.Add(new CmsCssDeclaration() { CmsCssRuleId = CmsCssRuleId, PropertyName = key, Value = value, Important = important });
                    }
                }
            }

            return declarations;
        }


        public static List<CssConvertResult> ConvertCss(CSSRuleList rulelist, Guid ParentStyleId, Guid ParentCssRuleId=default(Guid))
        {
            List<CssConvertResult> Result = new List<CssConvertResult>(); 

            int counter = rulelist.item.Count; 
            for (int i = 0; i < counter; i++)
            {
                CmsCssRule cmsrule = new CmsCssRule(); 
                cmsrule.ParentCssRuleId = ParentCssRuleId; 
                cmsrule.ParentStyleId = ParentStyleId;

                cmsrule.OwnerObjectId = ParentStyleId;
                cmsrule.OwnerObjectConstType = ConstObjectType.Style; 

                cmsrule.TempCssRuleIndex = i;

                CSSRule item = rulelist.item[i];

                cmsrule.selectorPositionIndex = item.EndSelectorIndex;

                if (item.type == enumCSSRuleType.STYLE_RULE)
                {
                    CSSStyleRule stylerule = item as CSSStyleRule;
                    cmsrule.CssText = stylerule.cssText;
                    cmsrule.ruleType = RuleType.StyleRule;
                    cmsrule.Properties = stylerule.style.item.Where(o=> o !=null && !string.IsNullOrWhiteSpace(o.propertyname)).Select(o => o.propertyname).ToList(); 
                }
                else if (item.type == enumCSSRuleType.IMPORT_RULE)
                {
                    CSSImportRule importrule = item as CSSImportRule;
                    cmsrule.CssText = importrule.cssText;
                    cmsrule.ruleType = RuleType.ImportRule;
                }
                else if (item.type == enumCSSRuleType.MEDIA_RULE)
                {
                    CSSMediaRule mediarule = item as CSSMediaRule;
                    cmsrule.CssText = mediarule.cssText;
                    cmsrule.ruleType = RuleType.MediaRule;  
                }
                else if (item.type == enumCSSRuleType.FONT_FACE_RULE)
                {
                    CSSFontFaceRule facerule = item as CSSFontFaceRule;
                    cmsrule.CssText = facerule.cssText;
                    cmsrule.ruleType = RuleType.FontFaceRule;
                }

                else
                { 
                    continue;
                }

                /// check duplicate in the current collections. 
                cmsrule.DuplicateIndex = CssService.GetDuplicateIndex(Result, cmsrule.SelectorText, cmsrule.ruleType);

                CssConvertResult converted = new CssConvertResult();
                converted.RuleId = cmsrule.Id;
                converted.CmsRule = cmsrule;
                converted.CssRule = item;  
                Result.Add(converted);

                if (item.type == enumCSSRuleType.MEDIA_RULE)
                {
                    var mediarule = item as Kooboo.Dom.CSS.CSSMediaRule;
                    var sub = ConvertCss(mediarule.cssRules, ParentStyleId, cmsrule.Id); 
                    if (sub !=null && sub.Count() >0)
                    {
                        Result.AddRange(sub); 
                    }
                }
            }

            return Result;
        }


        public static List<CssConvertResult> ConvertCss(string cssText, Guid StyleId, Guid ParentCssRuleId = default(Guid))
        {
            if (string.IsNullOrEmpty(cssText))
            {
                return new List<CssConvertResult>(); 
            }
            var stylesheet = Kooboo.Dom.CSSParser.ParseCSSStyleSheet(cssText);
            if (stylesheet == null)
            { return null;  } 
            return ConvertCss(stylesheet.cssRules, StyleId, default(Guid)); 
        }

        private static int GetDuplicateIndex(List<CssConvertResult> currentlist, string selector, RuleType ruleType)
        {
            int duplicate = 0;
            foreach (var item in currentlist)
            {
                if (item.CmsRule.ruleType == ruleType && item.CmsRule.SelectorText == selector)
                {
                    duplicate += 1;
                }
            }

            return duplicate;
        }

        public static Dictionary<Guid, string> ConvertUrlsToRoutes(List<string> urls, string baseRelativeUrl)
        {
            Dictionary<Guid, string> routes = new Dictionary<Guid, string>();

            foreach (var item in urls)
            {
                string relativeurl = Kooboo.Lib.Helper.UrlHelper.Combine(baseRelativeUrl, item);
                relativeurl = Kooboo.Lib.Helper.UrlHelper.RelativePath(relativeurl);
                Guid routeid = Data.IDGenerator.GetRouteId(relativeurl);
                routes.Add(routeid, relativeurl);
            }

            return routes;
        }

    }

    public class CssConvertResult
    { 
        public Guid RuleId { get; set; }

        public CSSRule CssRule { get; set; }

        public CmsCssRule CmsRule { get; set; } 
    }

    public class UrlInfo
    {
        public string RawUrl { get; set; }

        public int StartIndex { get; set; }

        public int EndIndex { get; set; }

        public bool isImportRule { get; set; }

        public bool isUrlToken { get; set; }

        public string PureUrl
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.RawUrl))
                {
                    return string.Empty;
                }

                string _rawurl;

                if (isUrlToken)
                {
                    int firstbracket = this.RawUrl.IndexOf("(");
                    int lastbracket = this.RawUrl.LastIndexOf(")");
                    if (firstbracket < 0 || lastbracket < 0)
                    {
                        _rawurl = this.RawUrl;
                    }
                    else
                    {
                        _rawurl = this.RawUrl.Substring(firstbracket + 1, lastbracket - firstbracket - 1);
                    }
                }
                else
                {
                    _rawurl = this.RawUrl;
                }
                string ends = "'\"";
                char[] chars = ends.ToCharArray();
                string newstring = _rawurl.TrimEnd(chars);
                newstring = newstring.TrimStart(chars);
                return newstring;
            }
        }

    }
}
