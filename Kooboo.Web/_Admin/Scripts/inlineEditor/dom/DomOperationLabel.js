function DomOperationLabel(item){
    return {
        update: function(value) {
            var action=item.action;
            switch(action){
                case Kooboo.SiteEditorTypes.ActionType.update:
                    var firstWrapObject = Kooboo.KoobooElementManager.getFirstWrapKoobooObject(item);
                    item.el = Kooboo.KoobooElementManager.getElementByEl(item.el, firstWrapObject);
                    Kooboo.dom.DomOperationHelper.updateHtml(item.el, value);
                    break;
            }
        }
    }
}