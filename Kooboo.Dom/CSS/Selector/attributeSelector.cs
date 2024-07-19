//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Dom.CSS
{


    // E[foo] an E element with a "foo" attribute	Attribute selectors	2
    //E[foo="bar"]	an E element whose "foo" attribute value is exactly equal to "bar"	Attribute selectors	2
    //E[foo~="bar"]	an E element whose "foo" attribute value is a list of whitespace-separated values, one of which is exactly equal to "bar"	Attribute selectors	2
    //E[foo^="bar"]	an E element whose "foo" attribute value begins exactly with the string "bar"	Attribute selectors	3
    //E[foo$="bar"]	an E element whose "foo" attribute value ends exactly with the string "bar"	Attribute selectors	3
    //E[foo*="bar"]	an E element whose "foo" attribute value contains the substring "bar"	Attribute selectors	3
    //E[foo|="en"]	an E element whose "foo" attribute has a hyphen-separated list of values beginning (from the left) with "en"	Attribute selectors



    public class attributeSelector : simpleSelector
    {
        public attributeSelector()
        {
            base.Type = enumSimpleSelectorType.attribute;
        }

        public string elementE;

        public string attributeName;

        public string attributeValue;

        public enumAttributeType matchType;

    }


    public enum enumAttributeType
    {
        defaultHas = 0,
        exactlyEqual = 1,
        whitespaceSeperated = 2,
        exactlyBegin = 3,
        exactlyEnd = 4,
        contains = 5,
        hyphenSeperated = 6
    }

}
