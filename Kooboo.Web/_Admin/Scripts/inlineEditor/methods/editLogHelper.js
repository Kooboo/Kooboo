function EditLogHelper(){
    function getAttributeData(item) {
        var decorateItem = getCommonItemData(item);
        decorateItem.attributeName = item.name;
        return decorateItem;
    }
    function getImageData(item) {
        var decorateItem = getCommonItemData(item);
        if (decorateItem.value) {
            var decorateItems = [];
            for (var key in decorateItem.value) {
                var newDecorateItem = Object.assign({}, decorateItem);
                if (["height", "width"].indexOf(key.toLowerCase()) > -1) {
                    newDecorateItem.editorType = Kooboo.SiteEditorTypes.EditorType.style;
                    newDecorateItem.property = key;
                    newDecorateItem.value = decorateItem.value[key] + "px";
                } else {
                    newDecorateItem.attributeName = key;
                    newDecorateItem.value = decorateItem.value[key];
                }

                decorateItems.push(newDecorateItem);
            }
            return decorateItems;
        }
        return decorateItem;
    }
    function getCommonItemData(item) {
        var context = getContext(item);
        var decorateItem = {
            action: item.action,
            editorType: item.editorType.toLocaleLowerCase(),
            koobooId: context.koobooId,
            value:removeUnnecessary(item.newValue),
            nameOrId: "",
            objectType: "" //page,htmlblock
        };
        if(item.action==Kooboo.SiteEditorTypes.ActionType.delete&&
        item.domOperationType == Kooboo.SiteEditorTypes.DomOperationType.html){
            var el=item.context.el;
            var koobooId= Kooboo.elementReader.readObject(el).koobooId;
            decorateItem.koobooId=koobooId;
            decorateItem.value="";
        }
        var domOperationType = item.domOperationType;
        var koobooObject =getRightKoobooObject(domOperationType, context);
        var objectType = koobooObject.type;
        var nameOrId = koobooObject.nameOrId;
        if (objectType.toLocaleLowerCase() == Kooboo.SiteEditorTypes.ObjectType.label) {
            nameOrId = item.el.getAttribute(koobooObject.attributeName);
        }
        decorateItem.nameOrId = nameOrId;
        decorateItem.objectType = objectType;
        return decorateItem;
    }
    function getTextContent(item) {
        var decorateItem = getCommonItemData(item);
        if (item.action == Kooboo.SiteEditorTypes.ActionType.delete) {
            decorateItem.nameOrId = item.nameOrId;
            return decorateItem;
        }
        if (item.action == Kooboo.SiteEditorTypes.ActionType.update) {
            decorateItem.fieldName = item.fieldName;
            return decorateItem;
        }

        if (item.action == Kooboo.SiteEditorTypes.ActionType.copy) {
            decorateItem.OrgNameOrId = item.OrgNameOrId;
            decorateItem.nameOrId = item.newNameOrId;
            decorateItem.value = "";
            return decorateItem;
        }
    }
    function getLabel(item) {
        var decorateItem = getCommonItemData(item);
        // switch(item.action){
        //     case SiteEditorTypes.ActionType.delete:
        //         var koobooObject = this._getRightKoobooObject(item.domOperationType, item.context);
        //         decorateItem.objectType = koobooObject.type;
        //         decorateItem.nameOrId = koobooObject.bindingValue;
        //         break;
        // }
        return decorateItem;
    }
    function getStyles(item) {
        var decorateItems = [],
            objectType = null,
            nameOrId = null;
        if (item.el) {
            var context = getContext(item);
            var domOperationType = item.domOperationType;
            var koobooObject = getRightKoobooObject(domOperationType, context);
            objectType = koobooObject.type;
            nameOrId = koobooObject.nameOrId;
        }
        $.each(item.changes, function(i, change) {
            var jsonRule = change.jsonRule;
            var styleTagKoobooId = jsonRule.styleTagKoobooId;
            if (change.el) {
                var context = getContext({ el: change.el });
                var domOperationType = item.domOperationType;
                var koobooObject = getRightKoobooObject(domOperationType, context);
                objectType = koobooObject.type;
                nameOrId = koobooObject.nameOrId;
            }
            var value = change.newValue;
            var property = change.property;
            if (change.shorthandProperty) {
                property = change.shorthandProperty;
                value = Kooboo.ShorthandPropertyManager.getShorthandPropertyValue(change.cssRule, change.shorthandProperty);
            }

            //inline style with ' and " is uncorrect in background.
            //so temp remove ' and "
            if (jsonRule.koobooId) {
                value = value.replace(/('|")/g, "");
            }

            var decorateItem = {
                action: item.action,
                editorType: item.editorType.toLocaleLowerCase(),
                objectType: objectType,
                nameOrId: nameOrId,
                property: property,
                value: value,
                ruleId: jsonRule.ruleId,
                styleId: jsonRule.styleId,
                pseudo: change.pseudo,
                selector: jsonRule.selector,
                important: jsonRule.important,
                styleSheetUrl: jsonRule.styleSheetUrl,
                styleTagKoobooId: styleTagKoobooId,
                koobooId: jsonRule.koobooId,
                mediaRuleList: jsonRule.mediaRuleList,
            };
            //styleTagKoobooId need get the parent(example:view/page)
            // if (change.el && styleTagKoobooId) {
            //     var context = this._getContext(item);
            //     var domOperationType = item.domOperationType;
            //     var koobooObject = this._getRightKoobooObject(domOperationType, context);
            //     decorateItem.objectType = koobooObject.type;
            //     decorateItem.nameOrId = koobooObject.nameOrId;
            // }
            decorateItems.push(decorateItem);
        });
        return decorateItems;
    }
    function getConverter(item) {
        // var context = this._getContext(item);
        // var domOperationType = item.domOperationType;
        // var koobooObject = this._getRightKoobooObject(domOperationType, context);
        // var objectType = koobooObject.type;
        // var nameOrId = koobooObject.nameOrId;
        // //如果是现有的menu,则修改name的名字
        // if (objectType == "menu") {
        //     item.newValue.Name = nameOrId;
        // }
        // return {
        //     action: item.action,
        //     editorType: item.editorType.toLocaleLowerCase(),
        //     nameOrId: nameOrId,
        //     objectType: objectType,
        //     convertResult: JSON.stringify(item.newValue)
        // }
        if (item.currentResult && item.currentResult.htmlBody)
            item.currentResult.htmlBody = removeUnnecessary(item.currentResult.htmlBody);
        return {
            action: item.action,
            editorType: item.editorType.toLocaleLowerCase(),
            convertResult: JSON.stringify(item.currentResult)
        };
    }

    function getImages(item) {
        var decorateItems = [];

        var contentChanges = _.filter(item.changes, function(change) {
            return change.isContentImage;
        });

        var styleChanges = _.filter(item.changes, function(change) {
            return !change.isContentImage;
        });
        $.each(contentChanges, function(i, change) {
            var newItem = Object.assign({}, item);
            newItem.el = change.el;
            newItem.newValue = {};
            newItem.editorType = change.editorType;
            newItem.newValue[change.attr] = change.newValue;
            decorateItems = decorateItems.concat(getImageData(newItem));
        });
        if (styleChanges.length > 0) {
            var newItem = Object.assign({}, item);
            newItem.changes = styleChanges;

            newItem.editorType = styleChanges[0].editorType;
            decorateItems = decorateItems.concat(getStyles(newItem));
        }

        return decorateItems;
    }
    function getChangeContext(el) {
        return Kooboo.elementReader.readObject(el);
    }
    function getLinks(item) {
        var decorateItems = [];
        $.each(item.changes, function(i, change) {
            var context = getChangeContext(change.el);
            var nameOrId = "";
            var objectType = "";
            var koobooObject = getRightKoobooObject(item.domOperationType, context);

            nameOrId = koobooObject.nameOrId;
            objectType = koobooObject.type;
            var decorateItem = {
                action: item.action,
                editorType: item.editorType.toLocaleLowerCase(),
                objectType: objectType,
                value: change.newValue,
                koobooId: context.koobooId,
                nameOrId: nameOrId,
                attributeName: change.name
            };

            decorateItems.push(decorateItem);

        });
        return decorateItems;
    }
    function getContext(item) {
        if (item.domOperationType == Kooboo.SiteEditorTypes.DomOperationType.content ||
            item.domOperationType == Kooboo.SiteEditorTypes.DomOperationType.contentRepeater) {
            //dom delete ,get original context
            return item.context;
        }
        var el = item.el;
        return Kooboo.elementReader.readObject(el);
        // var context;
        // if (item.context)
        //     context = item.context;
        // else {
        //     context = new ElementReader().ReadObject(item.el);
        // }
        // return context;
    }
    function getRightKoobooObject(domOperationType, context) {
        return Kooboo.KoobooObjectManager.getRightKoobooObjectByContext(domOperationType, context);
    }

    function removeUnnecessary(value) {
        if (typeof value != "string") return value;
        value = removeKoobooId(value);
        value = removeGuid(value);
        value = removeDragable(value);
        return value;
    };
    function removeKoobooId(value) {
        var reg = new RegExp("kooboo-id=\"[0-9-]+\"");
        if (!value) return "";

        while (reg.test(value)) {
            value = value.replace(reg, "");
        }

        return value;
    }
    function removeGuid(value) {
        var reg = new RegExp("guid=\"[0-9-]+\"");
        if (!value) return "";

        while (reg.test(value)) {
            value = value.replace(reg, "");
        }
        return value;
    }
    function removeDragable(value) {
        if (value.replace) {
            return value.replace("draggable=\"true\"", "");
        }
        return value;
    }
    function getStyleLogKey(editLog) {
        var keyArr = [];
        if (editLog.styleSheetUrl) {
            keyArr.push(editLog.styleSheetUrl);
            keyArr.push(editLog.selector);
            keyArr.push(editLog.property);
        } else {
            var partKeyArr = [];
            if (editLog.styleTagKoobooId) {
                partKeyArr.push(editLog.styleTagKoobooId);
                partKeyArr.push(editLog.selector);
            } else if (editLog.selector) {
                partKeyArr.push(editLog.selector);
            } else if (editLog.koobooId) {
                partKeyArr.push(editLog.koobooId);
            }
            if (partKeyArr.length > 0) {
                keyArr.push(editLog.nameOrId);
                keyArr.push(editLog.objectType);
                keyArr = keyArr.concat(partKeyArr);
                keyArr.push(editLog.property);
            }

        }
        key = keyArr.join("-");
        return key;
    }
    return {
        convert:function(item){
            var type = item.domOperationType;
            var domOperationType = Kooboo.SiteEditorTypes.DomOperationType;
            if (domOperationType.converter == type) {
                return getConverter(item);
            }

            if (domOperationType.styles == type) {
                return getStyles(item);
            }
            if (domOperationType.images == type) {
                return getImages(item);
            }
            if (domOperationType.links == type) {
                return getLinks(item);
            }

            if (domOperationType.content == type ||
                domOperationType.contentRepeater == type) {
                return getTextContent(item);
            }
            if (domOperationType.label == type) {
                return getLabel(item);
            }
            if (domOperationType.attribute == type) {
                //||domOperationType.oldStyles == type
                return getAttributeData(item);
            }
            if (domOperationType.image == type) {
                return getImageData(item);
            }
            return getCommonItemData(item);
        },
        removeKoobooId:removeKoobooId,
        getStyleLogKey:getStyleLogKey
    }
}