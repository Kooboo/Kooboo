function TestHelper(){
    return {
        createIframe:function(show){
            var iframe=document.getElementById("iframe");
            if(iframe){
                $(iframe).remove();
            }
            var iframe=document.createElement("iframe");
            iframe.id="iframe";
            iframe.src="about:blank";
            if(!show)
                iframe.style.display="none";
            document.body.appendChild(iframe);
            return iframe;
        },
        addIframeDoc:function(iframe,doc){
            var destDocument = iframe.contentDocument;
            var srcNode = doc.documentElement;
            var newNode = destDocument.importNode(srcNode, true);
            destDocument.replaceChild(newNode, destDocument.documentElement);
        },
        removeIframe:function(iframe){
            $(iframe).remove();
        }
    }
}