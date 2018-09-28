function KoobooActionMenuHtml(){
    k.setHtml("KoobooActionMenuHtml","KoobooActionMenu.html");
    return KoobooActionMenuHtml;
}
function KoobooActionMenu(param){
    var self=this;
    this.buttons = ko.observableArray(getButtons());
    this.hasPrev = ko.observable(false);
    this.hasNext = ko.observable(false);
    this.undo=undo;
    this.redo=redo;
    this.toggle=toggle;
    this.afterButtonsRender=afterButtonsRender;

    var context={
        editManager: param.editManager,
    }
    init();
    function init(){
        setActionMenuMovale();
        setEditLogUndoRedoStatusFn();
        setActionMenuFnToGlobal();
    }

    function updateUndoRedoStatus() {
        self.hasPrev(param.editManager.hasPrev());
        self.hasNext(param.editManager.hasNext());
    }
    function setActionMenuFnToGlobal() {
        var gl = window.__gl;
        if (gl)
            gl.actionMenu = {
                save: save
            }
    }
    function setEditLogUndoRedoStatusFn() {
        if (param.editManager && param.editManager.setUpdateCallback)
            param.editManager.setUpdateCallback(updateUndoRedoStatus);
    }
    function setActionMenuMovale() {
        if ($("#kbActionMenu").length > 0) {
            var domNode = $("#kbActionMenu").find(".kb-actions")[0];
            Kooboo.ActionMenuDragable(domNode,true).init();
        }
    }
    function bindButtonToolTip() {
        var $undoBtn = $("#kbActionMenu").find("#undoBtn");
        var $redoBtn = $("#kbActionMenu").find("#redoBtn");
        $undoBtn.add($redoBtn).tooltip();

        var buttons = $("#kbActionMenu").find(".menu").find("a");
        var $btnToggle = $("#kbActionMenu").find(".toggle-button");
        buttons.add($btnToggle).tooltip({
            container: 'body',
            placement: 'auto left'

        });
    }
    function editImages() {
        param.hideFloatMenu();
        Kooboo.PluginManager.click(Kooboo.actionMenu.EditImages,context);
        // var editor = new Kooboo.InlineEditor.UI.ActionMenu.PopupEditImages(self.context);
        // editor.open();
    }
    function editColors() {
        param.hideFloatMenu();
        Kooboo.PluginManager.click(Kooboo.actionMenu.EditColors,context);
    }
    function editLinks() {
        param.hideFloatMenu();
        Kooboo.PluginManager.click(Kooboo.actionMenu.EditLinks,context);

    }
    //not all Page have pageId in url,link(menu-->directorypage)
    function getPageId() {
        //if (self.pageId) return self.pageId;
        var pageId = Kooboo.getQueryString("PageId");
        if (!pageId) {
            var iframeDoc = Kooboo.InlineEditor.getIFrameDoc();
            var body = $(iframeDoc).find("body");
            if (body && body.length > 0) {
                var context = Kooboo.elementReader.readObject(body[0]);
                if (context && context.pageId) {
                    pageId = context.pageId;
                }
            }

        }
        return pageId

    }
    function save(reload) {

        var editLogs = param.editManager.convert();
        if (!editLogs && editLogs.length == 0) return;

        var pageid = getPageId();
        var data = JSON.stringify({ updates: editLogs, pageid: pageid });
        Kooboo.Editor.Update(data).then(function(res) {
            $(".page-loading").hide();

            if (res.success) {
                reload && window.location.reload(true);
            } else {
                var message = res.messages.join(",");
                alert("failed:" + message);
            }
        });

    }
    function getButtons() {
        return [{
                title: Kooboo.text.inlineEditor.editImages,
                name: "EditImages",
                baseClass: "fa fa-image",
                clickEvent: editImages

            }, {
                title: Kooboo.text.inlineEditor.editColors,
                name: "EditColors",
                baseClass: "fa fa-paint-brush",
                clickEvent: editColors
            }, {
                title: Kooboo.text.inlineEditor.editLinks,
                name: "EditLinks",
                baseClass: "fa fa-link",
                clickEvent: editLinks
            },
            {
                title: Kooboo.text.common.save,
                name: "Save",
                baseClass: "fa fa-save",
                clickEvent: function() {
                    save(true);
                }
            }

        ];
    }

    
    //koobooactionMenu.html invoke
    function afterButtonsRender(elements, data) {
        var buttons = $("#kbActionMenu").find(".menu").find("a");
        //is button all rendered
        if (buttons.length == self.buttons().length) {
            bindButtonToolTip();
        }

    }
    function undo() {
        param.hideFloatMenu();
        param.editManager.undo();
        updateUndoRedoStatus();

        //window.__gl.sandbox.remaskSandBox();
    }
    function redo() {
        param.hideFloatMenu();
        param.editManager.redo();
        updateUndoRedoStatus();
        //window.__gl.sandbox.remaskSandBox();
    }
    function toggle() {
        var animationTime = 300,
            $btnToggle = $("#kbActionMenu").find(".toggle-button");
        $btnToggle.toggleClass('active');
        if ($btnToggle.hasClass('active')) {
            $btnToggle.attr('data-original-title', Kooboo.text.inlineEditor.clickCollpase).tooltip('show');
        } else {
            $btnToggle.attr('data-original-title', Kooboo.text.inlineEditor.clickExpand).tooltip('show');
        }
        $btnToggle.find('i:visible').fadeOut(animationTime, function() {
            $(this).addClass('hide');
        });
        $btnToggle.find('i:hidden').fadeIn(animationTime, function() {
            $(this).removeClass('hide');
        });

        var menu = $("#kbActionMenu").find(".menu");
        menu.slideToggle(animationTime);
    }
}