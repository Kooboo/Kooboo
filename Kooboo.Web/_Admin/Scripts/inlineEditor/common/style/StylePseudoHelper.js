function StylePseudoHelper(){
    var styleEditorHelper=Kooboo.style.StyleEditorHelper;
    var requiredPseudoes=["hover"];
    
    function getRulePropertyValue(colorProperty, rule) {
        return rule.style.getPropertyValue(colorProperty);
    }
    function hasPropertyValue(colorProperty, rule) {
        var value = getRulePropertyValue(colorProperty, rule);
        if (!value) return false;
        return Kooboo.DomResourceManager.emptyValues.indexOf(value) == -1;
    }
    function hasElementPseudo(selectorText){
        var selectorTextsWithoutPseudo = selectorText.split(":")[0];
        return styleEditorHelper.hasElementSelector(selectorTextsWithoutPseudo);
    }
    function getPseudoData(pseudoRules, el) {
        var pseudoData = [];
        $.each(pseudoRules, function(i, rule) {
            var selectorTexts = getPseudoSelectedTexts(rule.selectorText, el);
            $.each(Kooboo.DomResourceManager.colorProperties, function(i, colorProperty) {
                if (hasPropertyValue(colorProperty, rule)) {
                    var value = getRulePropertyValue(colorProperty, rule);
                    $.each(selectorTexts, function(i, selectorText) {
                        var pseudo=selectorText.split(":")[1];
                        //only generate and add required pseudo element selector
                        if(hasElementPseudo(selectorText)){
                            if(requiredPseudoes.indexOf(pseudo.toLowerCase())==-1 ||
                            Kooboo.DomResourceManager.emptyValues.indexOf(value)>-1){
                                return false;
                            }
                            rule = Kooboo.style.StyleRuleGenerator.generatePseudoRule(el,colorProperty,pseudo,rule);
                        }
                        pseudoData.push({
                            "property": colorProperty,
                            "pseudo": pseudo,
                            "selector": rule.selectorText,
                            "value": value,
                            "color": value,
                            "pri": Kooboo.PriorityCalculator.calculate(rule.selectorText),
                            "rule": rule,
                            "el": el
                        });
                    });
                }
            });
        });
        return pseudoData;
    }
    //hover,link
    function getCombinePseudo(valueArr) {
        var pseudo = "";
        valueArr.forEach(function(item, index) {
            if (index == 0) {
                pseudo = item["pseudo"];
            } else if (index > 0) {
                pseudo += "," + item["pseudo"];
            }
            //when pseudo is long,pseudo which has blank space will be word break;
            pseudo += " ";
        });
        return pseudo;
    }
    function getGroupByProperty(pseudoData) {
        //color:{link:{},visite:{}}
        var groupByProperty = getMaxPriorityRuleOfSamePtAndPseudo(pseudoData);
        _.forEach(groupByProperty, function(value, colorProperty) {
            var groupByCssRule = _.groupBy(value, function(item) {
                return item.rule.cssText
            });
            _.forEach(groupByCssRule, function(valueArr, cssText) {
                var combinePseudo = getCombinePseudo(valueArr);
                valueArr[0]["pseudo"] = combinePseudo;
                groupByCssRule[cssText] = valueArr[0];
            });
            groupByProperty[colorProperty] = groupByCssRule;
        });
        //color:{"div":{pseudo:"link,visit"}}
        return groupByProperty;
    }
    function getPseudoSelectedTexts(selectorText, el) {
        var selectorTexts = selectorText.split(",");
        selectorTexts = selectorTexts.map(function(selectorText) { return selectorText.trim() });
        selectorTexts = selectorTexts.filter(function(selectorText) { return selectorText.indexOf(":") > -1 });
        selectorTexts = selectorTexts.filter(function(selectorText) {
            var selectorTextsWithoutPseudo = selectorText.split(":")[0];
            return isExistSelector(selectorTextsWithoutPseudo, el)
        });
        return selectorTexts;
    }
    function getMaxPriorityRuleOfSamePtAndPseudo(pseudoData) {
        var groupByProperty = _.groupBy(pseudoData, "property");
        _.forEach(groupByProperty, function(value, colorProperty) {
            //color:{link:[],visite:[]}
            groupByProperty[colorProperty] = _.groupBy(value, "pseudo");
            _.forEach(groupByProperty[colorProperty], function(valueArr, pseudo) {
                var maxValue;
                var maxPriority = 0;
                $.each(valueArr, function(i, value) {
                    if (value.pri[0].priority >= maxPriority) {
                        maxValue = value;
                        maxPriority = value.pri[0].priority;
                    }
                });
                groupByProperty[colorProperty][pseudo] = maxValue;
            });
        });
        return groupByProperty;
    }
    function getPseudoCssRules(el) {
        var rules = styleEditorHelper.getMatchedCSSRules(el, true);
        if (!rules) {
            return [];
        }
        rules = _.toArray(rules);
        return _.filter(rules, function(rule) {
            var selector = rule.selectorText;
            return styleEditorHelper.filterPseudoClass(selector).length > 0;
        });
    }
    function getPseudoRule(pseudoItem) {
        var affect = getPseudoAffect(pseudoItem),
            value = pseudoItem.value,
            prop = pseudoItem.property,
            pseudoClass = ":" + pseudoItem.pseudo;

        var pseudoRule = {
            "guid": Kooboo.Guid.NewGuid(),
            "color": value,
            "property": prop,
            "pseudoClass": pseudoClass,
            "isInline": false,
            "selector": pseudoItem.selector,
            "affects": [affect]
        }
        return pseudoRule;
    }
    function getPropertyPseudoList(pseudoItem) {
        var selector = pseudoItem.selector;
        var propertyPseudoList = [];
        selector.split(",").forEach(function(sel) {
            if (isExistSelector(sel, pseudoItem.el)) {
                if (styleEditorHelper.filterPseudoClass(sel) &&
                    styleEditorHelper.filterPseudoClass(sel).length > 0) {

                    var pseudoClass = styleEditorHelper.filterPseudoClass(sel)[0];
                    var key = pseudoItem.property + pseudoClass;
                    propertyPseudoList.push(key);
                }
            }
        });
        return propertyPseudoList;
    }
    function isExistSelector(selector, el) {
        return _.includes($(selector, el.ownerDocument), el);
    }
    function getPseudoAffect(pseudoItem) {
        var cssContext = getPseudoCssContext(pseudoItem),
            prop = pseudoItem.property,
            el = pseudoItem.el,
            styleDeclaration = pseudoItem.rule.style,
            important = styleDeclaration.getPropertyPriority(prop);

        var affect = {
            element: el,
            property: prop,
            important: !!important,
            originValue: styleEditorHelper.getOriginPropertyValue(el, prop), //只有当color为initial时才有值，否则为""
            cssContext: cssContext,
            inline: false
        };
        return affect;
    }
    function getPseudoCssContext(pseudoItem) {
        var externalData = getExternalData(pseudoItem),
            styleDeclaration = pseudoItem.rule.style,
            el = pseudoItem.el;
        var cssContext = {
            "externalDeclaration": styleDeclaration,
            "externalData": externalData,
            "inlineDeclaration": el.style,
            "inlineData": null,
            "inlineAttribute": "",
            "global": true,
            // "cssDeclaration": null,
            // "defined": true
        }
        return cssContext;
    }
    function getExternalData(pseudoItem) {
        var styleDeclaration = pseudoItem.rule.style,
            prop = pseudoItem.property,
            value = pseudoItem.value,
            el = pseudoItem.el,
            pseudoClass = ":" + pseudoItem.pseudo;

        var important = styleDeclaration.getPropertyPriority(prop);

        var externalData = {
            color: pseudoItem.color,
            cssProperty: prop,
            important: !!important,
            selector: pseudoItem.rule.selectorText,
            value: pseudoItem.value,
            mediaRuleList: "",
            styleTagKoobooId: "",
            styleSheetUrl: ""
        };

        if (styleEditorHelper.isMediaRule(pseudoItem.rule)) {
            externalData.mediaRuleList = styleEditorHelper.getMediaRuleList(pseudoItem.rule);
        }
        if (styleEditorHelper.isEmbeddedStyleSheet(pseudoItem.rule.parentStyleSheet)) {
            externalData.styleTagKoobooId = getKoobooIdByStyleSheet(pseudoItem.rule.parentStyleSheet);
            externalData.styleSheetUrl = "";
        } else {
            externalData.styleTagKoobooId = "";
            externalData.styleSheetUrl = pseudoItem.rule.parentStyleSheet.href;
        }
        return externalData;
    }
    function getKoobooIdByStyleSheet(styleSheet) {
        return styleSheet.ownerNode.getAttribute("kooboo-id")
    }
    return {
        getPseudoRules: function(el) {
            var pseudoCssRules = getPseudoCssRules(el);
            var pseudoData = getPseudoData(pseudoCssRules, el);
            var groupByProperty = getGroupByProperty(pseudoData);
            //color:{"div":{pseudo:"link,visit"}}
            //var items = [];
            var pseudoRules = [];
            _.forEach(groupByProperty, function(value, key) {
                _.forEach(groupByProperty[key], function(pseudoItem, k) {
                    var pseudoRule = getPseudoRule(pseudoItem);
                    pseudoRules.push(pseudoRule);
                });
            });
            return pseudoRules;
        },
    }
}