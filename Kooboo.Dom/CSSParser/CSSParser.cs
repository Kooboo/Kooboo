//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Dom.CSS;
using Kooboo.Dom.CSS.rawmodel;
using Kooboo.Dom.CSS.Tokens;

namespace Kooboo.Dom
{

    /// <summary>
    /// Parse low level CSS objects to high level CSS objects, that can be used and query. 
    /// </summary>
    public static class CSSParser
    {

        /// <summary>
        /// parse a style sheet. 
        /// </summary>
        /// <param name="cssText"></param>
        /// <param name="baseurl"></param>
        /// <returns></returns>
        public static CSSStyleSheet ParseCSSStyleSheet(string cssText, string baseurl, bool downloadImportRule)
        {
            //this.baseurl = baseurl;
            //this.downloadimportrule = downloadImportRule;

            if (!string.IsNullOrWhiteSpace(cssText))
            {
                TokenParser parser = new TokenParser();
                stylesheet rawStylesheet = parser.ParseStyleSheet(cssText);
                CSSStyleSheet stylesheet = ParseCSSStyleSheet(rawStylesheet, baseurl, downloadImportRule, ref cssText);
                stylesheet.cssText = cssText;
                return stylesheet;
            }
            else
            {
                return new CSSStyleSheet();
            }
        }

        public static CSSStyleSheet ParseCSSStyleSheet(string cssText, string baseurl)
        {
            return ParseCSSStyleSheet(cssText, baseurl, false);
        }

        public static CSSStyleSheet ParseCSSStyleSheet(string cssText)
        {
            return ParseCSSStyleSheet(cssText, string.Empty, false);
        }

        public static CSSStyleSheet ParseCSSStyleSheetFromUrl(string url)
        {
            return ParseCSSStyleSheetFromUrl(url, false);
        }

        public static CSSStyleSheet ParseCSSStyleSheetFromUrl(string url, bool DownloadImportRule)
        {

            string webstring = Loader.DownloadCss(url);

            if (!string.IsNullOrEmpty(webstring))
            {
                return ParseCSSStyleSheet(webstring, url, DownloadImportRule);
            }
            else
            {
                return null;
            }
        }

        public static CSSRule ParseOneCssRule(string CssText)
        {
            var stylesheet = ParseCSSStyleSheet(CssText);
            if (stylesheet != null && stylesheet.cssRules != null && stylesheet.cssRules.length > 0)
            {
                return stylesheet.cssRules.item[0];
            }
            return null;
        }
        /// <summary>
        /// The parsing algo from W3C does not parse CSS into right CSS object model, this is to convert to right CSSOM.
        /// </summary>
        /// <param name="rawstylesheet"></param>
        /// <returns></returns>
        private static CSSStyleSheet ParseCSSStyleSheet(stylesheet rawstylesheet, string baseurl, bool downloadImportRule, ref string OriginalCss)
        {
            CSSStyleSheet CssStyleSheet = new CSSStyleSheet();

            foreach (var item in rawstylesheet.items)
            {
                if (item.Type == enumRuleType.QualifiedRule)
                {
                    CSSRule rule = ParseQualifiedRule(item as QualifiedRule, ref OriginalCss);
                    if (rule != null)
                    {
                        rule.parentStyleSheet = CssStyleSheet;
                        CssStyleSheet.cssRules.appendRule(rule);
                    }
                }
                else if (item.Type == enumRuleType.AtRule)
                {
                    CSSRule rule = ParseAtRule(item as AtRule, CssStyleSheet, baseurl, downloadImportRule, ref OriginalCss);

                    if (rule != null)
                    {
                        rule.parentStyleSheet = CssStyleSheet;
                        CssStyleSheet.cssRules.appendRule(rule);
                    }
                }
            }

            return CssStyleSheet;
        }

        /// <summary>
        /// From W3C, in most case, th preclude matchs to Selector and block value contains declaration. 
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        private static CSSRule ParseQualifiedRule(QualifiedRule rule, ref string OriginalCss)
        {
            CSSStyleRule cssrule = new CSSStyleRule();
            string selectorText = string.Empty;

            int startindex = -1;
            int endindex = -1;

            int endindexselector = -1;

            selectorText = ComponentValueExtension.getString(rule.prelude, ref startindex, ref endindexselector, ref OriginalCss);

            cssrule.selectorText = selectorText;

            cssrule.style = ParseDeclarations(rule.block, ref endindex, ref OriginalCss);


            cssrule.StartIndex = rule.startindex;
            cssrule.EndIndex = rule.endindex;
            cssrule.EndSelectorIndex = endindexselector - cssrule.StartIndex + 1;

            return cssrule;

        }

