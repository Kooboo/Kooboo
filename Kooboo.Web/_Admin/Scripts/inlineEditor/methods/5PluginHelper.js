function PluginHelper(){
    return {
        imgRegExp: /^img$/i,
        getSpectrumConfig: function() {
            var spectrumConfig = {
                preferredFormat: "hex",
                showInput: true,
                localStorageKey: "Color_Editor_ColorItem",
                showButtons: true,
                showAlpha: true,
                showPalette: true,
                allowEmpty: true,
                cancelText: "cancel",
                chooseText: "OK",
                palette: [
                    ["#000", "#444", "#666", "#999", "#ccc", "#eee", "#f3f3f3", "#fff"],
                    ["#f00", "#f90", "#ff0", "#0f0", "#0ff", "#00f", "#90f", "#f0f"],
                    ["#f4cccc", "#fce5cd", "#fff2cc", "#d9ead3", "#d0e0e3", "#cfe2f3", "#d9d2e9", "#ead1dc"],
                    ["#ea9999", "#f9cb9c", "#ffe599", "#b6d7a8", "#a2c4c9", "#9fc5e8", "#b4a7d6", "#d5a6bd"],
                    ["#e06666", "#f6b26b", "#ffd966", "#93c47d", "#76a5af", "#6fa8dc", "#8e7cc3", "#c27ba0"],
                    ["#c00", "#e69138", "#f1c232", "#6aa84f", "#45818e", "#3d85c6", "#674ea7", "#a64d79"],
                    ["#900", "#b45f06", "#bf9000", "#38761d", "#134f5c", "#0b5394", "#351c75", "#741b47"],
                    ["#600", "#783f04", "#7f6000", "#274e13", "#0c343d", "#073763", "#20124d", "#4c1130"]
                ]
            };
            return spectrumConfig;
        },
        isContainDynamicData: function(html) {
            var objecttype = Kooboo.SiteEditorTypes.ObjectType;
            var objectNames = [];
            for (var key in objecttype) {
                objectNames.push(objecttype[key]);
            }
            objectNames.push("form");
            //var objectNames = ["content", "attribute", "htmlblock", "label", "view", "form"];
            var isContain = false;
            $.each(objectNames, function(i, objectName) {
                if (html && html.toLocaleLowerCase().indexOf("objecttype='" + objectName + "'") > -1) {
                    isContain = true;
                    return;
                }
            });
            return isContain;
        },
        isCanOperateType: function(context) {
            var koobooObjects = context.koobooObjects;
            var koobooObjectType = "page";
            var matchObject = null;
            $.each(koobooObjects, function(i, koobooObject) {
                if (koobooObject && koobooObject.type &&
                    (Kooboo.SiteEditorTypes.ObjectType[koobooObject.type] ||
                        koobooObject.type.toLowerCase() == "form")) {
                    matchObject = koobooObject;
                    return false;
                }
            });
            if (matchObject == null || !matchObject.type) return true;
            koobooObjectType = matchObject.type;
            var wrapperObjectType = [
                Kooboo.SiteEditorTypes.ObjectType.page,
                Kooboo.SiteEditorTypes.ObjectType.layout,
                Kooboo.SiteEditorTypes.ObjectType.view,
                "form"
            ];
            return wrapperObjectType.indexOf(koobooObjectType.toLocaleLowerCase()) > -1;
            //return koobooObjectType.toLocaleLowerCase() != "page" && koobooObjectType.toLocaleLowerCase() != "form";
        },
        isBindVariable: function() {
            return Kooboo.VariableDataManager.getModel()=="BindVariable";
        },
        isBodyTag: function(el) {
            return el && el.tagName && el.tagName.toLocaleLowerCase() == "body";
        },
        isEmptyKoobooId: function(el) {
            return _.isEmpty($(el).attr("kooboo-id"));
        },
        getContext: function(el,oldContext) {
            var context = Kooboo.elementReader.readObject(el);
            context.el = context.element;
            if(oldContext){
                context.editManager=oldContext.editManager;
                context.siteEditor =oldContext.siteEditor;
            }
                
            return context;
        },
        getTree: function(context) {
            var el = context.el;
            var tree = null;
            if (!this.isBodyTag(el)) {
                tree = Kooboo.Repeater.findSuperRepeater(el);
                tree = $(tree).filter(function() {
                    var $this = $(this);
                    return ($.trim($this.text()) != "" || $this.is(":has(img)")) && $this.is(":not(:has(iframe))");
                });
                if (!tree.length || (!tree.has(el).length && !tree.is(el)))
                    tree = null;
            }
            return tree;
        },
        isDynamicData: function(context) {
            if (Kooboo.KoobooObjectManager.isContentAttribute(context.el)) {
                var isImageSrcFromContentAttribute = false;
                $.each(context.koobooObjects, function(i, koobooObject) {

                    if (koobooObject && koobooObject.type && 
                        koobooObject.type == Kooboo.SiteEditorTypes.ObjectType.attribute) {
                        isImageSrcFromContentAttribute = koobooObject.attributeName == "src";
                    } else {
                        return false;
                    }
                });
                return isImageSrcFromContentAttribute;
            }
            var koobooObjects = context.koobooObjects;
            if (Kooboo.KoobooObjectManager.isHtmlBlock(koobooObjects)) return true;
            if (Kooboo.KoobooObjectManager.isMenu(koobooObjects)) return true;
            return false;
        },
        getElementSelector: function(selector) {
            //return $(this.nodeList).find(selector);
            return $(selector);
        },
        setNodeTextValue: function(node, value) {
            //颜色块后对应的颜色值
            var node = $(node).find('.colorValue')[0];
            node && (node.innerText = value);
        },
        getOffset:function(offsetObj) {
            if(offsetObj && offsetObj.getBoundingClientRect){
                return offsetObj.getBoundingClientRect();
            }
            var screen = $(window);
            return {
                top: screen.scrollTop(),
                left: screen.scrollLeft()
            }
        },
        _getWindowScroll: function() {
            var iframeWindow = Kooboo.InlineEditor.getIframeWindow();
            return {
                top: $(iframeWindow).scrollTop(),
                left: $(iframeWindow).scrollLeft(),
            }
        },
        scrollToElement: function(el,callback) {
            var iframeDoc = Kooboo.InlineEditor.getIFrameDoc();

            $('html,body', iframeDoc).scrollTop($(el).offset().top);
            callback();
            // $('html,body', iframeDoc).animate({
            //     scrollTop: $(el).offset().top
            // }, 100,function(){
            //     callback();
            // });
        },
        getContainer: function() {
            var self = this;
            var doc = Kooboo.InlineEditor.getIFrameDoc(),
                fbody = doc.body;
            if (!$("#__dialog_container", fbody).length) {
                $('<div id="__dialog_container" style="width:0px;height:0px;border:none;"></div>').appendTo(fbody);
            }
            var container = $("#__dialog_container", fbody).get(0);
            return container;
        },
        lighterElement: function(lighters, el, id, groupId) {
            var self = this;
            var lighter = self.getLighterByGroupAndId(lighters, groupId, id);
            if (!lighter) {
                lighter = self.createLighter();
                //lighter.appendTo(self.getContainer());
                self.saveLighterByGroupAndId(lighters, groupId, id, lighter);
            }
            var needLighterElement = self.getNeedLighterElement(el);
            var offset = self._getWindowScroll();
            lighter.mask({ el: needLighterElement }, offset);
        },
        unLighterByGroup: function(lighters, groupId) {
            var groupLighters = lighters[groupId];
            if (groupLighters) {
                _.forEach(groupLighters, function(ls) {
                    ls.unmask();
                });
            }
        },
        unLighterAll: function(lighters) {
            _.forEach(lighters, function(group, groupId) {
                self.unLighterByGroup(groupId);
            });
        },
        removeGroupLighters: function(lighters, groupId) {
            var group = lighters[groupId];
            _.forEach(group, function(lt, id) {
                lt.destroy();
                delete group[id];
            });
            delete lighters[groupId];
        },
        removeAllLighters: function(lighters) {
            var self = this;
            _.forEach(lighters, function(group, groupId) {
                self.removeGroupLighters(lighters, groupId);
            });
        },
        getNeedLighterElement: function(el) {
            if (el.nodeType == 1) {
                return el;
            } else if (el.nodeType == 3) {
                return el.parentNode;
            }
            return null;
        },
        saveLighterByGroupAndId: function(lighters, groupId, id, lighter) {
            if (!lighters[groupId]) {
                lighters[groupId] = {}
            }
            lighters[groupId][id] = lighter;
        },
        getLighterByGroupAndId: function(lighters, groupId, id) {
            if (!lighters[groupId] || !lighters[groupId][id])
                return null;
            return lighters[groupId][id];

        },
        createLighter: function() {
            var selector = new Kooboo.DomHoverSelector({
                borderColor: 'red',
                zIndex: Kooboo.InlineEditor.zIndex.lowerZIndex
            },window.document,window.document);
            return selector;
        },
        isCopyRepeater:function(domOperationType,context){
            var koobooObject=Kooboo.KoobooObjectManager.getRightKoobooObjectByContext(domOperationType,context);
            var copyFromId=Kooboo.dom.DomOperationHelper.getRptCopyFromId(koobooObject.nameOrId);
            if(copyFromId!=koobooObject.nameOrId){
                return true;
            }
            return false;
        }
    }
}