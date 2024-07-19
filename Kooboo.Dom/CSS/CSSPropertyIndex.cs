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
            if (_inheritedpropertylist == null)
            {
                _inheritedpropertylist = new List<string>();
                _inheritedpropertylist.Add("border-collapse");
                _inheritedpropertylist.Add("border-spacing");
                _inheritedpropertylist.Add("caption-side");
                _inheritedpropertylist.Add("color");
                _inheritedpropertylist.Add("cursor");
                _inheritedpropertylist.Add("direction");
                _inheritedpropertylist.Add("empty-cells");
                _inheritedpropertylist.Add("font-family");
                _inheritedpropertylist.Add("font-size");
                _inheritedpropertylist.Add("font-style");
                _inheritedpropertylist.Add("font-variant");
                _inheritedpropertylist.Add("font-weight");
                _inheritedpropertylist.Add("font");
                _inheritedpropertylist.Add("letter-spacing");
                _inheritedpropertylist.Add("line-height");
                _inheritedpropertylist.Add("list-style-image");
                _inheritedpropertylist.Add("list-style-position");
                _inheritedpropertylist.Add("list-style-type");
                _inheritedpropertylist.Add("list-style");
                _inheritedpropertylist.Add("orphans");
                _inheritedpropertylist.Add("quotes");
                _inheritedpropertylist.Add("text-align");
                _inheritedpropertylist.Add("text-indent");
                _inheritedpropertylist.Add("text-transform");
                _inheritedpropertylist.Add("visibility");
                _inheritedpropertylist.Add("white-space");
                _inheritedpropertylist.Add("widows");
                _inheritedpropertylist.Add("word-spacing");
            }

            return _inheritedpropertylist;

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

