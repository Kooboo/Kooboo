//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using Kooboo.Data.Models;
using Kooboo.Sites.Models;
using Kooboo.Dom;
using Kooboo.Sites.ViewModel;
using Kooboo.Sites.Repository;
using Kooboo.Sites.Helper; 

namespace Kooboo.Sites.Service
{
    public class MenuService
    {

        public static RawMenu FindRawMenu(Element element)
        {
            var alllinks = element.getElementsByTagName("a").item;
            if (alllinks != null && alllinks.Count() > 0)
            {
                var groupby = SimpleGroupBy(alllinks);

                RawMenu menu = new RawMenu();
                AssignSubMenu(groupby[0], menu);
                menu.SubItemContainer = element;
                return menu;
            }
            return null;
        }

        internal static List<List<Element>> SimpleGroupBy(List<Element> LinkElements)
        {
            Func<List<Element>, int> GetFirstLocation = (elements) =>
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

            var groupby = LinkElements.GroupBy(o => DomService.getParentPath(o));
            
            List<List<Element>> sortedlist = new List<List<Element>>(); 

            foreach (var item in groupby.OrderBy(o => GetFirstLocation(o.ToList())))
            {
                sortedlist.Add(item.ToList()); 
            } 
            return sortedlist; 
        }

        public static void AssignSubMenu(List<Element> Links, RawMenu ParentMenu)
        {
            var commonparent = DomService.FindParent(Links);

            foreach (var item in Links)
            {
                var menu = new RawMenu();
                menu.Parent = ParentMenu;
                menu.LinkElement = item;
                menu.ContainerElement = FindLinkContainer(item, Links, commonparent);
                menu.Parent = ParentMenu;
                ParentMenu.Children.Add(menu);
            }

            foreach (var item in ParentMenu.Children)
            {
                var sublinks = FindSubLinks(item);
                if (sublinks != null && sublinks.Count() > 0)
                {
                    var itemcontainer = DomService.FindParent(sublinks);
                    AssignSubMenu(sublinks, item);
                }
            }
            return;
        }

        public static List<Element> FindSubLinks(RawMenu menu)
        {
            var Sublinks = GetDirectSubLinks(menu.LinkElement, menu.ContainerElement);
            if (Sublinks != null && Sublinks.Count() > 0)
            {
                return Sublinks;
            }

            return GetSiblingSubLinks(menu.ContainerElement, menu.NextContainer());
        }

        internal static List<Element> GetDirectSubLinks(Element LinkElement, Element Container)
        {
            var alllinks = Container.getElementsByTagName("a").item;

            alllinks.Remove(LinkElement);

            if (alllinks.Count() > 0)
            {
                var groupby = GroupBy(alllinks);
                return groupby[0];
            }
            return null;
        }

        internal static List<Element> GetSiblingSubLinks(Element CurrentContainer, Element NextContainer)
        {
            List<Node> BetweenNodes = new List<Node>();

            int MaxSiblingIndex = 9999;
            if (NextContainer != null)
            {
                MaxSiblingIndex = NextContainer.siblingIndex;
                var parent = DomService.FindParent(CurrentContainer, NextContainer);

                var current = CurrentContainer;
                while (current.depth > parent.depth + 1)
                {
                    current = current.parentElement;
                }

                var next = NextContainer;
                while (next.depth > parent.depth + 1)
                {
                    next = next.parentElement;
                }
                var NodesInBetween = parent.childNodes.item.Where(o => o.siblingIndex > current.siblingIndex && o.siblingIndex < next.siblingIndex).ToList();

                var alllinks = DomService.GetElementsByTagName(NodesInBetween, "a").item;
                if (alllinks.Count() > 0)
                {
                    var group = GroupBy(alllinks);
                    return group[0];
                }

            }
            else
            {

                var parent = CurrentContainer.parentElement;

                var NodesInBetween = parent.childNodes.item.Where(o => o.siblingIndex > CurrentContainer.siblingIndex).ToList();

                var alllinks = DomService.GetElementsByTagName(NodesInBetween, "a").item;
                if (alllinks.Count() > 0)
                {
                    var group = GroupBy(alllinks);
                    return group[0];
                }

            }


            return null;

        }


