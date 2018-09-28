
function DomHoverSelector(option, sourceDoc, displayDoc) {
    var opt={
        borderColor: '#666',
        borderStyle: 'solid',
        zIndex: Kooboo.InlineEditor.zIndex.lowerZIndex,
        borderWidth:1,
    };
    $.extend(opt,option);
    opt.zIndex=1;
    
    var color="red"
    var hoverLines={
        topLine : lineEl(getInitCss("top",color)),
        rightLine : lineEl(getInitCss("right",color)),
        bottomLine: lineEl(getInitCss("bottom",color)),
        leftLine : lineEl(getInitCss("left",color))
    }
    function getInitCss(type,borderColor){
        return "position:absolute;width:1px;z-index: "+opt.zIndex+";display:none;border-"+type+"-style:"+ opt.borderStyle+
        ";border-"+type+"-color:"+borderColor+";border-"+type+"-width:"+opt.borderWidth+"px;";
    }
    function getHeight(el,height){
        if (height == 0) {
            var childrens = $(el).children();
            if (childrens && childrens.length > 0) {
                var children = childrens[0];

                var childrenPos = children.getBoundingClientRect();

                height = childrenPos.height;
                if (height == 0)
                    height = getHeight(children, height);
            }
        }
        return height;
    }
    function markBorder(el,lines) {
        if (el) {
            
            var rect = el.getBoundingClientRect(),
                offset=Kooboo.PluginHelper.getOffset(opt.offsetObj),
                height=getHeight(el,rect.height);
            
            $(lines.topLine).css({
                "width": rect.width - 1,
                "left": rect.left + offset.left+1,
                "top": rect.top + offset.top,
                "display": "block",
                "pointer-events": "none"
            });
            $(lines.rightLine).css({
                "width": rect.width,
                "height": height ? height : 1,
                "right": 0,
                "left": rect.left + offset.left,
                "top": rect.top + offset.top,
                "display": "block",
                "pointer-events": "none"
            });
            $(lines.bottomLine).css({
                "width": rect.width - 1,
                "left": rect.left + offset.left+1,
                "top": rect.top + offset.top+height-1,
                "display": "block",
                "pointer-events": "none"
            });
            $(lines.leftLine).css({
                "height": height ? height : 1,
                "left": rect.left + offset.left,
                "top": rect.top + offset.top,
                "display": "block",
                "pointer-events": "none"
            });
        }
    }
    function lineEl(css) { 
        var el = displayDoc.createElement("div");
        el.style = css;
        displayDoc.body.appendChild(el);
        return el;
    }

    return {
        mask: function(context) {
            markBorder(context.el,hoverLines);
        },
        unmask: function() {
            $(hoverLines.topLine).hide();
            $(hoverLines.leftLine).hide();
            $(hoverLines.rightLine).hide();
            $(hoverLines.bottomLine).hide();
        },
        destroy:function(){
            $(hoverLines.topLine).remove();
            $(hoverLines.leftLine).remove();
            $(hoverLines.rightLine).remove();
            $(hoverLines.bottomLine).remove();
        }
    }
}

