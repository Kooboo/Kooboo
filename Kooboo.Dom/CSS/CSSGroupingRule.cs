//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;

namespace Kooboo.Dom.CSS
{

    // 
    //interface CSSGroupingRule : CSSRule {
    //    readonly attribute CSSRuleList cssRules;
    //    unsigned long insertRule (DOMString rule, unsigned long index);
    //    void deleteRule (unsigned long index);
    //}

    /// <summary>
    /// The CSSGroupingRule interface represents an at-rule that contains other rules nested inside itself.
    /// </summary>
    [Serializable]
    public class CSSGroupingRule : CSSRule
    {

        public CSSRuleList cssRules = new CSSRuleList();

        public int insertRule(CSSRule rule, int insertIndex)
        {
            return cssRules.insertRule(rule, insertIndex);
        }

        public void deleteRule(int deleteIndex)
        {
            cssRules.deleteRule(deleteIndex);
        }


    }
}
