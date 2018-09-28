
function DomShadow(option,sourceDoc,displayDoc){
    var opt={
        zIndex:9999,backgroundColor:"rgba(0,0,0,0.2)"
    };
    $.extend(opt,option);
    var css="position: absolute;z-index: "+opt.zIndex+";display: block;cursor:not-allowed;"+
    "background-color: "+opt.backgroundColor+";"

    var topEl=shadowEl(css);
    var rightEl=shadowEl(css);
    var bottomEl=shadowEl(css);
    var leftEl=shadowEl(css);

    function shadowEl(css) { 
        var el = displayDoc.createElement("div");
        el.style = css;
        el.className="domShadowLine";
        displayDoc.body.appendChild(el);
        return el;
    }
    function setPosition(rect, maxWidth, maxHeight, offset) {
        $(leftEl).css({
            left: offset.left,
            top: offset.top,
            width: rect.left,
            height: rect.top + rect.height
        });
        $(rightEl).css({
            left: rect.width + rect.left + offset.left,
            top: rect.top + offset.top ,
            width: maxWidth - rect.left - rect.width,
            height: maxHeight - rect.top
        });
        $(topEl).css({
            left: rect.left + offset.left,
            top: offset.top,
            width: maxWidth - rect.left,
            height: Math.min(rect.top, maxHeight)
        });
        $(bottomEl).css({
            left: offset.left,
            top: rect.top + rect.height + offset.top,
            width: rect.left + rect.width,
            height: maxHeight - rect.top - rect.height
        });
    }
    function mask(context, container, offset){
        var rect = context.el.getBoundingClientRect(),
                    maxWidth, maxHeight;

        offset = offset || { top: 0, left: 0 };
        if (document == context.el.ownerDocument) {
            maxWidth = container ? container.scrollWidth : document.documentElement.clientWidth;
        } else {
            maxWidth = context.el.ownerDocument.documentElement.clientWidth;
        }
        maxHeight = container ? container.scrollHeight : document.documentElement.clientHeight;
        setPosition(rect, maxWidth, maxHeight, offset);
        
        leftEl.style.display="block";
        rightEl.style.display="block";
        topEl.style.display="block";
        bottomEl.style.display="block";
    }
    function unmask(){
        var tt= opt.test;
        leftEl.style.display="none";
        rightEl.style.display="none";
        topEl.style.display="none";
        bottomEl.style.display="none";
    }
    //other operation may delete lines,like tinymce Edit
    function reCreateShadowEl(){
        if($(".domShadowLine",displayDoc).length==0){
            topEl=shadowEl(css);
            rightEl=shadowEl(css);
            bottomEl=shadowEl(css);
            leftEl=shadowEl(css);
        }
       
    }
    return{
        mask:function(context, container, offset){
            reCreateShadowEl();
            mask(context, container, offset);
        },
        unmask:function(){
            reCreateShadowEl();
            unmask();
        }
    }
}