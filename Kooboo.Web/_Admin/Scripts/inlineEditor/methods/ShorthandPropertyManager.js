function FontRule(){
    return {
        shorthandProperty: "font",
        properties: ["font-style", "font-variant", "font-weight", "font-stretch", "font-size", "line-height", "font-family"],
        // [ [ <'font-style'> || <font-variant-css21> || <'font-weight'> || <'font-stretch'> ]? 
        // <'font-size'> [ / <'line-height'> ]? <'font-family'> ]
        getShorthandPropertyValue: function(propertyValueObj) {
            var properties = this.properties;
            var propertyValueArr = [];

            $.each(properties, function(i, property) {
                var value = propertyValueObj[property];
                if (value) {
                    if (property == "line-height") {
                        propertyValueArr.push("/" + value);
                    } else {
                        propertyValueArr.push(value);
                    }
                }

            });
            return propertyValueArr.join(" ");
        }
    }
}

function BackgroundRule(){
    return {
        shorthandProperty: "background",
        properties: ["background-image", "background-position", "background-size", "background-repeat", "background-attachment", "background-origin", "background-clip", "background-color"],
        //Formal syntax <bg-image> || <position> [ / <bg-size> ]? || <repeat-style> || <attachment> || <box> || <box> || <'background-color'>
        getShorthandPropertyValue: function(propertyValueObj) {
            var properties = this.properties;
            var propertyValueArr = [];
            $.each(properties, function(i, property) {
                var value = propertyValueObj[property];
                if (value) {
                    if (value != "initial") { //去掉初始值initial,initial 值会导致style无法显示
                        if (property == "background-size") {
                            propertyValueArr.push("/" + value);
                        } else {
                            propertyValueArr.push(value);
                        }
                    }
                }

            });
            return propertyValueArr.join(" ");
        }
    }   
}

function BordercolorRule(){
    return {
        shorthandProperty: "border-color",
        properties: ["border-top-color", "border-right-color", "border-bottom-color", "border-left-color"],
            //Formal syntax <rgb()> | <rgba()> | <hsl()> | <hsla()> 
        getShorthandPropertyValue: function(propertyValueObj) {
            var properties = this.properties;
            var propertyValueArr = [];
            $.each(properties, function(i, property) {
                var value = propertyValueObj[property];
                if (value)
                    propertyValueArr.push(value);
            });
            return propertyValueArr.join(" ");
        }
    }
}

function BorderRule(){
    return {
        shorthandProperty: "border",
        properties: ["border-width", "border-style", "border-color"],
        //Formal syntax br - width > || < br - style > || < color >
        getShorthandPropertyValue: function(propertyValueObj) {
            var properties = this.properties;
            var propertyValueArr = [];
            $.each(properties, function(i, property) {
                var value = propertyValueObj[property];
                if (value)
                    propertyValueArr.push(value);
            });
            return propertyValueArr.join(" ");
        }
    }
}

function ListStyleRule(){
    return {
        shorthandProperty: "list-style",
        properties: ["list-style-type", "list-style-image", "list-style-position"],
        //Formal <'list-style-type'> || <'list-style-image'> || <'list-style-position'>
        getShorthandPropertyValue: function(propertyValueObj) {
            var properties = this.properties;
            var propertyValueArr = [];
            $.each(properties, function(i, property) {
                var value = propertyValueObj[property];
                if (value) {
                    if (value != "initial") { //去掉初始值initial
                        propertyValueArr.push(value);
                    }
                }

            });
            return propertyValueArr.join(" ");
        }
    }
}

function OutlineRule(){
    return {
        shorthandProperty: "outline",
        properties: ["outline-color", "outline-style", "outline-width"],
        //[ <'outline-color'> || <'outline-style'> || <'outline-width'> ]
        getShorthandPropertyValue: function(propertyValueObj) {
            var properties = this.properties;
            var propertyValueArr = [];
            $.each(properties, function(i, property) {
                var value = propertyValueObj[property];
                if (value) {
                    if (value != "initial") { //去掉初始值initial
                        propertyValueArr.push(value);
                    }
                }

            });
            return propertyValueArr.join(" ");
        }
    }
}

