function EditLinks(){
    var param={
        context:null,
        lighters:[]
    }
    function getData(){
        var sortLinks = getSortLinks();
        return {
            link : Kooboo.text.common.link,
            amount : Kooboo.text.inlineEditor.amount,
            empty : Kooboo.text.common.empty,
            edit : Kooboo.text.common.edit,
            sortLinks:sortLinks
        }
    }
    function getLinks(doc) {
        var links = $("a[kooboo-id]", doc);
        return removeAttrLink(links)
    }
    function removeAttrLink(links) {
        return _.filter(links, function(link) {
            //过滤content中绑定的属性链接
            var kattr = $(link).attr("k-attributes");
            return !kattr;
        });
    }

    function cleanUrlLastSlash(url) {
        if (url == "/") {
            return url;
        }
        return _.trimEnd(url, "/");
    }
    function createLinkData(model, href, id, link) {
        if (!model[href]) {
            model[href] = {
                elements: []
            };
        }
        model[href]["elements"].push({
            id: id,
            el: link
        });
    }
    function getSortLinks() {
        var doc = Kooboo.InlineEditor.getIFrameDoc();
        var $links = getLinks(doc),
            model = {};
        for (var j = 0; j < $links.length; j++) {
            var link = $links[j],
                href = cleanUrlLastSlash(link.getAttribute("href")),
                id = link.getAttribute("kooboo-id");

            createLinkData(model, href, id, link);
        }
        var sortLinks = [];
        for (var url in model) {
            var elements = model[url]["elements"];
            if (elements && elements.length > 0) {
                sortLinks.push({
                    url: url,
                    defaultUrl: url,
                    elements: elements,
                    amount: elements.length
                });
            }

        }
        sortLinks = _.sortBy(sortLinks, "url");
        return sortLinks;
    }
    function mouseover(linkNode, link) {
        if (link.elements && link.elements.length > 0 && link.elements[0].el)
            Kooboo.PluginHelper.scrollToElement(link.elements[0].el,function(){
                $.each(link.elements, function(index, element) {
                    Kooboo.PluginHelper.lighterElement(param.lighters, element.el, element.id, link.url);
                });
            });
    }
    function mouseout(linkNode, link) {
        Kooboo.PluginHelper.unLighterByGroup(param.lighters, link.url);
    }
    function editLink(linkNode, link) {
        var context = {};
        if (link.elements.length > 0) {
            //context.el = link.elements[0].el;
            context=Kooboo.PluginHelper.getContext(link.elements[0].el,param.context);
        }
        
        Kooboo.plugins.EditLink.dialogSetting.beforeSave=function(){
            var oldUrl = link.url;
            link.url = Kooboo.plugins.EditLink.getLinkUrl();
            if (link.url != oldUrl) {
                $(linkNode).find("u").html(link.url)
                setValue(oldUrl, link.url, link);
            }
            Kooboo.plugins.EditLink.dialogSetting.beforeSave=null;
        }
        Kooboo.PluginManager.click(Kooboo.plugins.EditLink,context);
        
    }
    function getAtrr(el) {
        var attr = "href"
        if ($(el).attr("k-href")) {
            attr = "k-href";
        }
        return attr;
    }
    //remove oldUrl
    function setValue(oldUrl, url, link) {
        $.each(link.elements, function(i, element) {
            var el = element.el;
            var data={
                domOperationType: Kooboo.SiteEditorTypes.DomOperationType.links,
                name: getAtrr(el),
                el: el,
                oldValue: link.defaultUrl,
                newValue: url,
            };
            param.context.editManager.editLink(data);
        });
    }
    return {
        dialogSetting:{
            title:Kooboo.text.inlineEditor.editLinks,
            width: "500px",
        },
        getHtml:function(context){
            k.setHtml("linksHtml","EditLinks.html");
            var data=getData();
            param.data=data;
            param.context=context;
            return _.template(linksHtml)(data);
        },
        init:function(){
            var linkNodes = $(".link-container");
            $.each(linkNodes, function(i, linkNode) {
                var link = param.data.sortLinks[i];
                $(linkNode).bind("mouseover", function() {
                    mouseover(linkNode, link);
                });
                $(linkNode).bind("mouseout", function() {
                    mouseout(linkNode, link);
                });
                $(linkNode).find("button").bind("click", function() {
                    editLink(linkNode, link);
                });
            });
        },

    }
}