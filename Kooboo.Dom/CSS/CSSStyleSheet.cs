//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;

namespace Kooboo.Dom.CSS
{
    // http://dev.w3.org/csswg/cssom/#cssstylesheet
    // interface CSSStyleSheet : StyleSheet {
    //readonly attribute CSSRule? ownerRule;
    //[SameObject] readonly attribute CSSRuleList cssRules;
    //unsigned long insertRule(DOMString rule, unsigned long index);
    //void deleteRule(unsigned long index);
    [Serializable]
    public class CSSStyleSheet : StyleSheet
    {

        /// <summary>
        /// CSSStyleSheet.ownerRule
        //If this style sheet is imported into the document using an @import rule, the ownerRule property 
        // will return that CSSImportRule, otherwise it returns null.
        /// </summary>
        public CSSRule ownerRule;

        public CSSRuleList cssRules = new CSSRuleList();

        public CSSCharsetRule charsetRule;
        /// <summary>
        /// W3C standard API, supported, but this does not update cssText
        /// Kooboo application, use the insertOrUpdateRule
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="insertIndex">The position in the list to insert. 0 = begin position</param>
        /// <returns></returns>
        public int insertRule(CSSRule rule, int insertIndex)
        {
            return cssRules.insertRule(rule, insertIndex);
        }

        /// <summary>
        /// W3C standard, this does not update cssText. 
        /// </summary>
        /// <param name="deleteIndex"></param>
        public void deleteRule(int deleteIndex)
        {
            cssRules.deleteRule(deleteIndex);
        }

        #region "non-w3c"
        ///Below are kooboo implementation to manage the stylesheet file. Not W3C standard. 

        private string _cssText = string.Empty;

        /// <summary>
        /// Get or Set the css text, either it is embeded (href = null) or download it from the web. 
        /// When update the rulelist using non-w3c methods, this cssText will be updated accordingly. 
        /// </summary>
        public string cssText
        {
            set
            {
                _cssText = value;
            }

            get
            {
                if (string.IsNullOrEmpty(_cssText))
                {
                    if (!string.IsNullOrEmpty(this.href))
                    {
                        _cssText = Kooboo.Dom.Loader.DownloadCss(href);
                    }
                }
                return _cssText;
            }
        }

        /// <summary>
        /// Update the rule declaration block.
        /// Only support update of Media/Style/Import rule.
        /// </summary>
        /// <param name="newRule"></param>
        /// <param name="merge">Merge the same selector(for media/style) or the same url. 
        /// The first time use Merge=false, update should use merge. 
        /// This is to prevent duplicate selector in the rule list. That will cause issue for modifying the cssText.
        /// </param>
        public void updateRuleText(CSSRule newRule)
        {
            if (newRule.type == enumCSSRuleType.STYLE_RULE)
            {
                updateStyleRule((CSSStyleRule)newRule, true);
            }
            else
            {
                if (newRule.type == enumCSSRuleType.MEDIA_RULE)
                {
                    updateMediaRule((CSSMediaRule)newRule);
                }
                else
                {
                    if (newRule.type == enumCSSRuleType.IMPORT_RULE)
                    {
                        updateImportRule((CSSImportRule)newRule);
                    }
                }
            }
            ///The rest rule are not supported for update. 
        }

        /// <summary>
        /// update or insert a new CSSStyleRule. 
        /// </summary>
        /// <param name="newRule"></param>
        private void updateStyleRule(CSSStyleRule newRule, bool updateCssText)
        {
            string selectorText = newRule.selectorText.ToLower();

            // search to update exsiting records
            for (int i = 0; i < cssRules.length; i++)
            {
                if (cssRules.item[i].type == newRule.type)
                {
                    CSSStyleRule existingRule = (CSSStyleRule)cssRules.item[i];

                    if (existingRule.selectorText.ToLower() == selectorText)
                    {
                        existingRule.style.merge(newRule.style);

                        if (updateCssText)
                        {

                            // after merge, we need to update the cssText
                            int startSearchingPosition = _cssText.IndexOf(existingRule.selectorText);

                            // this must have value, otherwise, there is an error parsing. 
                            if (startSearchingPosition > 0)
                            {
                                int firstBracket = _cssText.IndexOf("{", startSearchingPosition);
                                int secondBracket = _cssText.IndexOf("}", firstBracket);

                                string firstString = _cssText.Substring(0, firstBracket + 1);
                                string secondString = _cssText.Substring(secondBracket);
                                _cssText = firstString + existingRule.cssText + secondString;

                            }
                            else
                            {
                                //TODO: error check. 
                            }
                        }
                    }
                }
            }

            // not found existing, insert a new one. 
            cssRules.insertRule(newRule, -1);

            if (updateCssText)
            {
                /// append the text to the end of csstext. 
                _cssText = _cssText + CSS.CSSSerializer.serializeCSSStyleRule(newRule);
            }
        }

        private void updateMediaRule(CSSMediaRule newRule)
        {
            //TODO:
        }

        private void updateImportRule(CSSImportRule newRule)
        {
            //TODO:
        }

        /// <summary>
        ///  This append rule does not cause an update of csstext property. 
        /// </summary>
        /// <param name="rule"></param>
        public void appendStyleRule(CSSStyleRule rule)
        {
            updateStyleRule(rule, false);
        }

        #endregion

    }
}
