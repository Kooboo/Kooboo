//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;

namespace Kooboo.Dom.Dom
{
    public class HtmlElementType
    {

        // http://www.w3.org/TR/html5/syntax.html#parsing
        // 8.1.2 Element
        //There are five different kinds of elements: void elements, raw text elements, escapable raw text elements, foreign elements, and normal elements.
        //Void elements
        //area, base, br, col, embed, hr, img, input, keygen, link, meta, param, source, track, wbr
        //Raw text elements
        //script, style
        //escapable raw text elements
        //textarea, title
        //Foreign elements
        //Elements from the MathML namespace and the SVG namespace.
        //Normal elements
        //All other allowed HTML elements are normal elements.
        //Tags are used to delimit the start and end of elements in the markup. Raw text, escapable raw text, and normal elements have a start tag to indicate where they begin, and an end tag to indicate where they end. The start and end tags of certain normal elements can be omitted, as described below in the section on optional tags. Those that cannot be omitted must not be omitted. Void elements only have a start tag; end tags must not be specified for void elements. Foreign elements must either have a start tag and an end tag, or a start tag that is marked as self-closing, in which case they must not have an end tag.

        //The contents of the element must be placed between just after the start tag (which might be implied, in certain cases) and just before the end tag (which again, might be implied in certain cases). The exact allowed contents of each individual element depend on the content model of that element, as described earlier in this specification. Elements must not contain content that their content model disallows. In addition to the restrictions placed on the contents by those content models, however, the five types of elements have additional syntactic requirements.

        //Void elements can't have any contents (since there's no end tag, no content can be put between the start tag and the end tag).

        //Raw text elements can have text, though it has restrictions described below.

        //Escapable raw text elements can have text and character references, but the text must not contain an ambiguous ampersand. There are also further restrictions described below.

        //Foreign elements whose start tag is marked as self-closing can't have any contents (since, again, as there's no end tag, no content can be put between the start tag and the end tag). Foreign elements whose start tag is not marked as self-closing can have text, character references, CDATA sections, other elements, and comments, but the text must not contain the character "<" (U+003C) or an ambiguous ampersand.

        public static bool isVoidElement(string tagName)
        {
            return voidElement().Contains(tagName);
        }

        private static List<string> _voidElement;
        /// <summary>
        /// Void elements can't have any contents (since there's no end tag, no content can be put between the start tag and the end tag).
        /// </summary>
        /// <returns></returns>
        public static List<string> voidElement()
        {
            if (_voidElement == null)
            {
                _voidElement = new List<string>();
                _voidElement.Add("area");
                _voidElement.Add("base");
                _voidElement.Add("br");
                _voidElement.Add("col");
                _voidElement.Add("command");
                _voidElement.Add("embed");
                _voidElement.Add("hr");
                _voidElement.Add("img");
                _voidElement.Add("input");
                _voidElement.Add("keygen");
                _voidElement.Add("link");
                _voidElement.Add("meta");
                _voidElement.Add("param");
                _voidElement.Add("source");
                _voidElement.Add("track");
                _voidElement.Add("wbr");
            }

            return _voidElement;

        }

    }




}
