
function FloatMenu(){
    k.setHtml("floatMenuHtml","FloatMenu.html");

    var opt={
        context:null
    }
    var isLock=false,
        toolbars=getToolbars(),
        menuItems=Kooboo.PluginManager.getPlugins(),
        hideFloatMenuHandler=null;

    function getToolbars(){
        var toolbars = [];
        toolbars.push({
            baseClass: "tool",
            text: "<i class='fa fa-expand'></i>",
            title: Kooboo.text.inlineEditor.expandSelection,
            isShow: function(context) {
                var el = context.el;
                var show = el && el.tagName.toLocaleLowerCase() != "body" && el.parentNode != undefined;
                return show;
            },
            click: function() {
                expandClick();
            }
        });
        toolbars.push({
            baseClass: "tool",
            text: "<i class='fa fa-thumb-tack fa-rotate-90'></i>",
            title: Kooboo.text.inlineEditor.pinTheMenu,
            isShow: function() {
                return true;
            },
            click: function(node) {
                pinkClick(node);
            }
        });
        return toolbars;
    }

    function setMenuItemVisible(showMenuItems){
        var menuItemNodes = $(".js-menu-item");
        $.each(menuItemNodes,function(i,menuItemNode){
            if(showMenuItems[i]){
                $(menuItemNode).css("display", "block");
            }else{
                $(menuItemNode).css("display", "none");
            }
        });
    }
    function setToolbarVisible(context){
        var toolbarItemNodes = $(".toolbar-item");
        $.each(toolbars, function(i, toolbar) {
            var node = toolbarItemNodes[i];
            if (toolbar.isShow(context)) {
                $(node).show();
            } else {
                $(node).hide();
            }
        });
    }
    
    function expandClick() {
        var context=opt.context;
        var parentEl = context.el.parentNode;
        selectElement(parentEl,context);
    }

    function selectElement(el,context,e){
        var context=Kooboo.PluginHelper.getContext(el,context);
        show(context,e);
        var domSelector = window.__gl.domSelector;

        if (domSelector) {
            domSelector.showHolderLines(context.el);
            var parentHoverEl = context.el.tagName && context.el.tagName.toLocaleLowerCase() !== "body" ?
                context.el.parentNode : context.el;
            domSelector.showHoverLines(parentHoverEl);
        }
    }

    function pinkClick(node) {
        isLock = !isLock;
        $(node).find("i").toggleClass("fa-rotate-90", !isLock);
        $(node).attr("data-original-title", isLock ? "Unpin this menu" : "Pin this menu");
        $(node).tooltip({
            container: "body"
        }).tooltip("show");
    }

    function getMenuItems(){
        var Editors = Kooboo.plugins;
        var menuItems=[];
        $.each(Editors, function(i, Editor) {
            menuItems.push(Editor());
        });
        return menuItems;
    }

    function getPosition(e){
        var el = opt.context.el;

        var screen = Kooboo.PluginHelper.getOffset(opt.context.offsetObj);
        var rect = $(".floatMenu-container")[0].getBoundingClientRect();
        var offset = el.ownerDocument.documentElement,
            dHeight = document.documentElement.clientHeight,
            maxWidth = offset.clientWidth,
            yOffset = 5,
            range = 20,
            x = e.clientX + screen.left,
            y = e.clientY + screen.top;
        x = (x + range + rect.width) > maxWidth ? (x - rect.width - range) : (x + range);
        y = y < 0 ? 1 : ((dHeight - y <= rect.height) ? (dHeight - rect.height - yOffset) : y);
        return {
            x: x,
            y: y
        };
    }
    function setPosition(e){
        var position=getPosition(e);
        if (!isLock) {
            $(".floatMenu-container").css({
                left: position.x + "px",
                top: position.y + "px"
            }).animate({
                opacity: 1
            }, 50);
        }
    }

    function show(context,e){
        opt.context= context;
        var showItems=Kooboo.PluginManager.getShowPlugins(context);
        setMenuItemVisible(showItems);
        setToolbarVisible(context);
        if (Object.keys(showItems).length>0) {
            $(".floatMenu-container").show();
            if(e)
                setPosition(e);
            setShowTimeOut();
        } else {
            $(".floatMenu-container").hide();
            var info = window.info;
            if (!info) {
                info = window.parent.info;
            }
            info.show(Kooboo.text.inlineEditor.cantEdit, false);
        }
    }
    function hide(force) {
        if (!isLock||force) {
            $(".floatMenu-container").animate({
                opacity: 0
            }, 160, function() {
                $(".floatMenu-container").hide();
            });
        }
    }

    function setShowTimeOut(){
        if(hideFloatMenuHandler){
            clearTimeout(hideFloatMenuHandler);
        }
        hideFloatMenuHandler = setTimeout(hide, 1000);
    }

    function getSubMenuParam(container) {
        return {
            docWidth: $(".block-fullscreen").width(),
            docHeight: $(".block-fullscreen").height(),
            offsetLeft: $(container).offset().left + $(container).width(),
            offsetTop: $(container).offset().top,
            subItemWidth: $(container).find(".sub-menu").width(),
            subItemHeight: $(container).find(".sub-menu").height(),
        }
    }
    function getSubMenuPosition(para) {
        var isToLow = para.docHeight <= para.offsetTop + para.subItemHeight,
            isTooHeight = para.docWidth <= para.offsetLeft + para.subItemWidth,
            top,
            left;
        if (isToLow) {
            top = para.docHeight - para.offsetTop - para.subItemHeight - 5;
        }
        if (isTooHeight) {
            left = "-181px";
        }

        if (!isToLow && !isTooHeight) {
            top = 0;
            left = "181px";
        }
        return {
            top: top,
            left: left
        }
    }
    function setSubMenuPosition(e){
        var subMenuContainer = $(e.target).closest(".subMenuContainer")[0];
        var para = getSubMenuParam(subMenuContainer);
        if (!para || !para.docWidth) return;
        var position = getSubMenuPosition(para);
        if (position.top) {
            $(e.currentTarget).find(".sub-menu").css({
                top: position.top

            });
        }
        if (position.left) {
            $(e.currentTarget).find(".sub-menu").css({
                left: position.left

            });
        }
        
    }
    function showSubMenu(e){
        var context=opt.context;
        if ($(e.currentTarget).find(".sub-menu").is(":visible")) return true;
        if (!_.isEmpty($(context.el).attr("kooboo-id"))) {
            $(e.currentTarget).find(".sub-menu").show();
        }
        
        setSubMenuPosition(e);

    }

    function bindShowTimeoutEvent() {
        $(".floatMenu-container").mousemove(function(e){
            if(hideFloatMenuHandler){
                clearTimeout(hideFloatMenuHandler);
            }
        });
        $(".floatMenu-container").mouseleave(function(e){
            setShowTimeOut();
        });
    }
    
    function bindToolbarEvent() {
        var toolbarItemNodes = $(".toolbar-item");
        $.each(toolbarItemNodes, function(i, node) {
            var toolbar = toolbars[i];
            $(node).bind("click", function() {
                toolbar.click(node);
            });
        });
    }
    function bindMenuItemsEvent(){
        var menuItemNodes = $(".js-menu-item");
        $.each(menuItemNodes, function(i, menuItemNode) {
            var plugin = menuItems[i];
            if (plugin.isConverter) {
                $(menuItemNode).bind("mouseover", function(e) {
                    showSubMenu(e);
                });
                $(menuItemNode).bind("mouseleave ", function(e) {
                    $(e.currentTarget).find(".sub-menu").hide();
                });
                $(menuItemNode).find(".sub-menu").find("a").bind("click", function(e) {
                    var target = e.currentTarget;
                    var selectConvertName = $(target).attr("data-convert-name");
                    opt.context.selectConvertName=selectConvertName;
                    hide(true);
                    //plugin.context = opt.context;
                    Kooboo.PluginManager.click(plugin,opt.context);
                });
            } else {
                $(menuItemNode).on("click", function(e) {
                    hide(true);
                    //plugin.context = opt.context;
                    Kooboo.PluginManager.click(plugin,opt.context);
                });
            }

        });
    }
    return {
        start:function(){
            var data={
                baseClass:"kb-inline-menu",
                zIndex:"100001",
                title:Kooboo.text.common.Menu,
                toolbars:toolbars,
                menuItems:menuItems
            }
            var html=_.template(floatMenuHtml)(data);
            document.body.append($(html)[0]);
            
            bindShowTimeoutEvent();
            bindToolbarEvent();
            bindMenuItemsEvent();
        },
        show:show,
        hide:hide,
        selectElement:selectElement
    }
}