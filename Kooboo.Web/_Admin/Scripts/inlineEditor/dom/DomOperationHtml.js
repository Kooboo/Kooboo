function DomOperationHtml(item){
    var param={
        item:item
    }
    function needResetValue() {
        if (param.item.newValue == "undefined") {
            return true;
        }
        if (typeof param.item.newValue != "string")
            return true;
        return false;
    }
    function hasStringValue(value) {
        return value && typeof value == "string";
    }
    return {
        update: function(value) {
            try {
                if (needResetValue()) {
                    switch (param.item.domOperationDetailType) {
                        case Kooboo.SiteEditorTypes.DomOperationDetailType.htmlTextToImage:
                            if (!param.item.context.replaceImageEl) {
                                param.item.context.parentEl = $(param.item.context.el).parent()[0];
                                
                                param.item.oldValue = Kooboo.InlineEditor.cleanUnnecessaryHtml(param.item.context.parentEl);
                                param.item.context.replaceImageEl = Kooboo.dom.DomOperationHelper.replaceWithImage(param.item.context.el, value);
                                Kooboo.KoobooElementManager.resetElementKoobooId(param.item.context.replaceImageEl);
                            } else {
                                param.item.oldValue = Kooboo.InlineEditor.cleanUnnecessaryHtml(param.item.context.parentEl);
                                Kooboo.dom.DomOperationHelper.updateImage(param.item.context.replaceImageEl, value);
                            }
                            param.item.el = param.item.context.parentEl;
                            param.item.newValue = Kooboo.InlineEditor.cleanUnnecessaryHtml(param.item.el);

                            break;
                        case Kooboo.SiteEditorTypes.DomOperationDetailType.htmlImageToText:
                            if (!param.item.context.replaceTextEl) {
                                param.item.context.parentEl = $(param.item.context.el).parent()[0];
                                
                                param.item.oldValue = Kooboo.InlineEditor.cleanUnnecessaryHtml(param.item.context.parentEl);
                                param.item.context.replaceTextEl = Kooboo.dom.DomOperationHelper.replaceWithText(param.item.context.el, value);
                                Kooboo.KoobooElementManager.resetElementKoobooId(param.item.context.replaceTextEl);
                            } else {
                                param.item.oldValue = Kooboo.InlineEditor.cleanUnnecessaryHtml(param.item.context.parentEl);
                                Kooboo.dom.DomOperationHelper.updateText(param.item.context.replaceTextEl, value)
                            }
                            param.item.el = param.item.context.parentEl;
                            param.item.newValue = Kooboo.InlineEditor.cleanUnnecessaryHtml(param.item.el);
                            break;
                        case Kooboo.SiteEditorTypes.DomOperationDetailType.copy:
                        case Kooboo.SiteEditorTypes.DomOperationDetailType.delete:
                            var $pt = $(param.item.context.el).parent();
                            var oldValue = Kooboo.InlineEditor.cleanUnnecessaryHtml($pt);
                            if (Kooboo.SiteEditorTypes.DomOperationDetailType.copy == param.item.domOperationDetailType) {
                                Kooboo.dom.DomOperationHelper.copyHtml(param.item.context.el);
                            } else {
                                Kooboo.dom.DomOperationHelper.deleteHtml(param.item.context.el);
                            }
                            var newValue = Kooboo.InlineEditor.cleanUnnecessaryHtml($pt[0]);
                            param.item.oldValue = oldValue;
                            param.item.newValue = newValue;
                            param.item.el =$pt[0];
                            break;
                        case Kooboo.SiteEditorTypes.DomOperationDetailType.editTreeData:
                            var replaceItems = $(param.item.replaceHtml);

                            var firstTreeItem = param.item.context.treeitems[0];
                            replaceItems.insertBefore(firstTreeItem);

                            $.each(param.item.context.treeitems, function(i, treeItem) {
                                treeItem.remove();
                            });
                            //reset copy element koobooid
                            Kooboo.KoobooElementManager.resetCloneElementsKoobooId(replaceItems);

                            param.item.context.treeitems = replaceItems;
                            param.item.newValue = Kooboo.InlineEditor.cleanUnnecessaryHtml(param.item.el);
                            break;
                    }
                    //save the element firstWrap object,to prevent the el reference change by other dom DomOperation
                    //like dom is delete 
                    if (param.item.el) {
                        Kooboo.KoobooElementManager.setFirstWrapObject(param.item);
                    }
                } else {
                    var firstWrapObject = Kooboo.KoobooElementManager.getFirstWrapKoobooObject(param.item);
                    param.item.el = Kooboo.KoobooElementManager.getElementByEl(param.item.el, firstWrapObject);
                    Kooboo.dom.DomOperationHelper.updateHtml(param.item.el, value);
                }
            } finally {
                
            }
        },
    }
}