//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Dom.CSS
{

    /// <summary>
    /// Summary
    ///A CSSRuleList is an array-like object containing an ordered collection of CSSRule objects.
    /// </summary>
    [Serializable]
    public class CSSRuleList
    {
        /// <summary>
        /// Each CSSRule can be accessed as rules.item(index), or simply rules[index], 
        /// where rules is an object implementing the CSSRuleList interface, 
        /// and index is the 0-based index of the rule, in the order as it appears in the style sheet CSS. The number of objects is rules.length.
        /// </summary>
        public List<CSSRule> item = new List<CSSRule>();

        public int length
        {
            get { return item.Count(); }
        }

        public CSSRule this[int index]
        {
            get
            {
                return this.item[index];
            }
            set
            {
                this.item[index] = value;
            }
        }


        /// <summary>
        /// insert a new rule.  use -1 to append new.
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="insertIndex">use -1 to insert new.</param>
        /// <returns></returns>
        public int insertRule(CSSRule rule, int insertIndex)
        {
            if (insertIndex > length || insertIndex < 0)
            {
                item.Add(rule);
                return item.Count() - 1;
            }
            else
            {
                item.Insert(insertIndex, rule);
                return insertIndex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deleteIndex">zero based index</param>
        public void deleteRule(int deleteIndex)
        {
            if (deleteIndex < item.Count())
            {
                item.RemoveAt(deleteIndex);
            }
        }

        #region "non-w3c"

        public void appendRule(CSSRule rule)
        {
            insertRule(rule, -1);
        }

        #endregion

    }
}
