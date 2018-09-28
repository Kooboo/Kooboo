function Repeater(){
    var DomService = Kooboo.NewDomService,
        TagGroup = Kooboo.TagGroup;

    function findSubRepeater(el,parentElement){
        var functionlist = [];

        functionlist.push(RepeaterHelper.GetSubByExamineTags);
        functionlist.push(RepeaterHelper.GetSubByTable);
        
        var result = null;
        for (var i = 0; i < functionlist.length; i++) {
            var item = functionlist[i];
            result = item(el,parentElement);
            if (result) {
                return result;
            }
        }
        return result;
    }
    function findSuperRepeater(el){
        function vaildMaxContains(items, el) {
            var hascontain = false;
            for (var i = 0; i < items.length; i++) {
                var it = items[i];
                if (it.contains(el)) {
                    hascontain = true;
                }
            }
            return hascontain;
        }

        var p = el,
            body = el.ownerDocument.body,
            result,
            maxlevel=5,
            level=0;;
        do {
            result = findSubRepeater(p);
            if (result && !vaildMaxContains(result, el)) {
                result = null;
            }
            p = p.parentElement;
            level++;
        } while (result == null && p && p.tagName && body.contains(p)&&level<maxlevel)
        return result;
    }

    function IsSamePosition(elements) {
        if (elements == null || elements.length < 2) {
            return false;
        }
        var rectlist = [];
        for (var i = 0; i < elements.length; i++) {
            var item = elements[i];
            rectlist.push(item.clientrect);
        }
        return DomService.IsPositionAlignByRect(rectlist);
    }
    function GetRepeatItemContainers(parent, subs) {
        var containers = [];
        for (var i = 0; i < subs.length; i++) {
            var item = subs[i];
            var container = DomService.FindContainer(item, parent);
            containers.push(container);
        }
        return containers;
    }
    var RepeaterHelper={
        KoobooTag:{
            Attribute: "k-attributes",
            Content: "k-content",
            Replace: "k-replace",
            Repeat: "k-repeat",
            RepeatSelf: "k-repeat-self",
            RepeatItemKey: "List_Item",
        },
        GetSubByExamineTags :function(element) {
            var results=[];
            FindRepeats(element,results);

            return RepeaterHelper.getBestRepeater(results);

            function FindRepeats(el,results) {
                var allsubs = DomService.GetChildElements(el);
                allsubs = RepeaterHelper.ExcludeScriptAndStyleTag(allsubs);

                if (allsubs != null && allsubs.length > 1) {
                    var sameTags = getSameTags(allsubs);

                    if (sameTags.length && DomService.IsPositionAlign(sameTags)) {
                        results.push(sameTags);
                    }
                }
                for (var i = 0; i < allsubs.length; i++) {
                    var each = allsubs[i];
                    FindRepeats(each,results);
                }

                function getSameTags(subs) {
                    var res = [];
                    var temp = {};

                    if (TagGroup.GetGroup(subs[0]) == TagGroup.EnumTagGroup.Grouping) {
                        return subs;
                    }
                    for (var i = 0; i < subs.length - 1; i++) {
                        if (IsSameTags([subs[i], subs[i + 1]])) {
                            temp[i] = subs[i];
                            temp[i + 1] = subs[i + 1];
                        }
                    }
                    res = _.values(temp);
                    return res;
                }

                function IsSameTags(subs) {
                    var tagname = "";
                    var subtags = "";

                    function subtagstring(el) {
                        var subels = DomService.GetChildElements(el);
                        var tags = "";
                        for (var i = 0; i < subels.length; i++) {
                            var sel = subels[i];
                            tags = tags + sel.tagName.toLowerCase();
                        }
                        return tags;
                    }
                    for (var i = 0; i < subs.length; i++) {
                        var item = subs[i];
                        if (tagname == "") {
                            tagname = item.tagName.toLowerCase();
                        } else {
                            if (item.tagName.toLowerCase() != tagname) {
                                return false;
                            }
                        }

                        if (subtags == "") {
                            subtags = subtagstring(item);
                        } else {
                            var newsubtags = subtagstring(item);
                            if (newsubtags != subtags&&newsubtags!="") {
                                return false;
                            }
                        }
                    }
                    return true;

                }
            }
        },
        //clone element don't have parentElement
        GetSubByTable : function(element,parentElement) {
            if (element == null) {
                return null;
            }
            var _trs,
                _tds;

            if(!parentElement){
                parentElement=element.parentElement;
            }
            switch (element.tagName.toLowerCase()) {
                case "table":
                    var tbody = element.getElementsByTagName("tbody");
                    if (tbody.length) {
                        _trs = tbody[0].getElementsByTagName("tr");
                        if (_trs.length > 1) {
                            return _.toArray(_trs);
                        }
                    }
                    break;
                case "tr":
                    _trs = parentElement.getElementsByTagName("tr");
                    if (_trs.length > 1) {
                        return _.toArray(_trs);
                    }
                    break;
                case "td":
                    _tds = parentElement.getElementsByTagName("td");
                    if (_tds.length > 1) {
                        return _.toArray(_tds);
                    }
                    break;
            }

            return null;
        },

        GetSubByPosition:function(container) {
            var nodes = DomService.ConvertToNodeArray(container.childNodes);
            if (nodes == null || nodes.length == 0) {
                return null;
            }

            var Flats = DomService.ConvertToFlatKoobooElements(nodes);

            var groupby = _.groupBy(Flats, function(o) { return o.Path; });

            var sortby = _.sortBy(groupby, function(item, key) {
                return -item.length;
            });

            for (var i = 0; i < sortby.length; i++) {
                var item = sortby[i];
                var oneitem = item[0];
                if (item.length == 1) {
                    continue;
                }
                if (oneitem.TagGroup != TagGroup.EnumTagGroup.Text &&
                    oneitem.TagGroup != TagGroup.EnumTagGroup.SpecialContent &&
                    oneitem.TagGroup != TagGroup.EnumTagGroup.Script) {
                    if (IsSamePosition(item)) {
                        var HtmlElementList = [];
                        for (var j = 0; j < item.length; j++) {
                            var oneitem = item[j];
                            HtmlElementList.push(oneitem.Node);
                        }
                        var parent = DomService.FindCommonParent(HtmlElementList);

                        var repeater = GetRepeatItemContainers(parent, HtmlElementList)

                        if (RepeaterHelper.IsRepeater(repeater)) {
                            return repeater;
                        }
                    }
                }
            }
        },
        ExcludeScriptAndStyleTag : function(allsubs) {
            _.remove(allsubs, function(sub) {
                return ["style", "script"].indexOf(sub.tagName.toLowerCase()) > -1;
            })
            return allsubs;
        },
        getBestRepeater:function(results){
            function isTableRepeaterElement(tagName){
                return ["td","tr","th"].indexOf(tagName.toLowerCase());
            }
            if(results.length>0){
                //default get first repeater
                var bestResult=results[0];
                for(var i=0;i<results.length;i++){
                    var result=results[i];
                    var tagName=result[0].tagName;
                    if(!isTableRepeaterElement(result[i].tagName)
                        ||tagName.toLowerCase()!="th"){
                        bestResult=result;
                        break;
                    }
                }

                //for unit test
                var result=_.find(bestResult,function(obj){
                    var koobooId=$(bestResult[i]).attr("kooboo-id");
                    if(koobooId)
                        return true;
                    return false;
                });
                if(result){
                    //html generate by js
                    for(var i=bestResult.length-1;i>=0;i--){
                        var koobooId=$(bestResult[i]).attr("kooboo-id");
                        if(!koobooId){
                            bestResult.splice(i,1);
                        }
                            
                    }
                }
                
                return bestResult;
            }
            return null;
        },
        _FindRepeater : function(flats, target) {
            if (target != null && target.length > 0) {

                var isElement = target[0].Node.nodeType == 1;

                var HtmlElementList = [];
                for (var i = 0; i < target.length; i++) {
                    var founditem = target[i];
                    var htmlelement;
                    if (isElement) {
                        htmlelement = founditem.Node;
                    } else {
                        htmlelement = founditem.Node.parentElement;
                    }
                    HtmlElementList.push(htmlelement);
                }

                var parent = DomService.FindCommonParent(HtmlElementList);
                var repeater = GetRepeatItemContainers(parent, HtmlElementList)
                if (RepeaterHelper.IsRepeater(repeater)) {
                    return repeater;
                }
            }

            return null;
        },
        GetNonEmptySubNodes : function(element) {
            var nodes = [];
            for (var i = 0; i < element.childNodes.length; i++) {
                var node = element.childNodes[i];
                if (!DomService.IsEmptyTextNode(node)) {
                    nodes.push(node);
                }
            }
            return nodes;
        },
        IsRepeater : function(elements) {
            /// test should be align position... 
            if (!DomService.IsPositionAlign(elements) || !HasSamePath(elements) || !HasSimiliarShape(elements)) {
                return false;
            }
            return true;

            function HasSamePath(els) {
                var path = "";
                for (var i = 0; i < elements.length; i++) {
                    var item = elements[i];
                    if (path == "") {
                        path = DomService.GetPath(item);
                    } else {
                        var newpath = DomService.GetPath(item);
                        if (path != newpath) {
                            return false;
                        }
                    }
                }
                return true;
            }

            function HasSimiliarShape(els) {

                var count = els.length;
                var totalwidth = 0;
                var totalheight = 0;
                for (var i = 0; i < els.length; i++) {
                    var item = els[i];
                    totalwidth = totalwidth + item.offsetWidth;
                    totalheight = totalheight + item.offsetHeight;
                }

                var avgwidth = Math.abs(totalwidth / count);
                var maxwidth = avgwidth * 2;
                var avgheight = Math.abs(totalheight / count);
                var maxheight = avgheight * 2;

                for (var i = 0; i < els.length; i++) {
                    var each = els[i];
                    if (each.offsetHeight > maxheight || each.offsetWidth > maxwidth) {
                        return false;
                    }
                }
                return true;
            }
        }
    }
    var TemplateManager ={
        GetTemplate : function(elements) {
            if (elements.length == 0) {
                return null;
            }
            if (elements.length == 1) {
                // TODO: in the case of only one element, also need to process it. 
            } else {
                var firstelement = elements[0].cloneNode(true);
                DomService.RemoveKoobooAttribute(firstelement);

                var others = elements.slice(1);
                var Data = {};

                for (var i = 0; i < elements.length; i++) {
                    Data[i] = {};
                }

                this.SetTemplate(firstelement, others, Data, new Counter());
                
                firstelement.setAttribute(RepeaterHelper.KoobooTag.Repeat, RepeaterHelper.KoobooTag.RepeatItemKey + " List");
                firstelement.setAttribute(RepeaterHelper.KoobooTag.RepeatSelf, "true");

                var template = {};
                template.Data = this.removeNoFieldData(Data);
                var firstElementTemplate= DomService.GetNoCommentOuterHtml(firstelement);
                var commonParent=DomService.FindCommonParent(elements);
                template.commonParent=commonParent;
                template.HtmlBody=TemplateManager.getHtmlBody(commonParent,elements,firstElementTemplate);
                return template;
            }
        },
        getHtmlBody:function(commonParent,elements,firstElementTemplate){
            var cloneParantElement=commonParent.cloneNode(true);
            var children=cloneParantElement.children;

            var firstElement=children[0];
            DomService.RemoveKoobooAttribute(firstElement);
            var firstRepeaterHtml=DomService.GetNoCommentOuterHtml(firstElement);
            for(var i=children.length-1;i>=1;i--){
                cloneParantElement.removeChild(children[i]);
            }
            
            DomService.RemoveKoobooAttribute(cloneParantElement);
            var html=DomService.GetNoCommentOuterHtml(cloneParantElement);
            
            return html.replace(firstRepeaterHtml, firstElementTemplate);
        },
        removeNoFieldData:function(data){
            var keys=Object.keys(data);
            for(i=0;i<keys.length;i++){
                var key=keys[i];
                var detail=data[key];
                if(!detail || Object.keys(detail).length==0){
                    delete data[key];
                }
            }
            return data;
        },
        SetTemplateAttribte : function(currentElement, Others, Data, counter) {
            var allelements = [];
            allelements.push(currentElement);
            for (var i = 0; i < Others.length; i++) {
                var item = Others[i];
                allelements.push(item);
            }
            var allnames = DomService.GetAttributeNames(currentElement);
            for (var i = 0; i < Others.length; i++) {
                var el = Others[i];
                var names = DomService.GetAttributeNames(el);
                for (var j = 0; j < names.length; j++) {
                    var name = names[j];
                    if (allnames.indexOf(name) == -1) {
                        allnames.push(name);
                    }
                }
            }
            var valuelist = {};

            for (var i = 0; i < allnames.length; i++) {
                var attname = allnames[i];
                var attvalues = [];
                attvalues.push(currentElement.getAttribute(attname));

                for (var j = 0; j < Others.length; j++) {
                    var el = Others[j];
                    attvalues.push(el.getAttribute(attname));
                }
                valuelist[attname] = attvalues;
            }
           
            var TalAttributes = [];
            var keys=Object.keys(valuelist);
            for (var i = 0; i < keys.length; i++) {
                var valuekey = keys[i];
                var values = valuelist[valuekey];
                if (valuekey == "href") {
                    var urlpattern = DomService.GetUrlPattern(values);
                    if (urlpattern) {
                        for (var j = 0; j < urlpattern.Names.length; j++) {
                            var name = urlpattern.Names[j];
                            var namekey = TemplateManager.GetNewName(name, currentElement, counter, Data);
                            if (namekey != name) {
                                urlpattern.UrlPath.replace("{" + name + "}", "{" + namekey + "}");
                            }
                            //  Data[namekey] = urlpattern.Data[name];
                            this.AddValueToData(Data, namekey, urlpattern.Data[name]);
                        }
                        currentElement.setAttribute("href", urlpattern.FinalUrl());
                    }
                } else if (!DomService.HasSameValue(values)) {
                    var pair = {};
                    pair.Key = valuekey

                    // if (DomService.CountDistinct(values) == 2) {

                    //     if (IsActiveClass(valuekey, allelements)) {
                    //         var classname = DomService.GetActiveClass(allelements);
                    //         pair.Value = "{repeat/active:" + classname + "}";
                    //     } else if (DomService.IsRepeatFirst(values)) {
                    //         pair.Value = "{repeat/first:" + values[0] + "}";
                    //     } else if (DomService.IsRepeatLast(values)) {
                    //         var len = _.toArray(values).length;
                    //         pair.Value = "{repeat/last:" + values[len - 1] + "}";
                    //     } else if(DomService.IsNVaulues(values)){
                    //         var n= DomService.getN(values);
                    //         pair.Value = "{repeat/"+(n-1)+"n+1:"+values[n-1]+"}";
                    //     }else if (DomService.IsOddEvenValues(values)) {
                    //         pair.Value = "{repeat/odd:" + values[0] + ", repeat/even:" + values[1] + "}"
                    //     }
                    // } else {
                    //     var attkey = TemplateManager.GetNewName(valuekey, currentElement, counter, Data);
                    //     pair.Value = "{" + attkey + "}";
                    //     pair.Value = RepeaterHelper.KoobooTag.RepeatItemKey + "." + attkey;
                    //     // Data[attkey] = values;
                    //     this.AddValueToData(Data, attkey, values);
                    // }
                    var attkey = TemplateManager.GetNewName(valuekey, currentElement, counter, Data);
                    pair.Value = "{" + attkey + "}";
                    pair.Value = RepeaterHelper.KoobooTag.RepeatItemKey + "." + attkey;
                    // Data[attkey] = values;
                    this.AddValueToData(Data, attkey, values);
                    TalAttributes.push(pair);
                }
            }

            var talAttValue = "";
            for (var i = 0; i < TalAttributes.length; i++) {
                var attitem = TalAttributes[i];
                talAttValue += attitem.Key + " " + attitem.Value + ";";
                currentElement.removeAttribute(attitem.Key);
            }

            if (talAttValue) {
                talAttValue = _.trimEnd(talAttValue, ";");
                currentElement.setAttribute(RepeaterHelper.KoobooTag.Attribute, talAttValue);
            }

            function IsActiveClass(key, els) {

                if (!key || key.toLowerCase() != "class") {
                    return false;
                }

                var classname = DomService.GetActiveClass(els);
                if (classname) {
                    return true;
                }
                return false;
            }
        },
        SetTemplateSubTextNodes : function(templateElement, Others, Data, counter) {
            var values = [];
            values.push(templateElement.innerHTML);
            for (var i = 0; i < Others.length; i++) {
                var item = Others[i];
                values.push(item.innerHTML);
            }
            
            var key = TemplateManager.GetNewName(null, templateElement, counter, Data);
            var keyvalue = RepeaterHelper.KoobooTag.RepeatItemKey + "." + key;
            templateElement.setAttribute(RepeaterHelper.KoobooTag.Content, keyvalue);
            // Data[key] = values;
            this.AddValueToData(Data, key, values);
        },
        GetNonEmptySubNodes : function(element) {
            var nodes = [];
            for (var i = 0; i < element.childNodes.length; i++) {
                var node = element.childNodes[i];
                if (!DomService.IsEmptyTextNode(node)) {
                    nodes.push(node);
                }
            }
            return nodes;
        },
        SetTemplate : function(template, OtherElements, Data, counter) {
            if (counter == null) {
                counter = new Counter();
            }

            this.SetTemplateAttribte(template, OtherElements, Data, counter);

            var SubNodes = this.GetNonEmptySubNodes(template);
         
            if(template.tagName.toLowerCase()=="img") return;
            if (OnlyTextNodes(SubNodes)) {
                this.SetTemplateSubTextNodes(template, OtherElements, Data, counter);
                return;
            }

            var OtherSubNodes = [];

            for (var i = 0; i < OtherElements.length; i++) {
                var el = OtherElements[i];
                OtherSubNodes.push(this.GetNonEmptySubNodes(el));
            }

            var currenti = -1;
            for (var i = 0; i < SubNodes.length; i++) {
                var currentsubnode = SubNodes[i];

                var othersubndoes = GetSameNodesByIndex(i, currentsubnode, OtherSubNodes);

                if (othersubndoes && othersubndoes.length > 0) {
                    if (currentsubnode.nodeType == 1) {
                        var currentel = currentsubnode;
                        this.SetTemplate(currentel, _.toArray(othersubndoes), Data, counter);
                    } else {
                        if (!IsSameNonElementNodes(currentsubnode, othersubndoes)) {
                            currenti = i - 1;
                            break;
                        }
                    }
                } else {
                    currenti = i - 1;
                    break;
                }
                currenti = i;
            }

            /// check the back elements.... 
            var backcount = 0;

            if (currenti != -1 && currenti < SubNodes.length - 1) {

                while ((backcount + currenti) < SubNodes.length - 1) {

                    var currentBackNode = SubNodes[SubNodes.length - 1 - backcount];
                    var otherBackNodes = GetSameBackNodesByIndex(currenti, backcount, currentBackNode, OtherSubNodes);

                    if (otherBackNodes && otherBackNodes.length > 0) {
                        if (currentBackNode.nodeType == 1) {
                            var currentel = currentBackNode;
                            this.SetTemplate(currentel, _.toArray(otherBackNodes), Data, counter);
                        } else {
                            if (!IsSameNonElementNodes(currentBackNode, otherBackNodes)) {

                                break;
                            }
                        }
                    } else {
                        break;
                    }
                    backcount += 1;
                }
            }

            SetMiddleContent(template, SubNodes, OtherSubNodes, currenti, backcount, Data, counter);

            function removeEmptyData(Data){
                if(Data && Data.length>0){
                    for(var i=Data.length-1;i=0;i--){
                        if($.isEmptyObject(Data[i])){
                            delete Data[i];
                        }
                    }
                }
            }
            function IsSameNonElementNodes(node, others) {
                if (node.nodeType == 3) {
                    var value = node.nodeValue;

                    for (var i = 0; i < others.length; i++) {
                        var item = others[i];
                        var othervalue = item.nodeValue;
                        if (value != othervalue) {
                            return false;
                        }
                    }
                } else {
                    for (var i = 0; i < others.length; i++) {
                        var item = others[i];
                        if (node.nodeType != item.nodeType) {
                            return false;
                        }
                    }
                }

                return true;

            }

            function GetSameNodesByIndex(index, currentNode, OtherNodes) {

                var nodename = currentNode.nodeName;
                if (nodename) {
                    nodename = nodename.toLowerCase();
                }

                for (var i = 0; i < OtherNodes.length; i++) {
                    var item = OtherNodes[i];

                    if (index >= item.length) {
                        return null;
                    }
                }
                var result = [];

                for (var i = 0; i < OtherNodes.length; i++) {
                    var item = OtherNodes[i];
                    var node = item[index];
                    var currentNodeName = node.nodeName;
                    if (currentNodeName && currentNodeName.toLowerCase() == nodename) {
                        result.push(node);
                    } else {
                        return null;
                    }
                }
                return result;
            }

            function GetSameBackNodesByIndex(index, backindex, currentNode, OtherNodes) {

                var totalcount = index + backindex;
                for (var i = 0; i < OtherNodes.length; i++) {
                    var item = OtherNodes[i];
                    if (totalcount >= item.length - 1) {
                        return null;
                    }
                }

                var nodename = currentNode.nodeName;
                if (nodename) {
                    nodename = nodename.toLowerCase();
                }

                var result = [];
                for (var i = 0; i < OtherNodes.length; i++) {
                    var item = OtherNodes[i];
                    var node = item[item.length - backindex - 1]
                    var currentNodeName = node.nodeName;
                    if (currentNodeName && currentNodeName.toLowerCase() == nodename) {
                        result.push(node);
                    } else {
                        return null;
                    }
                }
                return result;
            }

            function OnlyTextNodes(nodes) {
                for (var i = 0; i < nodes.length; i++) {
                    var item = nodes[i];
                    if (item.nodeType == 1) {
                        return false;
                        //var el = item as HTMLElement;
                        //if (TagGroup.GetGroup(el) != TagGroup.EnumTagGroup.Text) {
                        //    return false;
                        //}
                    }
                }
                return true;
            }

            function GetText(nodes) {
                if (!nodes || nodes.length == 0) {
                    return null;
                }
                var result = "";
                for (var i = 0; i < nodes.length; i++) {
                    var item = nodes[i];
                    if (item.nodeType == 1) {
                        var el = item;
                        var cloneEl=el.cloneNode(true);
                        DomService.RemoveKoobooAttribute(cloneEl);
                        result +=DomService.GetNoCommentOuterHtml(cloneEl);
                    } else if(item.nodeType !=8) {
                        result += item.nodeValue;
                    }
                }
                return result;
            }

            function SetMiddleContent(currentElement, currentSubNodes, otherSubNodes, forwardi, backcount, Data, counter) {

                var middlenodes = GetMiddleNodes(currentSubNodes, forwardi, backcount);

                var valuelist = [];
                valuelist.push(GetText(middlenodes));
                for (var i = 0; i < otherSubNodes.length; i++) {
                    var item = otherSubNodes[i];
                    var middles = GetMiddleNodes(item, forwardi, backcount);
                    valuelist.push(GetText(middles));
                }

                if (IsEmptyList(valuelist)) {
                    return;
                }

                var newfieldname = TemplateManager.GetNewName(null, null, counter, Data);
                // Data[newfielname] = valuelist;
                TemplateManager.AddValueToData(Data, newfieldname, valuelist);

                var parent = currentSubNodes[0].parentElement;

                var newel = currentElement.ownerDocument.createElement("var");

                var keyvalue = RepeaterHelper.KoobooTag.RepeatItemKey + "." + newfieldname;
                newel.setAttribute(RepeaterHelper.KoobooTag.Replace, keyvalue);

                if (middlenodes && middlenodes.length > 0) {
                    parent.replaceChild(newel, middlenodes[0]);
                } else {
                    if (backcount == 0) {
                        parent.appendChild(newel);
                    } else {
                        var locationEl = currentSubNodes[currentSubNodes.length - backcount];
                        parent.insertBefore(newel, locationEl);
                    }
                }
                if (middlenodes != null && middlenodes.length > 0) {
                    for (var i = 1; i < middlenodes.length; i++) {
                        parent.removeChild(middlenodes[i]);
                    }
                }

                function GetMiddleNodes(subnodes, forwardindex, backwardindex) {

                    if ((forwardindex + backwardindex) >= subnodes.length - 1) {
                        return null;
                    }
                    var result = [];
                    var maxindex = subnodes.length - backwardindex;
                    var startindex = forwardindex + 1;
                    for (var i = startindex; i < maxindex; i++) {
                        result.push(subnodes[i]);
                    }
                    return result;
                }

                function IsEmptyList(input) {
                    if (!input) {
                        return true;
                    }

                    for (var i = 0; i < input.length; i++) {
                        var item = input[i];
                        if (item && item != "") {
                            return false;
                        }
                    }
                    return true;
                }
            }
        },
        GetNewName : function(attributeName, currentnode, counter, Data) {
            if (counter == null) {
                counter = new Counter();
            }

            var name = "";

            if (attributeName) {
                name = attributeName.toLowerCase();
                var attcounter = counter.GetSetAttributeCounter(attributeName);
                if (attcounter > 0) {
                    name = name + attcounter.toString();
                }
            } else {

                if (currentnode && currentnode.nodeType == 1) {
                    var el = currentnode;
                    var tagname = el.tagName.toLowerCase();
                    if (tagname == "h1" || tagname == "h2" || tagname == "h3" || tagname == "h4" || tagname == "h5" || tagname == "h6") {

                        name = "title";
                        if (counter.title > 0) {
                            name = name + counter.title.toString();
                        }
                        counter.title += 1;
                    } else {
                        name = "content";
                        var currentcounter = counter.content;
                        if (counter.content > 0) {
                            name = name + counter.content.toString();
                        }
                        counter.content = counter.content + 1;
                    }
                } else {
                    name = "content";
                    var currentcounter = counter.content;
                    if (counter.content > 0) {
                        name = name + counter.content.toString();
                    }
                    counter.content = counter.content + 1;
                }
            }

            if (Data[name] || (Data[0] && Data[0][name])) {
                return this.GetNewName(attributeName, currentnode, counter, Data);
            } else {
                return name;
            }
        },
        AddValueToData : function(Data, keyname, valueobject) {
            if ($.isArray(valueobject)) {
                var valuelist = _.toArray(valueobject);

                for(var i=valuelist.length-1;i>=0;i--){
                    var value = valuelist[i];
                    if (!Data[i]) {
                        Data[i] = {};
                    }
                    Data[i][keyname] = value;
                }
            } else {

                Data[keyname] = valueobject;
            }
        }
    }

    var Counter = function() {
        this.content = 0;
        this.title = 0;
        this.itemName = "ListItem";
        this.attribute = [];
    }
    Counter.prototype.GetSetAttributeCounter = function(attribteName) {
        if (attribteName) {
            var lower = attribteName.toLowerCase();

            var find = _.find(this.attribute, function(attr) {
                return attr.Name == attribteName;
            });
            if (find) {
                var currentcounter = find.CurrentCount;
                find.CurrentCount = currentcounter + 1;
                return currentcounter;
            } else {
                var newcounter = { Name: lower, CurrentCount: 0 };
                this.attribute.push(newcounter);
            }
        }
        return 0;
    }
    return {
        findSuperRepeater:findSuperRepeater,
        findSubRepeater:findSubRepeater,
        RepeaterHelper:RepeaterHelper,
        TemplateManager:TemplateManager
    }
}