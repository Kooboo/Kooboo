function EditStyle(){
    var param={
        siteEditorImageNodeClass : ".siteEditorImageNode",
        btnImagePickClass : ".btnImagePick",
        btnImageRemoveClass : ".btnImageRemove",
        colorNodeClass:".styleEditorColorNode",
        fontFamilyNodeClass : ".fontFamilyNode",
        fontSizeNodeClass : ".fontSizeNode",
        fontSizeSpinnerClass : ".fontSizeSpinner",
        fontStyleNodeClass : ".fontStyleNode",
        fontWeightNodeClass:".fontWeightNode"
    }
    function getData(el){
        var style=Kooboo.StyleHelper.getStyle(el);
        var data={
            changeImage : Kooboo.text.inlineEditor.changeImage,
            remove : Kooboo.text.common.remove,
            add : Kooboo.text.common.add,
            colorTitle:Kooboo.text.inlineEditor.color,
            page : Kooboo.text.common.Page,
            external : Kooboo.text.common.external,
            pagesText : Kooboo.text.common.Pages,
            backgroundImage:style.backgroundImage,
            colorItems : style.colorItems,
            font:style.font
        }
        
        return data;
    }
    function setImageValue(newValue) {
        var imageItem = param.data.backgroundImage;
        //修改background-image
        if (Kooboo.DomResourceManager.isImage(newValue)) {
            imageItem.url = Kooboo.DomResourceManager.getStyleImageUrl(newValue);
            $(param.siteEditorImageNodeClass).attr("src", imageItem.url);
        }
        var oldValue = imageItem.cssRule[imageItem.property];
        var shorthandProperty = Kooboo.ShorthandPropertyManager.setPropertyKeyDic(imageItem.jsonRule, imageItem.cssRule, imageItem.property, param.context.el);

        param.context.editManager.updateStyle({
            domOperationType: Kooboo.SiteEditorTypes.DomOperationType.styles,
            el: param.context.el,
            jsonRule: imageItem.jsonRule,
            cssRule: imageItem.cssRule,
            important: imageItem.important,
            property: imageItem.property,
            oldValue: oldValue,
            newValue: newValue,
            shorthandProperty: shorthandProperty
        });

        imageItem.hasImage = Kooboo.DomResourceManager.isImage(newValue);
        imageItem.value = newValue;

        k.setHtml("bgImageHtml","EditStyleBgImage.html");

        var html=_.template(bgImageHtml)(param.data);
        $(".background-Image-Container").html(html);

        //rebinding backgroundImage event 
        initBackgroundImage();
    }
    function initBackgroundImage(){
        Kooboo.PluginHelper.getElementSelector(param.btnImagePickClass).bind("click", function() {
            var mediagridParams = {
                onAdd: function(selected) {
                    setImageValue('url("' + selected.url + '")');
                },
            }
            window.__gl.siteEditor.showMediagrid(mediagridParams);
        });
        Kooboo.PluginHelper.getElementSelector(param.btnImageRemoveClass).bind("click", function() {
            removeImage();
        });
    }
    function removeImage() {
        setImageValue('');
        removeAllMatchRules();
    }
    //when el has multi media rule , it needs to remove all the matched rules.
    function removeAllMatchRules() {
        if (param.context && param.context.el) {
            var rules = Kooboo.style.StyleEditorHelper.filterMatchedCSSRules(Kooboo.style.StyleEditorHelper.getMatchedCSSRules(param.context.el));
            var imageItem = param.data.backgroundImage;
            $.each(rules, function(i, rule) {
                if (rule && rule.style && rule.style[imageItem.property]) {
                    Kooboo.dom.DomOperationHelper.resetCssRule(rule.style, imageItem.property, "", "");
                }
            });
        }
    }
    function createColorSpectrum(colorItem, colorItemNode) {
        var spectrumConfig = Kooboo.PluginHelper.getSpectrumConfig();

        spectrumConfig = $.extend({}, spectrumConfig, {
            color: colorItem.color,
            move: function(tinycolor) {
                var value = Kooboo.style.StyleEditorHelper.convertColor(tinycolor);
                if (colorItem.color != value) {
                    Kooboo.PluginHelper.setNodeTextValue(colorItemNode,value);
                    setColorItemValue(value, colorItem);
                }
            },
            change: function(tinycolor) {
                var value = Kooboo.style.StyleEditorHelper.convertColor(tinycolor);
                if (colorItem.color != value) {
                    Kooboo.PluginHelper.setNodeTextValue(colorItemNode,value);
                    setColorItemValue(value, colorItem);
                }
            },
            cancelClick: function() {
                resetColorItem(colorItem,colorItemNode);
            },
            hide: function() {
                setPrevColor(colorItem);
            },
        });
        $(colorItemNode).find(param.colorNodeClass).spectrum(spectrumConfig);
    }
    function setPrevColor(colorItem) {
        colorItem.prevColor = colorItem.color;
        //self.color = self.value;
    }
    function setColorItemValue(newColor, colorItem) {
        if (newColor == "initial") {
            newColor = null;
        }
        var oldValue = colorItem.cssRule[colorItem.property];
        var newValue = Kooboo.style.StyleEditorHelper.getCssNewValue(oldValue, newColor);
        var shorthandProperty = Kooboo.ShorthandPropertyManager.setPropertyKeyDic(colorItem.jsonRule, colorItem.cssRule, colorItem.property, param.context.el);

        param.context.editManager.updateStyle({
            domOperationType: Kooboo.SiteEditorTypes.DomOperationType.styles,
            el: param.context.el,
            jsonRule: colorItem.jsonRule,
            cssRule: colorItem.cssRule,
            important: colorItem.important,
            property: colorItem.property,
            oldValue: oldValue,
            newValue: newValue,
            shorthandProperty: shorthandProperty
        });
        
        colorItem.color = newColor;
    }
    function resetColorItem(colorItem,colorItemNode) {
        colorItem.color = colorItem.prevColor || colorItem.defaultColor || colorItem.defaultValue;
        Kooboo.PluginHelper.setNodeTextValue(colorItemNode,colorItem.color);
        setColorItemValue(colorItem.color, colorItem);
    }
    function initColor(){
        var colorItemNodes = $(".colorItem-container");

        $.each(colorItemNodes, function(i, colorItemNode) {
            var colorItem = param.data.colorItems[i];
            createColorSpectrum(colorItem, colorItemNode);
        });
    }

    function initFont(){
        initFontFamily();
        initFontSize();
        initFontStyle();
        initFontWeight();
    }
    
    function initFontFamily(){
        if (!param.data.font.fontFamily) return;
        var fontFamily = param.data.font.fontFamily,
            formatedValues = Kooboo.StyleHelper.getFamilyFormatValues(fontFamily.value),
            sources = Kooboo.StyleHelper.getNewFamilySource(fontFamily.sources, formatedValues);

        $(".fontFamilyNode").on("change", function(e) {
            var value = $(this).val();

            if (!Kooboo.StyleHelper.isSameFamilyValue(value, fontFamily.value)) {
                if (!value) {
                    fontFamily.value = "";
                } else {
                    fontFamily.value = value.join(",");
                }

                var shorthandProperty = Kooboo.ShorthandPropertyManager.setPropertyKeyDic(fontFamily.jsonRule, fontFamily.cssRule, fontFamily.property, param.context.el);

                param.context.editManager.updateStyle({
                    domOperationType: Kooboo.SiteEditorTypes.DomOperationType.styles,
                    el: param.context.el,
                    jsonRule: fontFamily.jsonRule,
                    cssRule: fontFamily.cssRule,
                    important: fontFamily.important,
                    property: fontFamily.property,
                    oldValue: fontFamily.defaultValue,
                    newValue: fontFamily.value,
                    shorthandProperty: shorthandProperty
                });
            }
        }).select2({
            tags: true,
            data: sources,
            multiple: true,
            dropdownAutoWidth: true,
            formatSelection: function(state) {
                return state.status === 0 ? "<del class='text-muted'>" + state.text + "</del>" : state.text;
            },
            tokenSeparators: [","]
        }).val(formatedValues).trigger("change");
    }

    function initFontSize(){
        var fontSize = param.data.font.fontSize,
            sizeParam = Kooboo.StyleHelper.getFontSizeSpinnerParam(fontSize.value);

        Kooboo.PluginHelper.getElementSelector(param.fontSizeSpinnerClass).TouchSpin({
            min: -100000,
            max: 100000,
            step: sizeParam.step,
            decimals: sizeParam.decimals,
            verticalbuttons: true,
            postfix: sizeParam.unit,
            initval: 0
        }).on("blur", function() {
            this.value = parseFloat(this.value) || 0;
        }).on("change", function() {
            setFontSizeValue(this.value, sizeParam);
        }).on("input", function() {
            setFontSizeValue(this.value, sizeParam);
        }).on("propertychange", function() {
            setFontSizeValue(this.value, sizeParam);
        });

        var formatValue =Kooboo.StyleHelper.formatFontSizeValue(fontSize.value, sizeParam.decimals);
        Kooboo.PluginHelper.getElementSelector(param.fontSizeSpinnerClass).val(formatValue);
    }
    function setFontSizeValue(value, sizeParam) {
        var decimals=sizeParam.decimals,
            fontSize=param.data.font.fontSize,
            formatValue =Kooboo.StyleHelper.formatFontSizeValue(value, decimals);
        
        fontSize.value = (formatValue||0)+sizeParam.unit;
        
        var shorthandProperty = Kooboo.ShorthandPropertyManager.setPropertyKeyDic(fontSize.jsonRule, fontSize.cssRule, fontSize.property, param.context.el);
        param.context.editManager.updateStyle({
            domOperationType: Kooboo.SiteEditorTypes.DomOperationType.styles,
            el: param.context.el,
            jsonRule: fontSize.jsonRule,
            cssRule: fontSize.cssRule,
            important: fontSize.important,
            property: fontSize.property,
            oldValue: fontSize.defaultValue,
            newValue: fontSize.value,
            shorthandProperty: shorthandProperty
        });

        Kooboo.PluginHelper.getElementSelector(param.fontSizeSpinnerClass).val(formatValue);
    }

    function initFontStyle(){
        var fontStyle = param.data.font.fontStyle;
        Kooboo.PluginHelper.getElementSelector(param.fontStyleNodeClass).bootstrapSwitch({
            state: fontStyle.active,
            onSwitchChange: function(event, state) {
                fontStyle.active = state;
                var newValue =Kooboo.StyleHelper.getFontStyleValue(fontStyle.active);

                var shorthandProperty = Kooboo.ShorthandPropertyManager.setPropertyKeyDic(fontStyle.jsonRule, fontStyle.cssRule, fontStyle.property, param.context.el);

                param.context.editManager.updateStyle({
                    domOperationType: Kooboo.SiteEditorTypes.DomOperationType.styles,
                    el: param.context.el,
                    jsonRule: fontStyle.jsonRule,
                    cssRule: fontStyle.cssRule,
                    important: fontStyle.important,
                    property: fontStyle.property,
                    oldValue: fontStyle.defaultValue,
                    newValue: newValue,
                    shorthandProperty: shorthandProperty
                });
                fontStyle.value = newValue;
            }
        });
    }

    function initFontWeight(){
        var fontWeight = param.data.font.fontWeight;
        Kooboo.PluginHelper.getElementSelector(param.fontWeightNodeClass).bootstrapSwitch({
            state: fontWeight.active,
            onSwitchChange: function(event, state) {
                fontWeight.active = state;
                var newValue = fontWeight.active ? "bold" : "normal";
                var oldValue = fontWeight.cssRule[fontWeight.property];
                var shorthandProperty = Kooboo.ShorthandPropertyManager.setPropertyKeyDic(fontWeight.jsonRule, fontWeight.cssRule, fontWeight.property, param.context.el);

                param.context.editManager.updateStyle({
                    domOperationType: Kooboo.SiteEditorTypes.DomOperationType.styles,
                    el: param.context.el,
                    jsonRule: fontWeight.jsonRule,
                    cssRule: fontWeight.cssRule,
                    important: fontWeight.important,
                    property: fontWeight.property,
                    oldValue: oldValue,
                    newValue: newValue,
                    shorthandProperty: shorthandProperty
                });
            }
        });
    }
    return {
        dialogSetting:{
            title:Kooboo.text.inlineEditor.editStyle,
            width:"600px",
            note:Kooboo.text.inlineEditor.allPageAffected
        },
        menuName:Kooboo.text.inlineEditor.editStyle,
        isShow: function(context) {
            param.context = context;
            if (Kooboo.PluginHelper.isBindVariable()) return false;
            var el = context.el;
            var koobooObjects = context.koobooObjects;
            if (Kooboo.KoobooObjectManager.isMenu(koobooObjects)) return false;
            return !Kooboo.PluginHelper.isEmptyKoobooId(el) && 
                !Kooboo.PluginHelper.imgRegExp.test(el.tagName);
        },
        getHtml:function(){
            k.setHtml("styleHtml","EditStyle.html");
            var data=getData(param.context.el);
            param.data=data;
            return _.template(styleHtml)(data); 
        },
        init:function(){
            initBackgroundImage();
            initColor();
            initFont();
        }
    }
}