function DomOperationLinks(item){
   return {
    update: function(value) {
        var firstWrapObject = Kooboo.KoobooElementManager.getFirstWrapKoobooObject(item);
        //动态获取el,多次操作copy/delete，会导致el的引用发生变化.
        item.el = Kooboo.KoobooElementManager.getElementByEl(item.el, firstWrapObject);

        $(item.el).attr("href", value);
    }
   }
}