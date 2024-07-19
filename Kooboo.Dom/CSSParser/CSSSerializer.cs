//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Text.RegularExpressions;

namespace Kooboo.Dom.CSS
{
    public class CSSSerializer
    {
        /// <summary>
        /// parse the csstext into a style sheet.
        /// </summary>
        /// <param name="cssText"></param>
        /// <param name="basehref">the absolute url location, or the page url if embedded.</param>
        /// <returns></returns>
        public static CSSStyleSheet deserializeCSSStyleSheet(string cssText, string basehref)
        {
            CSSStyleSheet stylesheet = new CSSStyleSheet();
            stylesheet.href = basehref;

            if (string.IsNullOrEmpty(cssText) && !string.IsNullOrEmpty(basehref))
            {
                cssText = Loader.DownloadCss(basehref);
            }

            if (string.IsNullOrEmpty(cssText))
            {
                return null;
            }

            stylesheet.cssText = cssText;

            CSSRuleList rules = deserializeRuleList(cssText, basehref);

            foreach (var item in rules.item)
            {
                item.parentStyleSheet = stylesheet;
            }

            stylesheet.cssRules = rules;
            return stylesheet;
        }

        /// <summary>
        ///  desrialized rule list. ImportRule is at the beginning of file, and should be ignored. 
        /// </summary>
        /// <param name="cssText"></param>
        /// <param name="basehref">the base href for import rule. </param>
        /// <returns></returns>
        public static CSSRuleList deserializeRuleList(string cssText, string basehref)
        {
            CSSRuleList rules = new CSSRuleList();

            CSSFileScanner filescanner = new CSSFileScanner(cssText);

            CSSFileScannerResult result = filescanner.ReadNext();
            while (result != null)
            {
                if (result.type == enumCSSRuleType.STYLE_RULE)
                {
                    CSSStyleRule stylerule = deserializeCSSStyleRule(result.cssText);
                    if (stylerule != null)
                    {
                        rules.appendRule(stylerule);
                    }
                }
                else if (result.type == enumCSSRuleType.IMPORT_RULE)
                {
                    CSSImportRule importrule = deserializeCSSImportRule(result.cssText, basehref);
                    if (importrule != null)
                    {
                        rules.appendRule(importrule);
                    }
                }
                else if (result.type == enumCSSRuleType.MEDIA_RULE)
                {
                    CSSMediaRule mediarule = deserializeCSSMediaRule(result.cssText);
                    if (mediarule != null)
                    {
                        rules.appendRule(mediarule);
                    }
                }
                else
                {
                    //TODO: other rules are not implemented now. 

                }

                result = filescanner.ReadNext();
            }

            return rules;
        }

