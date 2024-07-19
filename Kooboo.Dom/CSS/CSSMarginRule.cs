//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Dom.CSS
{
    /// <summary>
    /// http://dev.w3.org/csswg/cssom/#the-cssmarginrule-interface
    /// </summary>
    public class CSSMarginRule : CSSRule
    {
        public CSSMarginRule()
        {
            base.type = enumCSSRuleType.MARGIN_RULE;
            this.style = new CSSStyleDeclaration();
        }
        public string name { get; set; }
        public CSSStyleDeclaration style { get; set; }
    }
}
