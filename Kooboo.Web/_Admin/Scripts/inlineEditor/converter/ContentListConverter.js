function ContentListConverter(convertResult,context){
    var param={
        iframe:null,
    }
    function beforeSave(){

        var bindResult=Kooboo.VariableDataManager.getBindResult();
        var bindObject = {
            contentUrl: convertResult.contentLink,
            bindResult: bindResult
        };

        var _bindExsit = _.filter(convertResult.bindObject, function(bo) {
            return bo.contentUrl === bindObject.contentUrl;
        });

        if (_bindExsit.length) {
            _.remove(convertResult.bindObject, function(bo) {
                return bo.contentUrl === bindObject.contentUrl
            });
        }

        convertResult.bindObject = bindObject;
        Kooboo.VariableDataManager.setModel("EditHtml");
        window.__gl.domSelector.destroy();
        window.__gl.domSelector=window.__gl.backDomSelector;
        return true;
    }
    function beforeClose(){
        Kooboo.VariableDataManager.clearCurrentVariables();
        Kooboo.VariableDataManager.setModel("EditHtml");
        window.__gl.domSelector.destroy();
        window.__gl.domSelector=window.__gl.backDomSelector;
    }
    function iframeLoaded(iframe){
        var domSelector=Kooboo.DomSelector({
            style:{borderWidth: 1,zIndex:Kooboo.InlineEditor.zIndex.middleZIndex,borderStyle: "solid"},
            onClick:function(e,element){
                var newcontext=Kooboo.PluginHelper.getContext(element,context);
                newcontext.offsetObj=param.iframe;
                Kooboo.FloatMenu.show(newcontext,e);
            },
            offsetObj:param.iframe
        }, iframe.contentDocument,window.document);
        domSelector.start();
        window.__gl.backDomSelector=window.__gl.domSelector;
        window.__gl.domSelector=domSelector;
    }
    return {
        dialogSetting: {
            title: Kooboo.text.inlineEditor.bindVariable,
            width:"100%",
            height:"750px",
            zIndex:2,
            beforeSave: beforeSave,
            beforeClose:beforeClose
        },
        getHtml:function(){
            var url=convertResult.contentLink + '?koobooinline=design';
            var scripts=["/_Admin/Scripts/InlineEditor/KoobooIframe.js"] 
            param.iframe= Kooboo.IframeDialog.getIframe(url,scripts,"750px",iframeLoaded,"testiframe");
            return param.iframe;
        },
        init:function(){
            Kooboo.IframeDialog.init(param.iframe);
            Kooboo.VariableDataManager.setModel("BindVariable");
            Kooboo.VariableDataManager.setConverterId(context.koobooId);
        },
        clear:function(){
            Kooboo.VariableDataManager.clear();
        }
    }
}