function DomOperationAttribute(item){
    function isKhref(name) {
        return name.toLowerCase() == "k-href";
    }
    function getAttrName(name) {
        var attName = name;
        if (isKhref(attName)) {
            attName = "href";
        }
        return attName;
    }
    return {
        update: function(value) {
            var attName = getAttrName(item.name);
            var firstWrapObject = Kooboo.KoobooElementManager.getFirstWrapKoobooObject(item);
            item.el = Kooboo.KoobooElementManager.getElementByEl(item.el, firstWrapObject);
            Kooboo.dom.DomOperationHelper.updateAttribute(item.el, attName, value);
        }
    }
}