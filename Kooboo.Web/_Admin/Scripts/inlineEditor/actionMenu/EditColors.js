function EditColors(){
    var param={
        context:null,
        lighters:[]
    }
    function getColors() {
        var iframeDoc = Kooboo.InlineEditor.getIFrameDoc();
        if (!iframeDoc) return [];
        var colors = Kooboo.DomResourceManager.getDocColors(iframeDoc);
        colors = getSortValues(colors);

        return colors;
    }
    function groupColor(colors) {
        return _.groupBy(colors, function(color) {
            return color.color || "transparent"
        });
    }
    function getCssRules(colors){
        var rules=[];
        $.each(colors, function(j, color) {
            if(color.cssRule){
                rules.push(color.cssRule);
            }
        });
        return rules;
    }
    function getSortValues(colors) {
        var groupColors = groupColor(colors);
        var fdoc = Kooboo.InlineEditor.getIFrameDoc(),
            sortedValue = [];
        
        $.each(groupColors, function(i, colors) {
            var refElements = [];
            var availableColors = [];

            var cssRules=getCssRules(colors);
            
            $.each(colors, function(j, color) {
                if (color.selector) {
                   var elements = $(color.selector, fdoc);
                   var body=fdoc.body;
                    if (elements.length > 0) {
      
                        var available = false;
                        $.each(elements, function(i, el) {
                            if((!$.contains(body,el)&& body != el) || $(el).attr("class")=="domShadowLine"){
                                return true;
                            }
                            var bestRule=Kooboo.style.StyleBestRule.getBestRule(cssRules,el,color.cssProperty,true);
                            //var bestRule=Kooboo.style.StyleBestRule.find(el,color.cssProperty,true);
                            var comp = Kooboo.style.StyleEditorHelper.getComputedStyle(el),
                            val = comp.getPropertyValue(color.cssProperty);
                            if(bestRule && bestRule.style && bestRule.style==color.cssStyleRule &&
                                Kooboo.DomResourceManager.convertRgbToHex(val) ==
                                Kooboo.DomResourceManager.convertRgbToHex(color.color)){
                                
                                available = true;
                                if(!_.includes(refElements,el))
                                    refElements.push(el);
                            }
                        });
                        if (available) {
                            availableColors.push(color);
                        }
                    }
                } else if (color.koobooId &&color.el) {
                    var comp = Kooboo.style.StyleEditorHelper.getComputedStyle(color.el),
                        val = comp.getPropertyValue(color.cssProperty);

                    if (Kooboo.DomResourceManager.convertRgbToHex(val) ==
                        Kooboo.DomResourceManager.convertRgbToHex(color.color)) {
                        if(!_.includes(refElements,color.el)){
                            refElements.push(color.el);
                            availableColors.push(color);
                        }
                        
                    }
                } 
            });
            if (refElements.length > 0) {
                sortedValue.push({
                    color: i,
                    defaultColor: i,
                    values: availableColors,
                    references: refElements.length,
                    elements: refElements
                });
            }
        });

        sortedValue = _.sortBy(sortedValue, function(it) {
            return it.references;
        });
        sortedValue.reverse();
        return sortedValue;
    }
    function getData(){
        var colors=getColors();
        return {
            allPageAffected : Kooboo.text.inlineEditor.allPageAffected,
            noteText : Kooboo.text.inlineEditor.note,
            referencesText : Kooboo.text.inlineEditor.references,
            colorText : Kooboo.text.inlineEditor.color,
            colors:colors
        }
    }
    function mouseoverFn(colorItem) {
        if (colorItem.elements && colorItem.elements.length > 0)
            Kooboo.PluginHelper.scrollToElement(colorItem.elements[0],function(){
                $.each(colorItem.elements, function(index, element) {
                    var koobooId = $(element).attr("kooboo-id");
                    Kooboo.PluginHelper.lighterElement(param.lighters, element, koobooId, colorItem.selector);
                });
            });
        
        
    }
    function mouseoutFn(colorItem) {
        Kooboo.PluginHelper.unLighterByGroup(param.lighters, colorItem.selector);
    }
    function createSpectrum(colorNode, colorItem) {
        var spectrumConfig = Kooboo.PluginHelper.getSpectrumConfig();
        var styleEditorHelper=Kooboo.style.StyleEditorHelper;
        spectrumConfig = $.extend({}, spectrumConfig, {
            color: colorItem.color,
            move: function(tinycolor) {
                var value = styleEditorHelper.convertColor(tinycolor);
                if (colorItem.color != value) {
                    Kooboo.PluginHelper.setNodeTextValue(colorNode, value);
                    setValue(value, colorNode, colorItem);
                }

            },
            //modify Color Trigger
            change: function(tinycolor) {
                var value = styleEditorHelper.convertColor(tinycolor);
                if (colorItem.color != value) {
                    Kooboo.PluginHelper.setNodeTextValue(colorNode, value);
                    setValue(value, colorNode, colorItem);
                }

            },
            cancelClick: function() {
                reset(colorNode, colorItem);
            },
            hide: function() {
                setPrevColor(colorItem);
            },
        });
        $(colorNode).find(".colorNode").spectrum(spectrumConfig);

    }
    function setValue(newColor, colorNode, colorItem) {
        for (var i = 0; i < colorItem.values.length; i++) {
            var json = colorItem.values[i];
            var cssRule = json.cssStyleRule;
            if (cssRule) {
                var el = json.el;
                var oldValue = cssRule[json.cssProperty];
                var newValue = Kooboo.style.StyleEditorHelper.getCssNewValue(oldValue, newColor);
                //change.el = el;
                var shorthandProperty = Kooboo.ShorthandPropertyManager.setPropertyKeyDic(json, cssRule, json.cssProperty, el);
                var tempLog = {
                    domOperationType: Kooboo.SiteEditorTypes.DomOperationType.styles,
                    el: el,
                    jsonRule: json,
                    cssRule: cssRule,
                    property: json.cssProperty,
                    important: json.important,
                    oldValue: oldValue,
                    newValue: newValue,
                    shorthandProperty: shorthandProperty
                }
                param.context.editManager.updateStyle(tempLog);
            }

        }
        colorItem.color = newColor;
    }
    function setPrevColor(colorItem) {
        colorItem.prevColor = colorItem.color;
    }
    function reset(colorNode, colorItem) {
        colorItem.color = colorItem.prevColor || colorItem.defaultColor;
        Kooboo.PluginHelper.setNodeTextValue(colorNode, colorItem.color);
        setValue(colorItem.color, colorNode, colorItem);
    }
    return {
        dialogSetting:{
            title:Kooboo.text.inlineEditor.editColors,
            width: "500px"
        },
        getHtml:function(context){
            k.setHtml("colorsHtml","EditColors.html");
            var data=getData();
            param.data=data;
            param.context=context;
            return _.template(colorsHtml)(data);
        },
        init:function(){
            var colorNodes = $(".color-item");
            $.each(colorNodes, function(i, colorNode) {
                var colorItem = param.data.colors[i];
                createSpectrum(colorNode, colorItem);
                $(colorNode).bind("mouseover", function() {
                    mouseoverFn(colorItem);
                });
                $(colorNode).bind("mouseout", function() {
                    mouseoutFn(colorItem);
                });
            });
        },
    }
}