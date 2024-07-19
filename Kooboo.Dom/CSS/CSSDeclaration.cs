//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;

namespace Kooboo.Dom.CSS
{

    //http://dev.w3.org/csswg/cssom/#css-declaration

    /// <summary>
    ///  A CSS declaration is an abstract concept that is not exposed as an object in the DOM. A CSS declaration has the following associated properties:
    /// </summary>
    [Serializable]
    public class CSSDeclaration
    {
        private string _propertyname;

        public string propertyname
        {
            get
            {
                return _propertyname;
            }
            set
            {
                _propertyname = value;
                if (!string.IsNullOrEmpty(_propertyname))
                {
                    _propertyname = _propertyname.Trim();
                }
            }
        }


        public string value
        {
            get; set;
        }

        public bool important { get; set; }

        /// <summary>
        /// //case-sensitive flag
        //Set if the property name is defined to be case-sensitive according to its specification, otherwise unset.
        /// </summary>
        public bool caseSensitive { get; set; }

    }
}
