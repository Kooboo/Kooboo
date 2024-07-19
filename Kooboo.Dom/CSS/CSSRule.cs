//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;

namespace Kooboo.Dom.CSS
{
    //http://dev.w3.org/csswg/cssom/#cssrule


    [Serializable]
    public class CSSRule
    {

        /// <summary>
        /// type
        /// </summary>
        /// <param name="type">A non-negative integer associated with a particular type of rule. This item is initialized when a rule is created and cannot change.</param>
        public CSSRule()
        {

        }

        /// <summary>
        /// Once this is created, it is not allowed to change. 
        /// </summary>
        public enumCSSRuleType type
        {
            get;
            protected set;
        }

        /// <summary>
        /// Represents the textual representation of the entire rule, e.g. "h1,h2 { font-size: 16pt }"
        /// </summary>
        public string cssText
        {
            get
            {
                if (string.IsNullOrEmpty(_csstext))
                {
                    int endindex = EndIndex;
                    int maxlen = this.parentStyleSheet.cssText.Length - 1;
                    if (endindex > maxlen)
                    {
                        endindex = maxlen;
                    }
                    _csstext = this.parentStyleSheet.cssText.Substring(StartIndex, endindex - StartIndex + 1);
                }
                return _csstext;
            }
            set
            {
                _csstext = value;
            }

        }

        private string _csstext;

        public string styleDeclarationText
        {

            get
            {
                if (string.IsNullOrEmpty(_styleDeclerationText))
                {
                    _styleDeclerationText = this.cssText.Substring(EndSelectorIndex + 1, EndIndex - StartIndex - EndSelectorIndex - 1);
                }
                return _styleDeclerationText;
            }
            set
            {
                _styleDeclerationText = value;
            }
        }

        private string _styleDeclerationText;

        private string _selectorText;

        /// <summary>
        /// Get or set the textual representation of the selector for this rule, e.g. "h1,h2" or "@import"
        /// </summary>
        public string selectorText
        {
            get
            {
                if (string.IsNullOrEmpty(_selectorText))
                {
                    _selectorText = this.cssText.Substring(0, this.EndSelectorIndex).Trim();
                }
                return _selectorText;
            }

            set
            {
                _selectorText = value;
            }
        }

        public CSSRule parentRule;

        public CSSStyleSheet parentStyleSheet;

        /// <summary>
        /// start index on the underlining file stream. 
        /// </summary>
        public int StartIndex;

        /// <summary>
        /// end index on the css text file stream.
        /// </summary>
        public int EndIndex;

        /// <summary>
        /// The end of Selector index within the cssText of this current rule,, this is used to get the rule text. 
        /// </summary>
        public int EndSelectorIndex;

    }


}
