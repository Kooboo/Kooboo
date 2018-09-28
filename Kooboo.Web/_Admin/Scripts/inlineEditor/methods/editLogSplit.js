function EditLogSplit(){
    function getItemByContext(context) {
        var splitItem = {
            action: Kooboo.SiteEditorTypes.ActionType.update,
            editorType: Kooboo.SiteEditorTypes.EditorType.dom,
            domOperationType: Kooboo.SiteEditorTypes.DomOperationType.html,
            oldValue: context.oldValue,
            el: context.el,
            newValue: removeGuid(context.newValue)

        };
        return splitItem;
    }
    function removeGuid(value) {
        var reg = new RegExp("guid=\"[0-9-]+\"");
        if (!value) return "";

        while (reg.test(value)) {
            value = value.replace(reg, "");
        }

        return value;
    }
    return {
        getSplitItems:function(item) {
            var items = [];
            if (item.domOperationType == Kooboo.SiteEditorTypes.DomOperationType.dragger) {
                items.push(getItemByContext(item.startDragContext));
                items.push(getItemByContext(item.endDragContext));
            } else {
                items.push(item);
            }
            return items;
        }
    }
}