//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Dom.CSS;

namespace Kooboo.Dom
{
    // Inherits properties from its parent, HTMLElement, and implements LinkStyle.

    public class HtmlStyleElement : Element
    {
        //HTMLStyleElement.media
        //Is a DOMString representing the intended destination medium for style information.
        public string media;

        //HTMLStyleElement.type
        //Is a DOMString representing the type of style being applied by this statement.
        public string type;

        //HTMLStyleElement.disabled
        //Is a Boolean value, with true if the stylesheet is disabled, and false if not.
        public bool disabled;

        //LinkStyle.sheet Read only
        //Returns the StyleSheet object associated with the given element, or null if there is none
        public StyleSheet LinkStyleSheet = new StyleSheet();

        //HTMLStyleElement.scoped
        //Is a Boolean value indicating if the element applies to the whole document (false) or only to the parent's sub-tree (true).
        public bool scoped;


    }


}
