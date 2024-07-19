//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;

namespace Kooboo.Dom.CSS
{
    /// <summary>
    /// http://dev.w3.org/csswg/cssom/#the-cssnamespacerule-interface
    /// </summary>
    [Serializable]
    public class CSSNamespaceRule : CSSRule
    {
        public CSSNamespaceRule()
        {
            base.type = enumCSSRuleType.NAMESPACE_RULE;
        }
        public string namespaceURI { get; set; }
        public string prefix { set; get; }
    }
}
