//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Dom.CSS
{
    public static class selectorMatch
    {

        public static bool Match(Element element, string selectorText)
        {
            List<simpleSelector> selectorList = SelectorParser.parseSelectorGroup(selectorText);

            return Match(element, selectorList);
        }

        public static bool Match(Element element, List<simpleSelector> selectorlist)
        {
            foreach (var item in selectorlist)
            {
                if (matchOneSelector(element, item))
                {
                    return true;
                }
            }
            return false;

        }

        public static bool Match(Element element, simpleSelector selector)
        {
            return matchOneSelector(element, selector);
        }

        private static bool matchOneSelector(Element element, simpleSelector selector)
        {

            switch (selector.Type)
            {
                case enumSimpleSelectorType.universal:
                    return true;

                case enumSimpleSelectorType.type:
                    return matchType(element, (typeSelector)selector);

                case enumSimpleSelectorType.attribute:
                    return matchAttribute(element, (attributeSelector)selector);

                case enumSimpleSelectorType.classSelector:
                    return matchClass(element, (classSelector)selector);

                case enumSimpleSelectorType.id:
                    return matchId(element, (idSelector)selector);

                case enumSimpleSelectorType.pseudoclass:
                    return matchPseudoClass(element, (pseudoClassSelector)selector);

                case enumSimpleSelectorType.combinator:
                    return matchCombinator(element, (combinatorSelector)selector);

                case enumSimpleSelectorType.pseudoElement:
                    return matchPseudoElement(element, (pseudoElementSelector)selector);

                default:
                    return false;
            }

        }

        private static bool matchType(Element element, typeSelector typeselector)
        {
            return (element.tagName == typeselector.elementE.ToLower());
        }

        private static bool matchClass(Element element, classSelector classselector)
        {
            if (!string.IsNullOrEmpty(classselector.elementE) && classselector.elementE != "*")
            {
                if (element.tagName != classselector.elementE.ToLower())
                {
                    return false;
                }
            }

            if (element.hasAttribute("class"))
            {
                string classvalue = element.getAttribute("class");

                if (string.IsNullOrEmpty(classvalue))
                {
                    if (classselector.classList.Count > 0)
                    {
                        return false;
                    }
                    return true;
                }

                foreach (var item in classselector.classList)
                {
                    if (!classvalue.Contains(item))
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        private static bool matchId(Element element, idSelector idselector)
        {

            if (!string.IsNullOrEmpty(idselector.elementE))
            {
                if (element.tagName != idselector.elementE.ToLower() && idselector.elementE != "*")
                {
                    return false;
                }
            }

            if (element.hasAttribute("id"))
            {
                string idvalue = element.getAttribute("id");

                if (idvalue == idselector.id)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }

        private static bool matchAttribute(Element element, attributeSelector attSelector)
        {
            if (!string.IsNullOrEmpty(attSelector.elementE) && attSelector.elementE != "*")
            {
                if (element.tagName != attSelector.elementE.ToLower())
                {
                    return false;
                }
            }

            ///E[foo]	an E element with a "foo" attribute
            //E[foo="bar"]	an E element whose "foo" attribute value is exactly equal to "bar"	Attribute selectors	2
            //E[foo~="bar"]	an E element whose "foo" attribute value is a list of whitespace-separated values, one of which is exactly equal to "bar"	Attribute selectors	2
            //E[foo^="bar"]	an E element whose "foo" attribute value begins exactly with the string "bar"	Attribute selectors	3
            //E[foo$="bar"]	an E element whose "foo" attribute value ends exactly with the string "bar"	Attribute selectors	3
            //E[foo*="bar"]	an E element whose "foo" attribute value contains the substring "bar"	Attribute selectors	3
            //E[foo|="en"]	an E element whose "foo" attribute has a hyphen-separated list of values beginning (from the left) with "en"	Attribute selectors	2
            //[att|=val]
            //Represents an element with the att attribute, its value either being exactly "val" or beginning with "val" immediately followed by "-" (U+002D). This is primarily intended to allow language subcode matches (e.g., the hreflang attribute on the a element in HTML) as described in BCP 47 ([BCP47]) or its successor. For lang (or xml:lang) language subcode matching, please see the :lang pseudo-class.


            switch (attSelector.matchType)
            {
                case enumAttributeType.exactlyEqual:
                    {
                        string attvalue = element.getAttribute(attSelector.attributeName);
                        return (attvalue == attSelector.attributeValue);
                    }
                case enumAttributeType.whitespaceSeperated:
                    {
                        string attvalue = element.getAttribute(attSelector.attributeName);
                        if (string.IsNullOrEmpty(attvalue))
                        {
                            return false;
                        }
                        string[] liststr = attvalue.Split(' ');

                        foreach (var item in liststr)
                        {
                            if (item == attSelector.attributeValue)
                            {
                                return true;
                            }
                        }
                        return false;
                    }

                case enumAttributeType.exactlyBegin:
                    {

                        string attvalue = element.getAttribute(attSelector.attributeName);

                        if (string.IsNullOrEmpty(attvalue))
                        {
                            return false;
                        }

                        if (attvalue.StartsWith(attSelector.attributeValue))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                case enumAttributeType.exactlyEnd:
                    {

                        string attvalue = element.getAttribute(attSelector.attributeName);

                        if (string.IsNullOrEmpty(attvalue))
                        {
                            return false;
                        }

                        if (attvalue.EndsWith(attSelector.attributeValue))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                case enumAttributeType.contains:
                    {
                        string attvalue = element.getAttribute(attSelector.attributeName);

                        if (string.IsNullOrEmpty(attvalue))
                        {
                            return false;
                        }

                        if (attvalue.Contains(attSelector.attributeValue))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                case enumAttributeType.hyphenSeperated:
                    {
                        /// The following selector represents an a element for which the 
                        /// value of the hreflang attribute begins with "en", 
                        /// including "en", "en-US", and "en-scouse":


                        string attvalue = element.getAttribute(attSelector.attributeName);

                        if (string.IsNullOrEmpty(attvalue))
                        {
                            return false;
                        }

                        if (attvalue == attSelector.attributeValue)
                        {
                            return true;
                        }

                        if (attvalue.StartsWith(attSelector.attributeValue + '\u002D'.ToString()))
                        {
                            return true;
                        }


                        return false;

                    }
                case enumAttributeType.defaultHas:
                    {
                        return element.hasAttribute(attSelector.attributeName);

                    }
                default:
                    {
                        return element.hasAttribute(attSelector.attributeName);
                    }
            }
        }

        private static bool matchCombinator(Element element, combinatorSelector combineSelector)
        {
            int index = combineSelector.item.Count() - 1;
            Element matched = element;
            simpleSelector upMatchSelector;

            combinatorClause clause = combineSelector.item[index];

            if (!matchOneSelector(matched, clause.selector))
            {
                return false;
            }

            for (int i = index; i >= 0; i--)
            {
                if (i == 0)
                {
                    return true;
                }

                combinator combinetype = combineSelector.item[i].combineType;

                upMatchSelector = combineSelector.item[i - 1].selector;

                switch (combinetype)
                {
                    case combinator.Descendant:
                        {
                            Element parentElement = matched.parentElement;
                            bool matchfound = false;
                            while (parentElement != null && parentElement.nodeType == enumNodeType.ELEMENT)
                            {
                                if (selectorMatch.matchOneSelector(parentElement, upMatchSelector))
                                {
                                    matched = parentElement;
                                    matchfound = true;
                                    break;
                                }
                                parentElement = parentElement.parentElement;
                            }
                            if (matchfound)
                            {
                                continue;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    case combinator.Child:
                        {
                            Element parentElement = matched.parentElement;

                            if (parentElement != null && parentElement.nodeType == enumNodeType.ELEMENT && selectorMatch.matchOneSelector(parentElement, upMatchSelector))
                            {
                                matched = parentElement;
                                continue;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    case combinator.AdjacentSibling:
                        {

                            Node previousnode = matched.previousSibling();
                            Element previouselement = null;

                            while (previousnode != null)
                            {
                                if (previousnode.nodeType == enumNodeType.ELEMENT)
                                {
                                    previouselement = (Element)previousnode;
                                    break;
                                }
                                previousnode = previousnode.previousSibling();
                            }

                            if (previouselement == null)
                            {
                                return false;
                            }
                            else
                            {
                                if (selectorMatch.matchOneSelector(previouselement, upMatchSelector))
                                {
                                    matched = previouselement;
                                    continue;
                                }
                                else
                                {
                                    return false;
                                }

                            }


                        }
                    case combinator.Sibling:
                        {

                            Node previousnode = matched.previousSibling();
                            bool matchedfound = false;


                            while (previousnode != null)
                            {
                                if (previousnode.nodeType == enumNodeType.ELEMENT)
                                {
                                    Element previouselement = (Element)previousnode;
                                    if (selectorMatch.matchOneSelector(previouselement, upMatchSelector))
                                    {
                                        matchedfound = true;
                                        matched = previouselement;
                                        break;
                                    }
                                }

                                previousnode = previousnode.previousSibling();

                            }

                            if (matchedfound)
                            {
                                continue;
                            }
                            else
                            {
                                return false;
                            }

                        }


                    default:
                        return false;
                }

            }

            return true;
        }

        private static bool matchPseudoClass(Element element, pseudoClassSelector pseudoClass)
        {
            if (!string.IsNullOrEmpty(pseudoClass.elementE))
            {
                if (!selectorMatch.Match(element, pseudoClass.ElementSelector))
                {
                    return false;
                }
            }

            pseudoClass.matchText = pseudoClass.matchText.ToLower().Trim();

            // E:root	an E element, root of the document	Structural pseudo-classes	3
            if (pseudoClass.matchText == "root")
            {
                if (element.tagName == "html" && element.depth == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            //E:nth-child(n)	an E element, the n-th child of its parent	Structural pseudo-classes	3
            else if (pseudoClass.matchText.Contains("nth-child"))
            {
                return _pseudoClassNthMatch(element, pseudoClass, false, false);
            }
            else if (pseudoClass.matchText.Contains("not"))
            {
                if (pseudoClass.NotSelector == null)
                {
                    return false;
                }

                var innermatch = selectorMatch.Match(element, pseudoClass.NotSelector);

                return !innermatch;
            }
            //E:nth-last-child(n)	an E element, the n-th child of its parent, counting from the last one	Structural pseudo-classes	3
            else if (pseudoClass.matchText.Contains("nth-last-child"))
            {
                return _pseudoClassNthMatch(element, pseudoClass, true, false);
            }

            //E:nth-of-type(n)	an E element, the n-th sibling of its type	Structural pseudo-classes	3
            else if (pseudoClass.matchText.Contains("nth-of-type"))
            {
                return _pseudoClassNthMatch(element, pseudoClass, false, true);
            }
            //E:nth-last-of-type(n)	an E element, the n-th sibling of its type, counting from the last one	Structural pseudo-classes	3
            else if (pseudoClass.matchText.Contains("nth-last-of-type"))
            {
                return _pseudoClassNthMatch(element, pseudoClass, true, true);
            }

            //E:first-child	an E element, first child of its parent	Structural pseudo-classes	2
            else if (pseudoClass.matchText == "first-child")
            {
                int counter = _getSilbingCount(element, pseudoClass, false, false);
                return (counter == 0);
            }
            //E:last-child	an E element, last child of its parent	Structural pseudo-classes	3

            else if (pseudoClass.matchText == "last-child")
            {
                int counter = _getSilbingCount(element, pseudoClass, true, false);

                return (counter == 0);
            }

            //E:first-of-type	an E element, first sibling of its type	Structural pseudo-classes	3
            else if (pseudoClass.matchText == "first-of-type")
            {
                int counter = _getSilbingCount(element, pseudoClass, false, true);

                return (counter == 0);
            }
            //E:last-of-type	an E element, last sibling of its type	Structural pseudo-classes	3
            else if (pseudoClass.matchText == "last-of-type")
            {
                int counter = _getSilbingCount(element, pseudoClass, true, true);
                return (counter == 0);
            }

            //E:only-child	an E element, only child of its parent	Structural pseudo-classes	3
            else if (pseudoClass.matchText == "only-child")
            {
                int counterbefore = _getSilbingCount(element, pseudoClass, false, false);
                int counterafter = _getSilbingCount(element, pseudoClass, true, false);
                return (counterafter == 0 && counterbefore == 0);
            }
            //E:only-of-type	an E element, only sibling of its type	Structural pseudo-classes	3
            else if (pseudoClass.matchText == "only-of-type")
            {
                int counterbefore = _getSilbingCount(element, pseudoClass, false, true);
                int counterafter = _getSilbingCount(element, pseudoClass, true, true);
                return (counterafter == 0 && counterbefore == 0);
            }

            //  E:empty	an E element that has no children (including text nodes)	Structural pseudo-classes	3
            else if (pseudoClass.matchText == "empty")
            {
                return (element.childNodes.length == 0);
            }
            //E:link
            //E:visited	an E element being the source anchor of a hyperlink of which the target is not yet visited (:link) or already visited (:visited)	The link pseudo-classes	1
            //E:active
            //E:hover
            //E:focus	an E element during certain user actions	The user action pseudo-classes	1 and 2

            //E:enabled
            //E:disabled	a user interface element E which is enabled or disabled	The UI element states pseudo-classes	3
            //E:checked	a user interface element E which is checked (for instance a radio-button or checkbox)	The UI element states pseudo-classes	3

            else if (pseudoClass.matchText.isOneOf("active", "hover", "focus"))
            {
                return true;
            }
            else if (pseudoClass.matchText.isOneOf("link", "visited"))
            {
                return (element.tagName == "a" || element.tagName == "area");
            }
            else if (pseudoClass.matchText.isOneOf("enabled", "disabled"))
            {
                return true;
            }
            else if (pseudoClass.matchText == "checked")
            {
                return element.tagName == "input";
            }

            //E:target	an E element being the target of the referring URI	The target pseudo-class	3
            else if (pseudoClass.matchText == "target")
            {
                // Not supported.
                return true;
            }

            //E:lang(fr)	an element of type E in language "fr" (the document language specifies how language is determined)	The :lang() pseudo-class	2
            else if (pseudoClass.matchText.StartsWith("lang"))
            {
                /// not supported, always return true because we do not know when the language will be switched. has to assume it will match. 
                return true;
            }

            else
            {
                //Unrecognized syntax. 
                return false;
            }
        }



        private static bool _pseudoClassNthMatch(Element element, pseudoClassSelector pseudoClass, bool last, bool sametype)
        {
            int firstindex = pseudoClass.matchText.IndexOf("(");
            int lastindex = pseudoClass.matchText.IndexOf(")");

            if (firstindex < 0 || lastindex < 0)
            {
                return false;
            }

            string strnumber = pseudoClass.matchText.Substring(firstindex + 1, lastindex - firstindex - 1).Trim();

            int counter = _getSilbingCount(element, pseudoClass, last, sametype);

            if (CommonIdoms.isAsciiDigit(strnumber))
            {
                int nth = Convert.ToInt32(strnumber);
                return (nth == (counter + 1));
            }
            else
            {
                if (strnumber.Contains("n"))
                {
                    int beforen = 0;
                    int aftern = 0;
                    string strbefore = strnumber.Substring(0, strnumber.IndexOf("n")).Trim();
                    if (string.IsNullOrEmpty(strbefore))
                    { return false; }
                    if (!int.TryParse(strbefore, out beforen))
                    {
                        return false;
                    }

                    if (CommonIdoms.isAsciiDigit(strbefore))
                    {
                        beforen = Convert.ToInt32(strbefore);
                    }

                    if (strnumber.IndexOf("+") > 0)
                    {
                        string strafter = strnumber.Substring(strnumber.IndexOf("+") + 1).Trim();
                        if (!string.IsNullOrEmpty(strafter))
                        {
                            if (!int.TryParse(strafter, out aftern))
                            {
                                return false;
                            }
                        }
                    }

                    if (beforen == 0)
                    {
                        return (counter + 1) == aftern;
                    }
                    else
                    {
                        return ((counter + 1) % beforen == aftern);
                    }

                }
                else if (strnumber == "odd")
                {
                    return ((counter + 1) % 2 == 1);
                }
                else if (strnumber == "even")
                {
                    return ((counter + 1) % 2 == 0);
                }
                else
                {
                    return false;
                }
            }
        }

        private static int _getSilbingCount(Element element, pseudoClassSelector pseudoClass, bool last, bool sametype)
        {
            int counter = 0;
            Node siblingNode;
            if (last)
            {
                siblingNode = element.nextSibling();
            }
            else
            {
                siblingNode = element.previousSibling();
            }

            while (siblingNode != null)
            {
                if (siblingNode.nodeType == enumNodeType.ELEMENT)
                {
                    if (sametype)
                    {
                        if (siblingNode.nodeName == pseudoClass.elementE)
                        {
                            counter += 1;
                        }
                    }
                    else
                    {
                        counter += 1;
                    }
                }

                if (last)
                {
                    siblingNode = siblingNode.nextSibling();
                }
                else
                {
                    siblingNode = siblingNode.previousSibling();
                }
            }
            return counter;
        }

        private static bool matchPseudoElement(Element element, pseudoElementSelector pseudoElement)
        {

            if (!string.IsNullOrEmpty(pseudoElement.elementE))
            {

                if (!selectorMatch.Match(element, pseudoElement.ElementSelector))
                {
                    return false;
                }
            }

            //E::first-line	the first formatted line of an E element	The ::first-line pseudo-element	1
            //E::first-letter	the first formatted letter of an E element	The ::first-letter pseudo-element	1
            //E::before	generated content before an E element	The ::before pseudo-element	2
            //E::after	generated content after an E element

            //TODO: when applying style of this class to element, it may requires to alter the element text. 
            return true;
        }
    }
}
