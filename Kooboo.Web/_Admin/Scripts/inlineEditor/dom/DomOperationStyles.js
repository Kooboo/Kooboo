function DomOperationStyles(item){
    return {
        update: function(value) {
            //new generate rule set to oldValue,need to remove cssRule.
            var isNewRule=item.jsonRule.isNewRule;
            var needRemove=isNewRule &&
                (value==item.oldValue && item.oldValue!=item.newValue);
            if(needRemove){
                Kooboo.dom.DomOperationHelper.removeCssRuleProp(item.cssRule, item.property);
            }else{
                Kooboo.dom.DomOperationHelper.resetCssRule(item.cssRule, item.property, value, item.important ? "!important" : "");
            }
            
        }
    }
}