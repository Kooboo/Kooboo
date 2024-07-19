//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Dom.CSS
{
    public class combinatorClause
    {

        public combinatorClause()
        {
            combineType = combinator.unknown;
        }

        public combinator combineType;

        public simpleSelector selector;
        public string selectorText;

    }


    /// <summary>
    ///E F	an F element descendant of an E element	Descendant combinator	1
    ///E > F	an F element child of an E element	Child combinator	2
    ///E + F	an F element immediately preceded by an E element	Adjacent sibling combinator	2
    ///E ~ F	an F element preceded by an E element
    /// </summary>
    public enum combinator
    {
        unknown = 0,
        Descendant = 1,
        Child = 2,
        AdjacentSibling = 3,
        Sibling = 4
    }

}
