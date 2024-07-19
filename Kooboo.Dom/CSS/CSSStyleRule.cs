//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;

namespace Kooboo.Dom.CSS
{

    //http://dev.w3.org/csswg/cssom/#cssstylerule
    //  interface CSSStyleRule : CSSRule {
    //  attribute DOMString selectorText;
    //  [SameObject, PutForwards=cssText] readonly attribute CSSStyleDeclaration style;
    //};
    //The selectorText attribute, on getting, must return the result of serializing the associated group of selectors.
    // 
    [Serializable]
    public class CSSStyleRule : CSSRule
    {

        public CSSStyleRule()
        {
            base.type = enumCSSRuleType.STYLE_RULE;
        }


        /// <summary>
        /// CSSStyleDeclaration == CSSDeclarationBlock
        /// </summary>
        public CSSStyleDeclaration style = new CSSStyleDeclaration();

        private List<simpleSelector> _selectors;

        public List<simpleSelector> selectors
        {
            get
            {
                if (_selectors == null)
                {
                    _selectors = SelectorParser.parseSelectorGroup(selectorText);
                }
                return _selectors;
            }
        }

    }
}
