function HtmlBlock(){
    return  {
        name: "HtmlBlock",
        displayName: "HtmlBlock",
        execute: function(element) {
            var clonedElement = element.cloneNode(true);
            var result = {
                convertToType: this.name,
                name: this.name,
                koobooId: $(element).attr("kooboo-id")
            }
            result.editElement = element;
            Kooboo.NewDomService.RemoveKoobooAttribute(clonedElement);
            result.htmlBody = Kooboo.NewDomService.GetNoCommentOuterHtml(clonedElement);
            result.hasResult = true;
            return result;
        },
    }
}