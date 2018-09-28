function ReplaceWithText(){
    var param={
        context:null,
        contentClass: ".tpContent",
        heightClass: ".tpHeight",
        widthClass: ".tpWidth",
    };
    function getValue() {
        return {
            value: Kooboo.PluginHelper.getElementSelector(param.contentClass).val(),
            height: Kooboo.PluginHelper.getElementSelector(param.heightClass).val(),
            width: Kooboo.PluginHelper.getElementSelector(param.widthClass).val()
        }
    }
    function getData(el){
        return {
            baseClass: "kb-text-panel",
            
            labelContent: Kooboo.text.common.Content,
            labelSize: Kooboo.text.common.size,
            value:"",
            height: $(el).height(),
            width: $(el).width()
        }
    }
    return {
        dialogSetting: {
            title: Kooboo.text.inlineEditor.replacewithText,
            width: "400px",
            showFoot: true
        },
        menuName: Kooboo.text.inlineEditor.replacewithText,
        isShow: function(context) {
            param.context = context;
            if (Kooboo.PluginHelper.isBindVariable()) return false;
            var el = context.el;
            var html = $(el).parent().html();
            return Kooboo.PluginHelper.isCanOperateType(context) && 
            Kooboo.PluginHelper.imgRegExp.test(el.tagName) &&
             !Kooboo.PluginHelper.isEmptyKoobooId(context.el) &&
                !Kooboo.PluginHelper.isContainDynamicData(html);
        },
        getHtml:function(){
            k.setHtml("textHtml","ReplaceWithText.html");
            var data=getData(param.context.el);
            var html=_.template(textHtml)(data);
            return html;
        },
        init:function(){
            var selectors = [param.contentClass, param.heightClass,param.widthClass];
            $.each(selectors, function(i, selector) {
                Kooboo.PluginHelper.getElementSelector(selector).bind("change", function() {
                    var newValue = getValue();
                    param.context.editManager.replaceWithText({
                        domOperationDetailType: Kooboo.SiteEditorTypes.DomOperationDetailType.htmlImageToText,
                        logType: Kooboo.SiteEditorTypes.LogType.tempLog,
                        context: param.context,
                        newValue: newValue
                    });
                });
            });
        },
    }
}