function CueRule(){
    return {
        shorthandProperty: "cue",
        properties: ["cue-before", "cue-after"],
            //Formal syntax  cue-before || cue-after 
        getShorthandPropertyValue: function(propertyValueObj) {
            var properties = this.properties;
            var propertyValueArr = [];
            $.each(properties, function(i, property) {
                var value = propertyValueObj[property];
                if (value)
                    propertyValueArr.push(value);
            });
            return propertyValueArr.join(" ");
        }
    }
}

function ShorthandPropertyManager(){
    var param={
        rules:getRules(),
        keyProperyDic:{},
    }
    function getRules(){
       return [Kooboo.FontRule, Kooboo.BackgroundRule, Kooboo.BordercolorRule,
        Kooboo.BorderRule, Kooboo.ListStyleRule, Kooboo.CueRule, Kooboo.OutlineRule]
    }
    function getRuleByProperty(shorthandProperty) {
        var matchRule = {};
        $.each(param.rules, function(i, rule) {
            if (rule.shorthandProperty == shorthandProperty) {
                matchRule = rule;
                return false
            }
        });
        return matchRule;
    }
    //获取property的值(如shorthand的值)
    function getPropertiesValueObj(cssRule, shorthandProperty) {
        var propertyValueObj = {};
        var rule = getRuleByProperty(shorthandProperty);
        var properties = rule.properties;
        $.each(properties, function(i, property) {
            propertyValueObj[property] = cssRule.getPropertyValue(property);
        });

        return propertyValueObj;
    }
    //第一次缓存值是获取
    function getShortPropertyByCssRule(cssRule, property) {
        var matchShorthandProperty = "";
        $.each(param.rules, function(i, rule) {
            var shorthandProperty = rule.shorthandProperty;
            var properties = rule.properties;
            if (properties.indexOf(property.toLowerCase()) > -1) {
                //shorthandProperty有值,才能保证是从shorthand过来
                if (cssRule[shorthandProperty]) {
                    matchShorthandProperty = shorthandProperty;
                }
                return false;
            }

        });
        return matchShorthandProperty;
    }
    //获取保存的key值
    function getKey(jsonRule, el) {
        var key = "";
        //koobooId,selector,styleSheetUrl,styleTagKoobooId
        if (jsonRule.styleSheetUrl) {
            key = jsonRule.styleSheetUrl + jsonRule.selector;
        } else if (jsonRule.styleTagKoobooId) {
            key = jsonRule.styleTagKoobooId + jsonRule.selector;
        } else if (el) {
            var koobooObject = Kooboo.KoobooObjectManager.getFirstWrapKoobooObject(el);
            if (koobooObject)
                key = jsonRule.koobooId + koobooObject.nameOrId.toLowerCase() + koobooObject.objectType.toLowerCase();
        } else {
            key = jsonRule.koobooId;
        }
        return key;
    }
    return {
        //根据shorthandProperty获取各个分开的属性值
        getShorthandPropertyValue: function(cssRule, shorthandProperty) {
            var propertyValueObj = getPropertiesValueObj(cssRule, shorthandProperty);
            var rule = getRuleByProperty(shorthandProperty);
            return rule.getShorthandPropertyValue(propertyValueObj)
        },
 
        //缓存所有第一次操作时的style的属性,听缓存原先是shorthand 的property
        setPropertyKeyDic: function(jsonRule, cssRule, property, el) {
            var key = getKey(jsonRule, el);
            var matchShorthandProperty = "";
            if (param.keyProperyDic[key]) {
                $.each(param.keyProperyDic[key], function(i, shorthand) {
                    var rule = getRuleByProperty(shorthand);
                    if (rule.properties.indexOf(property.toLowerCase()) > -1) {
                        matchShorthandProperty = shorthand;
                        return false;
                    }
                });
                return matchShorthandProperty;
            }
            var shorthandList = [];

            $.each(param.rules, function(i, rule) {
                var shorthandProperty = rule.shorthandProperty;
                //get all shorthand property in this cssrule
                if (cssRule[shorthandProperty]) {
                    shorthandList.push(shorthandProperty);
                    //get the property shorthand property
                    if (rule.properties.indexOf(property.toLowerCase()) > -1) {
                        matchShorthandProperty = shorthandProperty;
                    }
                }
            });
            if (shorthandList.length > 0) {
                param.keyProperyDic[key] = shorthandList;
            }
            return matchShorthandProperty;
        },
        
    }
}