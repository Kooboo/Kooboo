function SiteEditorTypes(){
    var types = {};
    types.ActionType = {
        add: "add",
        update: "update", //update
        delete: "delete",
        copy: "copy"
    };
    types.EditorType = {
        dom: "dom",
        htmlblock: "htmlblock",
        label: "label",
        content: "content",
        attribute: "attribute",
        style: "style",
        converter: "converter"
    };
    //element's belong to ObjectType
    types.ObjectType = {
        content: "content",
        contentrepeater: "contentrepeater",
        htmlblock: "htmlblock",
        label: "label",
        view: "view",
        menu: "menu",
        page: "page",
        style: "style", //update image-obsolete
        dom: "dom", //update image-obsolete
        attribute: "attribute",
        layout: "layout"
    };
    //View,Repeater
    types.DomOperationType = {
        html: "html",
        attribute: "attribute",
        style: "style",
        image: "image",
        images: "images",
        links: "links",
        bgImage: "bgImage",
        styles: "styles",
        attributes: "attributes",
        htmlblock: "htmlblock",
        content: "content",
        contentRepeater: "contentRepeater",
        converter: "converter",
        label: "label",
        dragger: "dragger",
        menu: "menu"
    };
    types.DomOperationDetailType = {
        htmlTextToImage: "htmlTextToImage",
        htmlImageToText: "htmlImageToText",
        copy: "copy",
        delete: "delete",
        edit: "edit",
        editTreeData: "editTreeData"
    };
    types.LogType = {
        tempLog: "tempLog",
        log: "log",
        noLog: "noLog"
    };
    return types;
}