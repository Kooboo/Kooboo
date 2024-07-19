//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;

namespace Kooboo.Dom.CSS
{
    /// <summary>
    /// http://dev.w3.org/csswg/cssom/#the-csscharsetrule-interface
    /// </summary>
    [Serializable]
    public class CSSCharsetRule : CSSRule
    {

        public CSSCharsetRule()
        {
            base.type = enumCSSRuleType.CHARSET_RULE;
        }
        public string encoding { get; set; }

    }
}
