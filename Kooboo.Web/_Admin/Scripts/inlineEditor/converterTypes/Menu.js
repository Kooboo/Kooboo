function Menu(){
    return {
        name: "Menu",
        displayName: "Menu",
        execute: function(element) {
            var clonedElement = element.cloneNode(true);
            var result = Kooboo.MenuHelper.GetClickConvertResult(element);
            result.convertToType=this.name;
            result.name=this.name;
            return result;
        },
    }
}