        public static void AddNewMenu(SiteDb SiteDb, Page page, MenuViewModel NewMenuViewModel)
        {
            // menu view can start only with item container. 
            Menu menu = CreateMenu(page.Dom, NewMenuViewModel);

            if (string.IsNullOrEmpty(menu.Name))
            {
                menu.Name = SiteDb.Menus.GetNewMenuName();
            }

            SiteDb.Menus.AddOrUpdate(menu);

            var maincontainer = NewMenuViewModel.ItemContainer;

            var element = DomService.GetElementByKoobooId(page.Dom, maincontainer);

            string newbody = page.Body.Substring(0, element.location.openTokenStartIndex);
            newbody += "<menu id='" + menu.Name + "'></menu>";
            newbody += page.Body.Substring(element.location.endTokenEndIndex + 1);

            page.Body = newbody;
            SiteDb.Pages.AddOrUpdate(page);

        }

        public static Element FindLinkContainer(Element Link, List<Element> LinkGroup, Element CommonParent)
        {
            var target = Link;

            var DirectParent = target;

            while (DirectParent != null && !DirectParent.isEqualNode(CommonParent))
            {
                foreach (var item in LinkGroup)
                {
                    if (!item.isEqualNode(Link))
                    {
                        if (DomService.ContainsOrEqualElement(DirectParent, item))
                        {
                            break;
                        }
                    }
                }

                target = DirectParent;
                DirectParent = target.parentElement;
            }

            return target;

        }

        public static Menu CreateMenu(Document doc, MenuViewModel MenuViewModel)
        {
            Menu menu = new Menu();
            Element LinkElement = null;
            Element ItemContainer = null;
            Element ContainerElement = null;

            int MenuTemplateStart = int.MaxValue;
            int MenuTemplaetEnd = 0;

            if (!string.IsNullOrEmpty(MenuViewModel.LinkElement) && !string.IsNullOrEmpty(MenuViewModel.ContainerElement))
            {
                var linkelement = DomService.GetElementByKoobooId(doc, MenuViewModel.LinkElement);
                if (linkelement != null)
                {
                    LinkElement = linkelement as Element;
                }

                var container = DomService.GetElementByKoobooId(doc, MenuViewModel.ContainerElement);
                if (container == null && linkelement != null)
                {
                    container = linkelement;
                }
                if (container != null)
                {
                    ContainerElement = container as Element;
                    MenuTemplateStart = ContainerElement.location.openTokenStartIndex;
                    MenuTemplaetEnd = ContainerElement.location.endTokenEndIndex;
                }
            }

            string SubMenuItemsTemplate = null;
            string SubMenuItemOrginalString = null;
            int SubMenuStart = int.MaxValue;
            int SubMenuEnd = 0;

            if (MenuViewModel.children.Count > 0)
            {
                List<Menu> SubMenus = new List<Menu>();

                foreach (var item in MenuViewModel.children)
                {
                    Menu submenu = CreateMenu(doc, item);
                    if (submenu != null)
                    {
                        SubMenus.Add(submenu);
                        if (submenu.tempdata.StartIndex < SubMenuStart)
                        {
                            SubMenuStart = submenu.tempdata.StartIndex;
                        }
                        if (submenu.tempdata.EndIndex > SubMenuEnd)
                        {
                            SubMenuEnd = submenu.tempdata.EndIndex;
                        }
                    }
                }

                ItemContainer = _GetItemContainer(doc, MenuViewModel);

                string SubMenuString = doc.HtmlSource.Substring(SubMenuStart, SubMenuEnd - SubMenuStart + 1);

                if (ItemContainer != null)
                {
                    // 1, sub item within the link element...
                    if (ContainerElement == null || DomService.ContainsOrEqualElement(ContainerElement, ItemContainer))
                    {
                        SubMenuItemOrginalString = ItemContainer.OuterHtml;
                        SubMenuItemsTemplate = SubMenuItemOrginalString.Replace(SubMenuString, MenuHelper.MarkSubItems); 
                    }
                    else if (ContainerElement.isEqualNode(ItemContainer))
                    {
                        SubMenuItemOrginalString = SubMenuString;
                        SubMenuItemsTemplate = MenuHelper.MarkSubItems;
                    }

                    else
                    {
                        var distance = DomService.GetTreeDistance(ContainerElement, ItemContainer);
                        bool sibling = false;
                        if (distance == 1)
                        {
                            sibling = true;
                        }
                        else if (distance < 5)
                        {
                            var nodes = DomService.GetNodesInBetween(doc, ContainerElement, ItemContainer);
                            if (nodes == null || nodes.Count == 0 || IsPossibleSeperator(nodes))
                            {
                                sibling = true;
                            }
                        }

                        if (sibling)
                        {
                            if (MenuTemplateStart > ItemContainer.location.openTokenStartIndex)
                            {
                                MenuTemplateStart = ItemContainer.location.openTokenStartIndex;
                            }

                            if (MenuTemplaetEnd < ItemContainer.location.endTokenEndIndex)
                            {
                                MenuTemplaetEnd = ItemContainer.location.endTokenEndIndex;
                            }

                            SubMenuItemOrginalString = ItemContainer.OuterHtml;
                            SubMenuItemsTemplate = SubMenuItemOrginalString.Replace(SubMenuString, MenuHelper.MarkSubItems);
                        }

                        else
                        {
                            //menu.RenderSubMenuSeperated = true;
                            SubMenuItemOrginalString = ItemContainer.OuterHtml;
                            SubMenuItemsTemplate = ItemContainer.OuterHtml.Replace(SubMenuString, MenuHelper.MarkSubItems);
                        }
                    }
                }

                if (SubMenus.Count > 0)
                {
                    menu.children.AddRange(SubMenus);
                }
            }

            if (MenuTemplateStart > 0 && MenuTemplaetEnd > 0)
            {
                string menutemplate = doc.HtmlSource.Substring(MenuTemplateStart, MenuTemplaetEnd - MenuTemplateStart + 1);

                if (!string.IsNullOrEmpty(SubMenuItemOrginalString))
                {
                    menutemplate = menutemplate.Replace(SubMenuItemOrginalString, MenuHelper.MarkSubItems);
                }

                string OriginalLink = LinkElement.OuterHtml;
                string NewLInk = DomService.ReplaceLink(LinkElement, MenuHelper.MarkHref, MenuHelper.MarkAnchorText);
                menutemplate = menutemplate.Replace(OriginalLink, NewLInk);
                menu.Template = menutemplate;
                menu.Name = MenuViewModel.text;
                menu.Url = MenuViewModel.href;
            }

            menu.SubItemContainer = SubMenuItemsTemplate;

            menu.tempdata.StartIndex = MenuTemplateStart;
            menu.tempdata.EndIndex = MenuTemplaetEnd;

            if (menu.tempdata.StartIndex == 0 || menu.tempdata.EndIndex == 0)
            {
                if (ItemContainer != null)
                {
                    menu.tempdata.StartIndex = ItemContainer.location.openTokenStartIndex;
                    menu.tempdata.EndIndex = ItemContainer.location.endTokenEndIndex;
                }
            }
            return menu;
        }

