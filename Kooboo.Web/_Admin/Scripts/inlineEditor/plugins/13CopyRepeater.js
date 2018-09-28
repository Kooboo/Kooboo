function CopyRepeater(){
    var param={
        context:null
    }
    return {
        menuName: Kooboo.text.inlineEditor.copyRepeaterItem,
        isShow: function(context) {
            param.context = context;
            if (Kooboo.PluginHelper.isBindVariable()) return false;
            var koobooObjects = context.koobooObjects;
            if (Kooboo.KoobooObjectManager.isContentRepeater(koobooObjects)) return true;
            return false;
        },
        getHtml:function(){
            "";
        },
        init:function(){

        },
        click: function() {
            var nameOrId = Kooboo.KoobooObjectManager.getSelectedRepeaterNameOrId(param.context.koobooObjects);
            param.context.editManager.copyContent({
                action: Kooboo.SiteEditorTypes.ActionType.copy,
                editorType: Kooboo.SiteEditorTypes.EditorType.content,
                domOperationType: Kooboo.SiteEditorTypes.DomOperationType.contentRepeater,
                logType: Kooboo.SiteEditorTypes.LogType.log,
                context: param.context,
                nameOrId: nameOrId,
            });
        },
        
    }
}