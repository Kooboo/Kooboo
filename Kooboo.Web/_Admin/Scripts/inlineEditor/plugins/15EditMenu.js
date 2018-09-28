function EditMenu(){
    var param={
        context:null
    };
    return {
        dialogSetting: {
            title: Kooboo.text.inlineEditor.editMenu,
            width:"80%",
            hideSaveBtn:true,
        },
        menuName: Kooboo.text.inlineEditor.editMenu,
        isShow: function(context) {
            param.context = context;
            if (Kooboo.PluginHelper.isBindVariable()) return false;
            var koobooObjects = param.context.koobooObjects;
            if (Kooboo.KoobooObjectManager.isMenu(koobooObjects)) return true;
            return false;
        },
        getHtml:function(){
            var nameOrId = Kooboo.KoobooObjectManager.getNameOrIdFromContext(param.context.koobooObjects, Kooboo.SiteEditorTypes.ObjectType.menu);
            var url = Kooboo.Route.Get(Kooboo.Route.Menu.DialogPage, {
                nameOrId: nameOrId
            });
            param.iframe= Kooboo.IframeDialog.getIframe(url);
            return param.iframe;
        },
        init:function(){
            Kooboo.IframeDialog.init(param.iframe);
            window.__gl.saveMenuFinish = function(newValue){
                param.context.editManager.editMenu({
                    action: Kooboo.SiteEditorTypes.ActionType.update,
                    editorType: Kooboo.SiteEditorTypes.EditorType.menu,
                    domOperationType: Kooboo.SiteEditorTypes.DomOperationType.menu,
                    logType: Kooboo.SiteEditorTypes.LogType.noLog,
                    context: param.context,
                    newValue: newValue
                });
            }
        }
    }
}