function ColorHelper(){
    var requiredProperties = ["background-color", "color"];
    function doGetColorItems(el, items) {
        if (!isElementCanEdit(el))
            return [];
        appendInlineStyleItems(el, items);
        appendExtendStyleItems(el, items);
        appendPseudoStyleItems(el, items);

        var children = el.children;
        if (children && children.length > 0) {
            for (var child = 0; child < children.length; child++) {
                var sub = children[child];
                if ($(sub).is(":visible")) {
                    items = doGetColorItems(sub, items);
                }
            }
        }
        return items;
    }
    function appendInlineStyleItems(el, colorItems) {
        var inlineStyle = Kooboo.style.StyleEditorHelper.getInlineCSSRule(el);
        if (inlineStyle && inlineStyle.length > 0) {
            setColorItemData(inlineStyle, el, true, colorItems);
        }
    }
    function appendExtendStyleItems(el, colorItems) {
        var extendStyles = Kooboo.style.StyleEditorHelper.getMatchedCSSRules(el);
        extendStyles = Kooboo.style.StyleEditorHelper.removePseudoRules(extendStyles);
        for (var i = 0; i < extendStyles.length; i++) {
            var style = extendStyles[i].style;
            setColorItemData(style, el, false, colorItems);
        }
    }
    function appendPseudoStyleItems(el, colorItems) {
        var pseudoRules = Kooboo.style.StylePseudoHelper.getPseudoRules(el);
        for (var i = 0; i < pseudoRules.length; i++) {
            var pseudoRule = pseudoRules[i];
            //a,a:hover will not add to pseudoStyleItem
            var isNeedAddPseudoRule = !existSameSelectorWithPseudo(colorItems, pseudoRule["selector"], pseudoRule["property"]);
            if (isNeedAddPseudoRule) {
                colorItems.push(pseudoRule);
            }
        }
    }
    function existSameSelectorWithPseudo(colorItems, selector, property) {
        var exist = false;
        $.each(colorItems, function(i, colorItem) {
            if (colorItem.property.toLowerCase() == property.toLowerCase() &&
                colorItem.selector == selector) {
                exist = true;
                return false;
            }
            // $.each(colorItem.affects, function(j, affect) {
            //     if (affect.property.toLowerCase() == property.toLowerCase() &&
            //         affect.selector == selector) {
            //         exist = true;
            //         return false;
            //     }
            // });
        });
        return exist;
    }
    function setRequiredProperties(el, colorItems) {
        var computedStyle = Kooboo.style.StyleEditorHelper.getComputedStyle(el);
        setColorItemData(computedStyle, el, true, colorItems, true);
    }
    function isMatchColor(value, computedValue) {
        return value && Kooboo.DomResourceManager.emptyValues.indexOf(value) == -1 && Kooboo.Color.equals(value, computedValue);
    }
    function isExistElementProperty(colorItems, property, el) {
        var item = _.find(colorItems, function(colorItem) {
            var sameProperty = colorItem.property.toLowerCase() == property.toLowerCase() &&
                !colorItem.pseudoClass;
            if (sameProperty) {
                var affect = _.find(colorItem.affects, function(affect) {
                    return affect.element == el;
                });
                return affect != null;
            }
            return false;
        })
        return item ? true : false;
    }
    function findSameColorItem(colorItems, colorItem) {
        var matchItem = null;

        $.each(colorItems, function(key, it) {
            //color,property,pseudoClass，isInline
            if (Kooboo.Color.equals(it.color, colorItem.color) &&
                it.isInline == colorItem.isInline &&
                it.property == colorItem.property &&
                (!it.pseudoClass || it.pseudoClass.length === 0)) {
                matchItem = it;
                return false;
            }
        });
        return matchItem;
    }
    function getSelectorByStyle(style) {
        var selector = "";
        if (style.parentRule && style.parentRule.selectorText) {
            selector = style.parentRule.selectorText;
        }
        return selector;
    }
    function setColorItemData(style, el, isInline, colorItems, isRequiredProperty) {
        var computedStyle = Kooboo.style.StyleEditorHelper.getComputedStyle(el);
        var properties = isRequiredProperty ? requiredProperties : Kooboo.DomResourceManager.colorProperties;
        for (var i = 0; i < properties.length; i++) {
            var prop = properties[i],
                value = style.getPropertyValue(prop),
                computedValue = computedStyle.getPropertyValue(prop);

            if (isRequiredProperty) {
                if (isExistElementProperty(colorItems, prop, el)) {
                    continue;
                }
            } else {
                if (!isMatchColor(value, computedValue)) {
                    continue;
                }
                if (!Kooboo.DomResourceManager.isNeedShowProperty(prop, computedStyle)) {
                    continue;
                }
            }

            var affect = {
                element: el,
                property: prop,
                important: style.getPropertyPriority(prop),
                selector: getSelectorByStyle(style),
                originValue: Kooboo.style.StyleEditorHelper.getOriginPropertyValue(el, prop), //只有当color为initial时才有值，否则为""
                //cssContext: cssContext,
                //inline: !cssContext["global"],
            };
            var colorItem = {
                property: prop,
                pseudoClass: "",
                color: value,
                isInline: isInline, //for default  updateGlobal
                affects: [affect]
            };
            colorItems.push(colorItem);
        }

    }
    function merge(colorItems) {
        var mergeColorItems = [];
        
        $.each(colorItems, function(i, colorItem) {
            var matchItem = findSameColorItem(mergeColorItems, colorItem);
            if (matchItem == null) {
                mergeColorItems.push(colorItem);
            } else {
                var affects = matchItem.affects;
                matchItem.affects = affects.concat(colorItem.affects);
            }
        })
        return mergeColorItems;
    }
    function optimize(colorItems) {
        colorItems = merge(colorItems);
        //1.by affect area
        colorItems = _.sortBy(colorItems, function(colorItem) {
            var affects = colorItem.affects;
            var area = 0;
            $.each(affects, function(i, affect) {
                var el = affect.element;
                if (el) {
                    var elementRect = el.getBoundingClientRect();
                    area += elementRect.width * elementRect.height;
                }
            });
            return -area;
        });
        //2.sort by affect element count
        //colorItems = _.sortBy(colorItems, function(item) { return -item.affects.length });
        return colorItems;
    }
    function isElementCanEdit(el) {
        return Kooboo.Tags.indexOf(el.tagName) > -1 && !_.isEmpty($(el).attr("kooboo-id"));
    }
    return {
        getColorItems: function(context) {
            var items = [],
                el = context.el,
                self = this;
            doGetColorItems(el, items);
            setRequiredProperties(el, items);

            if (!items || items.length == 0) {
                return [];
            }
            items = optimize(items);

            $.each(items, function(i, item) {
                if (item.color == "initial") {
                    item.color = item.affects[0].originValue;
                }
                //item.changeAllGlobalSelectorStatus = self.changeAllGlobalSelectorStatus;
                item.color = Kooboo.Color.searchString(item.color);
                //convert to hex
                //todo confirm
                item.color = Kooboo.DomResourceManager.convertRgbToHex(item.color);
                item.defaultColor = item.color;
                item.prevColor = item.color;

                item.property = item.property;

                item.propertyName = Kooboo.StyleHelper.getTitle(item.property, item.pseudoClass);
            });
            return items;
        },
    }
}