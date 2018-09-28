function HtmlConverter(beforeSave){
    return {
        dialogSetting:{
            title: Kooboo.text.inlineEditor.editHtml,
            width:"800px",
            zIndex:Kooboo.InlineEditor.zIndex.middleZIndex,
            beforeSave:beforeSave
        },
        getHtml:function(){
            return '<div class="kb-dialog-editor"><div class="htmlEditPopup"></div></div>';
        },
        init:function(){
            this.cm = CodeMirror($(".htmlEditPopup")[0], {
                lineNumbers: true,
                mode: "htmlmixed",
                indentUnit: 4,
                indentWithTabs: true,
                styleActiveLine: true,
                styleSelectedText: true,
                value: ""
            });
        },
        setConverterHtml:function(html){
            this.cm.setValue(html);
            this.cm.refresh(); //codeMirror bug ,need initialize first
        },
        getConverterHtml:function() {
            return this.cm.getValue();
        }
    }
}