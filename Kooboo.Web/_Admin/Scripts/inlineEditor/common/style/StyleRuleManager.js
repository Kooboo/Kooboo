function StyleRuleManager(){
    var styleEditorHelper=Kooboo.style.StyleEditorHelper,
        styleBestRule=Kooboo.style.StyleBestRule,
        styleRuleGenerator=Kooboo.style.StyleRuleGenerator;
    
    function setMatchedRules(el, items, properties, notFilterElementRule) {
        //edit style: background-image:need get the cssrule for remove.so it can't filter matched element cssRule
        var matchedCssRules=styleEditorHelper.getMatchedCSSRules(el);
        var rules = notFilterElementRule ? matchedCssRules :
            styleEditorHelper.filterMatchedCSSRules(matchedCssRules),
            dom = styleEditorHelper.getIFrameDoc();
        if (rules && rules.length) {
            for (var i = 0; i < rules.length; i++) {
                var rule = rules[i],
                    style = rule.style,
                    parentStyleSheet = rule.parentStyleSheet,
                    priorities = Kooboo.PriorityCalculator.calculate(rule.selectorText);
                for (var p = 0; p < priorities.length; p++) {
                    var pri = priorities[p],
                        sels = $(pri.selector, dom);
                    //is exist
                    if (!sels || !_.includes(sels, el)) {
                        continue;
                    }
                    var priority = pri.priority;

                    var newRule = {
                        cssRule: style,
                        parentStyleSheet: parentStyleSheet
                    }

                    setStyleRule(el, items, properties, newRule, rule.selectorText, priority);
                }
            }
        }
    }
    function setStyleRule(el, items, properties, rule, selectorText, priority) {
        var cssRule = rule.cssRule;
        if (!cssRule) return;

        var cssAndJsonRules = getCssAndJsonRule(el, properties, selectorText, rule);
        $.each(cssAndJsonRules, function(i, cssAndJsonRule) {
            var cssRule = cssAndJsonRule.cssRule,
                jsonRule = cssAndJsonRule.jsonRule,
                important = jsonRule.important,
                newpriority = styleEditorHelper.getPriority(priority, important),
                property = jsonRule.cssProperty,
                value = jsonRule.value;
            setItemByPriority(items, property, value, cssRule, jsonRule, newpriority);
        });
    }
    function getCssAndJsonRule(el, properties, selectorText, rule) {
        var cssAndJsonRules = [],
            cssRule = rule.cssRule,
            parentStyleSheet = rule.parentStyleSheet;
        var computedStyle = styleEditorHelper.getComputedStyle(el);
        for (var i = 0; i < properties.length; i++) {

            var property = properties[i],
                value = cssRule.getPropertyValue(property);

            var computeValue = computedStyle.getPropertyValue(property);
            var styleSheetUrl = parentStyleSheet ? parentStyleSheet.href : "";
            //font-size can't compare computeValue
            //like original value is .3em.but computevalue may be 14px
            if (["font-size", "font"].indexOf(property) > -1) {
                if (!value) {
                    continue;
                }
            } else if (!styleEditorHelper.isEqualValue(property, computeValue, value, styleSheetUrl)) {
                continue;
            }
            var isInline = styleEditorHelper.isInlineCssRule(cssRule);
            var jsonRule = {
                important: !!cssRule.getPropertyPriority(property),
                koobooId: isInline ? selectorText : "",
                color: value.replace(/\"/ig, '\''),
                value: value.replace(/\"/ig, '\''),
                cssProperty: property,
                selector: !isInline ? selectorText : "",
                styleSheetUrl: styleSheetUrl,
                styleTagKoobooId: styleEditorHelper.getStyleTagKoobooId(cssRule),
                mediaRuleList: styleEditorHelper.getMediaRuleList(cssRule)
            };
            var cssAndJsonRule = {
                cssRule: cssRule,
                jsonRule: jsonRule
            }
            cssAndJsonRules.push(cssAndJsonRule);
        }
        return cssAndJsonRules;
    }
    function isHighProperty(items, property, priority) {
        if (styleEditorHelper.isExistProperty(items, property)) {
            var old = items[property];
            if (old.priority <= priority)
                return true;
        } else {
            return true;
        }
        return false;
    }
    function setItemByPriority(items, property, value, cssRule, jsonRule, priority) {
        if (isHighProperty(items, property, priority)) {
            setItem(items, property, value, cssRule, jsonRule, priority);
        }
    }
    function setItem(items, property, value, cssRule, jsonRule, priority) {
        var dom = styleEditorHelper.getIFrameDoc();
        items[property] = {
            priority: priority,
            entity: {
                title: property,
                property: property,
                cssRule: cssRule,
                jsonRule: jsonRule,
                value: value,
                dom: dom,
            }
        }
    }
    //edit style: background image,background-color,color,font-family, font-size, font-weight, font-style
    function setRequiredProperties(element, items, requiredProperties) {
        for (var rp = 0; rp < requiredProperties.length; rp++) {

            var prop = requiredProperties[rp];

            if (!element) {
                continue;
            }
            if (styleEditorHelper.isExistProperty(items, prop) ||
                !styleEditorHelper.isElementExist(element)) {
                continue;
            }
            items[prop] = getRequireProp(element, prop);

        }
    }
    function getRequireProp(element, prop) {
        var doc = styleEditorHelper.getIFrameDoc(),
            computedRules = styleEditorHelper.getComputedStyle(element),
            newVal = computedRules.getPropertyValue(prop),
            cssRule = null,
            koobooId = element.getAttribute("kooboo-id"),
            json = {};

        rule = styleRuleGenerator.generateCssRule(element, prop, false);
        cssRule = rule.cssRule || rule;

        json = {
            color: newVal.replace(/\"/ig, '\''),
            cssProperty: prop,
            isNewRule:true,//new rule need remove changed property value
            important: cssRule.style ? !!cssRule.style.getPropertyPriority(prop) : "",
            koobooId: "",
            selector: cssRule.selectorText,
            value: newVal.replace(/\"/ig, '\''),
            styleSheetUrl: cssRule.parentStyleSheet ? cssRule.parentStyleSheet.href : "",
            styleTagKoobooId: styleEditorHelper.getStyleTagKoobooId(cssRule),
            mediaRuleList: styleEditorHelper.getMediaRuleList(cssRule)
        };
        return {
            priority: 0,
            entity: {
                title: prop,
                property: prop,
                value: newVal,
                cssRule: (cssRule || element)["style"],
                jsonRule: json,
                dom: doc
            }
        };

    }
    return {
        getMatchRules: function(el, properties, requiredProperties, notFilterElementRule) {
            var items = {},
                selectorText = el.getAttribute("kooboo-id"),
                inlinePriority = styleEditorHelper.importantBaseScore - 1,
                rule = {
                    cssRule: el.style,
                    parentStyleSheet: null
                };

            setStyleRule(el, items, properties, rule, selectorText, inlinePriority);

            setMatchedRules(el, items, properties, notFilterElementRule);

            setRequiredProperties(el, items, requiredProperties);
            return items;
        },
        
        //for blockcolors
        getCssContext: function(el, prop) {

            if (!el || el.tagName == 'script'.toUpperCase()) {
                return {
                    "koobooId": null,
                    "cssRule": null,
                    "json": null,
                    "inlineDeclaration": null,
                    "inlineData": null,
                    "inlineAttribute": null,
                    "global": false,
                    // "cssDeclaration": null,
                    // "defined": false
                };
            };

            var container = el,
                externalDeclaration = null,
                externalData = null,
                inlineDeclaration = el.style,
                inlineData = styleEditorHelper.getInlineCssJson(el, prop),
                //ownerDeclaration = self._getMaxPriorityMatchedDeclaration(el, prop),

                cssRule = styleBestRule.find(el, prop),
                //defined = ownerDeclaration && emptyValues.indexOf(ownerDeclaration.getPropertyValue(prop)) == -1,
                global = !!(cssRule && cssRule.style && Kooboo.DomResourceManager.emptyValues.indexOf(cssRule.style.getPropertyValue(prop)) == -1);
            if (!cssRule) {
                var newCssRule = styleRuleGenerator.generateCssRule(el, prop, true);
                cssRule = newCssRule.cssRule;
                externalData = newCssRule.json;
                //no koobooid
            } else {
                if (cssRule.selectorText) {

                    //1.cssRule.json is empty,original Id
                    externalData = {};

                    externalData.color = cssRule.style[prop];
                    externalData.cssProperty = prop;
                    externalData.important = !!cssRule.style.getPropertyPriority(prop);
                    externalData.koobooId = container.getAttribute("kooboo-id");
                    externalData.selector = cssRule.selectorText;
                    externalData.value = cssRule.style.getPropertyValue(prop);
                    if (styleEditorHelper.isMediaRule(cssRule)) {
                        externalData.mediaRuleList = styleEditorHelper.getMediaRuleList(cssRule);
                    } else {
                        externalData.mediaRuleList = null;
                    }
                    if (styleEditorHelper.isEmbeddedStyleSheet(cssRule.parentStyleSheet)) {
                        externalData.styleTagKoobooId = cssRule.parentStyleSheet.ownerNode.getAttribute("kooboo-id");
                        externalData.styleSheetUrl = "";
                    } else {
                        externalData.styleTagKoobooId = "";
                        externalData.styleSheetUrl = cssRule.parentStyleSheet.href;
                    }
                } else {

                    externalData = cssRule.json;
                }
            }
            try {
                var childCssRule = cssRule;
                //style rule
                while (childCssRule.type !== 1) {
                    childCssRule = childCssRule.cssRule;
                }
                externalDeclaration = childCssRule.style;
            } catch (exp) {
                externalDeclaration = {};
            }
            return {
                "koobooId": el.getAttribute("kooboo-id"),
                "externalDeclaration": externalDeclaration,
                "externalData": externalData,
                "inlineDeclaration": inlineDeclaration,
                "inlineData": inlineData,
                "inlineAttribute": el.getAttribute("style"),
                "global": !!global
            };
        },
    }
}