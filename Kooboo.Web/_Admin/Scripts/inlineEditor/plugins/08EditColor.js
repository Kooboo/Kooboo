function EditColor(){
    var param={
        context:null,
        styleEditorHelper:null,
        colorItems:[],
        colorItemsContainerClass:".colorItemsContainer",
        ckAllUpdateGlobalClass:".ckAllUpdateGlobal",
        ckIsGlobalClass : ".ckIsGlobal",
        attrNameNodeClass : ".attrNameNode",
        colorItemNodeClass : ".colorItemNode"
    };

    function getData(context){
        param.colorItems = Kooboo.ColorHelper.getColorItems(context);
        var note=param.colorItems.length==0 ? Kooboo.text.inlineEditor.noColorAbleToChange:"";
        return {
            selectAll:Kooboo.text.inlineEditor.selectAll,
            updateGlobal:Kooboo.text.inlineEditor.updateGlobal,
            colorItems:param.colorItems,
            note:note
        }
    }
    function isSelectAll() {
        var selectAll = true;
        var updateGlobalCheckboxes = Kooboo.PluginHelper.getElementSelector(".ckIsGlobal");
        $.each(updateGlobalCheckboxes, function(i, checkbox) {
            var checked = $(checkbox).prop("checked");
            if (!checked) {
                selectAll = false;
                return false;
            }
        });
        return selectAll;
    }
    function createColorSpectrum(colorItem, colorItemNode) {
        var spectrumConfig = Kooboo.PluginHelper.getSpectrumConfig();
        spectrumConfig = $.extend({}, spectrumConfig, {
            color: colorItem.color,
            move: function(tinycolor) {
                var value = Kooboo.style.StyleEditorHelper.convertColor(tinycolor);
                if (colorItem.color != value) {
                    Kooboo.PluginHelper.setNodeTextValue(colorItemNode,value);
                    setValue(value, colorItem, colorItemNode);
                }
            },
            //modify Color Trigger
            change: function(tinycolor) {
                var value = Kooboo.style.StyleEditorHelper.convertColor(tinycolor);
                if (colorItem.color != value) {
                    Kooboo.PluginHelper.setNodeTextValue(colorItemNode,value);
                    setValue(value, colorItem, colorItemNode);
                }

            },
            cancelClick: function() {
                reset(colorItem, colorItemNode);
            },
            hide: function() {
                //cancel,ok,click blank place will trigger hidden
                setPrevColor(colorItem);
            }

        });
        $(colorItemNode).find(param.colorItemNodeClass).spectrum(spectrumConfig);
    }
    function setValue(newColor, colorItem, colorItemNode) {
        colorItem.color = newColor;

        var changeGlobal = isColorItemSelectGlobal(colorItemNode);
        addLog(colorItem.affects, colorItem, colorItemNode);
    }
    function setPrevColor(colorItem) {
        colorItem.prevColor = colorItem.color;
        //prevColor:the color before multiChange
    }
    function reset(colorItem, colorItemNode) {
        var newValue = colorItem.prevColor || colorItem.defaultColor;
        Kooboo.PluginHelper.setNodeTextValue(colorItemNode,newValue);
        setValue(newValue, colorItem, colorItemNode);
    }

    function isColorItemSelectGlobal(colorItemNode) {
        return $(colorItemNode).find(param.ckIsGlobalClass)[0].checked;
    }

    function addLog(affects, colorItem, colorItemNode) {
        var changeGlobal = isColorItemSelectGlobal(colorItemNode);
        $.each(affects, function(i, affect) {
            if (!affect.cssContext) {
                var styleRuleManager = Kooboo.style.StyleRuleManager;
                var cssContext = styleRuleManager.getCssContext(affect.element, affect.property);
                affect.cssContext = cssContext;
                affect.isInline = !cssContext["global"];
            }

            if (changeGlobal) {
                addGlobalLog(affect, colorItem);
            } else {
                addInlineLog(affect, colorItem);
            }
        });
    }

    function addGlobalLog(affect, colorItem) {
        var inlineDeclaration = affect.cssContext.inlineDeclaration;
        var cssRule = inlineDeclaration;
        var property = affect.property;

        var jsonRule = {
            "cssProperty": affect.property,
            "value": "",
            "color": "",
            "selector": null,
            "koobooId": affect.element.getAttribute("kooboo-id"),
            "important": !!affect.important
        };
        var shorthandProperty = Kooboo.ShorthandPropertyManager.setPropertyKeyDic(jsonRule, cssRule, property, param.context.el);
        //pseudo rule can't modify inline style
        if (!affect.pseudoClass) {
            //去掉Inline的样式
            param.context.editManager.updateStyle({
                domOperationType: Kooboo.SiteEditorTypes.DomOperationType.styles,
                el: param.context.el,
                cssRule: cssRule,
                jsonRule: jsonRule,
                property: property,
                pseudo: colorItem.pseudoClass,
                important: jsonRule.important,
                oldValue: cssRule[property],
                newValue: "",
                shorthandProperty: shorthandProperty
            });
           
        }

        //affect.cssContext.externalData["color"] = colorItem.color;
        affect.cssContext.externalData["value"] = colorItem.color;
        delete affect.cssContext.externalData.koobooId;

        jsonRule = affect.cssContext.externalData;
        property = jsonRule.cssProperty;
        cssRule = affect.cssContext.externalDeclaration;

        //新增external的样式的样式
        var oldValue = cssRule[property];
        var newValue = Kooboo.style.StyleEditorHelper.getCssNewValue(oldValue, colorItem.color);

        shorthandProperty = Kooboo.ShorthandPropertyManager.setPropertyKeyDic(jsonRule, cssRule, property, param.context.el);
        param.context.editManager.updateStyle({
            domOperationType: Kooboo.SiteEditorTypes.DomOperationType.styles,
            el: param.context.el,
            cssRule: cssRule,
            jsonRule: jsonRule,
            property: property,
            pseudo: colorItem.pseudoClass,
            important: jsonRule.important,
            oldValue: oldValue,
            newValue: newValue,
            shorthandProperty: shorthandProperty
        });
    }
    function addInlineLog(affect, colorItem) {
        var inlineDeclaration = affect.cssContext.inlineDeclaration;
        var cssRule = inlineDeclaration;
        var property = affect.property;

        var jsonRule = {
            "cssProperty": affect.property,
            "value": colorItem.color,
            "color": colorItem.color,
            "selector": null,
            "koobooId": affect.element.getAttribute("kooboo-id"),
            "imortant": !!affect.important
        };
        var oldValue = cssRule[property];
        var newValue = Kooboo.style.StyleEditorHelper.getCssNewValue(oldValue, colorItem.color);
        var shorthandProperty = Kooboo.ShorthandPropertyManager.setPropertyKeyDic(jsonRule, cssRule, property, param.context.el);
        //2.添加Inline的样式
        param.context.editManager.updateStyle({
            domOperationType: Kooboo.SiteEditorTypes.DomOperationType.styles,
            el: param.context.el,
            cssRule: cssRule,
            jsonRule: jsonRule,
            property: property,
            pseudo: colorItem.pseudoClass,
            important: jsonRule.important,
            oldValue: oldValue,
            newValue: newValue,
            shorthandProperty: shorthandProperty
        });

        var important = affect.cssContext.externalDeclaration.getPropertyPriority(affect.property),
            origIsInline = affect.isInline;

        var origColor = affect.cssContext.externalData.color,
            origImportant = affect.cssContext.externalData.important ? "!important" : "";

        delete affect.cssContext.externalData.koobooId;
        jsonRule = affect.cssContext.externalData;
        property = jsonRule.cssProperty;
        cssRule = affect.cssContext.externalDeclaration;
        shorthandProperty = Kooboo.ShorthandPropertyManager.setPropertyKeyDic(jsonRule, cssRule, property, param.context.el);
        //1.去掉/恢复external的样式。
        //用来重置external原始值
        param.context.editManager.updateStyle({
            domOperationType: Kooboo.SiteEditorTypes.DomOperationType.styles,
            el: param.context.el,
            cssRule: cssRule,
            jsonRule: jsonRule,
            property: property,
            pseudo: colorItem.pseudoClass,
            important: jsonRule.important,
            oldValue: origColor && origIsInline === false ? origColor : "",
            newValue: origColor && origIsInline === false ? origColor : "",
            shorthandProperty: shorthandProperty
        });
    }

    function bindGlobalChangeEvent(colorItem, colorItemNode) {
        $(colorItemNode).find(param.ckIsGlobalClass).prop("checked", !colorItem.isInline);
        $(colorItemNode).find(param.ckIsGlobalClass).on("change", function() {
            var isGlobal = this.checked,
                value = colorItem.color; //get current value

            addLog(colorItem.affects, colorItem, colorItemNode);
            colorItem.isInline = !isGlobal;

            var selectAll = isSelectAll();
            Kooboo.PluginHelper.getElementSelector(param.ckAllUpdateGlobalClass).prop("checked", selectAll);
        });
    }
    
    return {
        dialogSetting:{
            width:"420px",
            note:Kooboo.text.inlineEditor.allPageAffected,
            title:Kooboo.text.inlineEditor.editColor
        },
        menuName:Kooboo.text.inlineEditor.editColor,
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
            k.setHtml("colorHtml","EditColor.html");
            var data=getData(param.context);

            return _.template(colorHtml)(data);
        },
        init:function(){
            Kooboo.PluginHelper.getElementSelector(param.ckAllUpdateGlobalClass).on("change", function() {
                var checked = this.checked;
                //select update global checkbox
                var updateGlobalCheckboxes = Kooboo.PluginHelper.getElementSelector(".ckIsGlobal");
                $.each(updateGlobalCheckboxes, function(i, it) {
                    var oldCheckBoxValue = $(it).prop("checked");
                    if (oldCheckBoxValue != checked) {
                        $(it).prop("checked", checked);
                        $(it).trigger("change");
                    }
                });
            });

            //bind color item event
            var colorItemNodes = Kooboo.PluginHelper.getElementSelector(".colorItem");

            $.each(colorItemNodes, function(i, colorItemNode) {
                var colorItem = param.colorItems[i];

                createColorSpectrum(colorItem, colorItemNode);
                bindGlobalChangeEvent(colorItem, colorItemNode);
            });

            var selectAll = isSelectAll();
            Kooboo.PluginHelper.getElementSelector(param.ckAllUpdateGlobalClass).prop("checked", selectAll);

            
        }
    }
}