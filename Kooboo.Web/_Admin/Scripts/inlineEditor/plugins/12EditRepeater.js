function EditRepeater() {
    var param = {
        context: null,
        nameOrId: "",
    }

    function beforeSave() {
        var result = window.__gl.saveContent();
        return result;
    }

    function getUrl() {
        var nameOrId = Kooboo.KoobooObjectManager.getSelectedRepeaterNameOrId(param.context.koobooObjects);

        var siteId = Kooboo.getQueryString("SiteId");
        param.nameOrId = nameOrId;

        var url = Kooboo.Route.Get(Kooboo.Route.TextContent.DialogPage, {
            Id: nameOrId,
            SiteId: siteId,
        });
        return url;
    }
    return {
        dialogSetting: {
            title: Kooboo.text.inlineEditor.editRepeaterItem,
            beforeSave: beforeSave,
            width: "90%"
        },
        menuName: Kooboo.text.inlineEditor.editRepeaterItem,
        isShow: function(context) {
            param.context = context;
            if (Kooboo.PluginHelper.isBindVariable()) return false;
            var el = context.el;
            var koobooObjects = context.koobooObjects;
            if (Kooboo.KoobooObjectManager.isContentAttribute(el)) return true;

            if (Kooboo.KoobooObjectManager.isContentRepeater(koobooObjects)) return true;
            if (Kooboo.KoobooObjectManager.isContent(koobooObjects)) return true;
            return false;
        },
        getHtml: function() {
            var isCopyRepeater = Kooboo.PluginHelper.isCopyRepeater(Kooboo.SiteEditorTypes.DomOperationType.contentRepeater, param.context);
            if (isCopyRepeater) {
                window.info.show(Kooboo.text.inlineEditor.cantEditCopyItem, false);
                return "";
            }
            var url = getUrl();
            param.iframe = Kooboo.IframeDialog.getIframe(url);
            return param.iframe;
        },
        init: function() {
            Kooboo.IframeDialog.init(param.iframe);
            window.__gl.saveContentFinish = function(data) {
                param.context.editManager.editContent({
                    action: Kooboo.SiteEditorTypes.ActionType.update,
                    editorType: Kooboo.SiteEditorTypes.EditorType.content,
                    domOperationType: Kooboo.SiteEditorTypes.DomOperationType.contentRepeater,
                    logType: Kooboo.SiteEditorTypes.LogType.noLog,
                    nameOrId: param.nameOrId,
                    context: param.context,
                    newValue: data,
                });
            }
        }
    }
}