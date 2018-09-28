function InlineEditor(){
    var zIndex={
        lowerZIndex:1,//domSelector,converter domMasker,converter domShadow,domHoverSelector
        middleZIndex:2,//dialog(plugin dialog,converter html,converter contentlist,converter data),
                        //contentlist iframe domSelector,converter label,convert toolbar,convert setting
        lowerthanTinymceZIndex:60000,
        //tinymce zindex is 65535-65537
        greaterthanTinymceZIndex:99999 //tinymce edit link dialog
    };
    return {
        zIndex:zIndex,
        getIframe:function() {
            var iframe = window.document.getElementById("iframe");
            return iframe;
        },
        getIframeWindow : function() {
            var iframe = this.getIframe();
            if (iframe)
                return iframe.contentWindow;
            return null;
        },
        getIFrameDoc : function() {
            var iframe = this.getIframe();
            if (iframe)
                return iframe.contentDocument;
            return null;
        },
        cleanUnnecessaryHtml : function(el) {
            var cloneEl = $(el).clone();
            var removeList = [
                ".domShadowLine"
            ];
            $(cloneEl).find("*").andSelf().remove(removeList.join(","));
            return Kooboo.trim($(cloneEl).html());
        }
    }
}