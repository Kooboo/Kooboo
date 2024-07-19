//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;

namespace Kooboo.Dom.CSS
{


    /// <summary>
    /// A simple selector is either a type selector, universal selector, attribute selector, class selector, ID selector, or pseudo-class.
    /// </summary>
    [Serializable]
    public class simpleSelector
    {

        public simpleSelector()
        {

        }

        public enumSimpleSelectorType Type;

        /// <summary>
        /// the original text. 
        /// </summary>
        public string wholeText;


    }
}
