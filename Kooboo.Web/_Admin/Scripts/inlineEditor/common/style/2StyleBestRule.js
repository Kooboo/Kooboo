function StyleBestRule(){
    var styleEditorHelper=Kooboo.style.StyleEditorHelper;
    function getAffectedRules(matchedRulesArray, prop, computedValue) {
        var affectedRules = [];
        if (matchedRulesArray && matchedRulesArray.length > 0) {
            //matchedRulesArray = Array.prototype.slice.call(matchedRulesArray, 0);
            //var computedValue=self.getComputedStyle(el).getPropertyValue(prop);
            affectedRules = matchedRulesArray.filter(function(matchedRule) {
                return Kooboo.Color.equals(matchedRule.style.getPropertyValue(prop), computedValue);
            });
        }
        return affectedRules;
    }
    function getMatchRulesFromParent(el) {
        var hasDeclare = false,
            matchedCSSRules = [];
        el = el;
        if (!el) {
            return null;
        }
        var parentNode = el.parentNode;

        while (parentNode && styleEditorHelper.isNear(el, parentNode) && !hasDeclare) {
            var matchedRules = styleEditorHelper.filterMatchedCSSRules(styleEditorHelper.getMatchedCSSRules(parentNode));
            if (matchedRules && matchedRules.length) {
                matchedCSSRules = matchedRules;
                hasDeclare = true;
                break;
            }
            parentNode = parentNode.parentNode;
        }
        return {
            rules: matchedCSSRules,
            el: parentNode
        };
    }
    function isMatchedRuleHasPropertyValue(matchedRulesArray, prop) {
        var hasPropertyValue = false;
        for (var i = 0; i < matchedRulesArray.length; i++) {

            if (matchedRulesArray[i].style[prop]) {
                hasPropertyValue = true;
                break;
            }
        }
        return hasPropertyValue;
    }
    function doGetBestRule(matchedRules, el, prop) {
        var list = [];
        for (var index = 0; index < matchedRules.length; index++) {
            var cssStyleRule = matchedRules[index],
                pris = Kooboo.PriorityCalculator.calculate(cssStyleRule.selectorText),
                completeSelector = cssStyleRule.selectorText,
                doc = el.ownerDocument;
            var baseScore = getCssRuleScore(cssStyleRule, el, prop, index);

            for (var p = 0; p < pris.length; p++) {
                var entity = pris[p];
                //remove Pseudo
                var selector = entity.selector.split(/:[\w\(\)-.]*/).join(" ");
                if (isEffectiveSelector(selector, el)) {
                    list.push({
                        completeSelector: completeSelector,
                        selector: _.trim(entity.selector),
                        priority: entity.priority + baseScore,
                        cssRule: cssStyleRule
                    });
                }
            }
        }

        var maxItem = _.maxBy(list.reverse(), function(it) { return it.priority });
        return maxItem;
    }
    function isAffectCssRule(cssDeclaration, el) {
        var isAffectedRule = cssDeclaration.parentRule.selectorText.split(",").some(function(item) {
            return _.includes($(item, el.ownerDocument), el) === true;
        });
        return isAffectedRule;
    }
    function getCssRuleScore(cssStyleRule, el, prop, index) {
        var cssDeclaration = cssStyleRule.style,
            baseScore = 0;

        
        if (cssDeclaration) {
            var isAffectedRule = isAffectCssRule(cssDeclaration, el);
            if (isAffectedRule) {

                baseScore += 1000;
                if (cssDeclaration.getPropertyValue(prop) &&
                    Kooboo.Color.equals(cssDeclaration.getPropertyValue(prop), styleEditorHelper.getComputedStyle(el).getPropertyValue(prop)) &&
                    Kooboo.DomResourceManager.emptyValues.indexOf(cssDeclaration.getPropertyValue(prop)) === -1) {

                    baseScore += 1000;
                    //CSS先后顺序对于权重的影响，越后面优先级越高
                    baseScore += index;
                }
            }
            if (cssDeclaration.getPropertyPriority(prop)) {
                baseScore += 10000;
                baseScore += index;
            };
        }
        return baseScore;
    }
    function isEffectiveSelector(selector, el) {
        var doc = el.ownerDocument;
        return _.includes($(selector, doc), el);
    }
    function getMatchRulesArray(el,notFilterElementRule){
        var matchedCssRules=styleEditorHelper.getMatchedCSSRules(el),
            matchedRulesArray=[];
        if(notFilterElementRule){
            for (var i = 0; i < matchedCssRules.length; i++) {
                matchedRulesArray.push(matchedCssRules[i]);
            }
        }else{
            matchedRulesArray=styleEditorHelper.filterMatchedCSSRules(matchedCssRules);
        }
        return matchedRulesArray;
    }
    return {
        find: function(el, prop,notFilterElementRule) {
            var doc = el.ownerDocument,
                matchedRulesArray=getMatchRulesArray(el,notFilterElementRule),
                result = null;

            var computedValue = styleEditorHelper.getComputedStyle(el).getPropertyValue(prop);
            var affectedRules = getAffectedRules(matchedRulesArray, prop, computedValue);
            
            if (affectedRules && affectedRules.length > 0) {
                matchedRulesArray = affectedRules;
            }
            if (!matchedRulesArray || !matchedRulesArray.length) {
                var parentSearchResult = getMatchRulesFromParent(el);
                matchedRulesArray = parentSearchResult.rules;
                if (!matchedRulesArray || !matchedRulesArray.length) {
                    result = null;
                } else {
                    if (isMatchedRuleHasPropertyValue(matchedRulesArray, prop)) {
                        result = this.getBestRule(matchedRulesArray, parentSearchResult.el, prop);
                    }

                }
            } else {
                result = this.getBestRule(matchedRulesArray, el, prop,notFilterElementRule);
            }
            return result;
        },
        getBestRule: function(matchedRules, el, prop,notFilterElementRule) {
            //var emptyValues = ["none", "initial", "", null, undefined, "transparent", "inherit"];
            //从已定义的里面挑一个权重最高的
            //有定义指定属性的优先使用(权重加10万分)
            var list = [],
                result = null,
                doc = el.ownerDocument;
            if (!matchedRules || !matchedRules.length) {
                return null;
            }

            var maxItem = doGetBestRule(matchedRules, el, prop),
                cssRule = maxItem ? maxItem["cssRule"] : null;
            if (!cssRule) {
                return result;
            };
            if (maxItem.completeSelector == cssRule.selectorText &&
                (maxItem.completeSelector != "*" || notFilterElementRule)) {
                result = cssRule;
            }
            return result;
        },
        
    }

}