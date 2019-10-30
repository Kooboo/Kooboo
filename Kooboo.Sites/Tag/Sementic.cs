//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System.Collections.Generic;

namespace Kooboo.Sites.Tag
{
    public class Sementic
    {
        private static List<string> _classNames;

        /// <summary>
        /// the lowercase name list of common sementic id/class names.
        /// </summary>
        public static List<string> ClassNames
        {
            get
            {
                if (_classNames == null)
                {
                    _classNames = new List<string>
                    {
                        "sidebar",
                        "side",
                        "left",
                        "right",
                        "content",
                        "wrapper",
                        "container",
                        "header",
                        "head",
                        "footer",
                        "main",
                        "site",
                        "menu",
                        "body",
                        "section",
                        "top",
                        "nav",
                        "bar",
                        "col"
                    };
                    // col based layout css like bootstrap.
                    //global-width
                }

                return _classNames;
            }
        }
    }
}