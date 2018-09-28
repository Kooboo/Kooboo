function KoobooElementManager(){
    function getCloneElements(elements) {
        var cloneElements = [];
        var koobooIds = [];
        $.each(elements, function(i, element) {
            var koobooId = $(element).attr("kooboo-id");
            //reset exist koobooId
            if (koobooId && koobooIds.indexOf(koobooId) > -1) {
                cloneElements.push(element);
            } else {
                koobooIds.push(koobooId);
            }
        });
        return cloneElements;
    }
    function doRestElementKoobooId(el, koobooId) {
        $(el).attr("kooboo-id", koobooId);

        var childrens = el.children;
        for (var i = 0; i < childrens.length; i++) {
            var child = childrens[i];
            var childKoobooId = koobooId + "-" + (i + 1);
            doRestElementKoobooId(child, childKoobooId);
        }
    }

    function ismatchNameAndType(nameOrId, objectType, elNameOrId, elObjectType){
        nameOrId = nameOrId || "";
        objectType = objectType || "";
        elNameOrId = elNameOrId || "";
        elObjectType = elObjectType || "";

        return nameOrId.toLowerCase() == elNameOrId.toLowerCase() &&
            objectType.toLowerCase() == elObjectType.toLowerCase();
    }
    function getNewKoobooId(koobooId) {
        if (!koobooId) return "";
        var partKoobooIds = koobooId.split("-");
        var newKoobooId = "";
        for (var i = 0; i <= partKoobooIds.length - 2; i++) {
            if (!newKoobooId) {
                newKoobooId = partKoobooIds[i];
            } else {
                newKoobooId += "-" + partKoobooIds[i];
            }
        }
        if (!newKoobooId) {
            newKoobooId = parseInt(partKoobooIds[partKoobooIds.length - 1]) + 1;
        } else {
            newKoobooId += "-" + (parseInt(partKoobooIds[partKoobooIds.length - 1]) + 1);
        }
        return newKoobooId;
    }

    function getMaxKoobooIdOfSiblingElement(el) {
        var maxkoobooId = $(el).attr("kooboo-id");

        var prevSiblingElement = el.previousElementSibling;
        while (prevSiblingElement) {
            var koobooId = $(prevSiblingElement).attr("kooboo-id");
            if (!maxkoobooId || koobooId > maxkoobooId) {
                maxkoobooId = koobooId;
            }
            prevSiblingElement = prevSiblingElement.previousElementSibling;
        }
        var nextElementSibling = el.nextElementSibling;
        while (nextElementSibling) {
            var koobooId = $(nextElementSibling).attr("kooboo-id");
            if (!maxkoobooId || koobooId > maxkoobooId) {
                maxkoobooId = koobooId;
            }
            nextElementSibling = nextElementSibling.nextElementSibling;
        }

        return maxkoobooId;
    }
    return {
        getCloneElements:getCloneElements,
        getNewKoobooId:getNewKoobooId,
        ismatchNameAndType:ismatchNameAndType,
        getMaxKoobooIdOfSiblingElement:getMaxKoobooIdOfSiblingElement,

        resetCloneElementsKoobooId: function(elements) {
            var cloneElements = getCloneElements(elements);
            this.resetElementsKoobooId(cloneElements);
        },
        resetNewElementsKoobooId: function(elements) {
            var self = this;
            $.each(elements, function(i, element) {
                var koobooId = $(element).attr("kooboo-id");

                if (!koobooId) {
                    self.resetElementKoobooId(element);
                }
                var childrens = element.children;
                if (childrens && childrens.length > 0) {
                    self.resetNewElementsKoobooId(childrens);
                }
            });
        },
        resetElementsKoobooId : function(elements) {
            var self = this;
            $.each(elements, function(i, element) {
                self.resetElementKoobooId(element);
            });
        },
        resetElementKoobooId : function(el) {
            var maxKoobooId = getMaxKoobooIdOfSiblingElement(el);
            var newKoobooId = "";
            if (!maxKoobooId) {
                var parentElement = el.parentElement;
                newKoobooId = $(parentElement).attr("kooboo-id") + "-1";
            } else {
                newKoobooId = getNewKoobooId(maxKoobooId);
            }
            if (newKoobooId)
                doRestElementKoobooId(el, newKoobooId);
        },
        //save the original kooboo object
        setFirstWrapObject : function(item) {
            if (!item.firstWrapObject) {
                item.firstWrapObject = Kooboo.KoobooObjectManager.getFirstWrapKoobooObject(item.el);
            }
        },
        getFirstWrapKoobooObject : function(item) {
            this.setFirstWrapObject(item);
            return item.firstWrapObject;
        },
        //像多次操作copy/delete,replaceWithimage,undo/redo,会使原先的el的reference变得无效。需要重新获取el的reference
        getElementByEl : function(el, koobooObject) {
            var koobooId = $(el).attr("kooboo-id");
            //动态获取el,多次操作copy/delete，会导致el的引用发生变化.
            var el = this.getElement(koobooId, koobooObject.nameOrId, koobooObject.objectType);
            return el;
        },
        getElement : function(koobooId, nameOrId, objectType) {
            var self = this;
            var fdoc = Kooboo.InlineEditor.getIFrameDoc();
            var elements = $("[kooboo-id='" + koobooId + "']", fdoc);

            if (!elements || elements.length == 0) return null;
            if (elements.length == 1)
                return elements[0];
            var el = elements[0];
            var elementMatchLevels = {};
            $.each(elements, function(i, element) {
                var obj = Kooboo.KoobooObjectManager.getFirstWrapKoobooObject(element);
                if (ismatchNameAndType(nameOrId, objectType, obj.nameOrId, obj.objectType)) {
                    el = element;
                    return false;
                }
            });
            return el;
        },
        getResetElement:function(element){
            var context = Kooboo.elementReader.readObject(element);
            var koobooObject=Kooboo.KoobooObjectManager.getFirstWrapKoobooObject(element);

            //label need select element with koobooid
            if(koobooObject&&koobooObject.objectType && koobooObject.objectType.toLowerCase()==Kooboo.SiteEditorTypes.EditorType.label){
                el= Kooboo.KoobooElementManager.getElement(koobooObject.koobooId,koobooObject.nameOrId,koobooObject.objectType);
                return el;
            }
            return null;
        }
        
    }
}