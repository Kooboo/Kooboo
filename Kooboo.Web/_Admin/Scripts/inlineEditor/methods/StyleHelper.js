function StyleHelper(){
    function getTitle(title, pseudoClass) {
        var newTitle = "";
        switch (title) {
            case "background-image":
                newTitle = Kooboo.text.inlineEditor.backgroundImage;
                break;
            case "background-color":
                newTitle = Kooboo.text.inlineEditor.backgroundColor;
                break;
            case "color":
                newTitle = Kooboo.text.inlineEditor.color;
                break;
            case "border-color":
                newTitle = Kooboo.text.inlineEditor.borderColor;
                break;
            case "border-top-color":
                newTitle = Kooboo.text.inlineEditor.borderTopColor;
                break;
            case "border-bottom-color":
                newTitle = Kooboo.text.inlineEditor.borderBottomColor;
                break;
            case "border-left-color":
                newTitle = Kooboo.text.inlineEditor.borderLeftColor;
                break;
            case "border-right-color":
                newTitle = Kooboo.text.inlineEditor.borderRightColor;
                break;
            case "outline-color":
                newTitle = Kooboo.text.inlineEditor.outlineColor;
                break;
            case "text-shadow":
                newTitle = Kooboo.text.inlineEditor.textShadow;
                break;
            case "box-shadow":
                newTitle = Kooboo.text.inlineEditor.boxShadow;
                break;
            default:
                newTitle = title;
                break;
        }
        if (pseudoClass) {
            newTitle += pseudoClass;
        }
        return newTitle;
    }
    function getStyleEditorItems(el, properties, requiredProperties, notFilterElementRule) {
        var styleRuleManager =  Kooboo.style.StyleRuleManager,
            items = styleRuleManager.getMatchRules(el, properties, requiredProperties, notFilterElementRule),
            newItems = [];

        $.each(items, function(i, item) {
            var entity = item.entity,
                property = entity.property,
                computedStyle = Kooboo.style.StyleEditorHelper.getComputedStyle(el);;
            if (!Kooboo.DomResourceManager.isNeedShowProperty(property, computedStyle)) {
                return true;
            }
            entity.title = getTitle(entity.title);

            newItems.push(item.entity);
        });
        return newItems;
    }
    function getBackgroundImageItem(el) {
        var backgroundItems = getStyleEditorItems(el, ["background-image"], ["background-image"], true);
        var item = {};
        if (backgroundItems && backgroundItems.length > 0) {
            item = backgroundItems[0];
            item.hasImage = Kooboo.DomResourceManager.isImage(item.value);
            if (item.hasImage) {
                var url = Kooboo.DomResourceManager.getStyleImageUrl(item.value);
                var containerUrl;

                if (item.jsonRule && item.jsonRule.styleSheetUrl) {
                    containerUrl = item.jsonRule.styleSheetUrl;
                } else {
                    var iframeWindow = Kooboo.InlineEditor.getIframeWindow();
                    containerUrl = iframeWindow.location.href;
                }
                var baseUrl = Kooboo.DomResourceManager.getBaseUrl();
                //获取URL的绝对路径
                var absoluteUrl = Kooboo.DomResourceManager.getImageAbsoluteUrl(baseUrl, containerUrl, url);
                item.url = absoluteUrl;
                item.value = item.value.replace(url, absoluteUrl);
            }
        }
        item.title = Kooboo.text.inlineEditor.backgroundImageTitle;

        return item;
    }
    function getColorItems(el) {
        var items = getStyleEditorItems(el, Kooboo.DomResourceManager.colorProperties, ["background-color", "color"]);
        $.each(items, function(i, item) {
            item.defaultColor = getColor(item.value, item);
            item.color = getColor(item.value, item);
            item.prevColor = item.color;
        });

        return items;
    }
    function getColor(value, colorItem) {
        var colorValue = Kooboo.Color.searchString(value || colorItem.value);
        var color = new Kooboo.Color(colorValue);
        return color.toString();
    }
    function getFont(el) {
        var fontProperties = ["font", "font-family", "font-size", "font-style", "font-weight"],
            requiredProperties = ["font-size", "font-weight", "font-style"];
        var fontItems = getStyleEditorItems(el, fontProperties, requiredProperties);

        var font = {
            title: Kooboo.text.inlineEditor.font
        };
        $.each(fontItems, function(i, fontItem) {
            var property = fontItem.property;
            switch (property) {
                case "font-family":
                    fontItem.defaultValue = getFamilyFormatValue(fontItem.value);
                    fontItem.sources = getFamilySources();
                    font.fontFamily = fontItem;

                    break;
                case "font-size":
                    fontItem.defaultValue = fontItem.value;
                    font.fontSize = fontItem;

                    break;
                case "font-style":
                    fontItem.defaultValue = fontItem.value;
                    fontItem.active = fontItem.value && fontItem.value === "italic";
                    font.fontStyle = fontItem;
                    break;
                case "font-weight":
                    fontItem.defaultValue = fontItem.value;
                    fontItem.active = !Kooboo.style.StyleEditorHelper.isNormalFontWeight(fontItem.value);
                    font.fontWeight = fontItem;
                    break;
            }
        });
        return font;
    }
    function getFamilyFormatValues(value) {
        var formatedValues = _.map(value.split(','), function(it) {
            //trim 
            var trimVal = it.replace(/^('|"|\s)+|('|"|\s)+$/gm, '');
            trimVal = trimVal.toLowerCase();
            //fontfamily has whitespace
            return trimVal.indexOf(' ') > -1 ? "\"" + trimVal + "\"" : trimVal;
        });
        return formatedValues;
    }
    function getFamilyFormatValue(value) {
        var formatedValues = getFamilyFormatValues(value);
        var formatedValue = formatedValues.join(",");
        return formatedValue;
    }
    function getFamilyFormatId(fontFamily) {
        return fontFamily.indexOf(' ') > -1 ? "\"" + fontFamily + "\"" : fontFamily;
    }
    function getFamilySources() {
        var fontFamilies = Kooboo.DomResourceManager.fontFamilies;
        var sources = _.map(fontFamilies, function(fontFamily) {
            return {
                id: getFamilyFormatId(fontFamily),
                text: fontFamily
            }
        });
        return sources;
    }
    // function isNormalFontWeight(value) {
    //     if (value &&
    //         (value === "bold" || value === "bolder" || parseInt(value) >= 700)) {
    //         //700相当于bold
    //         return false;
    //     }
    //     return true;
    // }
    return {
        getStyle:function(el){
            return {
                backgroundImage:getBackgroundImageItem(el),
                colorItems : getColorItems(el),
                font:getFont(el)
            }
        },
        getTitle: getTitle,
        getFamilyFormatValues: getFamilyFormatValues,
        isSameFamilyValue: function(changeValue, originalValueStr) {
            if (!changeValue && !originalValueStr) return true;

            if (changeValue && changeValue.length > 0) {
                if (!originalValueStr) return false;
                var originalValueArray = originalValueStr.split(",");
                originalValueArray = _.map(originalValueArray, function(originalValue) {
                    return Kooboo.trim(originalValue)
                });

                changeValue.sort(function(a, b) { return a.localeCompare(b) });
                originalValueArray.sort(function(a, b) { return a.localeCompare(b) });

                return originalValueArray.join(",").toLowerCase() == changeValue.join(",").toLowerCase();
            }
            return false;
        },
        //不存在相应的family时，添加到fontFamilySource中
        getNewFamilySource: function(fontFamilySource, formatedValues) {
            fontFamilySource = _.map(fontFamilySource, function(family) {
                family.id = family.id.toLowerCase();
                family.text = family.text.toLowerCase();
                return family;
            });
            $.each(formatedValues, function(i, formatValue) {
                var existFamily = _.find(fontFamilySource, function(source) {
                    return source.text.toLowerCase() == formatValue.toLowerCase();
                });
                if (!existFamily) {
                    fontFamilySource.push({
                        id: formatValue,
                        text: formatValue
                    })
                }
            });
            return fontFamilySource;
        },
        //1.3em/26px
        getFontSizeSpinnerParam: function(value) {
            var param = {
                unit: "px",
                decimals: 0,
                step: 1,
                value: 0
            };
            if (value) {
                var mathes = value.match(/[0-9]+([.]{1}[0-9]+)?/);
                if (mathes && mathes.length > 0) {
                    param.value = mathes[0];
                    if (param.value.indexOf(".") > -1) {
                        var decimals = param.value.split(".")[1].length;
                        param.decimals = decimals;
                        if (param.decimals > 0) {
                            param.step = param.step / Math.pow(10, decimals);
                        }
                    }
                    var unit = Kooboo.trim(value.replace(param.value, ""));
                    if (unit)
                        param.unit = unit;

                }
            }
            return param;
        },
        formatFontSizeValue: function(value, decimals) {
            var value = parseFloat(value);
            if (isNaN(value))
                return 0;
            value = Math.abs(value);
            if (decimals > 0) {
                value = value.toFixed(decimals);
            }
            return value;
        },
        getFontStyleValue: function(active) {
            return active ? "italic" : "normal";
        },
        
    }
}