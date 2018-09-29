//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
#region License
// 
// Copyright (c) 2013, Kooboo team
// 
// Licensed under the BSD License
// See the file LICENSE.txt for details.
// 
#endregion
using Kooboo.Dom.CSS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Dom.AntlrCss.Css3.Parser;

namespace Kooboo.Dom.CSSParser
{
    public class AntlrParser
    {
        public static CSSStyleSheet ParseStylesheet(string text,string basePath="")
        {
            var antlrObj = Css3Helper.ParseStylesheet(text);
            var w3cObj = ModelConverter.ConvertToCSSStyleSheet(antlrObj,basePath);
            return w3cObj;
        }

        public static CSSPageRule ParsePageRule(string text)
        {
            var antlrObj = Css3Helper.ParsePageRule(text);
            var w3cObj = ModelConverter.ConvertToCSSPageRule(antlrObj);
            return w3cObj;
        }

        public static CSSMediaRule ParseMediaRule(string text)
        {
            var antlrObj = Css3Helper.ParseMedia(text);
            var w3cObj = ModelConverter.ConvertToCSSMediaRule(antlrObj);
            return w3cObj;
        }

        public static CSSImportRule ParseImportRule(string text,string basePath="")
        {
            var antlrObj = Css3Helper.ParseImport(text);
            var w3cObj = ModelConverter.ConvertToCSSImportRule(antlrObj,basePath);
            return w3cObj;
        }

        public static CSSStyleRule ParseStyleRule(string text)
        {
            var antlrObj = Css3Helper.ParseRuleSet(text);
            var w3cObj = ModelConverter.ConvertToCSSStyleRule(antlrObj);
            return w3cObj;
        }

        public static CSSCharsetRule ParseCharsetRule(string text) {
            var antlrObj = Css3Helper.ParseCharSet(text);
            var w3cObj = ModelConverter.ConvertToCSSCharsetRule(antlrObj);
            return w3cObj;
        }

        public static CSSMarginRule ParseMarginRule(string text)
        {
            var antlrObj = Css3Helper.ParsePageMarginBox(text);
            var w3cObj = ModelConverter.ConvertToCSSMarginRule(antlrObj);
            return w3cObj;
        }		
    }
}
