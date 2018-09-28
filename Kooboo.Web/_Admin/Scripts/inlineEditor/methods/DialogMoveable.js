function DialogMoveable(handler,containerNode){
    var param={
        diffX:0,
        diffY:0,
        marginTop:0,
        moveing:false
    }
    function onMouseMove(e) {
        if (param.moveing) {
            $(containerNode).css({
                top: e.pageY + param.diffY - param.marginTop,
                left: e.pageX + param.diffX
            });
            return false;
        }
    }
    function onMouseUp(e) {
        if (e.button == 0) {
            $(handler.ownerDocument).off("mousemove").off("mouseup");
            param.moveing = false;
        }
    }
    return {
        init:function(){
            $(handler).on("mousedown",function(e){
                var isButton=$(e.toElement).closest("button").length>0;
                if (!isButton) {
                    var offset = $(containerNode).offset();
                    param.diffX = offset.left - e.pageX;
                    param.diffY = offset.top - e.pageY;

                    var position = $(containerNode).css('position');
                    if (['absolute', 'fixed'].indexOf(position) == -1) {
                        $(containerNode).css('position', 'absolute');
                    }

                    param.marginTop = parseInt($(containerNode).css('marginTop'));
                    $(containerNode).css({
                        top: offset.top - param.marginTop,
                        left: offset.left
                    });

                    $(handler.ownerDocument)
                        .on("mousemove", function(e){onMouseMove(e) })
                        .on("mouseup", function(e){onMouseUp(e) });

                    param.moveing = true;
                }
            })
        }
    }
}