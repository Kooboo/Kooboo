function Converter(){
    var converters=Kooboo.converter.ConverterManager.getConverters();
    var param={
        context:null,
        convertResult:{},
        convertElements:[],
    }
    function enableConvert(context){
        var element = context.el;

        if (!canOperateElement(element)) {
            return false;
        }
        //包含在组件中
        // if (isInComponent(context)) {
        //     return false;
        // }
        //转换的内容中是否包含组件
        if (isContainComponent(element)) {
            return false;
        }
        if (!isInPageObject(context,element)) {
            return false;
        }

        return true;
    }
    //是否可以操作的元素
    function canOperateElement(element) {
        var converterElements = param.convertElements;
        if (_.isEmpty(converterElements)) return true;
        return !Kooboo.converter.ConverterManager.isBeConvertedElement(converterElements, element) &&
            !Kooboo.converter.ConverterManager.isInConvertedElement(converterElements, element) &&
            !Kooboo.converter.ConverterManager.isContainConvertedElement(converterElements, element);
    }

    function isInPageObject(context,element) {
        var koobooObject = Kooboo.KoobooObjectManager.getFirstWrapKoobooObject(element);
        if (koobooObject.objectType &&
            koobooObject.objectType.toLowerCase() == Kooboo.SiteEditorTypes.ObjectType.page) {
            return true;
        }
        return false;
    }
    function isContainComponent(element) {
        var childrens = $(element).children();
        if (childrens.length == 0) return false;
        var isContain = false;
        //判断子元素中是否包含组件
        $.each(childrens, function(i, children) {
            var context = Kooboo.elementReader.readObject(children);
            if (isInComponent(context)) {
                isContain = true;
                return false;
            }
            isContain = isContainComponent(children);
            if (isContain) {
                isContain = true;
                return false;
            }
        });
        return isContain;
    }
    function isInComponent(context) {
        var inComponent = false;
        //不包含在组件中
        $.each(context.koobooObjects, function(i, koobooObject) {
            var objectType;
            if(koobooObject.type){
                objectType=Kooboo.SiteEditorTypes.ObjectType[koobooObject.type.toLowerCase()];
            }
            if (objectType && [Kooboo.SiteEditorTypes.ObjectType.page, Kooboo.SiteEditorTypes.ObjectType.layout].indexOf(objectType) == -1) {
                inComponent = true;
                return false;
            }
        });
        return inComponent;
    }
    function setConverterItemsVisible(el) {
        param.convertResult = Kooboo.converter.ConverterManager.getConverterResultByElement(el);
        
        var converterItemElements = $(".sub-menu .converter-sub");
        $.each(converterItemElements, function(i, convertItemElement) {
            var converterName = $(convertItemElement).find("a").attr("data-convert-name");
            if (param.convertResult[converterName]) {
                $(convertItemElement).show();
            } else {
                $(convertItemElement).hide();
            }
        });
    }
    function deleteConvert(convertResult) {

        var removeItem =param.context.editManager.remove(function(logItem) {
            return logItem.editorType == Kooboo.SiteEditorTypes.EditorType.converter &&
                convertResult.koobooId == logItem.currentResult.koobooId
        });
        if (removeItem) {
            Kooboo.converter.ConverterSelector.hideConverter(convertResult);
            Kooboo.converter.ConverterSelector.remove(param.convertElements, convertResult.koobooId);
        }
    }
    function toolbarSave() {
        var el = param.currentResult.editElement;

        Kooboo.converter.ConverterSelector.hideConverter(param.currentResult);
        Kooboo.converter.ConverterSelector.showConverterAfterSave(param.currentResult);

        var value = param.currentResult.oldHtml || param.currentResult.htmlBody;
        param.context.editManager.updateConverter({
            action: Kooboo.SiteEditorTypes.ActionType.add,
            editorType: Kooboo.SiteEditorTypes.EditorType.converter,
            domOperationType: Kooboo.SiteEditorTypes.DomOperationType.converter,
            logType: Kooboo.SiteEditorTypes.LogType.log,
            currentResult: param.currentResult,
            oldValue: value,
            newValue: value,
            el: el,
            undo: undo,
            redo: redo
        });
    }
    function toolbarClose() {
        Kooboo.converter.ConverterSelector.hideConverter(param.currentResult,true);
        Kooboo.converter.ConverterSelector.remove(param.convertElements, param.currentResult.koobooId);

        window.__gl.domSelector.hideHolderLines();
    }
    function undo(editlog) {
        var koobooId = $(editlog.el).attr("kooboo-id");
        Kooboo.converter.ConverterSelector.hideConverter(editlog.currentResult);
        Kooboo.converter.ConverterSelector.remove(param.convertElements, param.currentResult.koobooId);
        window.__gl.domSelector.hideHolderLines();

        param.currentResult.editElement = editlog.el;
    }
    function redo(editlog) {
        Kooboo.converter.ConverterSelector.addElements(param.convertElements, editlog.el);
        Kooboo.converter.ConverterSelector.showConverterAfterSave(editlog.currentResult);
        //this.replaceConvertHtml(editlog.el, editlog.newValue.newHtml || editlog.newValue.htmlBody, editlog.newValue.isHtmlBlockOrView);
        param.currentResult.editElement = editlog.el;
    }
    function getResult(context){
        var result = param.convertResult[param.context.selectConvertName];
        if(result.editElement){
            param.context=Kooboo.PluginHelper.getContext(result.editElement,context);
        }
        
        //var el = context.el;
        Kooboo.converter.ConverterSelector.addElements(param.convertElements, param.context.el);

        
        if (!result) {
            result = Kooboo.converter.ConverterManager.executeConverter(param.context.selectConvertName, param.context.el);
        }
        if (!result.hasResult) {
            window.info.show(Kooboo.text.inlineEditor.cantbeconvert, false);
            return;
        }
        return result;
    }
    return {
        isConverter:true,
        menuName : Kooboo.text.inlineEditor.convertTo + ":",
        converters:converters,
        isShow: function(context) {
            param.context = context;
            if (Kooboo.PluginHelper.isBindVariable() || 
                Kooboo.PluginHelper.isEmptyKoobooId(context.el)) 
                return false;
            setConverterItemsVisible(context.el);
            return enableConvert(context);
        },
        getHtml:function(){
            return "";
        },
        init:function(){

        },
        click:function(){
            
            var result=getResult(param.context);
            Kooboo.converter.ConverterSelector.init(deleteConvert,toolbarSave,toolbarClose,param.context);
            Kooboo.converter.ConverterSelector.showConverter(result);
            window.__gl.resetConvertersPosition = Kooboo.converter.ConverterSelector.resetPosition;
            param.currentResult = result;          
            param.context.selectConvertName = null;
        }

    }
}