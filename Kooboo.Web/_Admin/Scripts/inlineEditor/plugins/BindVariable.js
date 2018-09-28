function BindVariable(){
    var model= "BindVariable";
    return {
        menuName: Kooboo.text.inlineEditor.newVariable,
        isShow: function(context) {
            this.context = context;
            if (Kooboo.PluginHelper.isBindVariable()) return true;
            return false;
        },
        getHtml:function(){
            return "";
        },
        init:function(){

        },
        click: function() {
            var koobooid=$(this.context.el).attr("kooboo-id");
            var data = {
                type: "label",
                title: Kooboo.text.inlineEditor.newVariable,
                elem:this.context.el,
                text: Kooboo.VariableDataManager.getVariableName(koobooid),
                name:Kooboo.text.inlineEditor.createVariable,
                placeholder:Kooboo.text.inlineEditor.newVariable
            };
            var context=this.context;
            Kooboo.EventBus.publish("binding/edit", data);
            Kooboo.EventBus.subscribe("binding/save", function(data) {
                Kooboo.VariableDataManager.add($(data.elem).attr("kooboo-id"),data.text);
                Kooboo.EventBus.unsubscribe("binding/save");
            });
        },
    }
}