function PluginManager(){
    function save(context,beforeSave){
        if(beforeSave)
            beforeSave();
        context && context.editManager && context.editManager.saveLogs();
    }
    function close(context,beforeClose){
        if(beforeClose)
            beforeClose();
        context && context.editManager && context.editManager.removeLogs();
    }
    
    return {
        getPlugins:function(){
            var plugins = Kooboo.plugins;
            var menuItems=[];
            $.each(plugins, function(i, plugin) {
                menuItems.push(plugin);
            });
            return menuItems;
        },
        getShowPlugins:function(context){
            var showMenuItems={};
            var menuItems=this.getPlugins();
            $.each(menuItems, function(i, editor) {
                var isShow = editor.isShow(context);
                if(isShow)
                    showMenuItems[i]=editor;
            });
            return showMenuItems;
        },
        click:function(plugin,context){
            var html=plugin.getHtml(context);
            if(html){
                var dialog=Kooboo.Dialog({
                    save:save,
                    close:close,
                    context:context,
                    beforeSave:plugin.dialogSetting.beforeSave,
                    beforeClose:plugin.dialogSetting.beforeClose
                });
                var dialogHtml=dialog.getHtml(plugin.dialogSetting);
                var dialogContainer= $(dialogHtml).appendTo($("body"));
                dialog.setContainer(dialogContainer);
                $(dialogContainer).find(".dialog-body").append(html);
                
                dialog.init();
                plugin.init();
            }else{
                if(plugin.click)
                    plugin.click();
            }
        },

    }
}