        /// <summary>
        /// deserialize a CSSstyle rule.
        /// Example
        /// h1, h2
        /// {
        /// font-color:green;
        /// }
        /// </summary>
        /// <param name="cssText"></param>
        /// <returns></returns>
        public static CSSStyleRule deserializeCSSStyleRule(string cssText)
        {
            int firstBracket = cssText.IndexOf("{");
            int nextBracket = cssText.IndexOf("}");

            if (firstBracket <= 0 && nextBracket <= 0 && firstBracket > nextBracket)
            {
                // failed, did not find it. 
                return null;
            }

            string selector = cssText.Substring(0, firstBracket).Trim();

            if (string.IsNullOrEmpty(selector))
            {
                return null;
            }

            string innerCssText = cssText.Substring(firstBracket + 1, nextBracket - firstBracket - 1);

            CSSStyleDeclaration declartaion = deserializeDeclarationBlock(innerCssText);

            if (declartaion.item.Count == 0)
            {
                return null;
            }

            CSSStyleRule rule = new CSSStyleRule();

            rule.selectorText = selector;

            rule.cssText = innerCssText;

            rule.style = declartaion;

            return rule;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cssText">The text that contains the media blocks</param>
        /// <returns></returns>
        public static CSSMediaRule deserializeCSSMediaRule(string cssText)
        {
            CSSMediaRule mediaRule = new CSSMediaRule();


            int firstBracket = cssText.IndexOf("{");
            int lastBracket = cssText.LastIndexOf("}");

            if (firstBracket < 0 || lastBracket < 0)
            {
                return null;
            }

            string ruletext = cssText.Substring(firstBracket + 1, lastBracket - firstBracket - 1);

            mediaRule.cssText = ruletext;

            int mediaIndex = cssText.IndexOf("@media", StringComparison.OrdinalIgnoreCase);   // this should be 0. 

            string mediatext = cssText.Substring(mediaIndex + 6, firstBracket - mediaIndex - 1 - 5).Trim();

            mediaRule.selectorText = mediatext;

            if (string.IsNullOrEmpty(mediatext))
            {
                return null;
            }

            string[] mediaQuery = mediatext.Split(',');
            MediaList medialist = new MediaList();
            foreach (var item in mediaQuery)
            {
                medialist.appendMedium(item);
            }

            mediaRule.media = medialist;


            CSSRuleList rulelist = deserializeRuleList(ruletext, null);

            foreach (var item in rulelist.item)
            {
                item.parentRule = mediaRule;
            }

            mediaRule.cssRules = rulelist;

            return mediaRule;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cssText"></param>
        /// <param name="BaseHref">if null, URL must contains http, otherwise, return null</param>
        /// <returns></returns>
        public static CSSImportRule deserializeCSSImportRule(string cssText, string BaseHref)
        {
            string importPattern = @"@import\s+(url\(\'(?<url>.*?)\'\)|url\(\""(?<url>.*?)\""\)|url\((?<url>.*?)\)|(?<url>\S*?))(\s+(?<media>.*?)\;|\;|\s*\;)";

            Match m = Regex.Match(cssText, importPattern);

            if (m.Success)
            {

                string medias = m.Groups["media"].Value;
                string url = m.Groups["url"].Value;

                if (string.IsNullOrEmpty(url))
                {
                    return null;
                }

                if (!url.ToLower().StartsWith("http://"))
                {

                    url = PathHelper.combine(BaseHref, url);



                }

                CSSImportRule newRule = new CSSImportRule();
                newRule.href = url;

                if (!string.IsNullOrEmpty(medias))
                {

                    string[] mediaArray = medias.Split(',');

                    MediaList mediaList = new MediaList();

                    foreach (var item in mediaArray)
                    {
                        mediaList.appendMedium(item);

                    }
                    newRule.media = mediaList;
                }

                CSSStyleSheet importStyleSheet = deserializeCSSStyleSheet(string.Empty, url);

                if (importStyleSheet != null)
                {
                    newRule.stylesheet = importStyleSheet;

                    return newRule;
                }
                else
                {
                    return null;
                }


            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Parse the property name and value in the declaration block. 
        /// </summary>
        /// <param name="cssBlockText"></param>
        /// <returns></returns>
        public static CSSStyleDeclaration deserializeDeclarationBlock(string cssBlockText)
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

            CSSStyleDeclaration styleDeclaration = new CSSStyleDeclaration();

            if (string.IsNullOrEmpty(cssBlockText))
            {
                return styleDeclaration;
            }

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

            styleDeclaration.cssText = cssBlockText;

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
                        styleDeclaration.setProperty(key, value, important);
                    }
                }
            }
            return styleDeclaration;
        }

        public static string serializeCSSStyleRule(CSSStyleRule Rule)
        {
            string returnstring = string.Empty;

            returnstring += "\r\n" + Rule.selectorText + "\r\n";
            returnstring += "{\r\n";
            returnstring += Rule.style.cssText;
            returnstring += "\r\n}\r\n";
            return returnstring;
        }

        public static string serializeCSSMediaRule(CSSMediaRule Rule)
        {
            string returnstring = string.Empty;

            returnstring += "\r\n@media" + Rule.selectorText + "\r\n";    // SelectorText is the original media + query text. NON-W3C
            returnstring += "{\r\n";

            returnstring += Rule.cssText;
            returnstring += "\r\n}\r\n";

            return returnstring;
        }

        public static string serializeCSSImportRule(CSSImportRule Rule)
        {

            if (Rule.media.item.Count > 0)
            {
                string mediatext = string.Join(", ", Rule.media.item.ToArray());
                return "@import url('" + Rule.href + "') " + mediatext + ";";
            }
            else
            {

                return "@import url('" + Rule.href + "');";    // SelectorText is the original media + query text. NON-W3C
            }
        }

        public static string serializeDeclarationBlock(CSSStyleDeclaration styleDeclaration)
        {
            string cssText = string.Empty;
            bool first = true;
            foreach (var item in styleDeclaration.item)
            {
                string declarationText = null;
                if (first)
                {
                    declarationText = item.propertyname + ": " + item.value;
                    first = false;
                }
                else
                {
                    declarationText = "\r\n" + item.propertyname + ": " + item.value;
                }

                if (item.important)
                {
                    declarationText = declarationText + " !important";
                }
                declarationText = declarationText + ";";

                cssText += declarationText;
            }

            return cssText;
        }

        public static string serializeRuleList(CSSRuleList rulelist)
        {
            string cssText = string.Empty;

            foreach (var item in rulelist.item)
            {
                cssText += "\r\n";
                if (item.type == enumCSSRuleType.STYLE_RULE)
                {
                    cssText = cssText + serializeCSSStyleRule((CSSStyleRule)item);
                }
                else if (item.type == enumCSSRuleType.MEDIA_RULE)
                {
                    cssText = cssText + serializeCSSMediaRule((CSSMediaRule)item);
                }
                else if (item.type == enumCSSRuleType.IMPORT_RULE)
                {
                    cssText = cssText + serializeCSSImportRule((CSSImportRule)item);
                }
                else
                {
                    //TODO: others not supported yet. 
                }
            }

            return cssText;

        }
    }
}
