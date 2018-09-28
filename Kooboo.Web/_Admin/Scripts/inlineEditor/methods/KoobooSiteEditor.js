
function KoobooSiteEditor(iframeId,iframeJsArr){
    var editManager = Kooboo.EditManager,
        //floatMenu=new Kooboo.FloatMenu(),
        displayDoc=window.document;

    setGlobalEvent();
    var mediaDialogData=ko.observable(),
        actionMenuConfig=ko.observable();
    
    function onClick(e,element){
        var context = Kooboo.elementReader.readObject(element);        
        context.el = context.element;
        context.editManager=editManager;
        context.siteEditor = window.__gl.siteEditor;

        //can reset selected element,like select element in label without koobooid
        var el=Kooboo.KoobooElementManager.getResetElement(element);
        if(el){
            Kooboo.FloatMenu.selectElement(el,context,e);
        }else{
            Kooboo.FloatMenu.show(context,e);
        }
        
    }
    function setGlobalEvent(){
        window.__gl = {
            model: "EditHtml",
            editing: false,
            baseUrl:getBaseUrl(),
            relativeUrl: getIframeRelativeUrl(),
            siteEditor: getEventForIframe(),
        };
    }
    function getBaseUrl() {
        var url = window.location.href;
        var urlParts = url.split("_Admin");
        var baseUrl="";
        if (urlParts.length > 0) {
            baseUrl = urlParts[0];
        }
        return baseUrl;
    }

    function getIframeRelativeUrl() {
        var baseUrl = getBaseUrl();
        var pageUrl = Kooboo.getQueryString("pageUrl");
        if (!pageUrl) return null;
        pageUrl = decodeURIComponent(pageUrl);
        if (pageUrl.indexOf(baseUrl) == -1) return null;

        var relativeUrl = "/" + pageUrl.replace(baseUrl, "");
        return relativeUrl;
    }
    function loading(isShow){
        isShow ? $(".page-loading").show() : $(".page-loading").hide();
    }
    //设置页面加载后，在编辑过程中，水平方向不需要scroll。
    //防止影响页面的shadow及lighter的定位
    function setIframeHorizontalNotScroll(body) {
        $(body).css("overflow-x", "hidden");
    }
    function loadIframe() {
        //var iframe = parent.window.document.getElementById(iframeId);
        var iframe = document.getElementById(iframeId);
        bindIframeEvent(iframe, iframeJsArr);

        var url = Kooboo.getQueryString("PageUrl");
        url = addKoobooInlineToUrl(url);
        $("#" + iframeId).attr("src", url);
    }
    function addJsToIframe(jsArr) {
        if (jsArr instanceof Array) {
            $.each(jsArr, function(i, js) {
                var script = document.createElement("script"),
                    iframe = parent.window.document.getElementById(iframeId);
                script.src = js;
                iframe.contentWindow.document.getElementsByTagName("head")[0].appendChild(script);
            });
        }
    }
    function addKoobooInlineToUrl(url) {
        url = decodeURIComponent(url);
        var koobooInlineParam = "koobooInline=design";
        if (url.indexOf("?") > -1)
            url += "&";
        else
            url += "?";
        url += koobooInlineParam;
        return url;
    }

    function bindIframeEvent(frame, jsArr) {
        function afterLoad(){
            addJsToIframe(jsArr);
            setIframeHorizontalNotScroll(frame.contentDocument.body);
            saveLanguage();
            var domSelector=Kooboo.DomSelector({
                onClick:onClick
            }, frame.contentDocument,displayDoc);
        
            domSelector.start();

            //in iframe(can't mask the tinymce toolbar)
            var domShadow= Kooboo.DomShadow({
                zIndex:Kooboo.InlineEditor.zIndex.lowerthanTinymceZIndex
            },frame.contentDocument,frame.contentDocument);
            window.__gl.shadow=domShadow;
            window.__gl.domSelector=domSelector;
        }

        if (frame.attachEvent) {
            frame.attachEvent("onload", function() {
                afterLoad();
            });
        } else {
            parent.window.document.getElementById(iframeId).onload = function() {
                afterLoad();
            };
        }
    }
    function saveLanguage() {
        Kooboo.User.getLanguage().then(function(res) {
            if (res.success) {
                var lang = Kooboo.LanguageManager.getLang();
                if (!lang || (lang && lang !== res.model)) {
                    Kooboo.LanguageManager.setLang(res.model);
                }
            }
        });
    }
    function showMediagrid(mediagridParams) {
        Kooboo.Media.getList().then(function(res) {
            if (res.success) {
                res.model["show"] = true;
                res.model["onAdd"] = mediagridParams.onAdd;
                mediaDialogData(res.model);
            }
        });
    }
    function showPickPage(pickPageParams) {
        Kooboo.plugins.EditLink.dialogSetting.beforeSave=function(){
            var url = Kooboo.plugins.EditLink.getLinkUrl();
            if (pickPageParams.onSave) {
                pickPageParams.onSave(url);
            }
            Kooboo.plugins.EditLink.dialogSetting.beforeSave=null;
            delete Kooboo.plugins.EditLink.dialogSetting.zIndex
        }
        Kooboo.plugins.EditLink.dialogSetting.zIndex=Kooboo.InlineEditor.zIndex.greaterthanTinymceZIndex;
        Kooboo.PluginManager.click(Kooboo.plugins.EditLink,{});
        if (pickPageParams && pickPageParams.value) {
            Kooboo.plugins.EditLink.setLinkUrl(pickPageParams.value);
        } else {
            Kooboo.plugins.EditLink.setLinkUrl("");
        }
    }
    function getEventForIframe(){
        return {
            loading: loading,
            showMediagrid: showMediagrid,
            showPickPage: showPickPage,
        };
    }
    return {
        actionMenuConfig:actionMenuConfig,
        mediaDialogData:mediaDialogData,
        start:function(){
            actionMenuConfig({
                editManager: editManager,
                mediaDialogData: mediaDialogData,
                hideFloatMenu: Kooboo.FloatMenu.hide
            });
            Kooboo.FloatMenu.start();
            loadIframe();
            ko.applyBindings(this, document.getElementById("main"));
        }
    }
}