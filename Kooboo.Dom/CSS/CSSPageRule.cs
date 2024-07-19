//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;

namespace Kooboo.Dom.CSS
{

    [Serializable]
    public class CSSPageRule : CSSGroupingRule
    {

        // interface CSSPageRule : CSSGroupingRule {
        //   attribute DOMString selectorText;
        //  [SameObject, PutForwards=cssText] readonly attribute CSSStyleDeclaration style;
        //};


        //Example
        //  @page :left {
        //    margin-left: 4cm;
        //    margin-right: 3cm;
        //  }
        //The @page at-rule consists of an optional page selector (the :left pseudoclass), 
        //    followed by a block of properties that apply to the page when printed. In this way, 
        //    it’s very similar to a normal style rule, except that its properties don’t apply to any "element", 
        //    but rather the page itself.

        public CSSPageRule()
        {
            type = enumCSSRuleType.PAGE_RULE;
        }

        public string selectorText;

        public CSSStyleDeclaration style;

    }
}
