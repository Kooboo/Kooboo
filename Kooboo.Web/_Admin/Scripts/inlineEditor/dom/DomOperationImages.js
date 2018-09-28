function DomOperationImages(item){

    return {
        update: function(value) {
            if (item.isContentImage) {
                var firstWrapObject = Kooboo.KoobooElementManager.getFirstWrapKoobooObject(item);
                //动态获取el,多次操作copy/delete，会导致el的引用发生变化.
                item.el = Kooboo.KoobooElementManager.getElementByEl(item.el, firstWrapObject);
                $(item.el).attr(item.attr, value);
            } else {
                Kooboo.dom.DomOperationHelper.resetCssRule(item.cssRule, item.property, value, item.important);
            }
        },
    }
}