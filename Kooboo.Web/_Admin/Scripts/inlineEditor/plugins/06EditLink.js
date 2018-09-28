function EditLink(){
    var param={
        context:null,
        pages:null,
        currentEl:null,
        showList:false,
        externalTextInputClass: ".externalTextInput",
        linkTreeDivClass: ".linkTreeDiv",
        radioContainerClass: ".radioContainer",
    }

    function getData(el){
        var linkUrl = "";
        if (el) {
            var currentEl = el.tagName.toLocaleLowerCase() === "a" ? el : $(el).closest("a").get(0);
            linkUrl = currentEl.getAttribute("href");
            param.currentEl=currentEl;
            param.linkUrl=linkUrl;
        }
        return {
            baseClass: "kb-link-editor",
            linkUrl:linkUrl,
            linkTo: Kooboo.text.inlineEditor.linkTo,
            page: Kooboo.text.common.Page,
            external: Kooboo.text.common.external,
            pagesText: Kooboo.text.common.Pages,
        }
    }
    function isLink(context) {
        var el = context.el;
        if (el && el.tagName) {
            return el.tagName.toLocaleLowerCase() === "a" || $(el).closest("a").length > 0;
        }
        return false;
    }
    function getNewContext(context) {
        var el = context.el;
        if (el.tagName.toLocaleLowerCase() != "a" &&
            $(el).closest("a").length > 0) {
            el = $(el).closest("a").get(0);
            context = Kooboo.PluginHelper.getContext(el,context);
        }
        return context;
    }

    function isDynamicData(context) {
        var el = context.el.tagName.toLocaleLowerCase() === "a" ? 
                context.el : 
                $(context.el).closest("a").get(0);
        if (Kooboo.KoobooObjectManager.isContentAttribute(el)) {
            var koobooObject = Kooboo.KoobooObjectManager.getObjectByContext(context.koobooObjects, Kooboo.SiteEditorTypes.ObjectType.attribute);
            //k-href attr can edit
            if (koobooObject.attributeName != "k-href") return true;
        }
        if (Kooboo.KoobooObjectManager.isHtmlBlock(koobooObjects)) return true;
        var koobooObjects = context.koobooObjects;
        if (Kooboo.KoobooObjectManager.isMenu(koobooObjects)) return true;
        return false;
    }

    function getPages(){
        if (param.pages) return param.pages;
        var pages = {}
        var siteId = Kooboo.getQueryString("SiteId");
        var nodes = [];
        Kooboo.Link.SyncAll().then(function(data) {
            var pages = data.model.pages;
            if (!pages) return;
            $.each(pages, function(i, page) {
                var node = {
                    id: "",
                    name: "",
                    text: page.url,
                    icon: "fa fa-file icon-state-warning",
                    state: {
                        opened: true,
                        disabled: false,
                        selected: false
                    },
                    data: {
                        url: page.url
                    }
                }
                nodes.push(node);
            });
        });
        pages.nodes = nodes
        return pages;
    }
    function isShowPageTree(url) {
        var isShow = false;
        var pages = getPages();
        if (pages) {
            var nodes = pages["nodes"];
            if (nodes) {
                $.each(nodes, function(i, node) {
                    if (node.data && node.data.url == url) {
                        isShow = true;
                        return isShow;
                    }
                });
            }
        }
        return isShow;
    }
    function showPageTree(linkUrl) {
        var pages=getPages();
        var data = setSelectedPageByUrl(pages, linkUrl);
        bindPageTree(data);
        Kooboo.PluginHelper.getElementSelector(".linkTreeContainer").show();
        Kooboo.PluginHelper.getElementSelector(".externalTextInput").hide();
    }
    function setSelectedPageByUrl(pages, linkUrl) {
        var nodes = _.cloneDeep(pages["nodes"]);

        //set selected node
        var data = _.map(nodes, function(node) {
            if (_.isEqual(linkUrl, node.data.url)) {
                node["state"]["selected"] = true;
            }
            return node;
        });
        return data;
    }
    function bindPageTree(data) {
        destoryTree();
        Kooboo.PluginHelper.getElementSelector(param.linkTreeDivClass).jstree({
            "plugins": ["types", "checkbox"],
            'core': {
                "themes": {
                    "responsive": true
                },
                'data': data
            },
            "types": {
                "default": {
                    "icon": "fa fa-file icon-state-warning"
                }
            }
        }).on("select_node.jstree", function(e, selected) {
            if (!self.multi) {
                $.each(selected.selected, function() {
                    if (this.toString() !== selected.node.id) {
                        selected.instance.uncheck_node(this.toString());
                    }
                });
            }
            var selectedItems = $(this).jstree().get_selected(true);
            if (selectedItems && selectedItems[0] && selectedItems[0]["data"]) {
                var value = selectedItems[0]["data"]["url"];
                setTextInputUrl(value);
            }
        });
    }
    function destoryTree() {
        var linkTree = Kooboo.PluginHelper.getElementSelector(param.linkTreeDivClass);
        if (linkTree && linkTree.jstree)
            linkTree.jstree('destroy');
    }
    function getAtrr(el) {
        var attr = "href";
        if ($(el).attr("k-href")) {
            attr = "k-href";
        }
        return attr;
    }
    function setTextInputUrl(url) {
        Kooboo.PluginHelper.getElementSelector(param.externalTextInputClass).val(url || "");
        //pluginImages,tinymce link edit
        if (param.context.editManager) {
            var log = {
                domOperationType: Kooboo.SiteEditorTypes.DomOperationType.attribute,
                el: param.currentEl,
                name: getAtrr(param.currentEl),
                oldValue: param.linkUrl,
                newValue: url
            };
            param.context.editManager.editLink(log);
        }
        param.linkUrl=url;
    }
    function showTextInput(){
        Kooboo.PluginHelper.getElementSelector(".linkTreeContainer").hide();
        Kooboo.PluginHelper.getElementSelector(".externalTextInput").show();
    }
    return {
        dialogSetting: {
            title: Kooboo.text.inlineEditor.editLink,
            width: "600px",
            showFoot: true,
        },
        menuName: Kooboo.text.inlineEditor.editLink,
        isShow: function(context) {
            if (Kooboo.PluginHelper.isBindVariable()) return false;
            if (!isLink(context)) return false;

            context = getNewContext(context);

            return !Kooboo.PluginHelper.isEmptyKoobooId(context.el) && 
            !isDynamicData(context);
        },
        getHtml:function(context){
            k.setHtml("linkHtml","EditLink.html");
            var data=getData(context.el);
            param.context=context;

            var html=_.template(linkHtml)(data);
            return html;
        },
        init:function(){
            var show=isShowPageTree(param.linkUrl);
            Kooboo.PluginHelper.getElementSelector("input:radio[value=" + show + "]").attr("checked", true);

            if (show) {
                showPageTree(param.linkUrl); 
            } else {
                showTextInput();
            }
            
            Kooboo.PluginHelper.getElementSelector("input:radio").on("change", function() {
                var show = ($(this).val() === "true");
                if (show) {
                    showPageTree(param.linkUrl); 
                } else{
                    showTextInput();
                }
               
            });

            Kooboo.PluginHelper.getElementSelector(param.externalTextInputClass).on("change", function() {
                var url = $(this).val();
                setTextInputUrl(url);
            });
        },
        getLinkUrl:function(){
            return param.linkUrl;
        },
        setLinkUrl:function(linkUrl){
            var show=isShowPageTree(linkUrl);
            Kooboo.PluginHelper.getElementSelector("input:radio[value=" + show + "]").attr("checked", true);
            param.linkUrl=linkUrl;
            if (show) {
                showPageTree(linkUrl); 
            } else {
                showTextInput();
                setTextInputUrl(linkUrl);
            }
            
        }
    }
}