        private static CSSRule ParseAtRule(AtRule rule, CSSStyleSheet parentSheet, string baseurl, bool downloadImportRule, ref string OriginalCss)
        {
            /// the first item in rule is the atkeyword.

            if (rule.prelude[0].Type == CompoenentValueType.preservedToken)
            {
                PreservedToken token = rule.prelude[0] as PreservedToken;
                if (token.token.Type == enumTokenType.at_keyword)
                {
                    at_keyword_token keywordToken = token.token as at_keyword_token;

                    if (keywordToken.value.ToLower() == "import")
                    {
                        return ParseImportRule(rule, baseurl, downloadImportRule, ref OriginalCss);
                    }
                    else if (keywordToken.value.ToLower() == "media")
                    {
                        return ParseMediaRule(rule, parentSheet, ref OriginalCss);
                    }
                    else if (keywordToken.value.ToLower() == "font-face")
                    {
                        return ParseFontFace(rule, ref OriginalCss);
                    }
                }

            }

            return null;
        }

        /// <summary>
        /// The @import at-rule is a simple statement. After its name, it takes a single string or url() function to indicate the stylesheet that it should import.
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        private static CSSRule ParseImportRule(AtRule rule, string baseurl, bool downloadImportRule, ref string OriginalCss)
        {
            /// the import starts with import atkeyword token. 
            /// it should have been checked before calling this method, can be ignored. 
            PreservedToken token = rule.prelude[0] as PreservedToken;

            int count = rule.prelude.Count;

            CSSImportRule importrule = new CSSImportRule();
            string media = string.Empty;

            int startindex = -1;
            int endindex = -1;

            for (int i = 0; i < count; i++)
            {
                if (startindex < 0)
                {
                    startindex = rule.prelude[i].startindex;
                }

                if (rule.prelude[i].endindex > endindex)
                {
                    endindex = rule.prelude[i].endindex;
                }

                if (rule.prelude[i].Type == CompoenentValueType.preservedToken)
                {
                    PreservedToken preservedToken = rule.prelude[i] as PreservedToken;

                    /// ignore the whitespace and at-keyword token. 
                    if (preservedToken.token.Type == enumTokenType.at_keyword || (string.IsNullOrEmpty(importrule.href) && preservedToken.token.Type == enumTokenType.whitespace))
                    {
                        continue;
                    }

                    if (string.IsNullOrEmpty(importrule.href))
                    {
                        if (preservedToken.token.Type == enumTokenType.String)
                        {
                            string_token stringtoken = preservedToken.token as string_token;
                            string url = string.Empty;

                            if (string.IsNullOrEmpty(baseurl))
                            {
                                url = stringtoken.value;
                            }
                            else
                            {
                                url = PathHelper.combine(baseurl, stringtoken.value);
                            }

                            importrule.href = url;
                            if (downloadImportRule && !string.IsNullOrEmpty(url))
                            {
                                importrule.stylesheet = CSSParser.ParseCSSStyleSheetFromUrl(url);
                            }

                        }
                        else if (preservedToken.token.Type == enumTokenType.url)
                        {
                            url_token urltoken = preservedToken.token as url_token;

                            string url = string.Empty;
                            if (string.IsNullOrEmpty(baseurl))
                            { url = urltoken.value; }
                            else
                            {
                                url = PathHelper.combine(baseurl, urltoken.value);
                            }

                            importrule.href = url;
                            if (downloadImportRule && !string.IsNullOrEmpty(url))
                            {

                                importrule.stylesheet = CSSParser.ParseCSSStyleSheetFromUrl(url);
                            }
                        }
                        else
                        {
                        }
                    }
                    else
                    {
                        // the import rule has href already, next is the media rules. 
                        if (preservedToken.token.Type == enumTokenType.comma || preservedToken.token.Type == enumTokenType.semicolon)
                        {
                            if (!string.IsNullOrEmpty(media))
                            {
                                importrule.media.appendMedium(media.Trim());
                                media = string.Empty;
                            }
                        }
                        else
                        {
                            // can be delim token. 
                            if (string.IsNullOrEmpty(media) && preservedToken.token.Type == enumTokenType.whitespace)
                            {
                                // the start of whitespace will be ignored.
                            }
                            else
                            {
                                media += preservedToken.token.GetString(ref OriginalCss);
                            }
                        }

                    }

                }

                else if (rule.prelude[i].Type == CompoenentValueType.function)
                {
                    Function urlfunction = rule.prelude[i] as Function;
                    string href = string.Empty;
                    if (urlfunction.name == "url")
                    {
                        foreach (var item in urlfunction.value)
                        {
                            if (item.Type == CompoenentValueType.preservedToken)
                            {
                                PreservedToken pretoken = item as PreservedToken;
                                if (pretoken.token.Type == enumTokenType.String)
                                {
                                    string_token stringtoken = pretoken.token as string_token;
                                    href += stringtoken.value;
                                }
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(href))
                    {
                        importrule.href = href;
                    }
                }

                else if (rule.prelude[i].Type == CompoenentValueType.simpleBlock)
                {
                    // simple block is the block like  screen and (min-width:300);
                    SimpleBlock block = rule.prelude[i] as SimpleBlock;

                    string mediarule = string.Empty;
                    foreach (var item in block.value)
                    {
                        if (item.Type == CompoenentValueType.preservedToken)
                        {
                            PreservedToken pretoken = item as PreservedToken;
                            mediarule += pretoken.token.GetString(ref OriginalCss);

                            if (token.token.endIndex > endindex)
                            {
                                endindex = token.token.endIndex;
                            }

                        }
                    }

                    if (block.token.Type == enumTokenType.round_bracket_left || block.token.Type == enumTokenType.round_bracket_right)
                    {
                        mediarule = "(" + mediarule + ")";
                    }
                    else if (block.token.Type == enumTokenType.square_bracket_left || block.token.Type == enumTokenType.square_bracket_right)
                    {
                        mediarule = "[" + mediarule + "]";
                    }
                    else if (block.token.Type == enumTokenType.curly_bracket_left || block.token.Type == enumTokenType.curly_bracket_right)
                    {
                        mediarule = "{" + mediarule + "}";
                    }

                    media += mediarule;
                }

            }

            if (!string.IsNullOrEmpty(media))
            {
                importrule.media.appendMedium(media.Trim());
                media = string.Empty;
            }

            importrule.StartIndex = startindex;
            if (rule.endindex > endindex)
            {
                endindex = rule.endindex;
            }
            importrule.EndIndex = endindex;

            int endselectorindex = rule.prelude[0].endindex + 1;

            ///import rule does not have one extra char like {
           // importrule.EndSelectorIndex = endselectorindex - importrule.StartIndex + 1; 
            importrule.EndSelectorIndex = endselectorindex - importrule.StartIndex;

            return importrule;
        }

        private static CSSRule ParseFontFace(AtRule rule, ref string OriginalCss)
        {
            CSSFontFaceRule fontrule = new CSSFontFaceRule();

            int startindex = 0;
            int endindex = 0;
            int endselectindex = -1;

            foreach (var item in rule.prelude)
            {
                if (startindex == 0)
                {
                    startindex = item.startindex;
                }

                if (item.endindex > endselectindex)
                {
                    endselectindex = item.endindex;
                }
            }


            CSSStyleDeclaration style = ParseDeclarations(rule.block, ref endindex, ref OriginalCss);

            fontrule.style = style;
            fontrule.StartIndex = startindex;
            fontrule.EndIndex = endindex;

            fontrule.EndSelectorIndex = endselectindex - fontrule.StartIndex + 1;

            return fontrule;

        }

        private static CSSRule ParseMediaRule(AtRule rule, CSSStyleSheet parentSheet, ref string OriginalCss)
        {
            CSSMediaRule mediarule = new CSSMediaRule();
            mediarule.parentStyleSheet = parentSheet;

            string media = string.Empty;
            string wholeconditiontext = string.Empty;

            int startindex = -1;
            int endindex = -1;
            int endindexselector = -1;

            startindex = rule.prelude[0].startindex;

            // the first componentvalue is a preservedtoken and it media. 
            rule.prelude.RemoveAt(0);

            wholeconditiontext = ComponentValueExtension.getString(rule.prelude, ref startindex, ref endindexselector, ref OriginalCss);

            foreach (var item in wholeconditiontext.Split(','))
            {
                mediarule.media.appendMedium(item);
            }

            if (rule.block != null)
            {
                CSSRuleList blockrulelist = ParseMediaRuleList(rule.block, ref endindex, mediarule, ref OriginalCss);
                if (blockrulelist != null)
                {
                    mediarule.cssRules = blockrulelist;
                }
            }


            mediarule.conditionText = wholeconditiontext;

            //SelectorText is assigned in a different way now.
            // mediarule.selectorText = wholeconditiontext; /// NON-W3C.

            mediarule.StartIndex = rule.startindex;

            if (rule.endindex > endindex)
            {
                endindex = rule.endindex;
            }

            mediarule.EndIndex = endindex;
            mediarule.EndSelectorIndex = endindexselector - mediarule.StartIndex + 1;

            return mediarule;
        }

        /// <summary>
        /// consume a list of cssrule from simpleblock, 
        /// Recursive. 
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        private static CSSRuleList ParseMediaRuleList(SimpleBlock block, ref int endindex, CSSMediaRule parentmediarule, ref string OriginalCss)
        {
            int count = block.value.Count;
            CSSRuleList rulelist = new CSSRuleList();

            MediaRuleParseState state = MediaRuleParseState.init;

            CSSStyleRule stylerule = null;
            CSSMediaRule mediarule = null;

            string media = string.Empty;
            string wholeconditiontext = string.Empty;

            int startindex = -1;

            for (int i = 0; i < count; i++)
            {
                if (block.value[i].endindex > endindex)
                {
                    endindex = block.value[i].endindex;
                }

                if (startindex < 0)
                {
                    startindex = block.value[i].startindex;
                }

                switch (state)
                {
                    case MediaRuleParseState.init:
                        {
                            if (block.value[i].Type == CompoenentValueType.preservedToken)
                            {
                                PreservedToken pretoken = block.value[i] as PreservedToken;

                                if (pretoken.token.Type == enumTokenType.whitespace)
                                {
                                    // ignored whitespace at the beginning. 
                                }
                                else if (pretoken.token.Type == enumTokenType.at_keyword)
                                {
                                    // at keyword token, only handle media now. 
                                    // others to be added. 
                                    at_keyword_token token = pretoken.token as at_keyword_token;
                                    if (token.value.ToLower() == "media")
                                    {
                                        state = MediaRuleParseState.mediarule;
                                        i = i - 1; // reconsume to have correct startindex. 
                                    }
                                    else
                                    {
                                        // other at rules. 
                                        state = MediaRuleParseState.OtherAtRule;
                                        i = i - 1;
                                    }
                                }

                                /// else treat as regular style rule. 
                                else
                                {
                                    state = MediaRuleParseState.stylerule;
                                    i = i - 1; // reconsume. 
                                }
                            }
                            break;
                        }

                    case MediaRuleParseState.stylerule:
                        {
                            if (stylerule == null)
                            {
                                stylerule = new CSSStyleRule();
                                startindex = block.value[i].startindex;
                            }

                            if (block.value[i].Type == CompoenentValueType.preservedToken)
                            {
                                PreservedToken pretoken = block.value[i] as PreservedToken;
                                // not a defined way to parse the selector, assembly them back and give it to selector module.
                                // in the new way of getting selectorText, we have not need to assign it any more.  
                                //stylerule.selectorText += pretoken.token.getString();
                            }
                            else if (block.value[i].Type == CompoenentValueType.simpleBlock)
                            {
                                int endselectorindex = block.value[i].startindex;

                                stylerule.style = ParseDeclarations(block.value[i] as SimpleBlock, ref endindex, ref OriginalCss);

                                stylerule.StartIndex = startindex;
                                stylerule.EndIndex = endindex;

                                stylerule.EndSelectorIndex = endselectorindex - stylerule.StartIndex;

                                stylerule.parentRule = parentmediarule;

                                stylerule.parentStyleSheet = parentmediarule.parentStyleSheet;

                                rulelist.appendRule(stylerule);
                                stylerule = null;

                                state = MediaRuleParseState.init;
                                startindex = -1;
                            }


                            break;
                        }

                    case MediaRuleParseState.mediarule:
                        {
                            if (mediarule == null)
                            {
                                mediarule = new CSSMediaRule();
                                media = string.Empty;
                                wholeconditiontext = string.Empty;
                                startindex = block.value[i].startindex;

                                mediarule.parentStyleSheet = parentmediarule.parentStyleSheet;
                                mediarule.parentRule = parentmediarule;
                            }

                            if (block.value[i].Type == CompoenentValueType.preservedToken)
                            {
                                PreservedToken pretoken = block.value[i] as PreservedToken;

                                if (pretoken.token.Type == enumTokenType.comma)
                                {
                                    if (!string.IsNullOrEmpty(media))
                                    {
                                        mediarule.media.appendMedium(media.Trim());
                                        media = string.Empty;
                                    }
                                    wholeconditiontext += ",";
                                }
                                else
                                {
                                    // can be delim token. 
                                    if (string.IsNullOrEmpty(media) && pretoken.token.Type == enumTokenType.whitespace)
                                    {
                                        // the start of whitespace will be ignored.
                                    }
                                    else
                                    {
                                        media += pretoken.token.GetString(ref OriginalCss);
                                        wholeconditiontext += pretoken.token.GetString(ref OriginalCss);
                                    }
                                }
                            }
                            else if (block.value[i].Type == CompoenentValueType.simpleBlock)
                            {

                                CSSRuleList mediarulelist = ParseMediaRuleList(block.value[i] as SimpleBlock, ref endindex, mediarule, ref OriginalCss);

                                mediarule.cssRules = mediarulelist;

                                if (!string.IsNullOrEmpty(media))
                                {
                                    mediarule.media.appendMedium(media.Trim());
                                    wholeconditiontext += media;
                                }

                                mediarule.conditionText = wholeconditiontext;
                                mediarule.selectorText = wholeconditiontext; /// NON-W3C.

                                mediarule.StartIndex = startindex;
                                mediarule.EndIndex = endindex;

                                rulelist.appendRule(mediarule);

                                state = 0;
                                mediarule = null;
                                media = string.Empty;
                                wholeconditiontext = string.Empty;
                                startindex = -1;
                            }

                            break;
                        }

                    case MediaRuleParseState.OtherAtRule:
                        {
                            //if (mediarule == null)
                            //{
                            //    mediarule = new CSSMediaRule();
                            //    media = string.Empty;
                            //    wholeconditiontext = string.Empty;
                            //    startindex = block.value[i].startindex;

                            //    mediarule.parentStyleSheet = parentmediarule.parentStyleSheet;
                            //    mediarule.parentRule = parentmediarule;
                            //}

                            if (block.value[i].Type == CompoenentValueType.preservedToken)
                            {

                                //PreservedToken pretoken = block.value[i] as PreservedToken;

                                //if (pretoken.token.Type == enumTokenType.comma)
                                //{
                                //    if (!string.IsNullOrEmpty(media))
                                //    {
                                //        mediarule.media.appendMedium(media.Trim());
                                //        media = string.Empty;
                                //    }
                                //    wholeconditiontext += ",";
                                //}
                                //else
                                //{
                                //    // can be delim token. 
                                //    if (string.IsNullOrEmpty(media) && pretoken.token.Type == enumTokenType.whitespace)
                                //    {
                                //        // the start of whitespace will be ignored.
                                //    }
                                //    else
                                //    {
                                //        media += pretoken.token.GetString(ref OriginalCss);
                                //        wholeconditiontext += pretoken.token.GetString(ref OriginalCss);
                                //    }
                                //}
                            }
                            else if (block.value[i].Type == CompoenentValueType.simpleBlock)
                            {
                                // not implemented now. 
                                //CSSRuleList mediarulelist = ParseMediaRuleList(block.value[i] as SimpleBlock, ref endindex, mediarule, ref OriginalCss);

                                //mediarule.cssRules = mediarulelist;

                                //if (!string.IsNullOrEmpty(media))
                                //{
                                //    mediarule.media.appendMedium(media.Trim());
                                //    wholeconditiontext += media;
                                //}

                                //mediarule.conditionText = wholeconditiontext;
                                //mediarule.selectorText = wholeconditiontext; /// NON-W3C.

                                //mediarule.StartIndex = startindex;
                                //mediarule.EndIndex = endindex;

                                //rulelist.appendRule(mediarule);

                                state = MediaRuleParseState.init;
                                startindex = -1;
                            }

                            break;
                        }

                    default:
                        break;
                }

            }


            if (stylerule != null)
            {
                if (stylerule.EndIndex > stylerule.StartIndex)
                {
                    rulelist.appendRule(stylerule);
                }
                stylerule = null;
            }

            if (mediarule != null)
            {
                rulelist.appendRule(mediarule);
                mediarule = null;
            }
            return rulelist;
        }

        /// <summary>
        /// Consume a list of declaration from a simple block. 
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        private static CSSStyleDeclaration ParseDeclarations(SimpleBlock block, ref int endindex, ref string CssText)
        {
            CSSStyleDeclaration declarations = new CSSStyleDeclaration();
            string csstext = string.Empty;

            CSSDeclaration onedeclaration = null;
            bool colonfound = false;   // check value before or after colon. 

            int valuecount = block.value.Count;

            for (int i = 0; i < valuecount; i++)
            {
                ComponentValue item = block.value[i];

                if (item.endindex > endindex)
                {
                    endindex = item.endindex;
                }

                if (item.Type == CompoenentValueType.preservedToken)
                {
                    PreservedToken token = item as PreservedToken;

                    if (token.token.Type == enumTokenType.ident)
                    {
                        // the first ident is the start of one declaration.
                        if (onedeclaration == null)
                        {
                            onedeclaration = new CSSDeclaration();
                            ident_token identtoken = token.token as ident_token;
                            onedeclaration.propertyname = identtoken.value;
                            colonfound = false;
                        }
                        else
                        {
                            onedeclaration.value += token.token.GetString(ref CssText);
                        }
                    }
                    else if (token.token.Type == enumTokenType.semicolon || token.token.Type == enumTokenType.EOF)
                    {
                        // this is the end of one declaration. 
                        declarations.item.Add(onedeclaration);
                        onedeclaration = null;
                        colonfound = false;
                    }

                    else if (colonfound == false)
                    {
                        if (token.token.Type == enumTokenType.colon)
                        {
                            colonfound = true;
                        }
                        else if (token.token.Type == enumTokenType.whitespace)
                        {
                            // white space do nothing. 
                        }
                        else
                        {
                            // the next one must be white space or colon, otherwise, error. 
                            //TODO: onError.
                        }
                    }

                    else if (token.token.Type == enumTokenType.delim && ((delim_token)token.token).value == '!')
                    {
                        if ((i + 1) < valuecount)
                        {
                            if (block.value[i + 1].Type == CompoenentValueType.preservedToken)
                            {
                                PreservedToken important = block.value[i + 1] as PreservedToken;
                                if (important.token.Type == enumTokenType.ident)
                                {
                                    ident_token importantident = important.token as ident_token;
                                    if (importantident.value.ToLower() == "important" && onedeclaration != null)
                                    {
                                        onedeclaration.important = true;
                                    }
                                }
                            }

                            // has to consume the important now, because value has been processed. 
                            i = i + 1;
                        }
                    }
                    else
                    {
                        /// append the value to declaration value.
                        if (onedeclaration != null)
                        {
                            onedeclaration.value += token.token.GetString(ref CssText);
                        }
                    }

                }
                else if (item.Type == CompoenentValueType.function)
                {
                    Function func = item as Function;

                    if (onedeclaration == null)
                    {
                        onedeclaration = new CSSDeclaration();
                        onedeclaration.propertyname = func.getString(ref CssText);
                        colonfound = false;
                    }
                    else
                    {
                        if (colonfound)
                        {
                            onedeclaration.value += func.getString(ref CssText);
                        }
                        else
                        {
                            onedeclaration.propertyname += func.getString(ref CssText);
                        }

                    }



                }



            }



            if (onedeclaration != null && !string.IsNullOrEmpty(onedeclaration.propertyname))
            {
                declarations.item.Add(onedeclaration);
                onedeclaration = null;
                colonfound = false;
            }


            if (block.endindex > endindex)
            {
                endindex = block.endindex;
            }

            return declarations;


        }


        public enum MediaRuleParseState
        {
            init = 0,
            stylerule = 1,
            mediarule = 2,
            OtherAtRule = 3
        }

    }
}
