function ConverterLabel(text, setting, remove){
    var param={
        labelNode:null,
        el:null
    }
    return {
        init:function(){
            k.setHtml("labelHtml","ConverterLabel.html");
            var html=_.template(labelHtml)({text:text,zIndex:Kooboo.InlineEditor.zIndex.middleZIndex});
            param.labelNode=$(html).appendTo($("body"));

            $(param.labelNode).find(".btnSetting").bind("click", function() {
                setting();
            });

            $(param.labelNode).find(".btnRemove").bind("click", function() {
                remove();
            });
        },
        mask: function(context) {
            param.el = context.el;
            var width = $(param.labelNode).outerWidth(),
                rect = param.el.getBoundingClientRect();
            var offset = Kooboo.PluginHelper.getOffset();
            $(param.labelNode).css({
                top: rect.top - offset.top + 1,
                left: (rect.right + offset.left - width - 1 > 8) ? rect.right - width - 1 : 0
            });
        },
        showRemoveBtn: function() {
            $(param.labelNode).find(".btnRemove").show();
            this.mask({ el: param.el });
        },
        destroy: function() {
            delete param.el;
            $(param.labelNode).remove();
        }
        
    }
}