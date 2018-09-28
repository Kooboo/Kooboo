function StyleEditorHelper(){
    var cantMatchedPseudoes = [":visited", ":hover", ":active", ":focus"];
    //xx-small | x-small | small | medium | large | x-large | xx-large
    function getFontSize(size) {
        var size = size.replace("+", "");
        if (size == "") return size;
        if (size == 0)
            return "xx-small";
        if (size == 1)
            return "x-small";
        if (size == 2)
            return "small";
        if (size == 3)
            return "medium";
        if (size == 4)
            return "large";
        if (size == 5)
            return "x-large";
        if (size == 6)
            return "xx-large";
        return size;
    }
    function filterStyle(styleSheets) {
        var self = this;
        var styles = filterMediaStyle(styleSheets);
        var styleSheets = Kooboo.DomResourceManager.getInternalStyleSheets(styles);
        return styleSheets;
    }
    function filterMediaStyle(styleSheets) {
        var filterStyleSheets = [];

        //support media 
        //like <style media="print"> will be excludeValue
        var needMedias = ["screen", "all"];
        $.each(styleSheets, function(i, styleSheet) {

            if (styleSheet.media && styleSheet.media.length > 0) {
                var hasNeedMedia = false;
                for (var j = 0; j < styleSheet.media.length; j++) {
                    var media = styleSheet.media[j];
                    if (needMedias.indexOf(media.toLowerCase()) > -1) {
                        hasNeedMedia = true;
                        break;
                    }
                }
                if (hasNeedMedia) {
                    filterStyleSheets.push(styleSheet);
                }
            } else {
                filterStyleSheets.push(styleSheet);
            }
        });
        return filterStyleSheets;

    }
    function doGetMatchRules(result, cssRules, el, includePseudo) {
        var self = this;
        for (var c = 0; c < cssRules.length; c++) {
            var cssRule = cssRules[c];
            if (cssRule.type == CSSRule.MEDIA_RULE) {
                //this will filter not match rule by cssrule conditionText
                //like @media screen and (min-width: 300px):conditionText is  print and (min-width: 300px) will not match
                if (isMatchMedia(cssRule)) {
                    doGetMatchRules(result, cssRule.cssRules, el);
                }
            }else if(cssRule.type == CSSRule.IMPORT_RULE){
                var styleSheet=cssRule.styleSheet;
                if(styleSheet){
                    doGetMatchRules(result, styleSheet.cssRules, el);
                }
            }else if (cssRule.type == CSSRule.STYLE_RULE && cssRule.selectorText) {
                //two rule:a{} a:hover{}
                //withPseudo-->true: get rule with pseudo(a{} a:hover{})
                //withPseudo-->false:only get rule without pseudo(a{})
                var selectorText = includePseudo ? trimPseudoClass(cssRule.selectorText) :
                    cssRule.selectorText;
                try {
                    if (selectorText && matchesSelector(el, selectorText))
                        result[Object.keys(result).length] = cssRule;
                } catch (ex) {

                }
            }
        };
    }
    function matchesSelector(el, selectorText) {
        var matches = el.matches ||
            el.webkitMatchesSelector ||
            el.mozMatchesSelector ||
            el.oMatchesSelector ||
            el.msMatchesSelector;
        return matches.call(el, selectorText);

    }
    //a:hover--->a
    //div:nth-child(4n)-->div:nth-child(4n) this can be matched(not trim)
    function trimPseudoClass(selectorText) {
        var selectors = getSplitSelectors(selectorText);
        var selectorArr = [];
        for (var i = 0; i < selectors.length; i++) {
            var eachSelector = selectors[i];
            if (eachSelector) {
                eachSelector = eachSelector.toLowerCase();
                //inlineEdit can't be matched pseudoClass
                //filter from: https://developer.mozilla.org/en-US/docs/Web/CSS/Pseudo-classes
                $.each(cantMatchedPseudoes, function(i, pseudoClass) {
                    var index = eachSelector.indexOf(pseudoClass);
                    if (index > -1) {
                        eachSelector = eachSelector.replace(pseudoClass, "");
                        return false;
                    }
                });
                selectorArr.push(eachSelector);
            }
        }
        return selectorArr.join(",");
    }
    function isMatchMedia(cssRule) {
        var self = this;
        var frameWindow = Kooboo.InlineEditor.getIframeWindow();
        if (cssRule.conditionText) {
            var mediaQuery = frameWindow.matchMedia(cssRule.conditionText);
            return mediaQuery.matches;
        }
        return false;
    }
    function getSplitSelectors(selectorText) {
        if (!selectorText) return [];
        var selectors = selectorText.split(",");
        return selectors;
    }
    //need to get rule depending on id or class
    function isClassOrIdRule(rule) {
        return rule && /\.|#/g.test(rule.selectorText);
    }


    return {
        isClassOrIdRule:isClassOrIdRule,
        filterMediaStyle:filterMediaStyle,
        getFontSize:getFontSize,
        trimPseudoClass:trimPseudoClass,

        getInlineCSSRule: function(el) {
            el = el;
            if (!el) {
                return {};
            }
            var inlineRule = el.style;
            if (el.tagName.toLowerCase() == 'font') {
                inlineRule.color = el.color || inlineRule.getPropertyValue('color');
                inlineRule['font-family'] = el.face || inlineRule.getPropertyValue('font-family');

                inlineRule['font-size'] = getFontSize(el.size) || inlineRule.getPropertyValue('font-size');
                //inlineRule['background-color'] = '#' + el.bgcolor || inlineRule.getPropertyValue('background-color');
                $(el).removeAttr('color').removeAttr('face').removeAttr('size');
            }
            return inlineRule;
        },
        
        getIframeWindow: function() {
            return Kooboo.InlineEditor.getIframeWindow();
        },
        getIFrameDoc: function() {
            return Kooboo.InlineEditor.getIFrameDoc();
        },
        getIframeUrl: function() {
            var iframeWindow = this.getIframeWindow();
            return iframeWindow.location.href;
        },
        getComputedStyle: function(el, pseudo) {
            el = el;
            pseudo = pseudo || null;
            if (!el) {
                return {};
            }
            if (el.ownerDocument.defaultView) {
                return el.ownerDocument.defaultView.getComputedStyle(el, pseudo);
            } else if (window.getComputedStyle) {
                return window.getComputedStyle(el, pseudo);
            }
            return null;
        },
        //Edit Color,getMatch CssRule,and so on
        getMatchedCSSRules: function(el, withPseudo) {
            var self = this;
            el = el;
            var rules = {};
            if (!el) {
                return rules;
            }
            var dom = el.ownerDocument;
            if (!dom) {
                return rules;
            }
            var result = {};
            // if (dom.defaultView.getMatchedCSSRules !== undefined) {
            //     result = dom.defaultView.getMatchedCSSRules(el, pseudo);
            // } else {
            //     var styles = dom.styleSheets;
            //     for (var s = 0; s < styles.length; s++) {
            //         var styleSheet = styles[s],
            //             cssRules = styleSheet.cssRules;

            //         for (var c = 0; c < cssRules.length; c++) {
            //             var cssRule = cssRules[c];
            //             if (_.includes($(cssRule.selectorText, dom), el)) {
            //                 result[Object.keys(result).length] = cssRule;
            //             }
            //         };
            //     }
            //     result["length"] = Object.keys(result).length;
            // }
            // return result;
            var styleSheets = filterStyle(dom.styleSheets);
            for (var s = 0; s < styleSheets.length; s++) {
                var styleSheet = styleSheets[s],
                    cssRules = Kooboo.DomResourceManager.getCssRules(styleSheet);

                //optus-style-blessed1.css
                if (!cssRules || cssRules.length == 0) continue;
                doGetMatchRules(result, cssRules, el, withPseudo);
            }
            result["length"] = Object.keys(result).length;

            return result;
        },
        
        //id,class,pseudo,attr
        //don't contain element selector,to avoid if affect too much elements.
        //Edit style
        filterMatchedCSSRules: function(matchedRules) {
            var self = this;
            if (!matchedRules) {
                return [];
            }
            matchedRules = _.toArray(matchedRules);
            return _.filter(matchedRules, function(rule) {
                var selector = rule.selectorText;
                return self.hasElementSelector(selector) ? false : true;

            });
        },
        //是否为元素选择器
        hasElementSelector: function(selector) {
            var splitSelectors = selector.split(",");
            if (splitSelectors && splitSelectors.length > 0) {
                var elementSelector = _.find(splitSelectors, function(splitSelector) {
                    if (splitSelector == "*") return true;
                    var notElementSelector = /\.|#|:|[|]/igm.test(splitSelector);
                    if (!notElementSelector) return true;
                    return false;
                });
                if (elementSelector) return true
            };
            return false;
        },
        isNear: function(element, parentElement) {
            if (!element || !parentElement) {
                return false;
            }
            return (Math.abs(parentElement.offsetWidth - element.offsetWidth) < 10 &&
                Math.abs(parentElement.offsetHeight - element.offsetHeight) < 10);
        },
        removePseudoRules: function(rules) {
            var self = this;
            if (!rules) {
                return [];
            }
            rules = _.toArray(rules);
            return _.filter(rules, function(rule) {
                var selector = rule.selectorText;
                //selector:a Or selector:a,a:link,a:visited will not filter
                return self.filterPseudoClass(selector).length < selector.split(",").length;
            });
        },
        //edit color
        getPseudoRules: function(rules) {
            var self = this;
            if (!rules) {
                return [];
            }
            rules = _.toArray(rules);
            return _.filter(rules, function(rule) {
                var selector = rule.selectorText;
                return self.filterPseudoClass(selector).length > 0;
            });
        },
        
        filterPseudoClass: function(selectorText) {
            var selectors = getSplitSelectors(selectorText);
            var selectorArr = [];

            $.each(selectors, function(i, selector) {
                $.each(cantMatchedPseudoes, function(j, pseudoClass) {
                    if (selector.toLowerCase().indexOf(pseudoClass) > -1) {
                        selectorArr.push(pseudoClass);
                        return false; //break
                    }
                });
            });
            //return selectorText.match(/(\s+::[^\s\+>~\.\[:]+|:link|:visited|:hover|:active|:focus)/gi)
            return selectorArr;
        },
        isLegalClassName: function(className) {
            //.1class--->1
            var firstClassChar = className.substring(1, 2);
            if (isNaN(firstClassChar))
                return true;
            return false;
        },
        
        hasClassOrIdRules: function(matchedRules) {
            var self = this;
            if (!matchedRules || matchedRules.length == 0) return false;

            var hasClassOrIdRule = false;
            for (var i = 0; i < matchedRules.length; i++) {
                var rule = matchedRules[i];
                if (isClassOrIdRule(rule)) {
                    hasClassOrIdRule = true;
                    break;
                }
            }
            return hasClassOrIdRule;

        },
        isEmptyNodeOrHtmlNode: function(node) {
            return !node || !node.parentNode || node.parentNode.tagName.toLowerCase() == "html"
        },
        isElementExist: function(element) {
            return !element ? false : true;
        },
        isMediaRule: function(rule) {
            return Kooboo.DomResourceManager.isMediaRule(rule);
        },
        isEmbeddedStyleSheet: function(styleSheet) {
            return Kooboo.DomResourceManager.isEmbeddedStyleSheet(styleSheet);
        },
        getMediaRuleList: function(cssRule) {
            return Kooboo.DomResourceManager.getMediaRuleList(cssRule);
        },
        getInlineCssJson: function(el, prop) {
            var self = this,
                computedStyle = self.getComputedStyle(el),
                value = computedStyle.getPropertyValue(prop),
                important = computedStyle.getPropertyPriority(prop);
            return {
                "cssProperty": prop,
                "value": value,
                "color": value,
                "selector": null,
                "koobooId": el.getAttribute("kooboo-id"),
                "important": !!important
            };
        },
        getComputePropertyValue: function(el, property) {
            return this.getComputedStyle(el).getPropertyValue(property);
        },
        findRuleBySelector: function(sheet, selector) {
            var rule = _.find(sheet["rules"], function(r) {
                return r["selectorText"] && r["selectorText"].toLowerCase() == selector.toLowerCase();
            });
            return rule;
        },
        getIframeStyleSheets: function() {
            var iframeDoc = Kooboo.InlineEditor.getIFrameDoc();
            var iframeStyleSheets = iframeDoc.styleSheets;
            var styleSheets = Kooboo.DomResourceManager.getInternalStyleSheets(iframeStyleSheets);
            return styleSheets;
        },
        isExistProperty: function(items, property) {
            var item = items[property];
            return item != undefined && item != null;
        },
        getStyleTagKoobooId: function(cssRule) {
            return Kooboo.DomResourceManager.getStyleTagKoobooId(cssRule);
        },
        isEqualValue: function(property, computeValue, value, styleSheetUrl) {
            if (!computeValue) {
                computeValue = "";
            }
            if (!value) {
                value = "";
            }
            if (Kooboo.DomResourceManager.colorProperties.indexOf(property.toLowerCase()) > -1) {
                return Kooboo.Color.equals(computeValue, value);
            }
            if (Kooboo.DomResourceManager.noImageProperties.indexOf(property.toLowerCase()) == -1) {
                var url = Kooboo.DomResourceManager.getStyleImageUrl(value);
                //computeUrl will return full/absolute path
                var computeValueUrl = Kooboo.DomResourceManager.getStyleImageUrl(computeValue);

                var containerUrl;
                if (styleSheetUrl) {
                    containerUrl = styleSheetUrl;
                } else {
                    containerUrl = this.getIframeUrl();
                }
                var baseUrl = Kooboo.DomResourceManager.getBaseUrl();
                var imageUrl = Kooboo.DomResourceManager.getImageFullUrl(baseUrl, containerUrl, url);
                return computeValueUrl.toLowerCase() == imageUrl.toLowerCase();
            }
            if(property.toLowerCase()=="font-weight"){
                
                var isEqual=false;
                if(this.isNormalFontWeight(computeValue)){
                    isEqual=value.toLowerCase()=="normal"||computeValue==value;
                }else{
                    isEqual=["bold","bolder"].indexOf(value.toLowerCase())>-1||computeValue==value;
                }
                return isEqual;
            }

            return computeValue.toLowerCase() == value.toLowerCase();
        },
        isNormalFontWeight:function(value) {
            if (value &&
                (value === "bold" || value === "bolder" || parseInt(value) >= 700)) {
                //700相当于bold
                return false;
            }
            return true;
        },
        isInlineCssRule: function(cssRule) {
            return cssRule.parentRule === null ? true : false;
        },
        importantBaseScore: 1000000,
        getPriority: function(priority, important) {
            return priority + (important ? this.importantBaseScore : 0)
        },
        convertColor: function(tinycolor) {
            if (!tinycolor) {
                return "initial";
            }
            var value = tinycolor.toHexString();
            if (tinycolor._a == 0) {
                value = "transparent";
            } else if (tinycolor._a < 1) {
                //tinycolor._a < 1时,如rgba(0, 0, 0, 0.1) ,转成hex为#000000,与transparent值相同，所以转换成rgba
                value = tinycolor.toRgbString();
            }
            return value;
        },
        //box-shadow,text-shadow 
        getCssNewValue: function(oldValue, newValue) {
            var searchColor = Kooboo.Color.searchString(oldValue);
            if (searchColor) {
                newValue = oldValue.replace(searchColor, newValue);
            }
            return newValue;
        },
        getOriginPropertyValue: function(el, property) {
            if (property === "initial") {
                return this.getComputedStyle(el).getPropertyValue(property);
            } else {
                return "";
            }
        },
    }
}