        private static Element _GetItemContainer(Document doc, MenuViewModel MenuViewModel)
        {
            // find the item container. 
            if (!string.IsNullOrEmpty(MenuViewModel.ItemContainer))
            {
                var itemcontainer = DomService.GetElementByKoobooId(doc, MenuViewModel.ItemContainer) as Element;
                if (itemcontainer != null)
                {
                    return itemcontainer;
                }
            }

            List<Element> containerlist = new List<Element>();
            foreach (var item in MenuViewModel.children)
            {
                if (!string.IsNullOrEmpty(item.ContainerElement))
                {
                    var element = DomService.GetElementByKoobooId(doc, item.ContainerElement) as Element;
                    if (element != null)
                    {
                        containerlist.Add(element);
                    }
                }
            }

            if (containerlist.Count == 0)
            {
                foreach (var item in MenuViewModel.children)
                {
                    var element = DomService.GetElementByKoobooId(doc, item.LinkElement) as Element;
                    if (element != null)
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
                    MenuViewModel.ItemContainer = DomService.GetKoobooId(element);
                }
                return element;
            }
            return null;
        }

        /// <summary>
        /// Group by the links into different menu group. 
        /// </summary>
        /// <param name="linkelements"></param>
        /// <param name="ReduceGroup">Reduce the sub group within another group.... like sub menus within the main menu will be reduced.</param>
        /// <returns></returns>
        public static List<List<Element>> GroupBy(List<Element> linkelements, bool ReduceGroup = true)
        {
            List<List<Element>> result = new List<List<Element>>();

            int count = linkelements.Count;

            if (count == 0)
            {
                return result;
            }
            else if (count == 1)
            {
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
                ///The first one... 
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
                            /// if there is already a group and, this new member belongs to the same parent, then it belongs to the same group. 
                            if (result.LastOrDefault().Count > 1)
                            {
                                var currentlist = result.LastOrDefault();
                                var itemone = currentlist[currentlist.Count - 2];
                                var itemtwo = currentlist[currentlist.Count - 1];
                                var parent = Kooboo.Sites.Service.DomService.FindParent(itemone, itemtwo);

                                /// if there is longer distance.... 
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
                            else
                            {
                                // count = 1;     
                                var lastelement = result.LastOrDefault()[0];
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
                    List<Element> firstlist = new List<Element>();
                    firstlist.Add(sorted[i]);
                    result.Add(firstlist);
                }
                else
                {

                    if (result.Count > 0)
                    {
                        result.LastOrDefault().Add(current);
                    }
                }
            }
            if (ReduceGroup)
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
                /// there is sub menu...
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

                if (sublinks.Count() > 0)
                {
                    int SubGroupStartIndex;
                    int SubGroupEndIndex;

                    var groupby = GroupBy(sublinks);

                    if (groupby.Count == 1)
                    {
                        var submenulinks = groupby[0];

                        SubGroupStartIndex = submenulinks.First().location.openTokenStartIndex;
                        SubGroupEndIndex = submenulinks.Last().location.endTokenEndIndex;

                        var subcommonparent = DomService.FindParent(submenulinks);
                        bool includeparent = true;

                        if (subcommonparent.isEqualNode(linkElement) || subcommonparent.isEqualNode(parentElement))
                        {
                            includeparent = false;
                        }

                        var submenu = ConvertToMenu(submenulinks, subcommonparent, includeparent, website);

                        if (submenu.tempdata.EndIndex > SubGroupEndIndex)
                        {
                            SubGroupEndIndex = submenu.tempdata.EndIndex;
                        }

                        if (submenu.tempdata.StartIndex < SubGroupStartIndex)
                        {
                            SubGroupStartIndex = submenu.tempdata.StartIndex;
                        }

                        menu.SubItemContainer = submenu.Template;
                        // menu.SubItemSeperator = submenu.SubItemSeperator;
                        menu.children = submenu.children;
                    }

                    else
                    {
                        SubGroupStartIndex = groupby.First().First().location.openTokenStartIndex;
                        SubGroupEndIndex = groupby.Last().Last().location.endTokenEndIndex;

                        foreach (var item in groupby)
                        {
                            var commonparent = DomService.FindParent(item);
                            bool includeparentintemplate = false;

                            if (!commonparent.isEqualNode(parentElement) && commonparent.depth > parentElement.depth)
                            {
                                includeparentintemplate = true;
                            }

                            var submenu = ConvertToMenu(item, commonparent, includeparentintemplate, website);
                            // menu.Template = menu.Template.Replace(submenu.tempdata.TempOriginalText, placeholder);

                            if (submenu.tempdata.EndIndex > SubGroupEndIndex)
                            {
                                SubGroupEndIndex = submenu.tempdata.EndIndex;
                            }

                            if (submenu.tempdata.StartIndex < SubGroupStartIndex)
                            {
                                SubGroupStartIndex = submenu.tempdata.StartIndex;
                            }

                            menu.AddSubMenu(submenu);
                        }

                        // try to find the seperator.... 
                        List<Element> parents = new List<Element>();
                        parents.Add(groupby[0].Last());
                        parents.Add(groupby[1].First());
                        //var seperatorparent = FindCommonParent(parents);
                        //var seperator = FindSeperator(parents, seperatorparent);
                        //if (!string.IsNullOrEmpty(seperator))
                        //{
                        //    menu.SubItemSeperator = seperator;
                        //}
                    }

                    string subgrouptext = parentElement.ownerDocument.HtmlSource.Substring(SubGroupStartIndex, SubGroupEndIndex - SubGroupStartIndex + 1);

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
        /// <param name="IncludeParentInTemplate"></param>
        /// <param name="maxsiblingindex">if this links group share parents with another link group, should only check sub menu till end of next sibling start. </param>
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
                /// if the link is similiar, it is same level links instead of sub menu..
                if (isSimiliarLink(lastlink, item))
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
            /// 
        }

        /// <summary>
        /// convert one parent with sub links into a menu.... 
        /// </summary>
        /// <param name="links"></param>
        /// <param name="CommonParent"></param>
        /// <param name="IncludeParentInTemplate"></param>
        /// <param name="website"></param>
        /// <returns></returns>
        public static Menu ConvertToMenu(List<Element> links, Element CommonParent, bool IncludeParentInTemplate, WebSite website, int maxsiblingindex = 9999)
        {
            Menu menu = new Menu();

            links = links.OrderBy(o => o.location.openTokenStartIndex).ToList();

            List<Element> Parents = new List<Element>();

            int count = links.Count;
            for (int i = 0; i < count; i++)
            {
                var parent = links[i];
                while (!parent.parentElement.isEqualNode(CommonParent) && parent != null)
                {
                    parent = parent.parentElement;
                }
                Parents.Add(parent);
            }

            menu.tempdata.StartIndex = Parents[0].location.openTokenStartIndex;
            menu.tempdata.EndIndex = Parents[count - 1].location.endTokenEndIndex;

            for (int i = 0; i < count; i++)
            {
                var parent = Parents[i];
                var linkelement = links[i];

                Menu submenu = ConvertToMenu(parent, links[i], website);

                if (submenu.children.Count == 0)
                {
                    int nextSiblingEnds = maxsiblingindex;
                    if (i < count - 1)
                    {
                        var nextparent = Parents[i + 1];
                        nextSiblingEnds = nextparent.siblingIndex;
                    }

                    var tempsublinks = FindLinksAfter(CommonParent, links[i], nextSiblingEnds);

                    /// Check and make sure those sub links are not similar, same level links. 
                    List<Element> sublinks = new List<Element>();

                    if (tempsublinks.Count > 0)
                    {
                        foreach (var item in tempsublinks)
                        {
                            if (isSimiliarLink(item, linkelement))
                            {
                                break;
                            }
                            sublinks.Add(item);
                        }

                        var groupbylinks = GroupBy(sublinks);

                        foreach (var item in groupbylinks)
                        {
                            var subcommonparent = DomService.FindParent(item);

                            bool SubIncludeParent = false;

                            if (!subcommonparent.isEqualNode(CommonParent) && subcommonparent.depth < CommonParent.depth)
                            {
                                SubIncludeParent = true;
                            }
                            Menu subsubmenu = ConvertToMenu(item, subcommonparent, SubIncludeParent, website);

                            submenu.AddSubMenu(subsubmenu);
                        }
                    }
                }

                menu.AddSubMenu(submenu);
            }

            string menuitemsstring = CommonParent.ownerDocument.HtmlSource.Substring(menu.tempdata.StartIndex, menu.tempdata.EndIndex - menu.tempdata.StartIndex + 1);

            if (IncludeParentInTemplate)
            {

                string tempTemplate = CommonParent.OuterHtml;
                menu.Template = tempTemplate.Replace(menuitemsstring, "{items}");

                if (CommonParent.location.openTokenStartIndex < menu.tempdata.StartIndex)
                {
                    menu.tempdata.StartIndex = CommonParent.location.openTokenStartIndex;
                }

                if (CommonParent.location.endTokenEndIndex > menu.tempdata.EndIndex)
                {
                    menu.tempdata.EndIndex = CommonParent.location.endTokenEndIndex;
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
        /// <param name="CommonParent"></param>
        /// <param name="linkitem"></param>
        /// <param name="SiblingIndex"></param>
        /// <returns></returns>
        public static List<Element> FindLinksAfter(Element CommonParent, Element linkitem, int MaxSiblingIndex = 9999)
        {
            var parent = linkitem;
            while (!parent.parentElement.isEqualNode(CommonParent))
            {
                parent = parent.parentElement;
            }

            var NodesInBetween = CommonParent.childNodes.item.Where(o => o.siblingIndex > parent.siblingIndex && o.siblingIndex < MaxSiblingIndex).ToList();

            return Kooboo.Sites.Service.DomService.GetElementsByTagName(NodesInBetween, "a").item;

        }

        /// <summary>
        /// similar links means same level of menu instead of sub menu.... 
        /// </summary>
        /// <param name="linkx"></param>
        /// <param name="linky"></param>
        /// <returns></returns>
        private static bool isSimiliarLink(Element linkx, Element linky)
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
                Kooboo.Lib.DataType.MultiItems<int> newitem = new Lib.DataType.MultiItems<int>();
                newitem.Item1 = i;

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
            if (node is Element)
            {
                Element e = node as Element;
                var links = e.getElementsByTagName("a").item;
                if (links != null && links.Count > 0)
                {
                    return false;
                }
            }

            string textcontent = node.textContent.Trim(new char[] { '\r', '\n', ' ' });

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
                    else if (item is Kooboo.Dom.Text)
                    {
                        Kooboo.Dom.Text text = item as Kooboo.Dom.Text;
                        string content = text.textContent;
                        content = content.Trim(new char[] { '\r', '\n', ' ' });

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

            if (currentindex >= 0 && this.Parent.Children.Count() > 0)
            {
                var others = this.Parent.Children.Where(o => o.ContainerElement.location.openTokenStartIndex > currentindex);
                if (others != null && others.Count() > 0)
                {
                    return others.OrderBy(o => o.ContainerElement.location.openTokenStartIndex).First().ContainerElement;
                }
            }
            return null;
        }

    }

}
