//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;


namespace Kooboo.Dom.CSS
{
    /// <summary>
    /// See: http://www.w3.org/TR/css3-selectors/
    /// </summary>
    public static class Deinitions
    {

        private static List<char> _whitespace;
        public static List<char> WhiteSpace()
        {
            /// Only the characters "space" (U+0020), "tab" (U+0009), "line feed" (U+000A), "carriage return" (U+000D), and "form feed" (U+000C) can occur in whitespace.

            if (_whitespace == null)
            {
                _whitespace = new List<char>();
                _whitespace.Add('\u0020');
                _whitespace.Add('\u0009');
                _whitespace.Add('\u000A');
                _whitespace.Add('\u000D');
                _whitespace.Add('\u000C');

            }
            return _whitespace;


        }

        private static List<char> _combinator;

        /// <summary>
        /// Combinators are: whitespace, "greater-than sign" (U+003E, >), "plus sign" (U+002B, +) and "tilde" (U+007E, ~).
        /// </summary>
        /// <returns></returns>
        public static List<char> Combinator()
        {
            if (_combinator == null)
            {
                _combinator = new List<char>();

                List<char> whitespacelist = WhiteSpace();
                _combinator.AddRange(whitespacelist);

                _combinator.Add('\u003E');
                _combinator.Add('\u002B');
                _combinator.Add('\u007E');
            }

            return _combinator;

        }


    }


    public enum enumSimpleSelectorType
    {
        universal = 8,
        type = 1,
        attribute = 2,
        classSelector = 3,
        id = 4,
        pseudoclass = 5,
        combinator = 6,
        pseudoElement = 7,
        negativePseudoClass = 8,
        unknown = 0
    }

}



//Pattern	Meaning	Described in section	First defined in CSS level
//*	any element	Universal selector	2
//E	an element of type E	Type selector	1
//E[foo]	an E element with a "foo" attribute	Attribute selectors	2
//E[foo="bar"]	an E element whose "foo" attribute value is exactly equal to "bar"	Attribute selectors	2
//E[foo~="bar"]	an E element whose "foo" attribute value is a list of whitespace-separated values, one of which is exactly equal to "bar"	Attribute selectors	2
//E[foo^="bar"]	an E element whose "foo" attribute value begins exactly with the string "bar"	Attribute selectors	3
//E[foo$="bar"]	an E element whose "foo" attribute value ends exactly with the string "bar"	Attribute selectors	3
//E[foo*="bar"]	an E element whose "foo" attribute value contains the substring "bar"	Attribute selectors	3
//E[foo|="en"]	an E element whose "foo" attribute has a hyphen-separated list of values beginning (from the left) with "en"	Attribute selectors	2
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
//E:checked	a user interface element E which is checked (for instance a radio-button or checkbox)	The UI element states pseudo-classes	3
//E::first-line	the first formatted line of an E element	The ::first-line pseudo-element	1
//E::first-letter	the first formatted letter of an E element	The ::first-letter pseudo-element	1
//E::before	generated content before an E element	The ::before pseudo-element	2
//E::after	generated content after an E element	The ::after pseudo-element	2
//E.warning	an E element whose class is "warning" (the document language specifies how class is determined).	Class selectors	1
//E#myid	an E element with ID equal to "myid".	ID selectors	1
//E:not(s)	an E element that does not match simple selector s	Negation pseudo-class	3
//E F	an F element descendant of an E element	Descendant combinator	1
//E > F	an F element child of an E element	Child combinator	2
//E + F	an F element immediately preceded by an E element	Adjacent sibling combinator	2
//E ~ F	an F element preceded by an E element	General sibling combinator	3