//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Dom.CSS
{

    /// <summary>
    /// E::first-line	the first formatted line of an E element	The ::first-line pseudo-element	1
    //E::first-letter	the first formatted letter of an E element	The ::first-letter pseudo-element	1
    //E::before	generated content before an E element	The ::before pseudo-element	2
    //E::after	generated content after an E element	The ::after pseudo-element
    /// </summary>
    public class pseudoElementSelector : simpleSelector
    {

        public pseudoElementSelector()
        {
            base.Type = enumSimpleSelectorType.pseudoElement;
        }

        public string elementE { get; set; }

        public string matchElement { get; set; }

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

    }
}
