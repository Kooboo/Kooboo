function ConverterSelector(){
    var param={
        namePrefixIndexDic:{},
        doc:null,
        convertMasks: {},
        convertDashedLighters: {},
        convertLabels: {},
        convertToolbar:null,
        currentEl:null,
    }
    function createToolBar(toolbarSave,toolbarClose,context) {
        var convertToolbar = new Kooboo.converter.ConverterToolbar({
            save: toolbarSave,
            close: toolbarClose,
            doc: Kooboo.InlineEditor.getIFrameDoc(),
            context:context
        });
        var html= convertToolbar.getHtml();
        $("body").append(html);
        convertToolbar.init();

        return convertToolbar;
    }
    //get converterName
    function getName(namePrefix) {
        if (!namePrefix) {
            namePrefix = "temp";
        }
        var name = namePrefix;
        if (!param.namePrefixIndexDic[namePrefix] && param.namePrefixIndexDic[namePrefix] != 0) {
            param.namePrefixIndexDic[namePrefix] = 0;
        } else {
            var index = param.namePrefixIndexDic[namePrefix] + 1;
            param.namePrefixIndexDic[namePrefix] = index;
            name = name + index.toString();
        }
        return name;
    }
    function createShadow() {
        var shadow = Kooboo.DomShadow({
            zIndex: Kooboo.InlineEditor.zIndex.lowerZIndex
        },window.document,window.document);
        return shadow;
    }
    function createMask() {
        var masker =Kooboo.DomMasker({
            zIndex: Kooboo.InlineEditor.zIndex.lowerZIndex,
            backgroundColor: "rgba(62, 188, 202, 0.2)"
        },window.document,window.document)
        return masker;
    }
    function createLabel(text, convertResult,deleteConvert) {
        //var self = this;
        var label = Kooboo.converter.ConverterLabel(text, function() {
            param.convertToolbar.showSetting(convertResult);
        }, function() {
            deleteConvert(convertResult);
        });
        label.init();
        return label;
    }
    function getMaskByKoobooId(koobooId) {
        if (!param.convertMasks[koobooId]) {
            var masker=createMask();
            masker.koobooId=koobooId;
            param.convertMasks[koobooId] = masker;
            $("body").append(param.convertMasks[koobooId].domNode);
        }
        return param.convertMasks[koobooId];
    }
    function getLabelByKoobooId(koobooId, result,deleteConvert) {
        var text = result.convertToType;
        if (!param.convertLabels[koobooId] || param.convertLabels[koobooId].text != text) {
            var label = createLabel(text, result,deleteConvert);
            label.koobooId = koobooId;
            param.convertLabels[koobooId] = label;
            $("body").append(label.domNode);
        }
        return param.convertLabels[koobooId];
    }
    function deleteLabelById(koobooId) {
        param.convertLabels[koobooId].destroy();
        delete param.convertLabels[koobooId];
    }
    function getElementbyKoobooId(koobooId) {
        var el = $("[kooboo-id=" + koobooId + "]", param.doc).get(0);
        return el;
    }
    function removeConvertElements(convertElements, koobooId) {
        if (convertElements) {
            var matchIndex = -1;
            $.each(convertElements, function(i, convertElement) {
                if ($(convertElement.el).attr("kooboo-id") == koobooId) {
                    matchIndex = i;
                }
            });
            if (matchIndex > -1) {
                convertElements.splice(matchIndex, 1);
            }
        }
        return convertElements;
    }
    return {
        deleteConvert:null,
        toolbarSave:null,
        toolbarClose:null,
        init:function(deleteConvert,toolbarSave,toolbarClose,context){
            this.deleteConvert=deleteConvert;
            this.toolbarSave=toolbarSave;
            this.toolbarClose=toolbarClose;
            if(!param.convertToolbar){
                var convertToolbar=  createToolBar(toolbarSave,toolbarClose,context);
                param.convertToolbar=convertToolbar;
            }
                
            param.shadow= createShadow();
        },
        //呈现
        showConverter: function(result) {
            param.doc = Kooboo.InlineEditor.getIFrameDoc();
            var koobooId = result.koobooId,
                element = getElementbyKoobooId(koobooId);
            //1.set toolbar position
            param.currentEl=element;
            param.convertToolbar.setPosition(element);

            //2.set label position
            var label = getLabelByKoobooId(koobooId, result,this.deleteConvert);
            label.mask({ el: element });
            //3.set masker position
            var masker = getMaskByKoobooId(koobooId);
            masker.mask({ el: element });

            param.shadow.mask({ el: element }, param.doc.documentElement);
        },
        showConverterAfterSave: function(result) {
            var koobooId = result.koobooId,
                element = getElementbyKoobooId(koobooId);
            var masker = getMaskByKoobooId(koobooId);
            masker.mask({el:element});
            var label = getLabelByKoobooId(koobooId, result,this.deleteConvert);

            label.mask({ el: element });
            label.showRemoveBtn();
        },
        hideConverter: function(result) {
            var koobooId = result.koobooId,
                element = getElementbyKoobooId(koobooId);
            param.convertToolbar.hide(koobooId);
            var masker = getMaskByKoobooId(koobooId);
            masker.unmask();
            deleteLabelById(koobooId);

            param.shadow.unmask();
        },
        
        addElements: function(convertElements, el) {
            var name = getName("custom");
            var elements = [];
            elements.push(el);
            var convertElement = {
                name: name,
                el: el,
                elements: elements
            }
            convertElements.push(convertElement);
            return convertElements;
        },
        remove:function(convertElements, koobooId){
            param.convertToolbar.clear();
            delete param.convertMasks[koobooId];
            // $.each(param.convertMasks, function(i, mask) {
            //     var doc = Kooboo.InlineEditor.getIFrameDoc();
            //     var el = $("[kooboo-id='" + mask.koobooId + "']", doc)[0];
            //     mask.mask({ el: el });
            // });
            removeConvertElements(convertElements, koobooId);
        },
        isBeConvertedElement: function(convertElements, element) {
            var convertedElement = false;
            $.each(convertElements, function(i, convertElement) {
                convertedElement = convertElement.el == element;
                if (convertedElement)
                    return true;
            });
            return convertedElement;
        },
        isInConvertedElement: function(convertElements, element) {
            var inConvertedElement = false;
            $.each(convertElements, function(i, convertElement) {
                inConvertedElement = $(convertElement.el).find(element).length > 0;
                if (inConvertedElement)
                    return true;
            });
            return inConvertedElement;
        },
        isContainConvertedElement: function(convertElements, element) {
            var isContain = false;
            $.each(convertElements, function(i, convertElement) {
                isContain = $(element).find(convertElement.el).length > 0;
                if (isContain)
                    return true;
            });
            return isContain;
        },
        resetPosition:function(){
            $.each(param.convertLabels, function(i, label) {
                var doc = Kooboo.InlineEditor.getIFrameDoc();
                var el = $("[kooboo-id='" + label.koobooId + "']", doc)[0];
                label.mask({ el: el });
            });
            
            $.each(param.convertMasks, function(i, mask) {
                var doc = Kooboo.InlineEditor.getIFrameDoc();
                var el = $("[kooboo-id='" + mask.koobooId + "']", doc)[0];
                mask.mask({ el: el });
                
            });
            if (param.convertToolbar&& param.convertToolbar.isVisible()) {
                param.convertToolbar.setPosition(param.currentEl);
            }
        }
    }
}