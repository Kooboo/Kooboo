function DomOperationHelper(){
    var copyRepeaterFrom={};
    function getRptCopyFromId(contentId) {
        var copyFromId = contentId;
        while (copyRepeaterFrom[copyFromId]) {
            copyFromId = copyRepeaterFrom[contentId];
        }
        return copyFromId;
    }
    function addRptCopyFrom(newContentId, contentId) {
        copyRepeaterFrom[newContentId] = contentId;
    }
    return {
        removeCssRuleProp:function(cssRule,property){
            if(cssRule&& cssRule.removeProperty){
                cssRule.removeProperty(property);
            }
        },
        resetCssRule:function(cssRule, property, value, important) {
            //!important can't change the cssrule,only important can change the cssrule
            var priority = important ? "important" : "";
            if (cssRule && cssRule.setProperty) {
                cssRule.setProperty(property, value, priority);
            } else if (cssRule && cssRule.style && cssRule.style.setProperty) {
                cssRule.style.setProperty(property, value, priority);
            }
        },
        getHtml : function(el) {
            return $(el).html();
        },
        updateHtml:function(el, value){
            el.innerHTML = value;
        },
        updateAttribute:function(el, attrName, attributeValue){
            $(el).attr(attrName, attributeValue);
        },
        copyHtml:function(el) {
            var cloneEl = $(el).clone();
            $(cloneEl).insertAfter($(el));
            Kooboo.KoobooElementManager.resetElementKoobooId(cloneEl[0]);
        },
        deleteHtml:function(el) {
            $(el).remove();
        },
        updateStyle:function(el, styleValue) {
            $(el).attr("style", styleValue);
        },
        updateImage:function(el, imageParams) {
            var $el = $(el);
            $el.attr("src", imageParams.src);
            $el.attr("alt", imageParams.alt);
            $el.attr("title", imageParams.title);
    
            $(el).height(imageParams.height);
            $(el).width(imageParams.width);
        },
        replaceWithImage:function(el, value) {
            var $img = $("<img>");
            $img.attr("src", value["src"]);
            $img.attr("alt", value["alt"]);
            $img.attr("title", value["title"]);
    
            $img.height(value["height"]);
            $img.width(value["width"]);
            $(el).replaceWith($img);
            return $img[0];
        },
        replaceWithText:function(el, value) {
            var $div = $("<div>");
            $div.text(value["value"]);
            $div.height(value["height"]);
            $div.width(value["width"]);
            $div.css("color", "black");
            $(el).replaceWith($div);
            return $div[0];
        },
        updateText:function(el, value) {
            var $el = $(el);
            $el.text(value["value"]);
            $el.height(value["height"]);
            $el.width(value["width"]);
        },
        getDomOperationByType : function(type, item) {
            var domOperationType = Kooboo.SiteEditorTypes.DomOperationType;
            switch (type) {
                case domOperationType.contentRepeater:
                    return Kooboo.dom.DomOperationRepeater(item);
                case domOperationType.menu:
                    return  Kooboo.dom.DomOperationMenu(item);
                case domOperationType.html:
                case domOperationType.content:
                    return Kooboo.dom.DomOperationHtml(item);
                case domOperationType.label:
                    return Kooboo.dom.DomOperationLabel(item);
                case domOperationType.attribute:
                    return Kooboo.dom.DomOperationAttribute(item);
                case domOperationType.image:
                    return Kooboo.dom.DomOperationImage(item);
                case domOperationType.styles:
                    return Kooboo.dom.DomOperationStyles(item);
                case domOperationType.htmlblock:
                    return Kooboo.dom.DomOperationHtmlBlock(item);
                case domOperationType.images:
                    return Kooboo.dom.DomOperationImages(item);
                case domOperationType.links:
                    return Kooboo.dom.DomOperationLinks(item);
                case domOperationType.converter:
                    return Kooboo.dom.DomOperationConverter(item);
                // case domOperationType.dragger:
                //     return new DomOperation.Dragger(item);
            }
            return null;
        },
        addRptCopyFrom:addRptCopyFrom,
        getRptCopyFromId:getRptCopyFromId
    }
}