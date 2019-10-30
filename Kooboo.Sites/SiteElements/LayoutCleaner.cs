//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Sites.Models;
using System.Collections.Generic;

namespace Kooboo.Sites.SiteElements
{
    public class LayoutCleaner
    {
        public static bool CheckIsAllowedForLayout(DomElement element, List<DomElement> pageElements)
        {
            return VerifyTD(element, pageElements) && VerifyContainer(element);
        }

        private static bool VerifyContainer(DomElement element)
        {
            string[] parents = element.ParentPath.Split('/');
            foreach (var item in parents)
            {
                if (Kooboo.Sites.Tag.TagGroup.isText(item))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// verify whether this TD can be used as layout or not.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="pageElements"></param>
        /// <returns></returns>
        public static bool VerifyTD(DomElement element, List<DomElement> pageElements)
        {
            //if (element.Name == "td")
            //{
            //    bool onlytext = true;
            //    foreach (var item in element.SubElements)
            //    {
            //        if (!Kooboo.Sites.Tag.TagGroup.isText(item.Key))
            //        {
            //            onlytext = false;
            //            break;
            //        }
            //    }

            //    if (onlytext)
            //    {
            //        ///if only text and has many similar this kind of elements...

            //        int count = pageElements.FindAll(o => o.ParentPathHash == element.ParentPathHash && o.Name == element.Name && o.NodeAttributeHash == element.NodeAttributeHash).Count;

            //        if (count > 2)
            //        {
            //            return false;
            //        }
            //    }

            //}

            return true;
        }

        //public static List<PageElement> MergeLayout(List<PageElement> currentPlaceHolders, List<PageElement> AllPageElements)
        //{
        //    // merge the page elements that has the same parents.

        //}
    }
}