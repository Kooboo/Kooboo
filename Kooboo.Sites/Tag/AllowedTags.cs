//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System.Collections.Generic;

namespace Kooboo.Sites.Tag
{
    /// <summary>
    /// The tags that are allowed for each type of analysis.
    /// </summary>
    public static class AllowedTags
    {
        private static List<string> _layoutTag;

        public static List<string> layoutTag
        {
            get
            {
                if (_layoutTag == null)
                {
                    _layoutTag = new List<string>
                    {
                        "section",
                        "nav",
                        "article",
                        "aside",
                        "header",
                        "footer",
                        "div",
                        "td"
                    };

                }
                return _layoutTag;
            }
        }

        /// <summary>
        /// the tags that are allowed to be used as layout tag.
        /// </summary>
        /// <param name="tagName"></param>
        /// <returns></returns>
        public static bool IsLayoutTag(string tagName)
        {
            return layoutTag.Contains(tagName);
        }
    }
}