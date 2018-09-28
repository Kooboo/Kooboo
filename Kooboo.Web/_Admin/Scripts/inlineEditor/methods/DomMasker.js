function DomMasker(option,sourceDoc,displayDoc){
    var opt={
        zIndex: 9999,
        backgroundColor: "rgba(0,0,0,.2)",
        el:null,
    };
    $.extend(opt,option);
    
    var masker=createMasker();

    function createMasker(){
        var html='<div class="kb-masker" style="position:absolute !important;background-color:<%=backgroundColor%> !important;z-index:<%=zIndex%>;cursor: not-allowed"></div>'
        html=_.template(html)(opt);
        return $(html).appendTo($(displayDoc.body));
    }
    return{
        
        mask: function(context) {
            opt.el = context.el;

            var offset = opt.el.getBoundingClientRect();

            $(masker).css({
                left: offset.left,
                top: offset.top,
                width: offset.width,
                height: offset.height
            }).show();
            return this;
        },
        unmask: function() {
            $(masker).hide();
        },
    }
}