function IframeDialog() {
    function addScript(currentIframe, jsArr) {
        $.each(jsArr, function (i, js) {
            var script = document.createElement("script");
            script.src = js;

            if (currentIframe.contentWindow.document.getElementsByTagName('head')[0]) {
                currentIframe.contentWindow.document.getElementsByTagName('head')[0].appendChild(script);
            }
        });

    }
    return {
        getIframe: function (url, scripts, height, iframeLoaded, id) {
            var iframe = document.createElement("iframe");
            iframe.src = url;
            iframe.width = "100%";
            if (id) {
                iframe.id = id;
            }

            iframe.height = height ? (parseFloat(height.replace("px", "")) - 150) + "px" : "auto";
            iframe.style["border"] = "0";

            iframe.onload = function () {
                if (scripts) {
                    addScript(iframe, scripts);
                }
                if (iframeLoaded)
                    iframeLoaded(iframe);
            };
            return iframe;
        },

        init: function (iframe) {
            //set iframe height
            Kooboo.EventBus.subscribe("kb/component/modal/set/height", function (data) {
                if (data.hasTinyMceField) {
                    $(iframe).closest('.modal-dialog').css('margin', '10px auto');
                }
                var iframeHeight = data.hasTinyMceField ? (window.innerHeight - 180) : data.height;
                if (iframeHeight > 0) {
                    $(iframe).css('height', iframeHeight);
                }
            });
        }
    }
}