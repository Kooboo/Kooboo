function ContentList(){
    return {
        name: "ContentList",
        displayName: "Content List",
        execute: function(element) {
            var result = {
                convertToType: this.name,
                name: this.name
            }

            var repeater;
            try {
                repeater = Kooboo.Repeater.findSubRepeater(element);
                if(!repeater){
                    repeater = Kooboo.Repeater.findSuperRepeater(element);
                }
            } catch (ex) {

            }

            if (repeater != null && repeater.length > 1) {
                var tempate = Kooboo.Repeater.TemplateManager.GetTemplate(repeater);

                var link = (repeater[0].tagName.toLowerCase() !== "a") ? $(repeater[0]).find("a").attr("href") : $(repeater[0]).attr("href");
                if (!link) {
                    result.contentLink = null;
                } else if (!link.match(/^#/)) {
                    result.contentLink = link;
                }
                result.editElement = tempate.commonParent;
                result.data = tempate.Data;
                result.htmlBody = tempate.HtmlBody;
                result.hasResult = true;
                result.koobooId= $(tempate.commonParent).attr("kooboo-id");
            }
            return result;

        },
       
    }
}