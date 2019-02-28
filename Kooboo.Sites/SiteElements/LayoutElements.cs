//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.SiteElements
{
    public static class LayoutElements
    {

        /// <summary>
        /// find the common part as placeholders. 
        /// </summary>
        /// <param name="allXelements"></param>
        /// <param name="allYelements"></param>
        /// <returns></returns>
        public static List<DomElement> FindPlaceHolders(List<DomElement> allXelements, List<DomElement> allYelements)
        {
            List<DomElement> placeholders = new List<DomElement>();

            var xroot = allXelements.Find(o => o.Name == "body");
            var yroot = allYelements.Find(o => o.Name == "body");

            if (xroot != null && yroot != null)
            {
                _FindPlaceHolders(xroot, yroot, allXelements, allYelements, ref placeholders);
            }
            return placeholders;
        }

        /// <summary>
        ///  Find the commone parent of x and y. 
        /// </summary>
        /// <param name="xElement"></param>
        /// <param name="yElement"></param>
        /// <param name="allXelement"></param>
        /// <param name="allYelement"></param>
        /// <returns></returns>
        public static DomElement FindParent(DomElement xElement, DomElement yElement, List<DomElement> allXelement, List<DomElement> allYelement)
        {
            List<DomElement> xParents = new List<DomElement>();
            var xparent = allXelement.Find(o => o.Id == xElement.ParentId);
            while (xparent != null && xparent.Name != "body")
            {
                xParents.Add(xparent);

                xparent = allXelement.Find(o => o.ParentId == xparent.ParentId);
            }

            List<DomElement> yParents = new List<DomElement>();

            var yparent = allYelement.Find(o => o.ParentId == yElement.ParentId);

            while (yparent != null && yparent.Name != "body")
            {
                yParents.Add(yparent);
                yparent = allYelement.Find(o => o.ParentId == yparent.ParentId);
            }

            xParents.Reverse();
            yParents.Reverse();

            int xcount = xParents.Count;
            int ycount = yParents.Count;

            int count = xcount;
            if (count > ycount)
            {
                count = ycount;
            }
            for (int i = 0; i < count; i++)
            {
                
            }

            //TODO: implement to find the same parents.
            return null; 
        }

        /// <summary>
        /// find the right elements
        /// </summary>
        /// <param name="xElement"></param>
        /// <param name="yElement"></param>
        /// <param name="allXelements"></param>
        /// <param name="allYelements"></param>
        /// <param name="placeholders"></param>
        /// <returns>Ture= current elements can be used as layout. </returns>
        private static bool _FindPlaceHolders(DomElement xElement, DomElement yElement, List<DomElement> allXelements, List<DomElement> allYelements, ref List<DomElement> placeholders)
        {

            if (xElement.Name == yElement.Name && xElement.NodeAttributeHash == yElement.NodeAttributeHash)
            {

                if (xElement.SubElementHash == yElement.SubElementHash)
                {
                    // if all subs are the same, go to sub to check.
                    var xsubs = allXelements.Where(o => o.ParentId == xElement.Id).OrderBy(o => o.Sibling).ToList();
                    var ysubs = allYelements.Where(o => o.ParentId == yElement.Id).OrderBy(o => o.Sibling).ToList();

                    int count = xsubs.Count;
                    int ycount = ysubs.Count;

                    if (count != ycount)
                    {
                        return false;
                    }

                    bool hasplaceher = false;

                    for (int i = 0; i < count; i++)
                    {
                        bool hasplaceholder = _FindPlaceHolders(xsubs[i], ysubs[i], allXelements, allYelements, ref placeholders);
                        if (hasplaceholder)
                        {
                            hasplaceher = true;
                        }
                    }

                    return hasplaceher;
                }
                else
                {
                    // try to see if this can be a layout... 
                    // conditions to be a layout....
                    // there is no need to test sibling, because it is top to down, it will be removed already if it is not the same siblings..
                    // try to use edit path here.... if it is added something....

                    placeholders.Add(xElement);
                    return true;

                }



            }

            return false;
        }
         
    }
}
