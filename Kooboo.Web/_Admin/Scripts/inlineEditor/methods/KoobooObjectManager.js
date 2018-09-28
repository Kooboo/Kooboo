function KoobooObjectManager(){
    function getContextByElement(el) {
        var context = Kooboo.elementReader.readObject(el);
        return context;
    };
    function getRightKoobooObjectByEl(domOperationType, el) {
        var context = getContextByElement(el);
        return getRightKoobooObjectByContext(domOperationType, context);
    };
    function getRightKoobooObjectByContext(domOperationType, context) {
        var koobooObject = doGetRightKoobooObject(context.koobooObjects, 0, domOperationType);
        return koobooObject;
    };
    function doGetRightKoobooObject(koobooObjects, index, domOperationType) {
        var koobooObject = {};
        if (!koobooObjects || koobooObjects.length < index + 1)
            return koobooObject;
        koobooObject = koobooObjects[index];
        var objectType = koobooObject.type;
        if (!objectType || (objectType.toLocaleLowerCase() != Kooboo.SiteEditorTypes.ObjectType.page &&
                isNeedGetParentKoobooObject(objectType, domOperationType))) {
            koobooObject = doGetRightKoobooObject(koobooObjects, index + 1, domOperationType);
        }
        return koobooObject;
    };
    function isExistObjectType(objectType) {
        if (!objectType) return false;
        objectType = objectType.toLocaleLowerCase();
        return Kooboo.SiteEditorTypes.ObjectType[objectType] ? true : false;
    };
    function isNeedGetParentKoobooObject(objectType, domOperationType) {

        //is Exist object Type
        if (!isExistObjectType(objectType)) return true;
        //no domOperationType,get the closest koobooObject
        if (!domOperationType) return false;

        if(objectType.toLocaleLowerCase() == Kooboo.SiteEditorTypes.ObjectType.label&&
           domOperationType != Kooboo.SiteEditorTypes.DomOperationType.label ){
                return true;
           }

        //not edit content;
        if (domOperationType != Kooboo.SiteEditorTypes.DomOperationType.content &&
            (objectType.toLocaleLowerCase() == Kooboo.SiteEditorTypes.ObjectType.content || 
            objectType.toLocaleLowerCase() == Kooboo.SiteEditorTypes.ObjectType.attribute)) {
            return true;
        }

        //not copy/Or remove contentRepeater
        if (domOperationType != Kooboo.SiteEditorTypes.DomOperationType.contentRepeater &&
            objectType.toLocaleLowerCase() == Kooboo.SiteEditorTypes.ObjectType.contentrepeater) {
            return true;
        }
        return false;
    };
    function isNeedGetNewNameOrId(koobooObject) {
        var bindingValue = koobooObject.bindingValue;
        if (koobooObject.type && !Kooboo.SiteEditorTypes.ObjectType[koobooObject.type.toLowerCase()]) {
            return true;
        }

        if (koobooObject.type.toLowerCase() != Kooboo.SiteEditorTypes.ObjectType.content &&
            koobooObject.type.toLowerCase() != Kooboo.SiteEditorTypes.ObjectType.contentrepeater) {
            return true;
        }

        //is not right.
        if (bindingValue) {
            //don't need get parent Repeater,so remove this logic

            var splitbyDot = bindingValue.split(".");
            //ByFolder_Item.category.type
            if (splitbyDot.length > 2) {
                return true;
            }
        }


        return false;
    };
    return {
        isNeedGetParentKoobooObject:isNeedGetParentKoobooObject,
        isNeedGetNewNameOrId:isNeedGetNewNameOrId,

        getRightKoobooObjectByContext:getRightKoobooObjectByContext,
        getObjectType : function(context) {
            var koobooObject = getRightKoobooObjectByContext(null, context);
            if (koobooObject && koobooObject.type) {
                return koobooObject.type.toLowerCase();
            }
            return "";
        },
        getObjectByContext:function (koobooObjects, objectType) {
            var matchObject = null;
            if (koobooObjects) {
                $.each(koobooObjects, function(i, koobooObject) {
                    if (koobooObject && koobooObject.type && koobooObject.type == objectType) {
                        matchObject = koobooObject;
                        return false;
                    }
                });
            }
            return matchObject;
        },
        getNameOrIdFromContext:function (koobooObjects, objectType) {
            var matchObject = this.getObjectByContext(koobooObjects, objectType);
            if (matchObject && matchObject.nameOrId) {
                return matchObject.nameOrId;
            }
            return null;
        },
        isMenu: function(koobooObjects) {
            var nameOrId = this.getNameOrIdFromContext(koobooObjects, Kooboo.SiteEditorTypes.ObjectType.menu);
            return nameOrId != undefined;
        },
        //like count
        //<!--#kooboo--objecttype='content'--bindingvalue='ListByCategoryId.Count()'--fieldname='Count()'--koobooid='1-1'-->
        isNoNameIdContent: function(context) {
            var objectType = this.getObjectType(context);
            if (objectType.toLocaleLowerCase() == Kooboo.SiteEditorTypes.ObjectType.content) {
                return !this.isContent(context.koobooObjects);
            }
            return false;
        },
        isContent:function(koobooObjects) {
            var nameOrId = this.getNameOrIdFromContext(koobooObjects, Kooboo.SiteEditorTypes.ObjectType.content);
            return nameOrId != undefined;
        },
        isContentAttribute: function(el) {
            var koobooObject = this.getFirstWrapKoobooObject(el);
            if (koobooObject.objectType &&
                koobooObject.objectType.toLowerCase() == Kooboo.SiteEditorTypes.ObjectType.attribute) {
                var nameOrId = koobooObject.nameOrId;
                return nameOrId != undefined;
            }
            return false;
        },
        isHtmlBlock:function(koobooObjects) {
            var nameOrId = this.getNameOrIdFromContext(koobooObjects, Kooboo.SiteEditorTypes.ObjectType.htmlblock);
            return nameOrId != undefined;
        },
        isContentRepeater: function(koobooObjects) {
            var nameOrId = this.getNameOrIdFromContext(koobooObjects, Kooboo.SiteEditorTypes.ObjectType.contentrepeater);
            return nameOrId != undefined;
        },
        isMobileCanEdit: function(el) {
            var object = this.getFirstWrapKoobooObject(el);
            if (object && (object.objectType == Kooboo.SiteEditorTypes.ObjectType.page ||
                    object.objectType == Kooboo.SiteEditorTypes.ObjectType.layout)) {
                return true;
            }
            return false;
        },
        getFirstWrapKoobooObject: function(el) {
            var context = getContextByElement(el);
            var nameOrId = "";
            var objectType = "";
            var koobooId=""
            $.each(context.koobooObjects, function(i, koobooObject) {
                if (koobooObject.type &&
                    Kooboo.SiteEditorTypes.ObjectType[koobooObject.type.toLowerCase()]) {
                    nameOrId = koobooObject.nameOrId;
                    koobooId=koobooObject.koobooId;
                    objectType = koobooObject.type;
                    if (objectType && objectType.toLowerCase() == Kooboo.SiteEditorTypes.ObjectType.label) {
                        nameOrId = koobooObject.bindingValue;
                    }
                    return false;
                }
            });
            return {
                nameOrId: nameOrId || "",
                objectType: objectType || "",
                koobooId:koobooId
            };
        },
        getKoobooObjects: function(el) {
            var context = getContextByElement(el);
            return context.koobooObjects;
        },

        //根据点击元素的koobooObjects,获取需要修改的contentRepeater的nameOrid
        getSelectedRepeaterNameOrId: function(koobooObjects) {
            var matchKoobooObject = koobooObjects[0];
            $.each(koobooObjects, function(i, koobooObject) {
                if (!isNeedGetNewNameOrId(koobooObject)) {
                    matchKoobooObject = koobooObject;
                    return false;
                }
            });
            var nameOrId;
            if (matchKoobooObject && matchKoobooObject.nameOrId) {
                nameOrId = matchKoobooObject.nameOrId;
            } else {
                nameOrId = this.getNameOrIdFromContext(koobooObjects, Kooboo.SiteEditorTypes.ObjectType.attribute);
            }
            return nameOrId;
        },
       
    }
}