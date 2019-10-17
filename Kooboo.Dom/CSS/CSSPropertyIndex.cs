//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System.Collections.Generic;

namespace Kooboo.Dom.CSS
{
    public static class CSSPropertyIndex
    {
        private static List<string> _inheritedpropertylist;

        public static List<string> InheritedPeroptyList()
        {
            return _inheritedpropertylist ?? (_inheritedpropertylist = new List<string>
            {
                "border-collapse",
                "border-spacing",
                "caption-side",
                "color",
                "cursor",
                "direction",
                "empty-cells",
                "font-family",
                "font-size",
                "font-style",
                "font-variant",
                "font-weight",
                "font",
                "letter-spacing",
                "line-height",
                "list-style-image",
                "list-style-position",
                "list-style-type",
                "list-style",
                "orphans",
                "quotes",
                "text-align",
                "text-indent",
                "text-transform",
                "visibility",
                "white-space",
                "widows",
                "word-spacing"
            });
        }
    }

    public struct CSSProperty
    {
        public string Name;

        public string[] Values;

        public bool Inherited;

        public string DefaultValue;

        public string[] AppliedTo;
    }
}