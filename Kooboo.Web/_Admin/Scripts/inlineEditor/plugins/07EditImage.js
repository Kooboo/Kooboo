function EditImage(){
    var param={
        context:null
    }
    
    return {
        dialogSetting:{
            width:"400px",
            title:Kooboo.text.inlineEditor.editImage
        },
        menuName: Kooboo.text.inlineEditor.editImage,
        isShow: function(context) {
            param.context = context;
            if (Kooboo.PluginHelper.isBindVariable()) return false;

            return Kooboo.PluginHelper.imgRegExp.test(context.el.tagName) && 
            !Kooboo.PluginHelper.isDynamicData(context) && 
            !Kooboo.PluginHelper.isEmptyKoobooId(context.el);
        },
        getHtml:function(){
            return Kooboo.ImageHelper(param.context,true).getHtml();
        },
        init:function(){
            Kooboo.ImageHelper(param.context,true).init();
        },
    }
}