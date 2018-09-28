function Dialog(opt){
    var save=opt.save,
        close=opt.close,
        context=opt.context;
    //var container=".dialog-container";
    function destroy(){
        $(opt.dialogContainer).remove();
    }
    return {
        setContainer:function(container){
            opt.dialogContainer=container;
        },
        getHtml:function(dialogSetting){
            k.setHtml("dialogHtml","Dialog.html");

            var data={
                baseClass: "modal-dialog",
                title: "Title",
                width: "auto",
                height: "auto",
                zIndex: Kooboo.InlineEditor.zIndex.middleZIndex,
                note:null,
                showFoot:true,
                hideSaveBtn:false
            }
            $.extend(data,dialogSetting);
            var html=_.template(dialogHtml)(data);
            return html;
        },
        init:function(){
            $(opt.dialogContainer).modal();
            $(opt.dialogContainer).on('hidden.bs.modal', function() {
                close(context,opt.beforeClose);
                destroy();
            });

            var titleBar = $(opt.dialogContainer).find(".titleBar")[0];
            var contentNode = $(opt.dialogContainer).find(".contentNode")[0];
            Kooboo.DialogMoveable(titleBar,contentNode).init();
            
            $(opt.dialogContainer).find(".js-btn-close").bind("click", function() {
                close(context,opt.beforeClose);
                destroy();
            });
            $(opt.dialogContainer).find(".js-btn-save").bind("click", function() {
                save(context,opt.beforeSave);
                destroy();
            });
            $(opt.dialogContainer).css("overflow-y", "auto");

        }
    }
}