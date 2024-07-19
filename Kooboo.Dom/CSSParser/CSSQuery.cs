//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Dom.CSS
{

    /// <summary>
    /// NOTE: this is not a real query now. it is ONLY written to return the list of ruleset 
    /// that match media = all. 
    /// </summary>
    public static class CSSQuery
    {
        /// <summary>
        /// generate all rule for media = all. 
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static CSSRuleList ToRuleSet(this StyleSheetList list)
        {
            return getRuleSetForAll(list);
        }

        public static CSSRuleList getRuleSetForAll(CSSStyleSheet stylesheet)
        {
            CSSRuleList rulelist = new CSSRuleList();
            addStylesheetRules(stylesheet, rulelist);
            return rulelist;
        }

        /// <summary>
        /// Return the ruleset for media = all. 
        /// </summary>
        /// <param name="stylesheetlist"></param>
        public static CSSRuleList getRuleSetForAll(StyleSheetList stylesheetlist)
        {
            CSSRuleList ruleList = new CSSRuleList();

            foreach (var item in stylesheetlist.item)
            {
                bool isMatch = false;
                if (item.Medialist.item.Count > 0)
                {
                    foreach (var subitem in item.Medialist.item)
                    {
                        if (subitem.ToLower().Contains("all"))
                        {
                            isMatch = true;
                        }
                    }
                }
                else
                {
                    isMatch = true;
                }
                if (isMatch)
                {
                    addStylesheetRules((CSSStyleSheet)item, ruleList);
                }
            }
            return ruleList;
        }

        /// <summary>
        /// Get all the ruleset that define for @media.
        /// TODO: this implementation to be verify. 
        /// </summary>
        /// <param name="stylesheetlist"></param>
        /// <returns></returns>
        public static CSSRuleList getRuleSetForMedia(StyleSheetList stylesheetlist)
        {
            CSSRuleList ruleList = new CSSRuleList();

            foreach (var item in stylesheetlist.item)
            {
                bool isMatch = true;

                if (item.Medialist.item.Count > 0)
                {
                    foreach (var subitem in item.Medialist.item)
                    {
                        if (subitem.ToLower().Contains("all"))
                        {
                            isMatch = false;
                        }
                    }
                }
                else
                {
                    isMatch = false;
                }
                if (isMatch)
                {
                    addMediaStyleSheet((CSSStyleSheet)item, ruleList);
                }
                else
                {

                    addMediaRulesFromStyleSheet((CSSStyleSheet)item, ruleList);
                }
            }
            return ruleList;
        }

        /// <summary>
        /// for query media rule.
        /// </summary>
        /// <param name="stylesheet"></param>
        /// <param name="ownerRuleList"></param>
        private static void addMediaRulesFromStyleSheet(CSSStyleSheet stylesheet, CSSRuleList ownerRuleList)
        {

            foreach (var item in stylesheet.cssRules.item)
            {
                if (item.type == enumCSSRuleType.STYLE_RULE)
                {
                    //  ownerRuleList.appendRule(item);
                }
                else if (item.type == enumCSSRuleType.IMPORT_RULE)
                {
                    // addImportRule((CSSImportRule)item, ownerRuleList);
                }
                else if (item.type == enumCSSRuleType.MEDIA_RULE)
                {
                    // addMediaRule((CSSMediaRule)item, ownerRuleList);

                    CSSMediaRule mediarule = (CSSMediaRule)item;

                    foreach (var subitem in mediarule.cssRules.item)
                    {
                        if (subitem.type == enumCSSRuleType.STYLE_RULE)
                        {
                            ownerRuleList.appendRule(item);
                        }
                        else if (item.type == enumCSSRuleType.IMPORT_RULE)
                        {
                            //addImportRule((CSSImportRule)item, ownerRuleList);
                        }
                        else if (item.type == enumCSSRuleType.MEDIA_RULE)
                        {
                            //addMediaRule((CSSMediaRule)item, ownerRuleList);
                        }
                    }

                }
            }
        }

        /// <summary>
        /// for query media rules
        /// </summary>
        /// <param name="stylesheet"></param>
        /// <param name="ownerRuleList"></param>
        private static void addMediaStyleSheet(CSSStyleSheet stylesheet, CSSRuleList ownerRuleList)
        {
            foreach (var item in stylesheet.cssRules.item)
            {
                if (item.type == enumCSSRuleType.STYLE_RULE)
                {
                    ownerRuleList.appendRule(item);
                }
                else if (item.type == enumCSSRuleType.IMPORT_RULE)
                {
                    //addImportRule((CSSImportRule)item, ownerRuleList);
                }
                else if (item.type == enumCSSRuleType.MEDIA_RULE)
                {
                    //addMediaRule((CSSMediaRule)item, ownerRuleList);
                }
            }
        }

        private static void addStylesheetRules(CSSStyleSheet stylesheet, CSSRuleList ownerRuleList)
        {

            foreach (var item in stylesheet.cssRules.item)
            {
                if (item.type == enumCSSRuleType.STYLE_RULE)
                {
                    ownerRuleList.appendRule(item);
                }
                else if (item.type == enumCSSRuleType.IMPORT_RULE)
                {
                    addImportRule((CSSImportRule)item, ownerRuleList);
                }
                else if (item.type == enumCSSRuleType.MEDIA_RULE)
                {
                    addMediaRule((CSSMediaRule)item, ownerRuleList);
                }
            }
        }

        private static void addMediaRule(CSSMediaRule rule, CSSRuleList ownerRuleList)
        {
            bool isMatch = false;
            if (rule.media.item.Count > 0)
            {
                foreach (var subitem in rule.media.item)
                {
                    if (subitem.ToLower().Contains("all"))
                    {
                        isMatch = true;
                    }
                }
            }
            else
            {
                isMatch = true;
            }

            if (isMatch)
            {
                foreach (var item in rule.cssRules.item)
                {
                    if (item.type == enumCSSRuleType.STYLE_RULE)
                    {
                        ownerRuleList.appendRule(item);
                    }
                    else if (item.type == enumCSSRuleType.IMPORT_RULE)
                    {
                        addImportRule((CSSImportRule)item, ownerRuleList);
                    }
                    else if (item.type == enumCSSRuleType.MEDIA_RULE)
                    {
                        addMediaRule((CSSMediaRule)item, ownerRuleList);
                    }
                }
            }
        }

        private static void addImportRule(CSSImportRule rule, CSSRuleList ownerRuleList)
        {
            bool isMatch = false;
            if (rule.media.item.Count > 0)
            {
                foreach (var subitem in rule.media.item)
                {
                    if (subitem.ToLower().Contains("all"))
                    {
                        isMatch = true;
                    }
                }
            }
            else
            {
                isMatch = true;
            }

            if (isMatch)
            {
                addStylesheetRules(rule.stylesheet, ownerRuleList);
            }
        }

    }
}
