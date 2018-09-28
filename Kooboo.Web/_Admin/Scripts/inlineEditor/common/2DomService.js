function NewDomService(){
    var DomService = (function() {
        var DomService = function() {};
        var TagGroup = Kooboo.TagGroup;
        DomService.GetSubElementsByTag = function(El, tagName) {
            var result = [];
            var elements = El.getElementsByTagName(tagName);
            for (var i = 0; i < elements.length; i++) {
                result.push(elements[i]);
            }
            return result;
        };
        // get the elements that has Id defined. 
        DomService.GetElementsHasId = function(doc) {
            var col = [];
            _GetElementHasId(doc.body, col);
            return col;

            function _GetElementHasId(Element, Col) {

                var subElements = DomService.GetChildElements(Element);


                for (var i = 0; i < subElements.length; i++) {
                    var item = subElements[i];

                    if (item.id != "") {
                        Col.push(item);
                    }
                    _GetElementHasId(item, Col);
                }
            }
        };

        DomService.GetElementsHasClass = function(doc) {
            var col = [];
            _GetElementsHasClass(doc.body, col);
            return col;

            function _GetElementsHasClass(Element, Col) {

                var subElements = DomService.GetChildElements(Element);
                for (var i = 0; i < subElements.length; i++) {
                    var item = subElements[i];

                    if (item.className != "") {
                        Col.push(item);
                    }

                    _GetElementsHasClass(item, Col);
                }
            }
        };
        DomService.GetElementsByFilter = function(doc, filter) {
            var col = [];
            _getelements(doc.body, col);
            return col;

            function _getelements(Element, Col) {

                var subElements = DomService.GetChildElements(Element);

                for (var i = 0; i < subElements.length; i++) {
                    var item = subElements[i];
                    if (filter(item)) {
                        Col.push(item);
                    }
                    _getelements(item, Col);
                }
            }
        };
        ///Find the element that is parents of linkElement and right under parent. 
        DomService.FindContainer = function(linkElement, commonParent) {
            var parent = linkElement;
            if (parent.isSameNode(commonParent)) {
                return parent;
            }
            while (parent.parentElement != null && !parent.parentElement.isSameNode(commonParent)) {
                parent = parent.parentElement;
            }
            return parent;
        };
        DomService.GetChildElements = function(Element) {
            var Children = [];
            for (var i = 0; i < Element.childNodes.length; i++) {
                if (Element.childNodes[i].nodeType == 1) {
                    Children.push(Element.childNodes[i]);
                }
            }
            return Children;
        };
        DomService.GetAttributeNames = function(el) {
            var result = [];
            for (var item in el.attributes) {
                if (el.attributes.hasOwnProperty(item)) {
                    var name = el.attributes[item].name;
                    if (name != null) {
                        var lowername = name.toLowerCase();
                        if (lowername.indexOf("kooboo") == -1 && lowername.indexOf("kb-") == -1 && result.indexOf(lowername) == -1) {
                            result.push(lowername);
                        }
                    }
                }
            }
            return result;
        };
        /// get the string representative (to be hash) of sub nodes. 
        DomService.GetChildNodesHash = function(node) {
            var previousText = false;
            var result = "";

            for (var i = 0; i < node.childNodes.length; i++) {
                var child = node.childNodes[i];
                if (child.nodeType == 1) {
                    var element = child;
                    if (TagGroup.GetGroup(element) == TagGroup.EnumTagGroup.Text) {
                        if (!previousText) {
                            result = result + "|" + "Text";
                            previousText = true;
                        }
                    } else {
                        result = result + "|" + element.tagName;
                        previousText = false;
                    }
                } else {

                    if (!this.IsEmptyTextNode(node)) {

                        if (node.nodeType == 3) {
                            if (!previousText) {
                                result = result + "|" + "Text";
                                previousText = true;
                            }
                        } else {

                            result = result + "|" + node.nodeName;
                            previousText = false;
                        }

                    }
                }

            }
            return result;
        };
        DomService.GetChildNodes = function(Element) {
            var Children = [];
            for (var i = 0; i < Element.childNodes.length; i++) {
                Children.push(Element.childNodes[i]);
            }
            return Children;
        };
        DomService.GetChain = function(Sub, Parent, IncludeSelf) {
            var Chain = [];
            if (IncludeSelf) {
                Chain.push(Sub);
            }
            var directParent = Sub.parentElement;

            while (directParent != null && !directParent.isSameNode(Parent) && directParent.tagName.toLowerCase() != "body") {
                Chain.push(directParent);
                directParent = directParent.parentElement;
            }
            return Chain;
        };
        DomService.IsSimilarElement = function(x, y) {
            if (x.tagName != y.tagName) {
                return false;
            }

            if (x.isSameNode(y)) {
                return true;
            }

            // elements must be vertical or horizontal align.
            if (Math.abs(x.offsetLeft - y.offsetLeft) > 5 && Math.abs(x.offsetTop - y.offsetTop) > 5) {
                return false;
            }

            var xpath = this.GetPath(x);
            var ypath = this.GetPath(y);

            return (xpath == ypath);
        };
        DomService.IsNextSiblingElement = function(x, y) {
            var parentx = x.parentElement;
            var parenty = y.parentElement;
            if (!parentx.isSameNode(parenty)) {
                return false;
            }

            var xindex = this.GetSiblingIndex(x);
            var yindex = this.GetSiblingIndex(y);
            if (Math.abs(xindex - yindex) == 1) {
                return true;
            }
            return false;
        };
        DomService.FindNextSimilar = function(Parent, Container) {
            var elements = DomService.GetChildElements(Parent);
            //for of
            for (var i = 0; i < elements.length; i++) {
                var element = elements[i];
                if (this.IsSimilarElement(Container, element) && !Container.isSameNode(element)) {
                    return element;
                }
            }

            return null;
        };
        DomService.GetPath = function(node) {
            var element;
            if (node.nodeType == 1) {
                element = node;
            } else {
                element = node.parentElement;
            }
            var path = "";
            var chain = this.GetChain(element, element.ownerDocument.body);

            chain.reverse();

            var path;
            //for of
            for (var i = 0; i < chain.length; i++) {
                var item = chain[i];
                path = path + item.tagName.toLowerCase() + ">";
            }

            path = path + element.tagName.toLowerCase();

            if (node.nodeType != 1) {

                if (node.nodeType == 3) {
                    path = path + ">Text";
                } else {
                    path = path + ">" + node.nodeName;
                }
            }
            return path;
        };
        DomService.GetHref = function(els) {
            var hrefs = [];
            //for of
            for (var i = 0; i < els.length; i++) {
                var item = els[i];
                var href = item.getAttribute("href");
                hrefs.push(href);
            }
            return hrefs;
        };
        DomService.GetTagGroup = function(node) {
            if (node.nodeType == 1) {
                var element = node;
                return TagGroup.GetGroup(element);
            } else {
                return TagGroup.EnumTagGroup.Undefined;
            }
        };
        DomService.GetDepth = function(node) {
            var depthcount = 0;

            var parent = node.parentElement;
            while (parent != null && parent.tagName.toLowerCase() != "html") {
                depthcount += 1;
                parent = parent.parentElement;
            }

            return depthcount;
        };
        DomService.RemoveElement = function(Elements, item) {
            var col = [];
            //for of
            for (var i = 0; i < Elements.length; i++) {
                var element = Elements[i];
                if (!element.isSameNode(item)) {
                    col.push(element);
                }
            }
            return col;
        };
        DomService.HasElement = function(Elements, item) {
            //for of
            for (var i = 0; i < Elements.length; i++) {
                var element = Elements[i];
                if (element.isEqualNode(item)) {
                    return true;
                }
            }
            return false;
        };
        /// find the first match elements. 
        DomService.FindElement = function(Elements, key, value) {
            //for of
            for (var i = 0; i < Elements.length; i++) {
                var item = Elements[i];
                var itemvalue = DomService.GetTextOrAttribute(item, key);
                if (itemvalue == value) {
                    return item;
                }
            }
        };

        DomService.FindElements = function(Elements, key, value) {
            var result = [];
            //for of
            for (var i = 0; i < Elements.length; i++) {
                var item = Elements[i];
                var itemvalue = DomService.GetTextOrAttribute(item, key);
                if (itemvalue == value) {
                    result.push(item);
                }
            }
            return result;
        };
        DomService.GetTextOrAttribute = function(Element, key) {
            var inputkey = key.toLowerCase();

            if (inputkey == "id") {
                return Element.id;
            } else if (inputkey == "innertext" || inputkey == "text") {
                return Element.innerText;
            } else if (inputkey == "innerhtml" || inputkey == "html") {
                return Element.innerHTML;
            } else if (inputkey == "outerhtml") {
                return Element.outerHTML;
            } else if (inputkey == "class" || inputkey == "classname") {
                return Element.className;
            } else if (inputkey == "tagname" || inputkey == "tag") {
                return Element.tagName.toLowerCase();
            } else {

                var attvalue = Element.getAttribute(inputkey);
                if (attvalue != null && attvalue != "") {
                    return attvalue;
                }

                var stylevalue = Element.style.getPropertyValue(inputkey);
                if (stylevalue != null && stylevalue != "") {
                    return stylevalue;
                }

                return null;
            }
        };
        DomService.FindCommonParent = function(Elements) {
            if (Elements.length == 0) {
                return null;
            } else if (Elements.length == 1) {
                return this.UpgradeToContainer(Elements[0]);
            } else {

                var parent;
                //for of
                for (var i = 0; i < Elements.length; i++) {
                    var item = Elements[i];

                    if (parent == null) {
                        parent = item.parentElement;
                    } else {
                        parent = this.FindParent(parent, item);
                    }
                }

                return parent;

            }
        };
        DomService.FindParent = function(x, y) {
            var xdepth = this.GetDepth(x);
            var ydepth = this.GetDepth(y);

            if (xdepth == ydepth) {
                return this._FindParent(x, y);
            } else {
                if (xdepth > ydepth) {
                    x = x.parentElement;
                    return this.FindParent(x, y);
                } else {
                    y = y.parentElement;
                    return this.FindParent(x, y);
                }
            }
        };
        DomService._FindParent = function(x, y) {
            if (x == null || y == null) {
                return null;
            }
            if (x.isEqualNode(y)) {
                return x;
            } else {
                x = x.parentElement;
                y = y.parentElement;

                return this._FindParent(x, y);
            }
        };
        DomService.GetElementsInBetween = function(x, y, Container, tagName) {
            var doc = x.ownerDocument;

            var xindex = this.GetSiblingIndex(x);

            if (y != null) {
                var yindex = this.GetSiblingIndex(y);
                if (xindex > yindex) {
                    return this.GetElementsInBetween(y, x, Container, tagName);
                }
            }

            var treewalker = doc.createTreeWalker(Container);
            treewalker.currentNode = x;

            if (y == null) {
                y = Container;
            }

            var linkcol = [];

            var next = treewalker.nextSibling();

            while (next != null && !next.isSameNode(y)) {

                if (next.nodeType == 1) {
                    var el = next;
                    var links = this.GetSubElementsByTag(el, tagName);
                    //for of
                    for (var i = 0; i < links.length; i++) {
                        var link = links[i];
                        linkcol.push(link);
                    }
                }
                next = treewalker.nextSibling();
            }

            return linkcol;
        };
        DomService.IsNearByElement = function(x, y) {
            var distance = this.GetDistance(x, y);
            return distance < 16;
        };
        DomService.FindSameLinkGroup = function(linkElement) {
            var ParentElement = linkElement.parentElement;

            var linkcol = [];

            var found = false;

            while (ParentElement != null && ParentElement.tagName.toLowerCase() != "body") {

                _.remove(linkcol);

                var links = ParentElement.getElementsByTagName("a");

                if (links != null && links.length > 1) {
                    for (var i = 0; i < links.length; i++) {

                        if (this.IsSimilarElement(links[i], linkElement)) {

                            var link = links[i];
                            if (!DomService.HasElement(linkcol, link)) {
                                linkcol.push(links[i]);
                            }

                            if (!links[i].isSameNode(linkElement)) {
                                found = true;
                            }
                        }
                    }
                }

                if (found) {
                    break;
                }

                ParentElement = ParentElement.parentElement;
            }

            if (found) {
                var linkgroup = {};
                linkgroup.CommonParent = ParentElement;
                linkgroup.Links = linkcol;
                return linkgroup;
            } else {
                return null;
            }
        };
        ///if a container that does not have any other elements, then upgrade it.
        DomService.UpgradeToContainer = function(container) {
            if (container.nodeType == 1) {
                var tagName = container.tagName.toLowerCase();
                if (tagName == "ul" || tagName == "nav") {
                    return container;
                }
            }

            var parent = container.parentElement;
            if (parent == null || parent.tagName.toLowerCase() == "body") {
                return container;
            }

            var subelments = this.GetChildElements(parent);
            if (subelments.length == 1) {
                return this.UpgradeToContainer(parent);
            } else {
                return container;
            }
        };
        DomService._GetDistance = function(x, y) {
            if (x.left > y.left) {
                return this._GetDistance(y, x);
            }

            var HorizontalDistance = 0;
            var VerticalDistance = 0;

            if (y.left > x.right) {
                HorizontalDistance = y.left - x.right;
            }

            if (x.top < y.top) {

                if (y.top > x.bottom) {
                    VerticalDistance = y.top - x.bottom;
                }
            } else {
                if (x.top > y.bottom) {
                    VerticalDistance = x.top = y.bottom;
                }
            }

            var distance = Math.pow(VerticalDistance, 2) + Math.pow(HorizontalDistance, 2);
            return Math.sqrt(distance);
        };
        DomService.GetDistance = function(x, y) {
            var xrect = x.getBoundingClientRect();
            var yrect = y.getBoundingClientRect();
            return this._GetDistance(xrect, yrect);
        };
        DomService.FindFirstItemByIndex = function(list, Ascending) {
            if (list == null || list.length == 0) {
                return null;
            }
            var sorted = _.sortBy(list, "Index");

            if (Ascending) {
                return sorted[0];
            } else {
                return sorted[sorted.length - 1];
            }
        };
        DomService.GetSiblingIndex = function(sub) {
            if (sub == null || sub.parentElement == null) {
                return -1;
            }
            var parent = sub.parentElement;
            var subs = this.GetChildElements(parent);
            for (var i = 0; i < subs.length; i++) {
                if (subs[i].isEqualNode(sub)) {
                    return i;
                }
            }
            return -1;
        };
        DomService.IsWhiteSpace = function(input) {
            if (input == null || input == "") {
                return true;
            }
            var result = $.trim(input);
            if (input == null || input == "") {
                return true;
            }
            if (/^\s*$/.test(input)) {
                return true;
            }
            return false;
        };
        DomService.IsEmptyTextNode = function(node) {
            if (node.nodeType == 3) {
                return this.IsWhiteSpace(node.nodeValue);
            }
            return false;
        };
        DomService.IsPositionAlign = function(elements) {
            var RectList = [];
            //for of
            for (var i = 0; i < elements.length; i++) {
                var item = elements[i];
                var clientrect = item.getBoundingClientRect();
                if (clientrect == null) {
                    return false;
                }
                if (!this.IsAlignedPosition(RectList, clientrect)) {
                    return false;
                }
                RectList.push(clientrect);
            }
            return true;
        };
        DomService.IsAlignedPosition = function(RectList, Next) {
            if (Next == null) {
                return false;
            }

            if (RectList.length == 0) {
                return true;
            }
            var threshold = 10;
            //for of
            for (var i = 0; i < RectList.length; i++) {
                var item = RectList[i];
                if (item == null) {
                    return false;
                }
                var left = Math.abs(item.left - Next.left);
                var right = Math.abs(item.right - Next.right);
                var top = Math.abs(item.top - Next.top);

                if (left < threshold || right < threshold || top < threshold) {
                    return true;
                }
            }
            return false;
        };
        DomService.IsPositionAlignByRect = function(RectList) {
            var newrect = [];
            //for of
            for (var i = 0; i < RectList.length; i++) {
                var item = RectList[i];
                if (!this.IsAlignedPosition(newrect, item)) {
                    return false;
                }
                newrect.push(item);
            }
            return true;
        };
        DomService.ConvertToNodeArray = function(nodes) {
            var result = [];
            var count = nodes.length;
            for (var i = 0; i < count; i++) {
                var item = nodes[i];
                result.push(item);
            }
            return result;
        };
        DomService.ConvertToFlatKoobooElements = function(nodes) {
            var result = [];
            this._MakeFlat(nodes, result);
            return result;
        };
        DomService._MakeFlat = function(nodes, flats) {
            var count = nodes.length;
            if (count == 0) {
                return;
            }
            for (var i = 0; i < count; i++) {
                var item = nodes[i];
                //if (this.IsEmptyTextNode(item) == false && (item.nodeType == Node.TEXT_NODE || item.nodeType == Node.ELEMENT_NODE)) {

                if (item.nodeType == 1) {

                    var store = {};
                    store.Node = item;
                    store.Depth = this.GetDepth(item);
                    store.Path = this.GetPath(item);
                    store.TagGroup = DomService.GetTagGroup(item);
                    if (item.nodeType == 1) {
                        var htmlelement = item;
                        if (htmlelement != null) {
                            store.clientrect = htmlelement.getBoundingClientRect();
                        }
                    }

                    flats.push(store);
                    this._MakeFlat(this.ConvertToNodeArray(item.childNodes), flats);
                }
            }
        };
        DomService.RemoveKoobooAttribute = function(el) {
            if (el == null) {
                return;
            }
            var names = [];
            for (var item in el.attributes) {
                if (el.attributes.hasOwnProperty(item)) {
                    var name = el.attributes[item].name;

                    if (name.toLowerCase().indexOf("kooboo") != -1 || name.toLowerCase().indexOf("kb-") != -1) {
                        names.push(name);
                    }
                }
            }
            //for of
            for (var i = 0; i < names.length; i++) {
                var item = names[i];
                el.removeAttribute(item);
            }

            for (var i = 0; i < el.childNodes.length; i++) {
                var currentItem = el.childNodes[i];
                if (currentItem.nodeType == 1) {

                    var htmlitem = currentItem;

                    this.RemoveKoobooAttribute(htmlitem);

                }
            }
        };
        DomService.RemoveStringKoobooAttribute = function(input) {
            var element = document.createElement("div");
            element.innerHTML = input;
            this.RemoveKoobooAttribute(element);
            return element.innerHTML;
        };
        DomService.IsRepeater = function(elements) {
            /// test should be align position... 
            if (!DomService.IsPositionAlign(elements)) {
                return false;
            }

            /// test all path have the same... 
            var path = "";
            //for of
            for (var i = 0; i < elements.length; i++) {
                var item = elements[i];
                if (path == "") {
                    path = this.GetPath(item);
                } else {
                    var newpath = this.GetPath(item);
                    if (path != newpath) {
                        return false;
                    }
                }
            }
            return true;

        };
        DomService.GetWalkPath = function(root, current) {
            var paths = [];

            var currentelement = current;

            while (!currentelement.isEqualNode(root)) {

                var currentposition = {};
                currentposition.tagName = currentelement.tagName.toLowerCase();

                var dupposition = GetDuplicateIndexOfParent(currentelement);
                currentposition.DuplicateIndex = dupposition["index"];
                currentposition.BackDuplicateIndex = dupposition["backindex"];

                paths.push(currentposition);

                currentelement = currentelement.parentElement;
            }

            return paths.reverse();

            function GetDuplicateIndexOfParent(element) {

                var position = {};
                var parent = element.parentElement;
                var subelement = DomService.GetChildElements(parent);
                var tagName = element.tagName.toLowerCase();
                var count = -1;

                var EncounterKoobooTag = false;

                for (var i = 0; i < subelement.length; i++) {

                    if (subelement[i].isEqualNode(element)) {
                        if (count == -1) { count = 0; }
                        break;
                    }
                    var subTagName = subelement[i].tagName.toLowerCase();

                    if (subTagName == tagName) {
                        if (count == -1) {
                            count = 1;
                        } else {
                            count = count + 1;
                        }
                    }

                    if (subTagName.indexOf("kooboo") > -1) {
                        EncounterKoobooTag = true;
                        break;
                    }
                }

                var backcount = -1;

                if (EncounterKoobooTag) {
                    for (var i = subelement.length - 1; i > -1; i--) {
                        if (subelement[i].isEqualNode(element)) {
                            if (backcount == -1) { backcount = 0; }
                            break;
                        }
                        if (subelement[i].tagName.toLowerCase() == tagName) {
                            if (backcount == -1) {
                                backcount = 1;
                            } else {
                                backcount = backcount + 1;
                            }
                        }
                    }
                }

                if (backcount > -1) {
                    count = -1;
                }

                position["index"] = count;
                position["backindex"] = backcount;

                return position;
            }
        };
        DomService.GetElementByWalkPath = function(root, chains) {
            var current = root;
            //for (var item of chains) {
            //for of
            for (var i = 0; i < chains.length; i++) {
                var item = chains[i];
                var children = DomService.GetChildElements(current);

                if (item.DuplicateIndex == -1 && item.BackDuplicateIndex == -1) {
                    //for of
                    for (var j = 0; j < children.length; j++) {
                        var child = children[j];
                        if (item.tagName.toLowerCase() == item.tagName) {
                            current = child;
                            continue;
                        }
                    }
                    return null;
                }

                var elements;
                var reachcount;
                if (item.BackDuplicateIndex > -1) {
                    elements = children.reverse();
                    reachcount = item.BackDuplicateIndex;
                } else {
                    elements = children;
                    reachcount = item.DuplicateIndex;
                }
                var skipcount = 0;
                //for of
                for (var k = 0; k < elements.length; k++) {
                    var child = elements[k];
                    if (child.tagName.toLowerCase() == item.tagName) {
                        if (skipcount >= reachcount) {
                            current = child;
                            break;
                        } else {
                            skipcount = skipcount + 1;
                        }
                    }
                }
            }

            return current;
        };
        DomService.HasValue = function(ar) {
            var has = false;
            _.forEach(ar, function(a) {
                if (typeof a === "string" && a !== "") {
                    has = true;
                }
                if (a instanceof Array && a.length) {
                    has = true;
                }
            });
            return has;
        };
        DomService.IsSameValue = function(one, two) {
            if (one && two) {
                return one.toLowerCase() == two.toLowerCase();
            }
            return false;
        };
        DomService.GetLinkPattern = function(links, VariableReplacerName) {
            var alllinksegments = [];
            var count = 999;

            for (var i = 0; i < links.length; i++) {
                var href = links[i];
                var segs = href.split("/");
                alllinksegments.push(segs);
                if (segs.length < count) {
                    count = segs.length;
                }
            }

            var same = [];

            var hasdifference = false;

            for (var i = 0; i < count; i++) {
                var values = [];
                for (var j = 0; j < alllinksegments.length; j++) {
                    values.push(alllinksegments[j][i]);
                }
                if (hassamevalue(values)) {
                    same.push(values[0]);
                } else {
                    hasdifference = true;
                    break;
                }
            }

            var result = same.join("/");
            if (hasdifference) {
                result = result + "/{" + VariableReplacerName + "}";
            }
            return result;

            function hassamevalue(values) {
                var value = "";
                //for of
                for (var i = 0; i < values.length; i++) {
                    var item = values[i];

                    if (value == "") {
                        value = item;
                    } else {
                        if (value.toLowerCase() != item.toLowerCase()) {
                            return false;
                        }
                    }
                }
                return true;
            }
        };
        DomService.GetUrlPattern = function(links) {
            var linkpattern = new LinkPattern();

            var QueryString = [];
            var LinkPaths = [];

            SeperateQueryString(links, LinkPaths, QueryString);

            GetQueryString(QueryString, linkpattern);

            var AllLinksSegments = DomService.GetUrlSegmentList(LinkPaths);

            var length = GetAllSegSameLength(AllLinksSegments);

            if (length < 1) {
                return null;
            }

            if (length == 1) {
                return DomService.GetUrlFlatPattern(LinkPaths);
            }

            if (!ParseSegs(AllLinksSegments, linkpattern)) {
                return null;
            }

            if (linkpattern.UrlPath) {
                return linkpattern;
            }

            return null;

            function SeperateQueryString(links, LinkPaths, QueryString) {
                //for of
                for (var i = 0; i < links.length; i++) {
                    var href = links[i];
                    if (!href) {
                        return;
                    }
                    var markindex = href.indexOf("?");
                    if (markindex >= 0) {
                        LinkPaths.push(href.substring(0, markindex));
                        if (href.length - 1 > markindex) {
                            var querystring = href.substring(markindex + 1);
                            QueryString.push(querystring);
                        }
                    } else {
                        LinkPaths.push(href);
                        QueryString.push("");
                    }
                }
            }

            function ParseSegs(AllSegments, Result) {

                // check all have the same len... 
                var MinSegs = GetAllSegSameLength(AllSegments);
                if (MinSegs <= 1) {
                    return false;
                }
                var same = [];
                var listcount = AllSegments.length;
                var tempdata = {};

                for (var i = 0; i < MinSegs; i++) {
                    var values = [];
                    for (var j = 0; j < listcount; j++) {
                        var linevalue = AllSegments[j][i];
                        if (!linevalue) {
                            return false;
                        }
                        values.push(linevalue);
                    }
                    if (DomService.HasSameValue(values)) {
                        same.push(values[0]);
                    } else {
                        if (i == 0) {
                            // try to get flats
                            if (MinSegs == 1) {

                            } else { return false; }
                        }

                        if (i == MinSegs - 1 && !linkpattern.QueryString) {

                            var sublinkpattern = DomService.GetUrlFlatPattern(values, linkpattern);
                            if (sublinkpattern != null && sublinkpattern.UrlPath) {

                                same.push(sublinkpattern.UrlPath);
                                for (var subkey in sublinkpattern.Data) {
                                    tempdata[subkey] = sublinkpattern.Data[subkey];
                                }
                            } else {

                                var key = linkpattern.DetermineKeyName(values);
                                tempdata[key] = values;
                                same.push("{" + key + "}");
                            }
                        } else {
                            var key = linkpattern.DetermineKeyName(values);
                            tempdata[key] = values;
                            same.push("{" + key + "}");
                        }
                    }
                }

                if (same.length > 0) {
                    linkpattern.UrlPath = "/" + same.join("/");
                    for (var name in tempdata) {
                        linkpattern.Data[name] = tempdata[name];
                    }
                    return true;
                }
                return false;
            }

            function HasValue(querystrings) {
                if (querystrings == null || querystrings.length == 0) {
                    return false;
                }
                for (var i = 0; i < querystrings.length; i++) {
                    var item = querystrings[i];
                    if (item) {
                        return true;
                    }
                }
                return false;
            }

            function GetAllQueryNames(all) {
                var names = [];
                //for of
                for (var i = 0; i < all.length; i++) {
                    var item = all[i];
                    for (var j = 0; j < item.length; j++) {
                        var one = item[j];
                        if (names.indexOf(one.Key.toLowerCase()) == -1) {
                            names.push(one.Key.toLowerCase());
                        }
                    }
                }
                return names;
            }

            function GetQueryString(QueryList, linkpattern) {
                if (!HasValue(QueryList)) {
                    return;
                }

                var result = {};
                var resultquery = [];
                var AllQuerys = [];
                //for of
                for (var i = 0; i < QueryList.length; i++) {
                    var each = QueryList[i];
                    var pair = DomService.ParseQueryString(each);
                    AllQuerys.push(pair);
                }

                var allnames = GetAllQueryNames(AllQuerys);

                for (var i = 0; i < allnames.length; i++) {
                    var name = allnames[i];
                    var namevalues = [];

                    for (var j = 0; j < AllQuerys.length; j++) {
                        var query = AllQuerys[j];
                        var eachpair = _.find(query, function(o) { return o.Key.toLowerCase() == name; });
                        if (eachpair) {
                            namevalues.push(eachpair.Value);
                        } else {
                            namevalues.push(null);
                        }
                    }

                    if (HasValue(namevalues)) {
                        var resultpair = {};
                        if (DomService.HasSameValue(namevalues)) {
                            resultpair.Key = name;
                            resultpair.Value = namevalues[0];
                        } else {
                            var newkey = linkpattern.DetermineKeyName(namevalues);
                            resultpair.Key = name;
                            resultpair.Value = "{" + newkey + "}";
                            linkpattern.Data[newkey] = namevalues;
                        }

                        resultquery.push(resultpair);
                    }
                }

                if (resultquery.length > 0) {
                    var querystringlist = [];
                    for (var i = 0; i < resultquery.length; i++) {
                        var eachquery = resultquery[i];
                        querystringlist.push(eachquery.Key + "=" + eachquery.Value);
                    }
                    linkpattern.QueryString = querystringlist.join("&");
                }

            }

            function GetAllSegSameLength(Segments) {
                var MinSegs = -1;
                //for of
                for (var i = 0; i < Segments.length; i++) {
                    var item = Segments[i];
                    if (MinSegs == -1) {
                        MinSegs = item.length;
                    } else {
                        if (MinSegs != item.length) {
                            return -1;
                        }
                    }
                }
                return MinSegs;
            }
        };
        DomService.GetStringFormatPattern = function(values) {
            var begin = "";
            var end = "";
            var forwordindex = -1;
            var backindex = -1;
            var currentchar;
            while (forwordindex < 9999) {
                forwordindex = forwordindex + 1;
                currentchar = null;
                //for of
                for (var i = 0; i < values.length; i++) {
                    var value = values[i];
                    if (!value || value.length <= forwordindex) {
                        break;
                    }
                    var char = value.charAt(forwordindex);
                    if (currentchar == null) {
                        currentchar = char;
                    } else {
                        if (currentchar != char) {
                            break;
                        }
                    }
                }
                begin += currentchar;
            }
            while (backindex < 999) {
                backindex = backindex + 1;
                currentchar = null;
                for (var i = 0; i < values.length; i++) {
                    var value = values[i];
                    if (!value || value.length <= forwordindex + backindex) {
                        break;
                    }

                    var char = value.charAt(value.length - backindex - 1);
                    if (currentchar == null) {
                        currentchar = char;
                    } else {
                        if (currentchar != char) {
                            break;
                        }
                    }
                }
                end += end + currentchar;
            }

            if (backindex <= 0 && forwordindex <= 0) {

                return null;
            } else {
                var data = [];
                for (var i = 0; i < values.length; i++) {
                    var item = values[i];
                    var currentdata = item.substr(forwordindex, item.length - backindex - forwordindex - 1);
                    data.push(currentdata);
                }
                var linkpattern = new LinkPattern();
                var keyname = linkpattern.DetermineKeyName(data);
                var value0 = values[0];

                var pattern;

                if (forwordindex > 0) {
                    pattern = value0.substr(0, forwordindex);
                }
                pattern += "{" + keyname + "}";

                if (backindex > 0) {
                    pattern += value0.substr(value0.length - backindex);
                }

                linkpattern.Data[keyname] = data;
                linkpattern.UrlPath = pattern;
                return linkpattern;
            }
        };
        ///Get the pattern of urls like news_345.aspx, without segments.
        DomService.GetUrlFlatPattern = function(links, ParentPattern) {
            var seperators = [];
            seperators.push(".");
            seperators.push("_");
            seperators.push("-");
            for (var i = 0; i < seperators.length; i++) {
                var sep = seperators[i];
                var allsegs = DomService.GetUrlSegmentList(links, sep);
                var result = ParseSegs(allsegs, sep);
                if (result) {
                    return result;
                }
            }

            function ParseSegs(AllSegments, seperator) {
                var linkpattern = new LinkPattern();
                // check all have the same len... 
                var MinSegs = -1;
                //for of
                for (var i = 0; i < AllSegments.length; i++) {
                    var item = AllSegments[i];
                    if (MinSegs == -1) {
                        MinSegs = item.length;
                    } else {
                        if (MinSegs != item.length) {
                            return null;
                        }
                    }
                }

                var same = [];
                var listcount = AllSegments.length;
                var tempdata = {};

                for (var i = 0; i < MinSegs; i++) {
                    var values = [];
                    for (var j = 0; j < listcount; j++) {
                        var linevalue = AllSegments[j][i];
                        values.push(linevalue);
                    }
                    if (DomService.HasSameValue(values) || allNull(values)) {
                        same.push(values[0]);
                    } else {
                        if (i == 0) {
                            return null;
                        }
                        var sameEndValues=getSameEndValue(values,seperator);
                        var key;
                        if (ParentPattern == null) {
                            key = linkpattern.DetermineKeyName(sameEndValues.values);
                        } else {
                            key = ParentPattern.DetermineKeyName(sameEndValues.values);
                        }

                        tempdata[key] = sameEndValues.values;
                        var keyvalue="{" + key + "}";
                        if(sameEndValues.sameEndValue){
                            keyvalue+=sameEndValues.seperator+sameEndValues.sameEndValue;
                        }
                        same.push(keyvalue);
                    }
                }

                if (same.length > 0) {
                    linkpattern.UrlPath = same.join(seperator);
                    for (var name in tempdata) {
                        linkpattern.Data[name] = tempdata[name];
                    }
                    return linkpattern;
                }
                return null;
            }
            function allNull(values) {
                for (var i = 0; i < values.length; i++) {
                    var item = values[i];
                    if (item) {
                        return false;
                    }
                }
                return true;
            }
            function getSameEndValue(values,seperator){
                var sameEndValues={
                    values:values,
                    sameEndValue:"",
                    seperator:seperator
                };
                for (var i = 0; i < seperators.length; i++) {
                    var sep = seperators[i];
                    var allsegs = DomService.GetUrlSegmentList(values, sep);
                    var prefixValues=[];
                    var lastSegs=[];
                    for(var j=0;j<allsegs.length;j++){
                        var seg=allsegs[j];
                        if(seg.length>0){
                            lastSegs.push(allsegs[j][seg.length-1]);
                            allsegs[j].pop();
                            prefixValues.push(allsegs[j].join(sep));
                        }else{
                            lastSegs.push("");
                        }
                    }
                    if(DomService.HasSameValue(lastSegs) || allNull(lastSegs)){
                        sameEndValues.sameEndValue=lastSegs[0];
                        sameEndValues.values=prefixValues;
                        sameEndValues.seperator=sep;
                        break;
                    }
                }
                return sameEndValues;
            }
        };
        DomService.ParseQueryString = function(querystring) {
            // we'll store the parameters here 
            var valuelist = [];

            if (!querystring) {
                return valuelist;
            }
            var questionmarkindex = querystring.indexOf("?");
            if (questionmarkindex > -1) {
                if (querystring.length <= questionmarkindex + 1) {
                    return valuelist;
                } else {
                    querystring = querystring.substring(questionmarkindex + 1);
                }
            }

            // stuff after # is not part of query string, so get rid of it
            querystring = querystring.split('#')[0];

            // split our query string into its component parts
            var arr = querystring.split('&');

            for (var i = 0; i < arr.length; i++) {
                var obj = {};
                // separate the keys and the values
                var a = arr[i].split('=');
                // in case params look like: list[]=thing1&list[]=thing2
                var paramNum = undefined;
                var paramName = a[0].replace(/\[\d*\]/, function(v) {
                    paramNum = v.slice(1, -1);
                    return '';
                });

                // set parameter value (use 'true' if empty)
                var paramValue = typeof(a[1]) === 'undefined' ? "true" : a[1];

                // (optional) keep case consistent
                paramName = paramName.toLowerCase();

                obj.Key = paramName;
                obj.Value = paramValue;

                valuelist.push(obj);
            }

            return valuelist;
        };
        /// y>x=-1, -1 x>y=1, 0=same or contained by. 
        DomService.CompareDomPosition = function(x, y) {
            var chainx = this.GetChain(x, x.ownerDocument.body, true).reverse();
            var chainy = this.GetChain(y, y.ownerDocument.body, true).reverse();
            var count = chainx.length;
            if (chainy.length < count) {
                count = chainy.length;
            }
            for (var i = 0; i < count; i++) {
                var xx = chainx[i];
                var yy = chainy[i];
                var xsibling = this.GetSiblingIndex(xx);
                var ysibling = this.GetSiblingIndex(yy);
                if (xsibling > ysibling) {
                    return 1;
                } else if (xsibling < ysibling) {
                    return -1;
                }
            }
            return 0;
        };
        DomService.IsParentSubElement = function(parent, sub) {
            if (parent == null || sub == null) {
                return false;
            }
            var current = sub;
            while (this.GetDepth(current) >= this.GetDepth(parent)) {
                if (current.isEqualNode(parent)) {
                    return true;
                }
                current = current.parentElement;
            }
            return false;
        };
        DomService.RemoveAllWhiteSpace = function(input) {
            if (input) {
                var result = input.replace(/\s/g, '');
                if (result.length > 0) {
                    result = result.replace('&nbsp;', '');
                }
                return result;
            } else {
                return input;
            }
        };
        DomService.GetDocumentWidth = function(Body) {
            var currentWidth = Body.clientWidth;
            var el = Body;
            el = Down(el);
            var i = 0;
            while (i < 100) {
                i++;
                var subelements = DomService.GetChildElements(el);
                if (subelements.length == 0) {
                    break;
                }
                if (subelements.length == 1) {
                    var newwidth = subelements[0].clientWidth;
                    if (isSimilarwith(currentWidth, newwidth)) {
                        return this.GetDocumentWidth(subelements[0]);
                    } else {
                        return newwidth;
                    }
                } else {
                    return currentWidth;
                }
            }

            function Down(el) {
                var subs = DomService.GetChildElements(el);
                if (subs != null && subs.length == 1) {
                    return Down(subs[0]);
                }
                return el;
            }

            function isSimilarwith(parent, sub) {
                if (Math.abs(parent - sub) < 50) {
                    return true;
                }
                if (sub * 1.1 > parent) {
                    return true;
                }
                return false;
            }
        };
        DomService.GetExtraAttributes = function(el) {
            var results = [];
            for (var item in el.attributes) {
                if (el.attributes.hasOwnProperty(item)) {
                    var name = el.attributes[item].name;
                    var lowername = name.toLowerCase();
                    //k-
                    if (lowername.indexOf("kooboo") == -1 && lowername.indexOf("kb-") == -1 && lowername.indexOf("k-") == -1) {
                        if (lowername != "href" && lowername != "class" && lowername != "style" && lowername != "script") {
                            var value = el.getAttribute(name);
                            var result = {};
                            result.Key = name;
                            result.Value = value;
                            results.push(result);
                        }
                    }
                }
            }
            return results;
        };
        DomService.GetUrlSegments = function(input, seperator) {
            if (!seperator) {
                seperator = "/";
            }
            var result = [];
            var urls = input.split(seperator);
            for (var i = 0; i < urls.length; i++) {
                var item = urls[i];
                if (item) {
                    result.push(item);
                }
            }
            return result;
        };
        DomService.GetUrlSegmentList = function(input, seperator) {
            var result = [];
            //for of
            for (var i = 0; i < input.length; i++) {
                var item = input[i];
                var segments = this.GetUrlSegments(item, seperator);
                result.push(segments);
            }
            return result;
        };
        DomService.HasSamePath = function(links) {
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
        };
        DomService.HasSameTreeDistance = function(els) {
            var distance = 0;

            var count = els.length;
            for (var i = 0; i < count; i++) {
                if (i < count - 1) {
                    var current = els[i];
                    var next = els[i + 1];

                    if (distance == 0) {
                        distance = DomService.GetTreeDistance(current, next);
                    } else {
                        var newdistance = DomService.GetTreeDistance(current, next);
                        if (distance != newdistance) {
                            return false;
                        }
                    }
                }
            }

            return true;
        };
        DomService.GetTreeDistance = function(x, y) {
            var parent = this.FindParent(x, y);

            if (parent == null) {
                return 999999;
            }

            var xchain = [];
            var ychain = [];

            var xparent = x.parentElement;

            while (xparent != null && !xparent.isEqualNode(parent)) {
                xchain.push(xparent);
                xparent = xparent.parentElement;
            }

            xchain.reverse();

            var yparent = y.parentElement;

            while (yparent != null && !yparent.isEqualNode(parent)) {
                ychain.push(yparent);
                yparent = yparent.parentElement;
            }

            ychain.reverse();

            var xcount = xchain.length;
            var ycount = ychain.length;
            var count = xcount;
            if (count < ycount) {
                count = ycount;
            }

            var xvalue = this.GetSiblingIndex(x);
            var yvalue = this.GetSiblingIndex(y);

            for (var i = 0; i < xcount; i++) {

                var addvaluex = (this.GetSiblingIndex(xchain[i]) + 1) * Math.abs(Math.pow(10, (count - i)));
                xvalue = xvalue + addvaluex;
            }

            for (var i = 0; i < ycount; i++) {

                var addvaluey = (this.GetSiblingIndex(ychain[i]) + 1) * Math.abs(Math.pow(10, (count - i)));
                yvalue = yvalue + addvaluey;

            }

            return Math.abs(xvalue - yvalue);
        };
        DomService.IsMenuCategoryText = function(el) {
            var widththreshold = 300;
            var heightthreshold = 150;

            var text = DomService.RemoveAllWhiteSpace(el.innerText);

            if (text == null || text == "") {
                var img = el.getElementsByTagName("img");
                if (img == null || img.length == 0) {
                    return false;
                }
            }
            //special rule：to do confirm
            if (text.indexOf(",") != -1 || text.indexOf(";") != -1) {
                return false;
            }
            //special rule:to do confirm
            if (text.indexOf(".") != -1 && text.length > 40) {
                return false;
            }

            var rect = el.getBoundingClientRect();
            if (rect.width > widththreshold || rect.height > heightthreshold) {
                return false;
            }
            return true;
        };
        DomService.HasSameParent = function(els) {
            var parent;
            var count = els.length;
            for (var i = 1; i < count; i++) {
                var previous = els[i - 1];
                var current = els[i];
                if (parent == null) {
                    parent = this.FindParent(previous, current);
                } else {
                    var newparent = this.FindParent(previous, current);
                    if (!parent.isEqualNode(newparent)) {
                        return false;
                    }
                }
            }
            return true;
        };
        DomService.GetSameChain = function(chains) {
            var result = [];
            if (chains == null || chains.length == 0) {
                return result;
            }

            if (chains.length == 1) {
                return chains[0];
            }

            var minlen = 9999;
            // for of
            for (var i = 0; i < chains.length; i++) {
                var items = chains[i];
                if (items != null && items.length > 0) {
                    if (minlen > items.length) {
                        minlen = items.length;
                    }
                }
            }

            var currentSegs = [];
            var shouldstop = false;
            for (var i = 0; i < minlen; i++) {
                _.remove(currentSegs);
                shouldstop = false;
                //for of
                for (var j = 0; j < chains.length; j++) {
                    var item = chains[j];
                    if (!item) {
                        shouldstop = true;
                        break;
                    } else {
                        currentSegs.push(item[i]);
                    }
                }

                if (shouldstop) {
                    break;
                } else {
                    if (this.HasSameValue(currentSegs)) {
                        result.push(currentSegs[0]);
                    }

                }
            }

            return result;
        };
        DomService.HasSameValue = function(inputs) {
            if (inputs == null || inputs.length == 0) {
                return false;
            }
            var currentvalue = inputs[0];
            if (!currentvalue) {
                return false;
            }
            currentvalue = currentvalue.toLowerCase();
            //for of
            for (var i = 0; i < inputs.length; i++) {
                var item = inputs[i];
                if (!item || item.toLowerCase() != currentvalue) {
                    return false;
                }
            }
            return true;
        };
        DomService.ListHasValue = function(values) {
            if (values == null || values.length == 0) {
                return false;
            }
            //for of
            for (var i = 0; i < values.length; i++) {
                var item = values[i];
                if (item) {
                    return true;
                }
            }
            return false;
        };
        ///return an active class name, return null if not found...
        DomService.GetActiveClass = function(els) {
            if (els.length == 0 || els.length == 1) {
                return null;
            }
            var classvalues = [];
            //for of
            for (var i = 0; i < els.length; i++) {
                var item = els[i];
                var classname = item.className;
                classvalues.push(classname);
            }

            if (!this.ListHasValue(classvalues)) {
                return null;
            }
            if (this.CountDistinct(classvalues) != 2) {
                return null;
            }
            //for of
            for (var i = 0; i < els.length; i++) {
                var item = els[i];
                var ItemClassName = item.className;

                if (count(classvalues, ItemClassName) == 1 && this.ContainsCurrentUrl(item)) {
                    return ItemClassName;
                }
            }

            return null;

            function count(values, value) {
                var counter = 0;
                //for of
                for (var i = 0; i < values.length; i++) {
                    var item = values[i];
                    if (item == value) {
                        counter = counter + 1;
                    }
                }

                return counter;
            }
        };
        DomService.CountDistinct = function(values) {
            var uniquevalues = [];
            //for of
            for (var i = 0; i < values.length; i++) {
                var item = values[i];
                if (uniquevalues.indexOf(item) == -1) {
                    uniquevalues.push(item);
                }
            }

            return uniquevalues.length;
        };
        DomService.getCurrentUrl=function(el){
            return el.ownerDocument.URL.toLowerCase();
        }
        ///The element of subelemements contains a link to current url.
        /// Used to determine whether it is current active link element or not.
        DomService.ContainsCurrentUrl = function(el) {
            var currenturl = DomService.getCurrentUrl(el);
            var alllinks = new Array();
            if (el.tagName.toLowerCase() == "a") {
                alllinks.push(el);
            } else {
                var parent = GetParentA(el);
                if (parent != null) {
                    alllinks.push(parent);
                } else {
                    var allsub = el.getElementsByTagName("a");
                    for (var i = 0; i < allsub.length; i++) {
                        alllinks.push(allsub[i]);
                    }
                }
            }
            var result = false;
            _.forEach(alllinks, function(o) {
                var href = o.getAttribute("href");
                if (href) {
                    var absoluteurl = DomService.AbsoluteUrl(href).toLowerCase();
                    if (currenturl == absoluteurl) {
                        result = true;
                    }
                }
            });
            return result;

            function GetParentA(current) {
                var parent = current.parentElement;
                if (parent == null || parent.tagName.toLowerCase() == "body") {
                    return null;
                }
                if (el.tagName.toLowerCase() == "a") {
                    return parent;
                }
                return GetParentA(parent);
            }
        };
        DomService.AbsoluteUrl = function(url) {
            var a = document.createElement('a');
            a.href = url;
            return a.href;
        };
        /// Get the value if there is only set in the first item. 
        DomService.IsRepeatFirst = function(values) {
            if (DomService.CountDistinct(values) != 2) {
                return false;
            }
            if (values.length <= 2) {
                return false;
            }
            var first = values[0];
            for (var i = 1; i < values.length; i++) {
                var currentvalue = values[i];
                if (first == currentvalue) {
                    return false;
                }
            }
            return true;
        };
        /// If the the value is only set at the last item...
        DomService.IsRepeatLast = function(values) {
            if (DomService.CountDistinct(values) != 2) {
                return false;
            }
            var last = values[values.length - 1];
            for (var i = 0; i < values.length - 1; i++) {
                var currentvalue = values[i];
                if (last == currentvalue) {
                    return false;
                }
            }
            return true;
        };
        DomService.getN=function(values){
            var n=1;
            var firstValue=values[0];
            for (var i = 0; i < values.length; i++) {
                var value=values[i];
                if(i==0){
                    firstValue=value;
                }else if(firstValue==value){
                    n++;
                }else{
                    n++;
                    break;
                }
            }
            return n;
        }
        DomService.IsNVaulues=function(values){
            var isNValue=false
            var n= DomService.getN(values);
            var prevNValue=values[0];
            var nvalue=values[n-1];
           
            var mod=values.length%(n);
            if(mod>0){
                return false;
            }else{
                var count=values.length/(n);
                if(count==1) return false;
                isNValue=true;
                for(var i=0;i<values.length;i++){
                    if((i+1)%n==0){
                        if(nvalue!=values[i]){
                            isNValue=false;
                            break;
                        }
                    }else{
                        if(prevNValue!=values[i]){
                            isNValue=false;
                            break;
                        }
                    }
                }
            }
            return isNValue
        }
        ///The repeat is in an odd and even change way...
        DomService.IsOddEvenValues = function(values) {
            if (values.length > 2 && this.CountDistinct(values) == 2) {

                var current;
                //for of
                for (var i = 0; i < values.length; i++) {
                    var each = values[i];
                    if (!current) {
                        current = each;
                    } else {
                        if (current == each) {
                            return false;
                        } else {
                            current = each;
                        }
                    }
                }
                return true;

            }

            return false;
        };
        DomService.ContainOnlyTextSubNodes = function(el) {
            var nodes = el.childNodes;
            for (var i = 0; i < nodes.length; i++) {
                var item = nodes[i];
                if (item.nodeType == 1) {
                    return false;
                }
            }
            return true;
        };
        DomService.GetNoCommentOuterHtml=function(el){
            if(el){
                removeCommentNode(el);
                return el.outerHTML;
            }
            return "";

            function removeCommentNode(el){
                if(el.childNodes&& el.childNodes.length>0){
                    for(var i=0;i<el.childNodes.length;i++){
                        if(el.childNodes[i].nodeType==8){
                            el.childNodes[i].remove();
                        }else{
                            removeCommentNode(el.childNodes[i]);
                        }
                    }
                }
            }
        }
        
        return DomService;
    })();

    var LinkPattern = function() {
        this.UrlPath = "";
        this.Data = {};
        this.Names = [];
    }
    LinkPattern.prototype.FinalUrl = function() {
        if (!this.UrlPath) {
            return null;
        }
        var url = this.UrlPath;
        if (this.QueryString) {
            url = url + "?" + this.QueryString;
        }
        return url;
    };
    LinkPattern.prototype.GetKeyName = function(key) {
        key = key.toLowerCase();
        if (this.Names.indexOf(key) == -1) {
            this.Names.push(key);
            return key;
        } else {
            key = key + "1";
            return this.GetKeyName(key);
        }
    };
    LinkPattern.prototype.DetermineKeyName = function(values) {
        if (isNumber(values)) {
            return this.GetKeyName("Idx");
        } else if (isDateTime(values)) {
            return this.GetKeyName("Date");
        } else {
            return this.GetKeyName("key");
        }

        function isNumber(values) {
            //for of
            for (var i = 0; i < values.length; i++) {
                var item = values[i];
                if (!$.isNumeric(item)) {
                    return false;
                }
            }
            return true;
        }

        function isDateTime(values) {
            //for of
            for (var i = 0; i < values.length; i++) {
                var item = values[i];
                var value = Date.parse(item);
                if (value > 0) {
                    continue;
                } else {
                    return false;
                }
            }
            return true;
        }
    };  
    return DomService;
}