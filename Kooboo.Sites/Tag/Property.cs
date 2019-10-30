//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System.Collections.Generic;

namespace Kooboo.Sites.Tag
{
    public static class Property
    {
        private static List<string> _layoutProperties;

        /// <summary>
        /// the list of the properties that must be the same the in order to be an layout.
        /// </summary>
        /// <returns></returns>
        public static List<string> LayoutProperties
        {
            get
            {
                if (_layoutProperties == null)
                {
                    _layoutProperties = new List<string>
                    {
                        "width",
                        "position",
                        "left",
                        "right",
                        "buttom",
                        "top",
                        "max-height",
                        "min-height",
                        "max-width",
                        "min-width"
                    };


                    //  'border-spacing'
                    //   border-top-width' 'border-right-width' 'border-bottom-width' 'border-left-width'	<border-width> | inherit	medium	 no	 	visual
                    //'border-width'

                    //   'margin-right' 'margin-left'	<margin-width> | inherit	0	all elements except elements with table display types other than table-caption, table and inline-table	no	refer to width of containing block	visual
                    //'margin-top' 'margin-bottom'
                }

                return _layoutProperties;
            }
        }

        private static List<string> _inheritedProperties;

        public static List<string> InheritedProperties()
        {
            return _inheritedProperties ?? (_inheritedProperties = new List<string>());
        }

        private static List<string> _uriProperty;

        /// <summary>
        ///  Properties that might assigned a Uri value.
        /// </summary>
        private static List<string> UriProperty
        {
            get
            {
                if (_uriProperty == null)
                {
                    _uriProperty = new List<string>
                    {
                        "background-image",
                        "background",
                        "content",
                        "cue-before",
                        "cue",
                        "cue-after",
                        "src",
                        "cursor",
                        "play-during"
                    };
                }
                return _uriProperty;
            }
        }

        //properties that can define uri.
        public static bool CanHaveUri(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return false;
            }
            string name = propertyName.ToLower();

            return (name == "background-image"
                || name == "background"
                || name == "content"
                || name == "cue-before"
                || name == "cue"
                || name == "cue-after"
                || name == "src"
                || name == "cursor"
                || name == "play-during");
        }

        public static bool CanHaveColor(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return false;
            }
            string name = propertyName.ToLower();

            return (name == "background"
                || name == "background-color"
                || name == "border"
                || name == "border-bottom"
                || name == "border-bottom-color"
                || name == "border-color"
                || name == "border-left"
                || name == "border-left-color"
                || name == "border-right"
                || name == "border-right-color"
                || name == "border-top"
                || name == "border-top-color"
                || name == "outline"
                || name == "outline-color"
                || name == "box-shadow"
                || name == "column-rule"
                || name == "column-rule–color"
                || name == "color"
                || name == "text-shadow"
                || name == "text-emphasis"
                || name == "text-outline"
                || name == "scrollbar-3dlight-color"
                || name == "scrollbar-darkshadow-color"
                || name == "scrollbar-hightlight-color"
                || name == "scrollbar-shadow-color"
                || name == "scrollbar-arrow-color"
                || name == "scrollbar-face-color"
                || name == "scrollbar-track-color"
                || name == "scrollbar-base-color");
        }
    }
}