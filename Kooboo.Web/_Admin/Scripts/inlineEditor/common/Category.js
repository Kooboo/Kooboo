function Category(){
    var DomService = Kooboo.NewDomService,
            TagGroup = Kooboo.TagGroup;
    return {
        EnumCategoryType : { Direct: "direct", Sibling: "Sibling", Related: "Related", None: "None" },
        createCategoryTree:function() {
            return {
                LinkElement: null,
                ContainerElement: null,
                ItemContainer: null,
                Items: []
            }
        },
        createCategoryData:function() {
            return {
                href: null,
                linktext: null,
                Children: []
            }
        },
        FindCategoryLinks:function(Container) {
            var alllinks = Container.getElementsByTagName("a");

            var groupby = _.groupBy(alllinks, function(o) { return DomService.GetPath(o); });

            var sorted = _.sortBy(groupby, function(item, key) { return DomService.GetDepth(item[0]); });
            for (var i = 0; i < sorted.length; i++) {
                var item = sorted[i];

                if (this.IsCategoryLink(item)) {

                    if (DomService.HasSameParent(item)) {
                        return item;
                    }
                }
            }
            return null;
        },
        FindCategoryTree:function(Container) {
            var self = this;
            var links = this.FindCategoryLinks(Container);
            if (links == null || links.length == 0 || links.length == 1) {
                return null;
            }
            var tree = this.createCategoryTree();

            AddSubTree(tree, links);

            tree.ItemContainer = TreeUpgradeToContainer(tree.ItemContainer, Container);

            return tree;

            function AddSubTree(currenttree, sublinks) {

                var commoneparent = DomService.FindCommonParent(sublinks);
                currenttree.ItemContainer = commoneparent;

                var subtrees = [];

                for (var i = 0; i < sublinks.length; i++) {
                    var item = sublinks[i];
                    var subtree = self.createCategoryTree();
                    subtree.LinkElement = item;
                    subtree.ContainerElement = DomService.FindContainer(item, commoneparent);
                    subtrees.push(subtree);
                }

                var subtreelinks = Category.getSubLinks(subtrees, commoneparent);
                if (subtreelinks != null && subtreelinks.length > 0) {
                    for (var i = 0; i < subtrees.length; i++) {

                        var currentsubtree = subtrees[i];

                        var onesublinks = _.find(subtreelinks, function(o) { return o.Index == i; });

                        if (onesublinks != null && onesublinks.ELementList != null && onesublinks.ELementList.length > 0) {
                            AddSubTree(currentsubtree, onesublinks.ELementList);
                        }
                    }
                }
                currenttree.Items = subtrees;
            }

            function TreeUpgradeToContainer(itemcontainer, maincotnainer) {
                if (itemcontainer == null) {
                    return null;
                }

                if (DomService.GetDepth(itemcontainer) <= DomService.GetDepth(maincotnainer)) {
                    return itemcontainer;
                }

                var parent = itemcontainer.parentElement;
                if (parent == null || parent.tagName.toLowerCase() == "body" || parent.isEqualNode(maincotnainer)) {
                    return itemcontainer;
                }

                var subelments = DomService.GetChildElements(parent);
                if (subelments.length == 1) {
                    return TreeUpgradeToContainer(parent, maincotnainer);
                } else {
                    return itemcontainer;
                }
            }
        },
        getSubLinks:function(trees, CommonParent) {
            var result = [];
            var count = trees.length;

            for (var i = 0; i < count; i++) {
                var subtree = trees[i];
                var sublinks = [];
                sublinks = GetDirectSubLinks(subtree.LinkElement, subtree.ContainerElement);
                if (sublinks != null && sublinks.length > 0) {
                    var sublinkstore = {};
                    sublinkstore.Index = i;
                    sublinkstore.ELementList = sublinks;
                    result.push(sublinkstore);
                }
            }

            if (result.length > 0) {
                return result;
            }

            for (var i = 0; i < count; i++) {
                var subtree = trees[i];
                var sublinks = [];

                var nextcontainer;
                if (i < count - 1) {
                    nextcontainer = trees[i + 1].ContainerElement;
                } else {
                    nextcontainer = null;
                }

                sublinks = GetSiblingSubLinks(subtree.LinkElement, subtree.ContainerElement, nextcontainer, CommonParent);
                if (sublinks != null && sublinks.length > 0) {
                    var sublinkstore = {};
                    sublinkstore.Index = i;
                    sublinkstore.ELementList = sublinks;
                    result.push(sublinkstore);
                }
            }

            if (result.length > 0) {
                return result;
            }

            return null;


            function GetDirectSubLinks(link, container) {

                var alllinks = container.getElementsByTagName("a");

                if (alllinks == null || alllinks.length == 0 || (alllinks.length == 1 && alllinks[0].isEqualNode(link))) {
                    return;
                }
                var links = _.filter(alllinks, function(o) {
                    return o.isEqualNode(link) == false;
                });

                var groupby = _.groupBy(links, function(o) {
                    return DomService.GetPath(o);
                });

                var sorted = _.sortBy(groupby, function(key, item) { return DomService.GetDepth(key[0]); });

                if (this.IsCategoryLink(sorted[0])) {
                    return sorted[0];
                }
                return null;
            }


            function GetSiblingSubLinks(linkElement, linkContainer, nextContainer, ParentContainer) {

                var alllinks = DomService.GetElementsInBetween(linkContainer, nextContainer, ParentContainer, "a");

                if (alllinks == null || alllinks.length == 0 || (alllinks.length == 1 && alllinks[0].isEqualNode(linkElement))) {
                    return null;
                }

                var links = _.filter(alllinks, function(o) {
                    return o.isEqualNode(linkElement) == false;
                });

                var groupby = _.groupBy(links, function(o) {
                    return DomService.GetPath(o);
                });

                var sorted = _.sortBy(groupby, function(key, item) { return DomService.GetDepth(key[0]); });

                if (this.IsCategoryLink(sorted[0])) {
                    return sorted[0];
                }
                return null;
            }
        },
        GetCategoryLevels:function(tree) {
            var currentlevel = 0;
            var newlevel = 0;

            for (var i = 0; i < tree.Items.length; i++) {
                var item = tree.Items[i];
                var sublevel = GetSubLevel(item, currentlevel);
                if (sublevel > newlevel) {
                    newlevel = sublevel;
                }
            }

            return newlevel;

            function GetSubLevel(subtree, currentlevel) {
                var newsublevel = currentlevel + 1;
                var result = newsublevel;

                for (var i = 0; i < subtree.Items.length; i++) {
                    var subitem = subtree.Items[i];
                    var subsublevel = GetSubLevel(subitem, newsublevel);
                    if (subsublevel > result) {
                        result = subsublevel;
                    }
                }
                return result;
            }
        },
        GetData : function(tree) {
            var self = this;
            var result = [];

            for (var i = 0; i < tree.Items.length; i++) {
                var item = tree.Items[i];
                var data = self.createCategoryData();
                data.href = item.LinkElement.getAttribute("href");
                data.linktext = item.LinkElement.innerHTML;
                getsubdata(data, item.Items);
                result.push(data);
            }
            CutOffCommonLinkPart(result);
            return result;

            function getsubdata(parent, subs) {

                if (subs.length > 0) {
                    for (var i = 0; i < subs.length; i++) {
                        var item = subs[i];
                        var subdata = self.createCategoryData();
                        subdata.href = item.LinkElement.getAttribute("href");
                        subdata.linktext = item.LinkElement.innerHTML;
                        getsubdata(subdata, item.Items);
                        parent.Children.push(subdata);
                    }
                }
                CutOffCommonLinkPart(parent.Children);
            }

            function CutOffCommonLinkPart(datas) {

                var common = GetCommoneLinkPart(datas, false);

                for (var i = 0; i < datas.length; i++) {
                    var each = datas[i];
                    each.href = each.href.replace(common, "");
                }

                function GetCommoneLinkPart(trees, withhref) {
                    var links = [];
                    for (var i = 0; i < trees.length; i++) {
                        var item = trees[i];
                        links.push(item.href)
                    }
                    var commone = DomService.GetLinkPattern(links, "href");
                    if (!withhref) {
                        commone = commone.replace('{href}', "");
                    }
                    return commone;
                }
            }
        },
        GetTemplate:function(tree) {
            return GetSubItems(tree);

            function GetSubItems(tree) {

                if (tree.Items.length == 0) {
                    return null;
                }

                var repeatItemMatch = "{category}";

                var subtree = tree.Items[0];
                var subtype = Category.GetSubCategoryType(subtree);
                var el = subtree.LinkElement;
                var elcopy = el.cloneNode(true);
                var linkpattern = GetHrefPattern(tree.Items);

                elcopy.removeAttribute("href");
                elcopy.setAttribute("tal-href", linkpattern);

                elcopy.setAttribute("tal-content", "{linktext}");

                var container = subtree.ContainerElement;
                var containercopy;
                var RelatedSubitemTemplate = [];
                var subitemTemlate = GetSubItems(subtree);

                var Repeater = "";
                if (container.isEqualNode(el)) {

                    if (subtype == Category.EnumCategoryType.Sibling) {
                        Repeater = "<var tal-omit-tag=true tal-repeat='" + repeatItemMatch + "'>"
                        Repeater = Repeater + elcopy.outerHTML;
                        Repeater = Repeater + subitemTemlate;
                        Repeater = Repeater + "</var>";
                    } else if (subtype == Category.EnumCategoryType.Related) {
                        // TODO:  
                    } else {
                        elcopy.setAttribute("tal-repeat", repeatItemMatch);
                        Repeater = elcopy.outerHTML;
                    }
                } else {
                    containercopy = container.cloneNode(true);

                    if (subtype == Category.EnumCategoryType.Direct) {
                        containercopy.setAttribute("tal-repeat", repeatItemMatch);
                        Repeater = containercopy.outerHTML;
                        Repeater = Repeater.replace(el.outerHTML, elcopy.outerHTML);
                        Repeater = Repeater.replace(subtree.ItemContainer.outerHTML, subitemTemlate);
                    } else if (subtype == Category.EnumCategoryType.Sibling) {
                        Repeater = "<var tal-omit-tag=true tal-repeat='" + repeatItemMatch + "'>"
                        var inner = containercopy.outerHTML;
                        inner = inner.replace(el.outerHTML, elcopy.outerHTML);
                        Repeater = Repeater + inner;
                        Repeater = Repeater + subitemTemlate;
                        Repeater = Repeater + "</var>";

                    } else if (subtype == Category.EnumCategoryType.Related) {
                        // TODO: 
                    } else {

                        containercopy.setAttribute("tal-repeat", repeatItemMatch);
                        Repeater = containercopy.outerHTML;
                        Repeater = Repeater.replace(el.outerHTML, elcopy.outerHTML);
                    }
                }


                var tobereplace = GetSubItemsMaxString(tree);

                var alltext = tree.ItemContainer.outerHTML;

                var result = alltext.replace(tobereplace, Repeater);

                return result;
            }

            function GetHrefPattern(items) {

                var links = [];
                for (var i = 0; i < items.length; i++) {
                    var item = items[i];
                    var href = item.LinkElement.getAttribute("href");
                    links.push(href);
                }
                return DomService.GetLinkPattern(links, "href");
            }

            function GetSubItemsMaxString(tree) {

                if (tree.Items.length == 0) {
                    return null;
                }

                var start = tree.Items[0].ContainerElement;
                var end = tree.Items[0].ContainerElement;

                for (var i = 0; i < tree.Items.length; i++) {
                    var item = tree.Items[i];

                    if (item.ContainerElement != null && DomService.CompareDomPosition(start, item.ContainerElement) == 1) {

                        start = item.ContainerElement;
                    }
                    if (item.ItemContainer != null && DomService.CompareDomPosition(start, item.ItemContainer) == 1) {

                        start = item.ItemContainer;
                    }

                    if (item.ContainerElement != null && DomService.CompareDomPosition(end, item.ContainerElement) == -1) {

                        end = item.ContainerElement;
                    }

                    if (item.ItemContainer != null && DomService.CompareDomPosition(end, item.ItemContainer) == -1) {

                        end = item.ItemContainer;
                    }

                }

                if (start.isEqualNode(end)) {
                    return start.outerHTML;
                }

                var alltext = tree.ItemContainer.outerHTML;

                var startindex = alltext.indexOf(start.outerHTML);
                var endindex = alltext.indexOf(end.outerHTML);
                endindex = endindex + end.outerHTML.length;

                return alltext.substring(startindex, endindex);

            }

            function GetSubItemLinkData(tree) {
                var alllinks = [];

                for (var i = 0; i < tree.Items.length; i++) {
                    var item = tree.Items[i];
                    alllinks.push(item.LinkElement);
                }
                //TODO: not implemented yet
                return null;
            }
        },
         ///The type of sub category, direct, sibling, or related.
        GetSubCategoryType:function(tree) {
            if (tree.Items.length == 0) {
                return this.EnumCategoryType.None;
            }

            if (DomService.IsParentSubElement(tree.ContainerElement, tree.ItemContainer)) {
                return this.EnumCategoryType.Direct;
            } else {

                if (DomService.IsNextSiblingElement(tree.ContainerElement, tree.ItemContainer)) {
                    return this.EnumCategoryType.Sibling;
                } else {
                    return this.EnumCategoryType.Related;
                }
            }
        },
        IsCategoryLink:function(els) {
            return (CheckLinkPattern(els) && CheckChars(els) && IsCategoryList(els) && AllowedTags(els));

            function CheckLinkPattern(els) {
                var hrefs = DomService.GetHref(els);

                if (!IsInternalLink(hrefs)) {
                    return false;
                }

                var linkpattern = DomService.GetUrlPattern(hrefs);
                if (linkpattern == null) {
                    return false;
                } else {
                    return true;
                }

                function IsInternalLink(links) {

                    for (var i = 0; i < links.length; i++) {
                        var item = links[i];
                        var itemlink = item.toLowerCase();
                        if (itemlink.indexOf("http://") == 0 || item.indexOf("https://") == 0) {
                            return false;
                        }
                    }
                    return true;
                }

            }

            function CheckChars(els) {
                for (var i = 0; i < els.length; i++) {
                    var el = els[i];

                    if (!DomService.IsMenuCategoryText(el)) {
                        return false;
                    }
                }
                return true;
            }

            function IsCategoryList(els) {

                var commoneparent = DomService.FindCommonParent(els);

                var contains = [];

                for (var i = 0; i < els.length; i++) {
                    var item = els[i];
                    var container = DomService.FindContainer(item, commoneparent);
                    contains.push(container);
                }

                for (var i = 0; i < contains.length; i++) {
                    var each = contains[i];
                    if (!IsCategoryElement(each)) {
                        return false;
                    }
                }

                return true;

                function IsCategoryElement(el) {

                    var allsubs = DomService.GetChildNodes(el);

                    for (var i = 0; i < allsubs.length; i++) {
                        var sub = allsubs[i];
                        if (sub.nodeType == 1) {
                            var subel = sub;
                            if (subel.tagName.toLowerCase() != "a") {

                                var group = TagGroup.GetGroup(subel);

                                if (group == TagGroup.EnumTagGroup.Text || group == TagGroup.EnumTagGroup.Title) {

                                    var eltext = subel.textContent;
                                    eltext = DomService.RemoveAllWhiteSpace(eltext);
                                    if (eltext.indexOf(",") != -1 || eltext.indexOf(";") != -1) {
                                        return false;
                                    }
                                    eltext = eltext.replace('.', '');
                                    if (eltext.length > 4) {
                                        return false;
                                    }
                                } else {
                                    if (!IsCategoryElement(subel)) {
                                        return false;
                                    }
                                }
                            }

                        } else if (sub.nodeType == 3) {
                            if (!DomService.IsEmptyTextNode(sub)) {

                                var text = sub.nodeValue;
                                text = DomService.RemoveAllWhiteSpace(text);

                                if (text.indexOf(",") != -1 || text.indexOf(";") != -1) {
                                    return false;
                                }
                                if (text.indexOf(".") != -1 && text.length > 50) {
                                    return false;
                                }
                            }
                        }
                    }

                    return true;
                }

            }

            function AllowedTags(els) {

                var noallowedtags = [];
                noallowedtags.push("p");
                noallowedtags.push("span");
                noallowedtags.push("i");
                var el = els[0];
                while (el != null && el.tagName.toLowerCase() != "body") {
                    for (var i = 0; i < noallowedtags.length; i++) {
                        var item = noallowedtags[i];
                        if (item == el.tagName.toLowerCase()) {
                            return false;
                        }
                    }
                    el = el.parentElement;
                }

                return true;
            }
        }

    }
}