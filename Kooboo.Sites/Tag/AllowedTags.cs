//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Tag
{
    /// <summary>
    /// The tags that are allowed for each type of analysis. 
    /// </summary>
  public static  class AllowedTags
    {

        private static List<string> _layoutTag;
        public static List<string> layoutTag
        {
            get
            {
                if (_layoutTag == null)
                {
                    _layoutTag = new List<string>();

                    _layoutTag.Add("section");
                    _layoutTag.Add("nav");
                    _layoutTag.Add("article");
                    _layoutTag.Add("aside");
                    _layoutTag.Add("header");
                    _layoutTag.Add("footer");
                    _layoutTag.Add("div");
                    _layoutTag.Add("td");
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
