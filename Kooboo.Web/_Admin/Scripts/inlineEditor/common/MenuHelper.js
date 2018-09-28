function MenuHelper(){
    var DomService = Kooboo.NewDomService,
         TagGroup = Kooboo.TagGroup;
    return {
        Mark : { href: "{href}", anchortext: "{anchortext}", items: "{items}" },
        SubMenuType : { direct: "direct", sibling: "sibling", related: "related", none: "none" },
        CreateMenu:function() {
            return {
                LinkElement: null, // the A tag, like <a>
                ContainerElement: null, // the like <li>
                ItemContainer: null, // The common parent element of subitems... like <ul>
                SubMenuType: "", /// direct, sibling or related...
                chilren: [],
            }
        },
        GetRightLink:function(el){
            // rule 1, it ip a A tag link or Area link. 
            if (el.tagName.toLowerCase() == "a") {
                var href = el.getAttribute("href");
                if (href != null) {
                    return el;
                } else {
                    return null;
                }
            }
            // rule 2 include in a A tag. 
            var parent = el.parentElement;
            while (parent != null && parent.tagName.toLowerCase() != "body") {
                if (parent.tagName.toLowerCase() == "a") {
                    var href = parent.getAttribute("href");
                    if (href != null) {
                        return parent;
                    } else {
                        return null;
                    }
                }
                parent = parent.parentElement;
            }
            return GetSubAlink(el);

            function GetSubAlink(el) {
                for (var i = 0; i < el.children.length; i++) {
                    var item = el.children[i];
                    if (item.nodeType == Node.ELEMENT_NODE) {
                        if (item.tagName.toLocaleLowerCase() == "a") {
                            return item;
                        } else {
                            return GetSubAlink(item);
                        }
                    }
                }
                return null;
            }
        },
        // GetMenuTT:function(ClickedElement){
        //     var menu = this.CreateMenu(),
        //     linkElement = this.GetRightLink(ClickedElement),
        //     linkgroup = DomService.FindSameLinkGroup(linkElement);

        //     menu.ItemContainer = linkgroup.CommonParent;
        //     this.AddSubMenu(linkgroup.Links, linkgroup.CommonParent, menu);
        //     menu.SubMenuType = this.SubMenuType.direct;
        //     menu.ItemContainer = DomService.UpgradeToContainer(menu.ItemContainer);
        //     return menu;

        //     function upgradetocontainer(container) {
        //         if (container.nodeType == 1) {
        //             var tagName = container.tagName.toLowerCase();
        //             if (tagName == "ul" || tagName == "nav") {
        //                 return container;
        //             }
        //         }

        //         var parent = container.parentElement;
        //         if (parent == null || parent.tagName.toLowerCase() == "body") {
        //             return container;
        //         }

        //         var children = DomService.GetChildElements(parent);
        //         if (children.length == 1) {
        //             return upgradetocontainer(parent);
        //         } else {
        //             return container;
        //         }
        //     }
        // },
        // GetClickConvertResult:function(clickedEl){
        //     var result = {};
        //     result.name = "Menu";
        //     result.convertToType = "Menu";

        //     var menu = this.GetMenu(clickedEl);
        //     if (menu != null && menu.chilren.length > 0) {
        //         result.koobooId = menu.ItemContainer.getAttribute("kooboo-id");

        //         var clonedElement = menu.ItemContainer.cloneNode(true);
        //         DomService.RemoveKoobooAttribute(clonedElement);
        //         result.htmlBody =DomService.GetNoCommentOuterHtml(clonedElement);
        //         result.data = this.ConvertToJson(menu);
        //         result.hasResult = true;
        //     }
        //     return result;
        // },
        ConvertToJson:function(menu) {
            var data = {};
    
            if (menu.LinkElement != null) {
    
                var copyel = menu.LinkElement.cloneNode(true);
                DomService.RemoveKoobooAttribute(copyel);
    
                data["url"] = copyel.getAttribute("href");
                data["name"] = copyel.innerHTML;
    
                copyel.setAttribute("href", this.Mark.href);
                copyel.innerHTML = this.Mark.anchortext;
                var linktemplate = copyel.outerHTML
    
                var template = menu.ContainerElement.outerHTML.replace(menu.LinkElement.outerHTML, linktemplate);
    
                if (menu.SubMenuType == this.SubMenuType.direct) {
                    template = template.replace(menu.ItemContainer.outerHTML, this.Mark.items);
                    data["submenutype"] = "direct";
                } else if (menu.SubMenuType == this.SubMenuType.sibling) {
                    template = GetSilbingElementString(menu.ContainerElement, menu.ItemContainer);
                    template = template.replace(menu.LinkElement.outerHTML, linktemplate);
                    template = template.replace(menu.ItemContainer.outerHTML, this.Mark.items);
                    data["submenutype"] = "sibling";
                } else if (menu.SubMenuType == this.SubMenuType.related) {
                    template = menu.ContainerElement.outerHTML.replace(menu.LinkElement.outerHTML, linktemplate);
                    data["submenutype"] = "related";
                }
    
                data["template"] = DomService.RemoveStringKoobooAttribute(template);
            }
    
            if (menu.ItemContainer != null) {
                var subitemstring = GetSubItemsMaxString(menu);
                var itemcontainer = menu.ItemContainer.outerHTML.replace(subitemstring, this.Mark.items);
                data["SubItemContainer"] = DomService.RemoveStringKoobooAttribute(itemcontainer);
            }
    
            var submenus = [];
    
            for (var i = 0; i < menu.chilren.length; i++) {
                var submenu = menu.chilren[i];
    
                var subdata = this.ConvertToJson(submenu);
                submenus.push(subdata);
            }
    
            data["children"] = submenus;
    
            return data;
    
            function GetSubItemsMaxString(menu) {
    
                if (menu.chilren.length == 0) {
                    return null;
                }
    
                var start = menu.chilren[0].ContainerElement;
                var end = menu.chilren[menu.chilren.length-1].ContainerElement;
    
                for (var i = 0; i < menu.chilren.length; i++) {
                    var item = menu.chilren[i];
    
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
    
                if (start==end) {
                    return start.outerHTML;
                }
                var alltext = menu.ItemContainer.outerHTML;
    
                var startindex = alltext.indexOf(start.outerHTML);
                var endindex = alltext.lastIndexOf(end.outerHTML);
                endindex = endindex + end.outerHTML.length;
    
                return alltext.substring(startindex, endindex);
    
            }
    
            function GetSilbingElementString(x, y) {
                if (DomService.GetDepth(y) > DomService.GetDepth(x)) {
                    y = y.parentElement;
                    return GetSilbingElementString(x, y);
                }
    
                var strx = x.outerHTML;
                var stry = y.outerHTML;
                var parent = x.parentElement;
                var all = parent.outerHTML;
    
                var indexx = all.indexOf(strx);
                var indexy = all.indexOf(stry);
    
                var result = all.substring(indexx, indexy + stry.length);
    
                return result;
            }
        },
        GetClickConvertResult:function(el){
            var menu=this.GetMenu(el);

            if(menu && !this.isContainerClickEl(menu.chilren,el)){
                menu=null;
            }
            var result = {};

            if (menu != null && menu.chilren.length > 0) {
                result.koobooId= $(menu.ItemContainer).attr("kooboo-id");
                result.editElement = menu.ItemContainer;
                var cloneItemContainer=menu.ItemContainer.cloneNode(true);
                DomService.RemoveKoobooAttribute(cloneItemContainer);
                result.htmlBody =DomService.GetNoCommentOuterHtml(cloneItemContainer);
                //result.htmlBody =DomService.GetNoCommentOuterHtml(menu.ItemContainer);
                result.data = this.ConvertToJson(menu);
                result.hasResult = true;
            }
            return result;
        },
        GetMenu:function(el){
            var p = el,
                body = el.ownerDocument.body,
                menu,
                maxlevel=5,
                level=0;
            
            do {
                menu = this.GetMenuWithinContainer(p);
                if(!menu)
                    p = p.parentElement;
                    level++;
            } while (menu == null && p && p.tagName && body.contains(p)&& level<maxlevel)

            return menu;
            
        },
        isContainerClickEl:function(chilren,el){
            var isContain=false;
            for(var i=0;i<chilren.length;i++){
                var linkElement=chilren[i].LinkElement;
                if(DomService.IsParentSubElement(el,linkElement)||el==linkElement){
                    isContain=true;
                    break;
                }
                isContain=this.isContainerClickEl(chilren[i].chilren,el);
                if(isContain){
                    break;
                }
            }
            return isContain;
        },
        GetMenuWithinContainer:function(menuContainer) {
            var self = this;
            var links = findMenuLinks(menuContainer);
            if (links != null && links.length > 1) {
                var parent = DomService.FindCommonParent(links);
                var menu = this.CreateMenu();
                menu.ItemContainer = parent;
                this.AddSubMenu(links, parent, menu);
                menu.SubMenuType = this.SubMenuType.direct;
                menu.ItemContainer=DomService.UpgradeToContainer(menu.ItemContainer);
                //menu.ItemContainer = upgradetocontainer(menu.ItemContainer, menuContainer);
                return menu;
            }
            return null;
    
            // function upgradetocontainer(container, layout) {
    
            //     var parent = container.parentElement;
            //     if (parent == null || parent.isSameNode(layout)) {
            //         return container;
            //     }
                 
            //     var children = DomService.GetChildElements(parent);
            //     if (children.length == 1) {
            //         return upgradetocontainer(parent, layout);
            //     } else {
            //         return container;
            //     }
            // }
    
            function findMenuLinks(container) {
                var alllinks = container.getElementsByTagName("a");
                var groupby = _.groupBy(alllinks, function(o) { return DomService.GetPath(o); });
    
                var sorted = _.sortBy(groupby, function(item, key) { return DomService.GetDepth(item[0]); });
    
    
                for (var i = 0; i < sorted.length; i++) {
                    var item = sorted[i];
    
                    if (self.IsMenuLink(item)) {
                        return item;
                    }
                }
                return null;
            }
        },
        FindSimilarLinks:function(linkElement, nearestCommonParent){
            var Chain = DomService.GetChain(linkElement, nearestCommonParent);
            Chain.reverse();

            var ElementCol = DomService.GetChildElements(nearestCommonParent);
            var SubList = [];

            for(var i=0;i<Chain.length;i++){
                var ChainItem=Chain[i];
                for(var j=0;j<ElementCol.length;j++){
                    var el=ElementCol[j];
                    if (DomService.IsSimilarElement(ChainItem, el)) {
                        var childElements=DomService.GetChildElements(el);
                        for(var k=0;k<childElements.length;k++){
                            SubList.push(subitem);
                        }
                    }
                }
            }

            var Result = [];

            for(var i=0;i<ElementCol.length;i++){
                if (DomService.IsSimilarElement(linkElement, item)) {
                    Result.push(item);
                }
            }
            return Result;
        },
        AddSubMenu:function(Links, CommonParent, ParentMenu) {
            var count = Links.length;
            var SubMenus = [];
    
            for (var i = 0; i < count; i++) {
    
                var SubMenu = this.CreateMenu();
                var link = Links[i];
                SubMenu.LinkElement = link;
                SubMenu.ContainerElement = DomService.FindContainer(link, CommonParent);
                SubMenus.push(SubMenu);
            }
    
            var submenulinks = [];
    
            /// get directsublinks.
            for (var i = 0; i < count; i++) {
                var submenu = SubMenus[i],
                    sublinks = [];
                sublinks = this.GetDirectSubMenuLinks(submenu.LinkElement, submenu.ContainerElement);
                if (sublinks != null && sublinks.length > 0) {
                    var sublinkstore = {};
                    sublinkstore.Index = i;
                    sublinkstore.ELementList = sublinks;
                    submenulinks.push(sublinkstore);
                    submenu.SubMenuType = this.SubMenuType.direct;
                }
            }
    
            // getting siblings sublinks..
            if (submenulinks.length == 0) {
    
                for (var i = 0; i < count; i++) {
                    var submenu = SubMenus[i],
                        sublinks = [];
    
                    var nextcontainer;
                    if (i < count - 1) {
                        nextcontainer = SubMenus[i + 1].ContainerElement;
                    } else {
                        nextcontainer = null;
                    }
    
                    sublinks = this.GetSiblingSubMenuLinks(submenu.LinkElement, submenu.ContainerElement, nextcontainer, CommonParent);
                    if (sublinks != null && sublinks.length > 0) {
                        var sublinkstore = {};
                        sublinkstore.Index = i;
                        sublinkstore.ELementList = sublinks;
                        submenulinks.push(sublinkstore);
                        submenu.SubMenuType = this.SubMenuType.sibling;
                    }
                }
            }
    
            ///get by sub menu links position.
            if (submenulinks.length == 0) {
    
                for (var i = 0; i < count; i++) {
                    var submenu = SubMenus[i];
                    var sublinks = [];
                    //  sublinks = this.GetSubMenuLinksByPosition(submenu.LinkElement, submenu.ContainerElement, CommonParent);
                    sublinks = this.GetSubMenuLinksByRelated(submenu.LinkElement, submenu.ContainerElement);
                    if (sublinks != null && sublinks.length > 0) {
                        var sublinkstore = {};
                        sublinkstore.Index = i;
                        sublinkstore.ELementList = sublinks;
                        submenulinks.push(sublinkstore);
                        submenu.SubMenuType = this.SubMenuType.related;
                    }
                }
            }
    
            /// parse submenu of submenu.
            if (submenulinks.length > 0) {
                for (var i = 0; i < count; i++) {
    
                    var submenu = SubMenus[i];
                    var onemenusublinks = _.find(submenulinks, function(o) { return o.Index == i; });
                    if (onemenusublinks != null && onemenusublinks.ELementList != null && onemenusublinks.ELementList.length > 0) {
                        var subcommonparent = DomService.FindCommonParent(onemenusublinks.ELementList);
                        submenu.ItemContainer = subcommonparent;
                        
                        if(!existParentElement(subcommonparent,onemenusublinks.ELementList)){
                            this.AddSubMenu(onemenusublinks.ELementList, subcommonparent, submenu);
                        }
                        
                    }
                }
            }
            ParentMenu.ItemContainer = CommonParent;
            // append to ParentMenu.
            for (var i = 0; i < SubMenus.length; i++) {
                var menuitem = SubMenus[i];
                ParentMenu.chilren.push(menuitem);
            }

            function existParentElement(parentElement,elementList){
                var exist=false;
                if(parentElement && elementList && elementList.length>0){
                    for(var i=0;i<elementList.length;i++){
                        var element=elementList[i];
                        if(element.isEqualNode(parentElement)){
                            exist=true;
                            break;
                        }
                    }
                }
                return exist;
            }
        },
        IsSubMenuOfParentByPosition:function(container, subLink){
            var sublinkposition = subLink.getBoundingClientRect();
            var containerposition = container.getBoundingClientRect();

            if (sublinkposition.top < containerposition.top || sublinkposition.left < containerposition.left) {
                return false;
            }

            var distance = this.GetPossibleSubMenuLinkDistance(container, subLink);

            return (distance < 20);
        },
        GetPossibleSubMenuLinkDistance:function(container, sublink){
            var sublinkposition = sublink.getBoundingClientRect();
            var containerposition = container.getBoundingClientRect();

            var comparecontainer;

            var linkgroup = DomService.FindSameLinkGroup(sublink);

            if (linkgroup != null) {
                if (linkgroup.CommonParent != null) {
                    comparecontainer = linkgroup.CommonParent;
                }
            }
            if (comparecontainer == null) {
                comparecontainer = DomService.UpgradeToContainer(sublink);
            }

            return DomService.GetDistance(container, comparecontainer);
        },
        GetDirectSubMenuLinks:function(linkElement, LinkContainer) {
            var links = DomService.GetSubElementsByTag(LinkContainer, "a");

            if (links == null || links.length == 0) {
                return;
            }

            if (links.length == 1 && links[0].isEqualNode(linkElement)) {
                return;
            }

            links = DomService.RemoveElement(links, linkElement);

            var CloseElement;

    
            for (var i = 0; i < links.length; i++) {
                var element = links[i];
                if (CloseElement == null) {
                    CloseElement = element;
                } else {
                    if (DomService.GetDepth(CloseElement) > DomService.GetDepth(element)) {
                        CloseElement = element;
                    }
                }
            }

            if (CloseElement == null) {
                return;
            }

            var firstlevellinks = [];

            for (var i = 0; i < links.length; i++) {
                var link = links[i];
                if (DomService.IsSimilarElement(CloseElement, link)) {
                    firstlevellinks.push(link);
                }
            }

            return firstlevellinks;
        },
        GetSubMenuLinksByPosition:function(linkElement, LinkContainer, ParentContainer){
            // find links that next to the position of the current container.. linkElement. 
            var MenuPositionContainer = linkElement;
            if (LinkContainer != null) {
                MenuPositionContainer = LinkContainer;
            }
            if (ParentContainer != null) {
                MenuPositionContainer = ParentContainer;
            }

            var ParentRect = MenuPositionContainer.getBoundingClientRect();
            var MenuRect = linkElement.getBoundingClientRect();

            $(linkElement).trigger("mouseover");

            var alllinks = linkElement.ownerDocument.links;

            var candicates = [];

            for (var i = 0; i < alllinks.length; i++) {

                var link = alllinks[i];

                if (link != null && !DomService.IsSimilarElement(link, linkElement)) {

                    /// right now, only accept sub menu of down/right position....
                    var linkrect = link.getBoundingClientRect();

                    if (linkrect.top < ParentRect.top || linkrect.left < ParentRect.left) {
                        continue;
                    }

                    if (linkrect.top < MenuRect.top || linkrect.left < MenuRect.left) {
                        continue;
                    }

                    var distance = this.GetPossibleSubMenuLinkDistance(MenuPositionContainer, link);

                    if (distance < 30) {
                        var newcandadate = {};
                        newcandadate.Index = distance;
                        newcandadate.Element = link;
                        candicates.push(newcandadate);
                    }
                }

            }

            if (candicates != null && candicates.length > 0) {

                while (true) {

                    var item = DomService.FindFirstItemByIndex(candicates, true);
                    if (item == null) {
                        return null;
                    }

                    var linkgroups = DomService.FindSameLinkGroup(item.Element);
                    if (linkgroups != null && linkgroups.Links != null && linkgroups.Links.length > 0) {
                        return linkgroups.Links;
                    }
                    else {
                        break;
                    }
                }

            }

            return null;
        },
        GetSiblingSubMenuLinks:function(linkElement, linkContainer, nextContainer, ParentContainer) {
            var links = DomService.GetElementsInBetween(linkContainer, nextContainer, ParentContainer, "a");
    
            if (links == null || links.length == 0 || (links.length == 1 && links[0].isEqualNode(linkElement))) {
                return null;
            }
    
            var CloseElement;
    
            for (var i = 0; i < links.length; i++) {
                var element = links[i];
                if (CloseElement == null) {
                    CloseElement = element;
                } else {
                    if (DomService.GetDepth(CloseElement) > DomService.GetDepth(element)) {
                        CloseElement = element;
                    }
                }
            }
    
            var firstlevellinks = [];
            for (var i = 0; i < links.length; i++) {
                var link = links[i];
                if (DomService.IsSimilarElement(CloseElement, link)) {
                    firstlevellinks.push(link);
                }
            }
            return firstlevellinks;
        },
        GetSubMenuLinksByRelated:function(linkElement, linkContainer) {
            var self = this;
            var result = [];
            var linkattribute = CleanAttribute(DomService.GetExtraAttributes(linkElement));
            var containerattribute = CleanAttribute(DomService.GetExtraAttributes(linkContainer));
    
    
            for (var i = 0; i < containerattribute.length; i++) {
                var item = containerattribute[i];
                linkattribute.push(item);
            }
    
            if (linkattribute.length == 0) {
                return result;
            }
            var values = [];
            for (var i = 0; i < linkattribute.length; i++) {
                var att = linkattribute[i];
                values.push(att.Value);
            }
    
            var elements = FindElementWithAttributeValue(values, linkElement.ownerDocument);
    
            for (var i = 0; i < elements.length; i++) {
                var element = elements[i];
                if (isSameOrIncludedElement(linkElement, linkContainer, element)) {
                    continue;
                }
                var menulink = findMenuLinks(element);
                if (menulink != null && menulink.length > 0) {
                    return menulink;
                }
            }
            return null;
    
            function isSameOrIncludedElement(linkElement, linkContainer, element) {
                return element.isEqualNode(linkElement) ||
                    element.isEqualNode(linkContainer) ||
                    $(element).find(linkElement).length > 0 ||
                    $(element).find(linkContainer).length > 0;
            }
    
            function FindElementWithAttributeValue(values, doc) {
    
                var result = [];
    
                _find(doc.body, values, result);
    
                return result;
    
                function _find(el, values, result) {
    
                    var attributes = CleanAttribute(DomService.GetExtraAttributes(el))
    
    
                    for (var i = 0; i < attributes.length; i++) {
                        var item = attributes[i];
                        if (values.indexOf(item.Value) != -1) {
                            result.push(el);
                            return;
                        }
                    }
    
                    var children = DomService.GetChildElements(el);
                    for (var i = 0; i < children.length; i++) {
                        var child = children[i];
                        _find(child, values, result);
                    }
    
                }
    
            }
    
            function CleanAttribute(values) {
    
                var attValues = [];
                if (values != null || values.length > 0) {
                    for (var i = 0; i < values.length; i++) {
                        var item = values[i];
                        if (item.Key != "class" &&
                            item.Key != "style" &&
                            item.Key != "script" &&
                            item.Key != "javascript" &&
                            item.Key != "id") {
                            attValues.push(item);
                        }
                    }
                }
                return attValues;
            }
    
            function findMenuLinks(container) {
    
                var alllinks = container.getElementsByTagName("a");
    
                var groupby = _.groupBy(alllinks, function(o) { return DomService.GetPath(o); });
    
                var sorted = _.sortBy(groupby, function(item, key) { return DomService.GetDepth(item[0]); });
    
    
                for (var i = 0; i < sorted.length; i++) {
                    var item = sorted[i];
    
                    if (self.IsMenuLink(item)) {
                        return item;
                    }
                }
                return null;
            }
        },
        IsMenuLink:function(els) {
            if (els == null || els.length < 2) {
                /// if it is the only home page link, yes it can be menu...
                if (els.length == 1) {
                    var el = els[0];
                    var href = el.getAttribute("href");
                    if (href) {
                        var link = href.toLowerCase();
                        if (link.indexOf("default") > -1 || link.indexOf("index") > -1 || link.indexOf("home") > -1) {
                            var img = el.getElementsByTagName("img");
                            if (img == null || img.length == 0) {
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
    
    
            if (!IsMenuChars(els)) {
                return false;
            }
    
            if (!IsPositionAlign(els)) {
                return false;
            }
    
            if (!NoContainsContent(els)) {
                return false;
            }
    
            if (!LinkPattern(els)) {
                return false;
            }
    
            if (!DomService.HasSameParent(els)) {
                return false;
            }
    
            return true;
    
    
            function IsMenuChars(els) {
                for (var i = 0; i < els.length; i++) {
                    var el = els[i];
    
                    if (!DomService.IsMenuCategoryText(el)) {
                        return false;
                    }
                }
                return true;
            }
    
            function IsPositionAlign(els) {
                return DomService.IsPositionAlign(els);
            }
    
            // this is to check that menu should not contains a lot of text...
            function NoContainsContent(els) {
    
                for (var i = 0; i < els.length; i++) {
                    var item = els[i];
    
                    var parent = item.parentElement;
    
                    if (parent != null) {
                        var parentsubs = DomService.GetChildElements(parent);
    
                        for (var j = 0; j < parentsubs.length; j++) {
                            var sub = parentsubs[j];
                            if (!sub.isEqualNode(item)) {
                                var tagroup = TagGroup.GetGroup(sub);
                                if (tagroup == TagGroup.EnumTagGroup.Title) {
                                    return false;
                                }
                                if (tagroup == TagGroup.EnumTagGroup.Text &&
                                    !DomService.IsMenuCategoryText(sub)) {
    
                                    var text = DomService.RemoveAllWhiteSpace(sub.innerText);
                                    if (text != null && text != "") {
                                        return false;
                                    }
                                }
                            }
                        }
                    }
                }
    
                return true;
            }
    
            function LinkPattern(els) {
    
                var links = [];
    
    
                for (var i = 0; i < els.length; i++) {
                    var item = els[i];
                    var href = item.getAttribute("href");
                    if (href && href != "#") {
                        links.push(href);
                    }
                }
    
                // if (ExternalHttpLinks(links)) {
                //     debugger;
                //     return false;
                // }
    
                var linksegs = [];
                for (var i = 0; i < links.length; i++) {
                    var each = links[i];
                    var ones = DomService.GetUrlSegments(each);
                    linksegs.push(ones);
                }
    
                var samechain = DomService.GetSameChain(linksegs);
    
                if (samechain == null || samechain.length == 0) {
                    return true;
                } else if (samechain.length == 1) {
                    var prefix = samechain[0];
                    var path = DomService.GetPath(els[0]);
                    if (IsDocumentLinkPrefix(els[0].ownerDocument, prefix, path)) {
                        return true;
                    } else {
                        return false;
                    }
                } else {
                    return false;
                }
    
                function ExternalHttpLinks(links) {
    
                    var httplinks = _.filter(links, function(o) {
                        return o ? (o.toLowerCase().indexOf("http://") > -1 || o.toLowerCase().indexOf("https://") > -1) : false;
                    });
    
                    if (httplinks == null || httplinks.length == 0) {
                        return false;
                    }
    
                    if (httplinks.length == links.length) {
                        return true;
                    }
    
                    /// check partial http
                    if (httplinks == null || httplinks.length < 2) {
                        return true;
                    } else {
                        return false;
                    }
                }
    
                function HasSamePath(links) {
    
                    if (links.length == 0) {
                        return false;
                    }
    
                    if (links.length == 1) {
                        return true;
                    }
    
                    if (links.length == 2) {
                        var ones = DomService.GetUrlSegments(links[0]);
                        var twos = DomService.GetUrlSegments(links[1]);
                        var samecount = SamePathCount(ones, twos);
                        if (samecount > 0) {
                            return true;
                        }
                        return false;
                    }
    
                    var samecount = -1;
                    var previous;
                    var next;
    
    
                    for (var i = 0; i < links.length; i++) {
                        var link = links[i];
                        if (previous == null) {
                            previous = DomService.GetUrlSegments(link);
                        } else {
                            next = DomService.GetUrlSegments(link);
                            if (samecount == -1) {
                                samecount = SamePathCount(previous, next);
                                if (samecount == 0) {
                                    return false;
                                }
                            } else {
                                var newsamecount = SamePathCount(previous, next);
                                if (samecount != newsamecount) {
                                    return false;
                                }
                            }
                            previous = next;
                        }
                    }
    
                    return true;
    
                    function SamePathCount(one, two) {
                        if (one == null || one.length == 0) {
                            return 0;
                        }
                        if (two == null || two.length == 0) {
                            return 0;
                        }
                        var onelen = one.length;
                        var twolen = two.length;
                        var count = onelen;
                        if (twolen < onelen) {
                            count = twolen;
                        }
    
                        var samepathcount = 0;
    
                        for (var i = 0; i < count; i++) {
                            if (one[i] && two[i]) {
                                if (!DomService.IsSameValue(one[i], two[i])) {
                                    break;
                                } else {
                                    samepathcount = samepathcount + 1;
                                }
                            }
                        }
    
                        return samepathcount;
                    }
                }
    
                function IsDocumentLinkPrefix(doc, prefix, exclPath) {
                    
                    var alllinks = doc.links;
                    var otherlinks = _.filter(alllinks, function(o) {
                        return (DomService.GetPath(o) != exclPath);
                    });
    
                    var lowerprefix = prefix.toLowerCase();
    
                    if (otherlinks == null || otherlinks.length == 0) {
                        return true;
                    }
    
                    var withprefix = _.filter(otherlinks, function(o) {
                        var href = o.getAttribute("href");
                        if (href) {
                            if (href.indexOf(lowerprefix) > -1) {
                                return true;
                            }
                        }
                        return false;
                    });
    
                    if (withprefix == null || withprefix.length == 0) {
                        return false;
                    }
    
                    if (withprefix.length * 2 < otherlinks.length) {
                        return false;
                    } else {
                        return true;
                    }
                }
            }
        }
 
    }
}