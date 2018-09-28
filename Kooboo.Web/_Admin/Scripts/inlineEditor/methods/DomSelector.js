/*
Dom Element selector.  
var option = { 
    style = {},
    OnClick = function(el){ 
        // the event to fire after click. 
    };
}  
DomSelector(option, document);  
*/

function DomSelector(option, sourceDoc, displayDoc) {
    var opt = {
        style: { borderStyle: "solid", borderWidth: 1,zIndex:1 },
        isEditing:false,
    };
 
    $.extend(opt, option);

    if (!sourceDoc)
    {
        sourceDoc = document; 
    }

    if (!displayDoc)
    {
        displayDoc = document; 
    }

    var hoverColor="#51a8ff",
    hoverLines={
        topLine : lineEl(getInitCss("top",hoverColor)),
        rightLine : lineEl(getInitCss("right",hoverColor)),
        bottomLine: lineEl(getInitCss("bottom",hoverColor)),
        leftLine : lineEl(getInitCss("left",hoverColor))
    },
     holderColor="red",
     holderLines={
        topLine : lineEl(getInitCss("top",holderColor)),
        rightLine : lineEl(getInitCss("right",holderColor)),
        bottomLine: lineEl(getInitCss("bottom",holderColor)),
        leftLine : lineEl(getInitCss("left",holderColor))
        
    };
  
    function getInitCss(type,borderColor){
        return "position:absolute;width:1px;z-index: "+opt.style.zIndex+";display:none;border-"+type+"-style:"+ opt.style.borderStyle+
        ";border-"+type+"-color:"+borderColor+";border-"+type+"-width:"+opt.style.borderWidth+"px;";
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

    function isJsTriggerClickEvent(e){
        return e.clientX == undefined && e.clientY == undefined;
    }
    
    function selectorDom(e){
        if(isJsTriggerClickEvent(e)||opt.isEditing) return false;
        e.preventDefault();
        e.stopPropagation();
        markBorder(e.toElement,holderLines);
        opt.onClick(e,e.toElement);
    }
    return {
        start: function()
        {
            $(sourceDoc.body).mouseover(function (e) {
                if(!opt.isEditing){
                    markBorder(e.toElement,hoverLines);
                    opt.hoverEl=e.toElement;
                }
                    
            });

            //remove all element click event
            if(opt.onClick){
                var excludedTags=["html","head"];
                $("*", sourceDoc).unbind('click');
                $("*", sourceDoc).click(function(e){
                    if(excludedTags.indexOf(e.target.tagName.toLowerCase())>-1){
                        return;
                    }
                    
                    selectorDom(e);
                    opt.holderEl=e.toElement;
                });
                //$("*",sourceDoc).removeAttr("onclick");
            }
           
            $(sourceDoc.body).click(function (e) {
                if (e && e.toElement && opt.onClick) {
                    selectorDom(e);
                    opt.holderEl=e.toElement;
                }
            })
        },
        setEdit:function(isEditing){
            opt.isEditing=isEditing
        },
        showHoverLines:function(el){
            markBorder(el,hoverLines);
        },
        showHolderLines:function(el){
            markBorder(el,holderLines);
        },
        hideHolderLines:function(el){

            holderLines.topLine.style.display="none";
            holderLines.rightLine.style.display="none";
            holderLines.bottomLine.style.display="none";
            holderLines.leftLine.style.display="none";
        },
        resetPosition:function(el){
            markBorder(opt.hoverEl,hoverLines);
            markBorder(opt.holderEl,holderLines);
        },
        destroy:function(){
            hoverLines.topLine.remove();
            hoverLines.rightLine.remove();
            hoverLines.bottomLine.remove();
            hoverLines.leftLine.remove();

            holderLines.topLine.remove();
            holderLines.rightLine.remove();
            holderLines.bottomLine.remove();
            holderLines.leftLine.remove();

        }
    }
}

