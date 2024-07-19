//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Dom.CSS
{
    [Serializable]
    public class StyleSheetList
    {
        //http://dev.w3.org/csswg/cssom/#stylesheetlist
        // [ArrayClass]
        //interface StyleSheetList {
        //  getter StyleSheet? item(unsigned long index);
        //  readonly attribute unsigned long length;
        //};

        public List<StyleSheet> item = new List<StyleSheet>();

        public int length
        {
            get
            {
                return item.Count();
            }
        }

        #region NON-W3C

        public void appendStyleSheet(StyleSheet styleSheet)
        {
            item.Add(styleSheet);
        }


        #endregion

    }
}