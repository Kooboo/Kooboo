function EditTreeData(){
    var param={
        context:null,
        treeparent:null,
        tree:null,
        oldValue:"",
        editContentClass: ".kooboo-editContent",
        btnRemoveClass: ".btnRemove",
        btnCopyClass: ".btnCopy",
    }
    function getData(context){
        var el = context.el;
        var tree = null;
        if (!Kooboo.PluginHelper.isBodyTag(el)) {
            tree = Kooboo.Repeater.findSuperRepeater(el);
            tree = $(tree).filter(function() {
                var $this = $(this);
                return ($.trim($this.text()) != "" || $this.is(":has(img)")) && $this.is(":not(:has(iframe))");
            });
            if (!tree.length || (!tree.has(el).length && !tree.is(el)))
                tree = null;
        }

        return {
            baseClass:"kb-tree-editor",
            $items:tree
        };
    }

    function remove(e){
        //tinymceSave();
        $(e.target).closest("tr").remove();
        setData();
    }
    function copy(e){
        //this._save();
        var $newTr = $(e.target).closest("tr").clone();
        $newTr.insertAfter($(e.target).closest("tr"));

        setData();
        bindRowEvent($newTr);
    }
    function edit(e){
        e.preventDefault();
        e.stopPropagation();
        var el = $(e.target).closest("tr").find("td").get(0);

        if (param.editor &&
            param.editor.ed &&
            param.editor.ed.targetElm &&
            param.editor.ed.targetElm.contains(el)) {
            return;
        }
        tinymceSave();
        var editor = new Kooboo.KoobooTinymceEditor(window,{
            el: el,
            initFn: function() {},
            saveFn: function() {
                setData(el);
            },
            pickImage: window.__gl.siteEditor.showMediagrid,
            pickPage: window.__gl.siteEditor.showPickPage
        });
        editor.create();
        param.editor = editor;
    }
    function bindRowEvent($newTr) {
        $newTr.find(param.btnRemoveClass).bind("click",remove);
        $newTr.find(param.btnCopyClass).bind("click", copy);
        $newTr.find(param.editContentClass).bind("click", edit);
    }
    function setData(el){
        if (el && el.children)
            Kooboo.KoobooElementManager.resetNewElementsKoobooId(el.children);
        var data = param.data.$items,
            $newTds = Kooboo.PluginHelper.getElementSelector(param.editContentClass),
            $lastEl, $nextEl;
        tinymceSave();

        var newHtml = [];
        $newTds.each(function(idx, td) {
            var $td = $(td);
            newHtml.push($(td).html());
        });

        //replace-->html
        param.context.editManager.editTreeDom({
            domOperationDetailType: Kooboo.SiteEditorTypes.DomOperationDetailType.editTreeData,
            logType: Kooboo.SiteEditorTypes.LogType.tempLog,
            context: param.context,
            el: param.treeparent,
            oldValue: param.oldValue,
            replaceHtml: newHtml.join(" "),
        });
    }
    function tinymceSave(){
        if (param.editor && param.editor.ed && param.editor.ed.getDoc()) {
            try {
                if (!hasElementTag(param.editor.ed.bodyElement.innerHTML)) {
                    param.editor.ed.setContent("<span>" + param.editor.ed.bodyElement.outerHTML + "</span>");
                }
                //tinymce select all text and save ,then will throw error.
                //not affect dom edit,so catch the exception
                param.editor.ed.save();
                param.editor.ed.remove();
            } catch (ex) {

            }
        }
    }

    function hasElementTag(html) {
        if ($(html).length > 0 && $(html)[0].tagName) {
            return true;
        }
        return $(html).length > 0 && $(html)[0].tagName;
    }
    return {
        dialogSetting: {
            title: Kooboo.text.inlineEditor.editData,
            width: "1000px",
        },
        menuName: Kooboo.text.inlineEditor.editData,
        isShow: function(context) {
            param.context = context;
            if (Kooboo.PluginHelper.isBindVariable()) return false;
            param.tree = Kooboo.PluginHelper.getTree(context);
            var isTree = param.tree?true:false;
            if (param.tree && ["td", "tr"].indexOf(param.tree[0].tagName.toLocaleLowerCase()) !== -1) {
                return false;
            }
            var $parent;
            if (param.tree && param.tree.parent()) {
                $parent = param.tree.parent();
            }
            if ($parent && $parent.length > 0) {
                var html = $parent.html();
                if (Kooboo.PluginHelper.isContainDynamicData(html)) return false;
            }
            return !Kooboo.PluginHelper.isBodyTag(context.el) && 
                isTree && 
                Kooboo.PluginHelper.isCanOperateType(context) && 
                !Kooboo.PluginHelper.isEmptyKoobooId(context.el);
        },
        getHtml:function(){
            k.setHtml("treeHtml","EditTreeData.html");

            var data=getData(param.context);
            param.data=data;
            return _.template(treeHtml)(data);
        },
        init:function(){
            param.treeparent = param.tree.parent()[0];
            param.context.treeitems = param.tree;
            param.oldValue = Kooboo.InlineEditor.cleanUnnecessaryHtml(param.tree.parent());

            Kooboo.PluginHelper.getElementSelector(param.btnRemoveClass).bind("click", remove);
            Kooboo.PluginHelper.getElementSelector(param.btnCopyClass).bind("click", copy);
            Kooboo.PluginHelper.getElementSelector(param.editContentClass).bind("click",edit);
        }
    }
}