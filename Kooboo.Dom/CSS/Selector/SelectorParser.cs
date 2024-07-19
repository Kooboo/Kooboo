//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;

namespace Kooboo.Dom.CSS
{
    public static class SelectorParser
    {

        /// <summary>
        /// A comma-separated list of selectors represents the union of all elements selected by each of the individual selectors in the list. (A comma is U+002C.) For example, in CSS when several selectors share the same declarations, they may be grouped into a comma-separated list. White space may appear before and/or after the comma.
        /// </summary>
        /// <param name="selectorGroup"></param>
        /// <returns></returns>
        public static List<simpleSelector> parseSelectorGroup(string selectorGroup)
        {
            List<simpleSelector> list = new List<simpleSelector>();

            string[] selectorlist = selectorGroup.Split(',');
            foreach (var item in selectorlist)
            {
                if (!string.IsNullOrEmpty(item.Trim()))
                {
                    list.Add(parseOneSelector(item.Trim()));
                }

            }

            return list;
        }

        public static simpleSelector parseOneSelector(string oneSelector)
        {
            simpleSelectorParser parser = new simpleSelectorParser(oneSelector);

            return parser.Parse();
        }


    }
}
