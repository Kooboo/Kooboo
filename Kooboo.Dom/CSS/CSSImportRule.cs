//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.

namespace Kooboo.Dom.CSS
{
    //http://dev.w3.org/csswg/cssom/#cssimportrule

    public class CSSImportRule : CSSRule
    {
        public CSSImportRule()
        {
            base.type = enumCSSRuleType.IMPORT_RULE;
            media = new MediaList();
        }

        public string href;

        public string urlToken => cssText.Length < 8 ? string.Empty : cssText.Substring(8).TrimEnd(';', ' ');

        /// <summary>
        /// The media attribute must return the value of the media attribute of the associated CSS style sheet.
        /// </summary>
        public MediaList media;

        /// <summary>
        /// The styleSheet attribute must return the associated CSS style sheet.
        /// Even is the stylesheet parse failed, it should return the stylesheet with rulelist item count=0
        /// </summary>
        public CSSStyleSheet stylesheet;
    }
}