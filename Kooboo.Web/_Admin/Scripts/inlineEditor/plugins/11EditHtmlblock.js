function EditHtmlblock(){
    var param={
        context:null,
        iframe:null
    }
     //绑定保存的事件
    function beforeSave() {
        if (window.__gl.saveHtmlblock) {
            window.__gl.saveHtmlblock();
        }
        return true;
    }
    return {
        dialogSetting: {
            title: Kooboo.text.inlineEditor.editHtmlblock,
            width:"80%",
            beforeSave: beforeSave
        },
        menuName: Kooboo.text.inlineEditor.editHtmlblock,
        isShow: function(context) {
            param.context = context;
            if (Kooboo.PluginHelper.isBindVariable()) return false;
            return Kooboo.KoobooObjectManager.isHtmlBlock(context.koobooObjects);
        },
        getHtml:function(){
            var nameOrId = Kooboo.KoobooObjectManager.getNameOrIdFromContext(param.context.koobooObjects, Kooboo.SiteEditorTypes.ObjectType.htmlblock);
            var url = Kooboo.Route.Get(Kooboo.Route.HtmlBlock.DialogPage, { nameOrId: nameOrId });
            param.iframe= Kooboo.IframeDialog.getIframe(url);
            return param.iframe;
        },
        init:function(){
            Kooboo.IframeDialog.init(param.iframe);
            window.__gl.saveHtmlblockFinish = function(newValue){
                param.context.editManager.editHtmlBlock({
                    action: Kooboo.SiteEditorTypes.ActionType.update,
                    editorType: Kooboo.SiteEditorTypes.EditorType.htmlblock,
                    domOperationType: Kooboo.SiteEditorTypes.DomOperationType.htmlblock,
                    logType: Kooboo.SiteEditorTypes.LogType.noLog,
                    context: param.context,
                    newValue: newValue
                });
            }
        }
    }
}