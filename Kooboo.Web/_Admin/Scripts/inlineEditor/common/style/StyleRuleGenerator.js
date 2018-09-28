function StyleRuleGenerator(){
    var styleEditorHelper=Kooboo.style.StyleEditorHelper,
        styleBestRule=Kooboo.style.StyleBestRule;
    
    var cachedGeneratedRules={};
    //parent has class or Id Rule
    function getParentMatchRulesAndSelectors(el) {
        var parentMatchedRules = {},
            selectors = [],
            hasClassOrIdRule = false;
        var selector = generateElementSelector(el);
        selectors.push(selector);

        var parentNode = el;
        while (parentNode && !hasClassOrIdRule) {
            if (styleEditorHelper.isEmptyNodeOrHtmlNode(parentNode)) {
                break;
            }
            parentNode = parentNode.parentNode;

            if (parentNode) {
                var tempSelector = generateElementSelector(parentNode);

                selectors.push(tempSelector);
                parentMatchedRules = styleEditorHelper.getMatchedCSSRules(parentNode);
                hasClassOrIdRule = styleEditorHelper.hasClassOrIdRules(parentMatchedRules);
            }

        }
        return {
            parentMatchedRules: parentMatchedRules,
            selectors: selectors,
            hasClassOrIdRule: hasClassOrIdRule,
            parentNode: hasClassOrIdRule ? parentNode : null
        }
    }
    function generateElementSelector(el) {
        var className = _(el.classList).map(function(cs) {
            if (styleEditorHelper.isLegalClassName(cs)) {
                return "." + cs;
            }

        }).value().join("");

        var selector = el.tagName.toLowerCase();
        selector += className;

        return selector;
    }
    function getSheetByparentMatchRules(parentMatchRulesAndSelectors, prop) {
        var sheet = null;
        if (parentMatchRulesAndSelectors.hasClassOrIdRule) {
            var parentNode = parentMatchRulesAndSelectors.parentNode,
                matchedRules = parentMatchRulesAndSelectors.parentMatchedRules;

            var parentRule = styleBestRule.getBestRule(matchedRules, parentNode, prop) || {};
            sheet = parentRule.parentStyleSheet;
        }
        return sheet;
    }
    function generateNewCssRuleSelectorText(parentMatchRulesAndSelectors) {
        var selectors = parentMatchRulesAndSelectors.selectors;
        selectors = getProcessedSeletors(selectors);

        var selectorText = selectors.join(' ')
        return selectorText;
    }
    function getProcessedSeletors(selectors) {
        //self._resetTopLevelSelector(selectorText, selectors, el);

        selectors.reverse();

        // selectors = selectors.map(function(item) {
        //     return item.replace(/\.[0-9]+\w+/ig, "");
        // });
        //to let selector not to long
        selectors = selectors.slice(-6);
        return selectors;
    }
    function doGenerateCssRule(sheet, selector) {
        if (selector) {
            if (sheet && !$.isEmptyObject(sheet)) {
                var rule = styleEditorHelper.findRuleBySelector(sheet, selector);

                if (!rule) {
                    var iframeStyleSheets = styleEditorHelper.getIframeStyleSheets();
                    if (iframeStyleSheets) {
                        $.each(iframeStyleSheets, function(i, styleSheet) {
                            var cssRules= Kooboo.DomResourceManager.getCssRules(styleSheet);
                            if (cssRules) {
                                $.each(cssRules, function(j, cssRule) {
                                    if (cssRule["selectorText"] &&
                                        cssRule["selectorText"].toLowerCase() == selector.toLowerCase()) {

                                        rule = cssRule;
                                        return false;
                                    }

                                })
                            }
                        })
                    }
                }
                if (!rule) {
                    rule = insertRuleToSheet(sheet, selector);
                }
                return rule;
            } else {
                var iframeStyleSheets = styleEditorHelper.getIframeStyleSheets();

                //有style文件的情况，插入最后面的style文内件
                if (iframeStyleSheets && iframeStyleSheets.length) {
                    var lastSheet = getLastRightSheet(iframeStyleSheets);
                    if (lastSheet) {
                        rule = insertRuleToSheet(lastSheet, selector);
                        return rule;
                    }
                    return addRuleToHead(selector);
                } else {
                    return addRuleToHead(selector);

                }

            }

        }
        return {};
    }
    //获取最后一个sheet,如果sheet为外部链接时，取最后一个站点的sheet
    function getLastRightSheet(iframeStyleSheets) {
        for (var i = iframeStyleSheets.length - 1; i >= 0; i--) {
            var sheet = iframeStyleSheets[i];
            var cssRules=Kooboo.DomResourceManager.getCssRules(sheet);
            if (cssRules && cssRules.length > 0) {
                return sheet;
            }
        }
        return null;
    }
    function insertRuleToSheet(sheet, selector) {
        var cssRules=Kooboo.DomResourceManager.getCssRules(sheet);
        if(cssRules){
            var index = sheet.insertRule(selector + "{}", cssRules.length);
            rule = cssRules[index];
            return rule;
        }
        return {};
    }
    function addRuleToHead(selector) {
        //没有style文件的情况，新增style标签插入当前页面
        var iframeDoc =Kooboo.InlineEditor.getIFrameDoc();
        var headTag = $("head", iframeDoc);
        var styleTag = iframeDoc.createElement("style");
        $(styleTag).text(selector + "{}");
        $(headTag).append(styleTag);
        return iframeDoc.styleSheets[iframeDoc.styleSheets.length - 1].cssRules[0]
    }
    function getNewRule(sheet,selectorText){
        var newCssRule;
        if(cachedGeneratedRules[selectorText]){
            newCssRule=cachedGeneratedRules[selectorText]
        }else{
            newCssRule = doGenerateCssRule(sheet, selectorText);
            cachedGeneratedRules[selectorText]=newCssRule;
        }
        return newCssRule;
    }
    return {
        generateCssRule: function(el, prop, excludeValue) {
            var selectors = [],
                computedStyle = styleEditorHelper.getComputedStyle(el),
                computedValue = computedStyle.getPropertyValue(prop),
                important = computedStyle.getPropertyPriority(prop),
                //koobooId = el.getAttribute("kooboo-id"),
                result = {
                    cssRule: null,
                    json: {},
                    priority: 0
                };
            //get parentRule which has Id Or Class 
            var parentMatchRulesAndSelectors = getParentMatchRulesAndSelectors(el);
            //getSheetBy parent Rules
            var sheet = getSheetByparentMatchRules(parentMatchRulesAndSelectors, prop);
            var selectorText = generateNewCssRuleSelectorText(parentMatchRulesAndSelectors);

            var newCssRule=getNewRule(sheet,selectorText);
            if (!excludeValue && newCssRule && newCssRule.style && newCssRule.style.setProperty) {
                newCssRule.style.setProperty(prop, computedValue, important);
            }

            newJson = {};
            newJson["selector"] = selectorText;
            newJson["koobooId"] = "";
            newJson["cssProperty"] = prop;
            newJson["value"] = computedValue;
            newJson["color"] = Kooboo.Color.searchString(computedValue);
            newJson["important"] = !!important;
            newJson["mediaRuleList"] = null;
            newJson["styleSheetUrl"] = "";

            result["json"] = newJson;
            result["cssRule"] = newCssRule;
            return result;
        },
        generatePseudoRule:function(el,prop,pseudo,rule){
            var value= rule.style.getPropertyValue(prop),
                important=rule.style.getPropertyPriority(prop);

            var parentMatchRulesAndSelectors = getParentMatchRulesAndSelectors(el);
            var selectorText = generateNewCssRuleSelectorText(parentMatchRulesAndSelectors);
            selectorText +=":"+pseudo;
            //getSheetBy parent Rules
            //var sheet = getSheetByparentMatchRules(parentMatchRulesAndSelectors, prop);
            var sheet=rule.parentStyleSheet;
           
            var newCssRule=getNewRule(sheet,selectorText);

            
            if (newCssRule && newCssRule.style && newCssRule.style.setProperty) {
                newCssRule.style.setProperty(prop, value, important);
            }
            return newCssRule;
        }
        
    }
}