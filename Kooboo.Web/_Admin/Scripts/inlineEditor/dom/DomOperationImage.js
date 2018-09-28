function DomOperationImage(item){
    return {
        update: function(value) {
            var firstWrapObject = Kooboo.KoobooElementManager.getFirstWrapKoobooObject(item);
            item.el = Kooboo.KoobooElementManager.getElementByEl(item.el, firstWrapObject);
            Kooboo.dom.DomOperationHelper.updateImage(item.el, value);
        }
    }
}