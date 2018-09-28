function Delete(){
    var helper=Kooboo.PluginHelper;
    return {
        menuName: Kooboo.text.common.delete,
        isShow: function(context) {
            this.context = context;
            if (helper.isBindVariable()) return false;
            var el = this.context.el;
            var html = $(el).parent().html();
            var object = Kooboo.KoobooObjectManager.getFirstWrapKoobooObject(el);
            if(object&& object.objectType.toLowerCase()==Kooboo.SiteEditorTypes.ObjectType.label){
                this.objectType=Kooboo.SiteEditorTypes.ObjectType.label;
                return true;
            }
            
            return !helper.isBodyTag(this.context.el) && 
                helper.isCanOperateType(this.context) && 
                !helper.isEmptyKoobooId(this.context.el) &&
                !helper.isContainDynamicData(html);
        },
        getHtml:function(){
            "";
        },
        init:function(){

        },
        click: function() {
            if(this.objectType==Kooboo.SiteEditorTypes.ObjectType.label){
                this.context.editManager.deleteDom({
                    domOperationDetailType: Kooboo.SiteEditorTypes.DomOperationDetailType.delete,
                    logType: Kooboo.SiteEditorTypes.LogType.log,
                    context: this.context,
                    el:this.context.el
                    //el: $pt[0],
                    //oldValue: oldValue,
                    //newValue: newValue
                });
            }else{
                this.context.editManager.updateDom({
                    domOperationDetailType: Kooboo.SiteEditorTypes.DomOperationDetailType.delete,
                    logType: Kooboo.SiteEditorTypes.LogType.log,
                    context: this.context,
                    //el: $pt[0],
                    //oldValue: oldValue,
                    //newValue: newValue
                });
            }
            
        },
    }
}