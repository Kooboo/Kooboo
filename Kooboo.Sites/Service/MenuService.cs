//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Models;
using Kooboo.Dom;
using Kooboo.Sites.Helper;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using Kooboo.Sites.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Sites.Service
{
    public class MenuService
    {
        public static RawMenu FindRawMenu(Element element)
        {
            var alllinks = element.getElementsByTagName("a").item;
            if (alllinks != null && alllinks.Any())
            {
                var groupby = SimpleGroupBy(alllinks);

                RawMenu menu = new RawMenu();
                AssignSubMenu(groupby[0], menu);
                menu.SubItemContainer = element;
                return menu;
            }
            return null;
        }

        internal static List<List<Element>> SimpleGroupBy(List<Element> linkElements)
        {
            Func<List<Element>, int> getFirstLocation = (elements) =>
            {
                int result = int.MaxValue;
                foreach (var item in elements)
                {
                    if (item.location.openTokenStartIndex < result)
                    {
                        result = item.location.openTokenStartIndex;
                    }
                }
                return result;
            };

            var groupby = linkElements.GroupBy(o => DomService.getParentPath(o));

            List<List<Element>> sortedlist = new List<List<Element>>();

            foreach (var item in groupby.OrderBy(o => getFirstLocation(o.ToList())))
            {
                sortedlist.Add(item.ToList());
            }
            return sortedlist;
        }

        public static void AssignSubMenu(List<Element> links, RawMenu parentMenu)
        {
            var commonparent = DomService.FindParent(links);

            foreach (var item in links)
            {
                var menu = new RawMenu
                {
                    Parent = parentMenu,
                    LinkElement = item,
                    ContainerElement = FindLinkContainer(item, links, commonparent)
                };
                menu.Parent = parentMenu;
                parentMenu.Children.Add(menu);
            }

            foreach (var item in parentMenu.Children)
            {
                var sublinks = FindSubLinks(item);
                if (sublinks != null && sublinks.Any())
                {
                    DomService.FindParent(sublinks);
                    AssignSubMenu(sublinks, item);
                }
            }
        }

        public static List<Element> FindSubLinks(RawMenu menu)
        {
            var sublinks = GetDirectSubLinks(menu.LinkElement, menu.ContainerElement);
            if (sublinks != null && sublinks.Any())
            {
                return sublinks;
            }

            return GetSiblingSubLinks(menu.ContainerElement, menu.NextContainer());
        }

        internal static List<Element> GetDirectSubLinks(Element linkElement, Element container)
        {
            var alllinks = container.getElementsByTagName("a").item;

            alllinks.Remove(linkElement);

            if (alllinks.Any())
            {
                var groupby = GroupBy(alllinks);
                return groupby[0];
            }
            return null;
        }

        internal static List<Element> GetSiblingSubLinks(Element currentContainer, Element nextContainer)
        {
            List<Node> betweenNodes = new List<Node>();

            if (nextContainer != null)
            {
                var parent = DomService.FindParent(currentContainer, nextContainer);

                var current = currentContainer;
                while (current.depth > parent.depth + 1)
                {
                    current = current.parentElement;
                }

                var next = nextContainer;
                while (next.depth > parent.depth + 1)
                {
                    next = next.parentElement;
                }
                var nodesInBetween = parent.childNodes.item.Where(o => o.siblingIndex > current.siblingIndex && o.siblingIndex < next.siblingIndex).ToList();

                var alllinks = DomService.GetElementsByTagName(nodesInBetween, "a").item;
                if (alllinks.Any())
                {
                    var group = GroupBy(alllinks);
                    return group[0];
                }
            }
            else
            {
                var parent = currentContainer.parentElement;

                var nodesInBetween = parent.childNodes.item.Where(o => o.siblingIndex > currentContainer.siblingIndex).ToList();

                var alllinks = DomService.GetElementsByTagName(nodesInBetween, "a").item;
                if (alllinks.Any())
                {
                    var group = GroupBy(alllinks);
                    return group[0];
                }
            }

            return null;
        }

        public static void AddNewMenu(SiteDb siteDb, Page page, MenuViewModel newMenuViewModel)
        {
            // menu view can start only with item container.
            Menu menu = CreateMenu(page.Dom, newMenuViewModel);

            if (string.IsNullOrEmpty(menu.Name))
            {
                menu.Name = siteDb.Menus.GetNewMenuName();
            }

            siteDb.Menus.AddOrUpdate(menu);

            var maincontainer = newMenuViewModel.ItemContainer;

            var element = DomService.GetElementByKoobooId(page.Dom, maincontainer);

            string newbody = page.Body.Substring(0, element.location.openTokenStartIndex);
            newbody += "<menu id='" + menu.Name + "'></menu>";
            newbody += page.Body.Substring(element.location.endTokenEndIndex + 1);

            page.Body = newbody;
            siteDb.Pages.AddOrUpdate(page);
        }

        public static Element FindLinkContainer(Element link, List<Element> linkGroup, Element commonParent)
        {
            var target = link;

            var directParent = target;

            while (directParent != null && !directParent.isEqualNode(commonParent))
            {
                foreach (var item in linkGroup)
                {
                    if (!item.isEqualNode(link))
                    {
                        if (DomService.ContainsOrEqualElement(directParent, item))
                        {
                            break;
                        }
                    }
                }

                target = directParent;
                directParent = target.parentElement;
            }

            return target;
        }

        public static Menu CreateMenu(Document doc, MenuViewModel menuViewModel)
        {
            Menu menu = new Menu();
            Element linkElement = null;
            Element itemContainer = null;
            Element containerElement = null;

            int menuTemplateStart = int.MaxValue;
            int menuTemplaetEnd = 0;

            if (!string.IsNullOrEmpty(menuViewModel.LinkElement) && !string.IsNullOrEmpty(menuViewModel.ContainerElement))
            {
                var linkelement = DomService.GetElementByKoobooId(doc, menuViewModel.LinkElement);
                if (linkelement != null)
                {
                    linkElement = linkelement as Element;
                }

                var container = DomService.GetElementByKoobooId(doc, menuViewModel.ContainerElement);
                if (container == null && linkelement != null)
                {
                    container = linkelement;
                }
                if (container != null)
                {
                    containerElement = container as Element;
                    if (containerElement != null)
                    {
                        menuTemplateStart = containerElement.location.openTokenStartIndex;
                        menuTemplaetEnd = containerElement.location.endTokenEndIndex;
                    }
                }
            }

            string subMenuItemsTemplate = null;
            string subMenuItemOrginalString = null;
            int subMenuStart = int.MaxValue;
            int subMenuEnd = 0;

            if (menuViewModel.children.Count > 0)
            {
                List<Menu> subMenus = new List<Menu>();

                foreach (var item in menuViewModel.children)
                {
                    Menu submenu = CreateMenu(doc, item);
                    if (submenu != null)
                    {
                        subMenus.Add(submenu);
                        if (submenu.tempdata.StartIndex < subMenuStart)
                        {
                            subMenuStart = submenu.tempdata.StartIndex;
                        }
                        if (submenu.tempdata.EndIndex > subMenuEnd)
                        {
                            subMenuEnd = submenu.tempdata.EndIndex;
                        }
                    }
                }

                itemContainer = _GetItemContainer(doc, menuViewModel);

                string subMenuString = doc.HtmlSource.Substring(subMenuStart, subMenuEnd - subMenuStart + 1);

                if (itemContainer != null)
                {
                    // 1, sub item within the link element...
                    if (containerElement == null || DomService.ContainsOrEqualElement(containerElement, itemContainer))
                    {
                        subMenuItemOrginalString = itemContainer.OuterHtml;
                        subMenuItemsTemplate = subMenuItemOrginalString.Replace(subMenuString, MenuHelper.MarkSubItems);
                    }
                    else if (containerElement.isEqualNode(itemContainer))
                    {
                        subMenuItemOrginalString = subMenuString;
                        subMenuItemsTemplate = MenuHelper.MarkSubItems;
                    }
                    else
                    {
                        var distance = DomService.GetTreeDistance(containerElement, itemContainer);
                        bool sibling = false;
                        if (distance == 1)
                        {
                            sibling = true;
                        }
                        else if (distance < 5)
                        {
                            var nodes = DomService.GetNodesInBetween(doc, containerElement, itemContainer);
                            if (nodes == null || nodes.Count == 0 || IsPossibleSeperator(nodes))
                            {
                                sibling = true;
                            }
                        }

                        if (sibling)
                        {
                            if (menuTemplateStart > itemContainer.location.openTokenStartIndex)
                            {
                                menuTemplateStart = itemContainer.location.openTokenStartIndex;
                            }

                            if (menuTemplaetEnd < itemContainer.location.endTokenEndIndex)
                            {
                                menuTemplaetEnd = itemContainer.location.endTokenEndIndex;
                            }

                            subMenuItemOrginalString = itemContainer.OuterHtml;
                            subMenuItemsTemplate = subMenuItemOrginalString.Replace(subMenuString, MenuHelper.MarkSubItems);
                        }
                        else
                        {
                            //menu.RenderSubMenuSeperated = true;
                            subMenuItemOrginalString = itemContainer.OuterHtml;
                            subMenuItemsTemplate = itemContainer.OuterHtml.Replace(subMenuString, MenuHelper.MarkSubItems);
                        }
                    }
                }

                if (subMenus.Count > 0)
                {
                    menu.children.AddRange(subMenus);
                }
            }

            if (menuTemplateStart > 0 && menuTemplaetEnd > 0)
            {
                string menutemplate = doc.HtmlSource.Substring(menuTemplateStart, menuTemplaetEnd - menuTemplateStart + 1);

                if (!string.IsNullOrEmpty(subMenuItemOrginalString))
                {
                    menutemplate = menutemplate.Replace(subMenuItemOrginalString, MenuHelper.MarkSubItems);
                }

                string originalLink = linkElement?.OuterHtml;
                string newLInk = DomService.ReplaceLink(linkElement, MenuHelper.MarkHref, MenuHelper.MarkAnchorText);
                menutemplate = menutemplate.Replace(originalLink, newLInk);
                menu.Template = menutemplate;
                menu.Name = menuViewModel.text;
                menu.Url = menuViewModel.href;
            }

            menu.SubItemContainer = subMenuItemsTemplate;

            menu.tempdata.StartIndex = menuTemplateStart;
            menu.tempdata.EndIndex = menuTemplaetEnd;

            if (menu.tempdata.StartIndex == 0 || menu.tempdata.EndIndex == 0)
            {
                if (itemContainer != null)
                {
                    menu.tempdata.StartIndex = itemContainer.location.openTokenStartIndex;
                    menu.tempdata.EndIndex = itemContainer.location.endTokenEndIndex;
                }
            }
            return menu;
        }

        private static Element _GetItemContainer(Document doc, MenuViewModel menuViewModel)
        {
            // find the item container.
            if (!string.IsNullOrEmpty(menuViewModel.ItemContainer))
            {
                if (DomService.GetElementByKoobooId(doc, menuViewModel.ItemContainer) is Element itemcontainer)
                {
                    return itemcontainer;
                }
            }

            List<Element> containerlist = new List<Element>();
            foreach (var item in menuViewModel.children)
            {
                if (!string.IsNullOrEmpty(item.ContainerElement))
                {
                    if (DomService.GetElementByKoobooId(doc, item.ContainerElement) is Element element)
                    {
                        containerlist.Add(element);
                    }
                }
            }

            if (containerlist.Count == 0)
            {
                foreach (var item in menuViewModel.children)
                {
                    if (DomService.GetElementByKoobooId(doc, item.LinkElement) is Element element)
                    {
                        containerlist.Add(element);
                    }
                }
            }
            if (containerlist.Count > 1)
            {
                var element = DomService.FindParent(containerlist);
                if (element != null)
                {
                    menuViewModel.ItemContainer = DomService.GetKoobooId(element);
                }
                return element;
            }
            return null;
        }

        /// <summary>
        /// Group by the links into different menu group.
        /// </summary>
        /// <param name="linkelements"></param>
        /// <param name="reduceGroup">Reduce the sub group within another group.... like sub menus within the main menu will be reduced.</param>
        /// <returns></returns>
        public static List<List<Element>> GroupBy(List<Element> linkelements, bool reduceGroup = true)
        {
            List<List<Element>> result = new List<List<Element>>();

            int count = linkelements.Count;

            switch (count)
            {
                case 0:
                    return result;
                case 1:
                    result.Add(linkelements);
                    return result;
            }

            List<Element> sorted = linkelements.OrderBy(o => o.depth).ThenBy(o => o.location.openTokenStartIndex).ToList();

            Element previous = null;
            Element current = null;
            bool addnewgroup = false;

            for (int i = 0; i < count; i++)
            {
                addnewgroup = false;
                //The first one...
                if (i == 0)
                {
                    addnewgroup = true;
                }
                else
                {
                    previous = sorted[i - 1];
                    current = sorted[i];

                    if (previous.depth != current.depth)
                    {
                        addnewgroup = true;
                    }
                    else
                    {
                        if (!IsElementNextToEachOther(current, previous))
                        {
                            addnewgroup = true;
                        }
                        else
                        {
                            // if there is already a group and, this new member belongs to the same parent, then it belongs to the same group.
                            if (result.LastOrDefault().Count > 1)
                            {
                                var currentlist = result.LastOrDefault();
                                if (currentlist != null)
                                {
                                    var itemone = currentlist[currentlist.Count - 2];
                                    var itemtwo = currentlist[currentlist.Count - 1];
                                    var parent = Kooboo.Sites.Service.DomService.FindParent(itemone, itemtwo);

                                    // if there is longer distance....
                                    if (Kooboo.Sites.Service.DomService.GetTreeDistance(current, itemtwo) > Kooboo.Sites.Service.DomService.GetTreeDistance(itemtwo, itemone))
                                    {
                                        addnewgroup = true;
                                    }
                                    else
                                    {
                                        var currentparent = Kooboo.Sites.Service.DomService.FindParent(itemone, current);
                                        if (!parent.isEqualNode(currentparent))
                                        {
                                            addnewgroup = true;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                // count = 1;
                                var lastelement = result.LastOrDefault()?[0];
                                var parent = Kooboo.Sites.Service.DomService.FindParent(current, lastelement);

                                if (parent.tagName != "ul" || parent.tagName != "nav")
                                {
                                    if (parent.tagName == "body")
                                    {
                                        addnewgroup = true;
                                    }

                                    //TODO: to be improve more rules......
                                }
                            }
                        }
                    }
                }

                if (addnewgroup)
                {
                    List<Element> firstlist = new List<Element> {sorted[i]};
                    result.Add(firstlist);
                }
                else
                {
                    if (result.Count > 0)
                    {
                        result.LastOrDefault()?.Add(current);
                    }
                }
            }
            if (reduceGroup)
            {
                ReduceGroupBy(result);
            }
            return result;
        }

        private static string ConvertToTemplate(Element linkElement, string href, string anchortext)
        {
            string all = linkElement.OuterHtml;
            all = all.Replace(anchortext, "{anchortext}");
            all = all.Replace(href, "{href}");
            return all;
        }

        /// <summary>
        /// Test that this two elements are next to each other, this is for menu detection.
        /// for the menu with ul/li, this seems like enough.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool IsElementNextToEachOther(Element x, Element y)
        {
            if (x.depth != y.depth)
            {
                return false;
            }

            if (x.tagName != y.tagName)
            {
                return false;
            }

            // first check their physical location is very closer to each other.
            if (x.location.openTokenStartIndex > y.location.openTokenStartIndex)
            {
                Element t = x;
                x = y;
                y = t;
            }

            int betweencount = y.location.openTokenStartIndex - x.location.endTokenEndIndex;

            if (betweencount < 10)
            {
                return true;
            }
            else
            {
                string betweentext = x.ownerDocument.HtmlSource.Substring(x.location.endTokenEndIndex + 1, betweencount);

                betweentext = betweentext.Replace(" ", "");
                betweentext = betweentext.Replace(Environment.NewLine, "");

                if (betweentext.Length < 10)
                {
                    return true;
                }
            }

            // check parents.
            if (x.parentElement.tagName == "li" && y.parentElement.tagName == "li")
            {
                return IsElementNextToEachOther(x.parentElement, y.parentElement);
            }

            int treedistance = Kooboo.Sites.Service.DomService.GetTreeDistance(x, y);

            //TODO: Test that they have the same parent path and distance % 10 ==0.
            if (treedistance < 10 || treedistance % 10 == 0)
            {
                if (Kooboo.Sites.Service.DomService.getParentPath(x) == Kooboo.Sites.Service.DomService.getParentPath(y))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Convert one link item with its parent element into one menu item...
        /// example: Li, a, website..... Convert the a under li tag into one mentu....
        /// </summary>
        /// <param name="parentElement"></param>
        /// <param name="linkElement"></param>
        /// <param name="website"></param>
        /// <returns></returns>
        public static Menu ConvertToMenu(Element parentElement, Element linkElement, WebSite website)
        {
            Menu menu = new Menu();

            string template = parentElement.OuterHtml;

            string anchortext = linkElement.InnerHtml;
            string href = linkElement.getAttribute("href");
            template = template.Replace(linkElement.OuterHtml, ConvertToTemplate(linkElement, href, anchortext));

            menu.Template = template;
            menu.Name = anchortext;
            menu.Url = href;

            // Check if there is a submenu....
            var alllinks = parentElement.getElementsByTagName("a").item;

            if (alllinks.Count > 1)
            {
                // there is sub menu...
                List<Element> sublinks = new List<Element>();

                string mainhref = linkElement.getAttribute("href");

                foreach (var item in alllinks)
                {
                    string subhref = item.getAttribute("href");

                    if (subhref != mainhref)
                    {
                        sublinks.Add(item);
                    }
                }

                if (sublinks.Any())
                {
                    int subGroupStartIndex;
                    int subGroupEndIndex;

                    var groupby = GroupBy(sublinks);

                    if (groupby.Count == 1)
                    {
                        var submenulinks = groupby[0];

                        subGroupStartIndex = submenulinks.First().location.openTokenStartIndex;
                        subGroupEndIndex = submenulinks.Last().location.endTokenEndIndex;

                        var subcommonparent = DomService.FindParent(submenulinks);
                        bool includeparent = !(subcommonparent.isEqualNode(linkElement) || subcommonparent.isEqualNode(parentElement));

                        var submenu = ConvertToMenu(submenulinks, subcommonparent, includeparent, website);

                        if (submenu.tempdata.EndIndex > subGroupEndIndex)
                        {
                            subGroupEndIndex = submenu.tempdata.EndIndex;
                        }

                        if (submenu.tempdata.StartIndex < subGroupStartIndex)
                        {
                            subGroupStartIndex = submenu.tempdata.StartIndex;
                        }

                        menu.SubItemContainer = submenu.Template;
                        // menu.SubItemSeperator = submenu.SubItemSeperator;
                        menu.children = submenu.children;
                    }
                    else
                    {
                        subGroupStartIndex = groupby.First().First().location.openTokenStartIndex;
                        subGroupEndIndex = groupby.Last().Last().location.endTokenEndIndex;

                        foreach (var item in groupby)
                        {
                            var commonparent = DomService.FindParent(item);
                            bool includeparentintemplate = !commonparent.isEqualNode(parentElement) && commonparent.depth > parentElement.depth;

                            var submenu = ConvertToMenu(item, commonparent, includeparentintemplate, website);
                            // menu.Template = menu.Template.Replace(submenu.tempdata.TempOriginalText, placeholder);

                            if (submenu.tempdata.EndIndex > subGroupEndIndex)
                            {
                                subGroupEndIndex = submenu.tempdata.EndIndex;
                            }

                            if (submenu.tempdata.StartIndex < subGroupStartIndex)
                            {
                                subGroupStartIndex = submenu.tempdata.StartIndex;
                            }

                            menu.AddSubMenu(submenu);
                        }

                        // try to find the seperator....
                        List<Element> parents = new List<Element> {groupby[0].Last(), groupby[1].First()};
                        //var seperatorparent = FindCommonParent(parents);
                        //var seperator = FindSeperator(parents, seperatorparent);
                        //if (!string.IsNullOrEmpty(seperator))
                        //{
                        //    menu.SubItemSeperator = seperator;
                        //}
                    }

                    string subgrouptext = parentElement.ownerDocument.HtmlSource.Substring(subGroupStartIndex, subGroupEndIndex - subGroupStartIndex + 1);

                    menu.Template = menu.Template.Replace(subgrouptext, "{items}");
                }
            }

            return menu;
        }

        /// <summary>
        /// Convert one group of links into menu, links must be within one group.
        /// </summary>
        /// <param name="links"></param>
        /// <param name="website"></param>
        /// <returns></returns>
        public static Menu ConvertToMenu(List<Element> links, WebSite website)
        {
            bool includeparentintemplate = true;
            int maxsilbingindex = 9999;

            links = links.OrderBy(o => o.location.openTokenStartIndex).ToList();
            Element commonparent = DomService.FindParent(links);
            var lastlink = links.LastOrDefault();

            TestTemplate(ref includeparentintemplate, ref maxsilbingindex, commonparent, lastlink);

            return ConvertToMenu(links, commonparent, includeparentintemplate, website, maxsilbingindex);
        }

        /// <summary>
        /// This is to test whether the parentelement can be used as the menu template or not.
        /// </summary>
        /// <param name="includeparentintemplate"></param>
        /// <param name="maxsilbingindex"></param>
        /// <param name="commonparent"></param>
        /// <param name="lastlink"></param>
        private static void TestTemplate(ref bool includeparentintemplate, ref int maxsilbingindex, Element commonparent, Element lastlink)
        {
            var linksafter = FindLinksAfter(commonparent, lastlink);

            foreach (var item in linksafter)
            {
                // if the link is similiar, it is same level links instead of sub menu..
                if (IsSimiliarLink(lastlink, item))
                {
                    includeparentintemplate = false;
                    var parent = item;
                    while (!item.isEqualNode(commonparent))
                    {
                        parent = parent.parentElement;
                    }
                    maxsilbingindex = parent.siblingIndex;
                    break;
                }
            }

            var subparent = lastlink;
            while (true)
            {
                var tempparent = subparent.parentElement;

                if (tempparent != null && !tempparent.isEqualNode(commonparent) && tempparent.depth > commonparent.depth)
                {
                    subparent = tempparent;
                }
                else
                {
                    break;
                }
            }

            foreach (var item in commonparent.childNodes.item.Where(o => o.siblingIndex > subparent.siblingIndex).ToList())
            {
                if (!IsPossibleSeperator(item))
                {
                    includeparentintemplate = false;
                    maxsilbingindex = item.siblingIndex;
                    break;
                }
            }
        }

        /// <summary>
        /// convert one parent with sub links into a menu....
        /// </summary>
        /// <param name="links"></param>
        /// <param name="commonParent"></param>
        /// <param name="includeParentInTemplate"></param>
        /// <param name="website"></param>
        /// <param name="maxsiblingindex"></param>
        /// <returns></returns>
        public static Menu ConvertToMenu(List<Element> links, Element commonParent, bool includeParentInTemplate, WebSite website, int maxsiblingindex = 9999)
        {
            Menu menu = new Menu();

            links = links.OrderBy(o => o.location.openTokenStartIndex).ToList();

            List<Element> parents = new List<Element>();

            int count = links.Count;
            for (int i = 0; i < count; i++)
            {
                var parent = links[i];
                while (!parent.parentElement.isEqualNode(commonParent) && parent != null)
                {
                    parent = parent.parentElement;
                }
                parents.Add(parent);
            }

            menu.tempdata.StartIndex = parents[0].location.openTokenStartIndex;
            menu.tempdata.EndIndex = parents[count - 1].location.endTokenEndIndex;

            for (int i = 0; i < count; i++)
            {
                var parent = parents[i];
                var linkelement = links[i];

                Menu submenu = ConvertToMenu(parent, links[i], website);

                if (submenu.children.Count == 0)
                {
                    int nextSiblingEnds = maxsiblingindex;
                    if (i < count - 1)
                    {
                        var nextparent = parents[i + 1];
                        nextSiblingEnds = nextparent.siblingIndex;
                    }

                    var tempsublinks = FindLinksAfter(commonParent, links[i], nextSiblingEnds);

                    // Check and make sure those sub links are not similar, same level links.
                    List<Element> sublinks = new List<Element>();

                    if (tempsublinks.Count > 0)
                    {
                        foreach (var item in tempsublinks)
                        {
                            if (IsSimiliarLink(item, linkelement))
                            {
                                break;
                            }
                            sublinks.Add(item);
                        }

                        var groupbylinks = GroupBy(sublinks);

                        foreach (var item in groupbylinks)
                        {
                            var subcommonparent = DomService.FindParent(item);

                            bool subIncludeParent = !subcommonparent.isEqualNode(commonParent) && subcommonparent.depth < commonParent.depth;

                            Menu subsubmenu = ConvertToMenu(item, subcommonparent, subIncludeParent, website);

                            submenu.AddSubMenu(subsubmenu);
                        }
                    }
                }

                menu.AddSubMenu(submenu);
            }

            string menuitemsstring = commonParent.ownerDocument.HtmlSource.Substring(menu.tempdata.StartIndex, menu.tempdata.EndIndex - menu.tempdata.StartIndex + 1);

            if (includeParentInTemplate)
            {
                string tempTemplate = commonParent.OuterHtml;
                menu.Template = tempTemplate.Replace(menuitemsstring, "{items}");

                if (commonParent.location.openTokenStartIndex < menu.tempdata.StartIndex)
                {
                    menu.tempdata.StartIndex = commonParent.location.openTokenStartIndex;
                }

                if (commonParent.location.endTokenEndIndex > menu.tempdata.EndIndex)
                {
                    menu.tempdata.EndIndex = commonParent.location.endTokenEndIndex;
                }
            }
            else
            {
                menu.Template = "{items}";
            }

            return menu;
        }

        /// <summary>
        /// Find link items after one item....
        /// </summary>
        /// <param name="commonParent"></param>
        /// <param name="linkitem"></param>
        /// <param name="maxSiblingIndex"></param>
        /// <returns></returns>
        public static List<Element> FindLinksAfter(Element commonParent, Element linkitem, int maxSiblingIndex = 9999)
        {
            var parent = linkitem;
            while (!parent.parentElement.isEqualNode(commonParent))
            {
                parent = parent.parentElement;
            }

            var nodesInBetween = commonParent.childNodes.item.Where(o => o.siblingIndex > parent.siblingIndex && o.siblingIndex < maxSiblingIndex).ToList();

            return Kooboo.Sites.Service.DomService.GetElementsByTagName(nodesInBetween, "a").item;
        }

        /// <summary>
        /// similar links means same level of menu instead of sub menu....
        /// </summary>
        /// <param name="linkx"></param>
        /// <param name="linky"></param>
        /// <returns></returns>
        private static bool IsSimiliarLink(Element linkx, Element linky)
        {
            return (linkx.depth == linky.depth && Kooboo.Sites.Service.DomService.getParentPath(linkx) == Kooboo.Sites.Service.DomService.getParentPath(linky));
        }

        internal static void ReduceGroupBy(List<List<Element>> groupbylist)
        {
            //remove links that are within range of another links.
            List<Kooboo.Lib.DataType.MultiItems<int>> items = new List<Lib.DataType.MultiItems<int>>();

            int groupcount = groupbylist.Count;

            for (int i = 0; i < groupcount; i++)
            {
                Kooboo.Lib.DataType.MultiItems<int> newitem = new Lib.DataType.MultiItems<int> {Item1 = i};

                var lists = groupbylist[i].OrderBy(o => o.location.openTokenStartIndex).ToList();

                newitem.Item2 = lists[0].location.openTokenStartIndex;
                newitem.Item3 = lists[lists.Count - 1].location.endTokenEndIndex;

                items.Add(newitem);
            }

            List<int> toberemoved = new List<int>();

            for (int i = 0; i < groupcount; i++)
            {
                List<Element> item = groupbylist[i];

                var itemlist = item.OrderBy(o => o.location.openTokenStartIndex).ToList();
                int beginindex = itemlist.FirstOrDefault().location.openTokenStartIndex;
                int endindex = itemlist.LastOrDefault().location.endTokenEndIndex;

                foreach (var positionitem in items)
                {
                    if (i != positionitem.Item1)
                    {
                        if (beginindex >= positionitem.Item2 && endindex <= positionitem.Item3)
                        {
                            toberemoved.Add(i);
                            break;
                        }
                    }
                }
            }

            toberemoved.Reverse();

            foreach (var item in toberemoved)
            {
                groupbylist.RemoveAt(item);
            }
        }

        private static bool IsPossibleSeperator(Node node)
        {
            if (node is Element e)
            {
                var links = e.getElementsByTagName("a").item;
                if (links != null && links.Count > 0)
                {
                    return false;
                }
            }

            string textcontent = node.textContent.Trim('\r', '\n', ' ');

            //if (string.IsNullOrEmpty(textcontent))
            //{ // return false; //}
            textcontent = System.Web.HttpUtility.HtmlDecode(textcontent);

            foreach (var item in textcontent.ToCharArray())
            {
                if (Kooboo.Dom.CommonIdoms.isAlphanumeric(item))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool IsPossibleSeperator(List<Node> nodes)
        {
            foreach (var item in nodes)
            {
                if (!IsPossibleSeperator(item))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// used when there is only one link
        /// </summary>
        /// <param name="currentParentFound"></param>
        /// <returns></returns>
        public static Element TryUpgrade(Element currentParentFound)
        {
            // rule 1, if the parent has only one sub element and no other element, should upgrade...
            var parent = currentParentFound.parentElement;

            foreach (var item in parent.childNodes.item)
            {
                if (!item.isEqualNode(currentParentFound))
                {
                    if (item is Element)
                    {
                        return currentParentFound;
                    }
                    else if (item is Text text)
                    {
                        string content = text.textContent;
                        content = content.Trim('\r', '\n', ' ');

                        if (!string.IsNullOrEmpty(content))
                        {
                            return currentParentFound;
                        }
                    }
                }
            }

            return TryUpgrade(parent);
        }
    }

    public class RawMenu
    {
        public RawMenu()
        {
            this.Children = new List<RawMenu>();
        }

        public Element LinkElement { get; set; }

        public Element ContainerElement { get; set; }

        public Element SubItemContainer { get; set; }

        public List<RawMenu> Children { get; set; }

        public RawMenu Parent { get; set; }

        public Element NextContainer()
        {
            if (Parent == null)
            {
                return null;
            }
            int currentindex = this.ContainerElement.location.openTokenStartIndex;

            if (currentindex >= 0 && this.Parent.Children.Any())
            {
                var others = this.Parent.Children.Where(o => o.ContainerElement.location.openTokenStartIndex > currentindex);
                if (others != null && others.Any())
                {
                    return others.OrderBy(o => o.ContainerElement.location.openTokenStartIndex).First().ContainerElement;
                }
            }
            return null;
        }
    }
}