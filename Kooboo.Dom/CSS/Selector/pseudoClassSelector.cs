//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Dom.CSS
{

    /// <summary>
    /// 
    /// </summary>
    public class pseudoClassSelector : simpleSelector
    {

        public pseudoClassSelector()
        {
            base.Type = enumSimpleSelectorType.pseudoclass;
        }

        public string elementE { get; set; }

        public string matchText { get; set; }


        private simpleSelector _elementSelector;
        public simpleSelector ElementSelector
        {
            get
            {
                if (_elementSelector == null && !string.IsNullOrEmpty(elementE))
                {
                    _elementSelector = SelectorParser.parseOneSelector(elementE);
                }

                return _elementSelector;
            }
        }

        private simpleSelector _notselector;

        public simpleSelector NotSelector
        {
            get
            {
                if (_notselector == null)
                {
                    if (!string.IsNullOrEmpty(matchText))
                    {
                        if (matchText.ToLower().Contains("not"))
                        {
                            var begin = matchText.IndexOf("(");
                            var end = matchText.LastIndexOf(")");

                            if (begin > 0 && end > 0)
                            {
                                int len = end - begin - 1;
                                if (len > 0)
                                {
                                    string innerMatch = matchText.Substring(begin + 1, len);
                                    _notselector = SelectorParser.parseOneSelector(innerMatch);
                                }
                            }
                        }
                    }
                }

                return _notselector;

            }
        }
    }
}


//E:root	an E element, root of the document	Structural pseudo-classes	3
//E:nth-child(n)	an E element, the n-th child of its parent	Structural pseudo-classes	3
//E:nth-last-child(n)	an E element, the n-th child of its parent, counting from the last one	Structural pseudo-classes	3
//E:nth-of-type(n)	an E element, the n-th sibling of its type	Structural pseudo-classes	3
//E:nth-last-of-type(n)	an E element, the n-th sibling of its type, counting from the last one	Structural pseudo-classes	3
//E:first-child	an E element, first child of its parent	Structural pseudo-classes	2
//E:last-child	an E element, last child of its parent	Structural pseudo-classes	3
//E:first-of-type	an E element, first sibling of its type	Structural pseudo-classes	3
//E:last-of-type	an E element, last sibling of its type	Structural pseudo-classes	3
//E:only-child	an E element, only child of its parent	Structural pseudo-classes	3
//E:only-of-type	an E element, only sibling of its type	Structural pseudo-classes	3
//E:empty	an E element that has no children (including text nodes)	Structural pseudo-classes	3
//E:link
//E:visited	an E element being the source anchor of a hyperlink of which the target is not yet visited (:link) or already visited (:visited)	The link pseudo-classes	1
//E:active
//E:hover
//E:focus	an E element during certain user actions	The user action pseudo-classes	1 and 2
//E:target	an E element being the target of the referring URI	The target pseudo-class	3
//E:lang(fr)	an element of type E in language "fr" (the document language specifies how language is determined)	The :lang() pseudo-class	2
//E:enabled
//E:disabled	a user interface element E which is enabled or disabled	The UI element states pseudo-classes	3
//E:checked	a user interface element E which is checked (for instance a radio-button or checkbox)	The UI element states pseudo-classes