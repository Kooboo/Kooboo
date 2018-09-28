function Copy(){
    var param={
        context:null
    }
    return {
        menuName: Kooboo.text.inlineEditor.copyBlock,
        isShow: function(context) {
            param.context = context;
            if (Kooboo.PluginHelper.isBindVariable()) return false;
            var el = context.el;
            var html = $(el).parent().html();
    
            return !Kooboo.PluginHelper.isBodyTag(context.el) && 
                    Kooboo.PluginHelper.isCanOperateType(context) &&//cotent/contentRepeater can't be copy.
                    !Kooboo.PluginHelper.isEmptyKoobooId(context.el) &&
                    !Kooboo.PluginHelper.isContainDynamicData(html);
        },
        getHtml:function(){
            return "";
        },
        init:function(){

        },
        click: function() {
            param.context.editManager.updateDom({
                domOperationDetailType: Kooboo.SiteEditorTypes.DomOperationDetailType.copy,
                logType: Kooboo.SiteEditorTypes.LogType.log,
                context: param.context,
            });
        }
    }
    
}