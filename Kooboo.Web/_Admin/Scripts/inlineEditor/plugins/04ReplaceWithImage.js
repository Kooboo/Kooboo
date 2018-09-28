function ReplaceWithImage(){
    var param={
        context:null,
    }
    return {
        dialogSetting:{
            width:"400px",
            title:Kooboo.text.inlineEditor.replacewithImage
        },
        menuName: Kooboo.text.inlineEditor.replacewithImage,
        isShow: function(context) {
            param.context = context;
            if (Kooboo.PluginHelper.isBindVariable()) return false;
            var el = context.el;
            var html = $(el).parent().html();
            return !Kooboo.PluginHelper.isBodyTag(context.el) && 
                Kooboo.PluginHelper.isCanOperateType(context) && 
                !Kooboo.PluginHelper.imgRegExp.test(el.tagName) && 
                !Kooboo.PluginHelper.isEmptyKoobooId(context.el) &&
                !Kooboo.PluginHelper.isContainDynamicData(html);
        },
        getHtml:function(){
            return Kooboo.ImageHelper(param.context,false).getHtml();
        },
        init:function(){
            Kooboo.ImageHelper(param.context,false).init();
        },
       
    }
}