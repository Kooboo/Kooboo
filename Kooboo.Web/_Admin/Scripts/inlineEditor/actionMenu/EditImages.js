function EditImages(){
    var param={
        lighters:[],
        context:null,
        c:[],
    }
    function getData(images){
        return {
            thumbnail : Kooboo.text.inlineEditor.thumbnail,
            dimension : Kooboo.text.inlineEditor.dimension,
            change : Kooboo.text.common.change,
            images:images
        }
    }
    function getContentImages () {
        var iframeDoc = Kooboo.InlineEditor.getIFrameDoc(),
            baseUrl = Kooboo.DomResourceManager.getBaseUrl(),
            pageUrl = getPageUrl(),
            contentImagesData = Kooboo.DomResourceManager.getContentImages(iframeDoc, baseUrl, pageUrl),
            contentImagesData = groupUrl(contentImagesData),
            images = [];
        $.each(contentImagesData, function(i, groupImages) {
            images.push({
                url: groupImages[0].url,
                width: groupImages[0].width,
                height: groupImages[0].height,
                isContentImage: true,
                defaultImage: {
                    url: groupImages[0].url,
                    width: groupImages[0].width,
                    height: groupImages[0].height,
                },
                groupImages: groupImages
            });
        });
        return images;
    }
    function getStyleImages() {
        var iframeDoc = Kooboo.InlineEditor.getIFrameDoc(),
            baseUrl = Kooboo.DomResourceManager.getBaseUrl(),
            pageUrl = getPageUrl(),
            styleImagesData = Kooboo.DomResourceManager.getStyleImages(iframeDoc, baseUrl, pageUrl),
            styleImagesData = groupUrl(styleImagesData),
            images = [];
        $.each(styleImagesData, function(i, groupImages) {
            images.push({
                url: groupImages[0].url,
                width: groupImages[0].width,
                height: groupImages[0].height,
                defaultImage: {
                    url: groupImages[0].url,
                    width: groupImages[0].width,
                    height: groupImages[0].height,
                },
                groupImages: groupImages
            });
        });
        return images;
    }
    function groupUrl(images) {
        return _.groupBy(images, function(image) {
            return image.url;
        });
    }
    function getPageUrl() {
        var iframeWindow = Kooboo.InlineEditor.getIframeWindow();
        var pageUrl = iframeWindow.location.href;
        return pageUrl;
    }
    function getTabs(){
        var tabs = [];
        var contentImages = getContentImages();
        k.setHtml("imageDetailHtml","EditImageDetail.html");
        var data=getData(contentImages);
        var content =_.template(imageDetailHtml)(data);
        tabs.push({
            title: Kooboo.text.inlineEditor.contentImages,
            content: content,
            active: true,
            images: contentImages
        });

        var styleImages = getStyleImages();
        data=getData(styleImages);
        content =_.template(imageDetailHtml)(data);
        tabs.push({
            title: Kooboo.text.inlineEditor.styleImages,
            content: content,
            images: styleImages
        });
        param.tabs=tabs;
        return tabs;
    }
    function clickFn(imageNode, imageItem) {
        var mediagridParams = {
            onAdd: function(selected) {
                imageItem.oldUrl = imageItem.url;
                imageItem.url = selected.url;
                imageItem.width = selected.width;
                imageItem.height = selected.height;

                $(imageNode).find("img").attr("src", imageItem.url);
                $(imageNode).find("img").attr("data-src", imageItem.url);
                $(imageNode).find(".size").html(imageItem.width + " X " + imageItem.height);
                replaceImage(imageItem);
            },
            files: [imageItem.url]
        };

        window.__gl.siteEditor.showMediagrid(mediagridParams);
    }
    function replaceImage(imageItem) {
        var oldUrl = imageItem.oldUrl,
            url = imageItem.url,
            fdoc = Kooboo.InlineEditor.getIFrameDoc();

        Kooboo.PluginHelper.removeGroupLighters(param.lighters, url);
        $.each(imageItem.groupImages, function(i, image) {

            var els = image.els;
            if (!els) return false;
            $.each(els, function(i, el) {
                if (imageItem.isContentImage) {
                    var koobooId = $(el).attr("kooboo-id");
                    if (el) {
                        param.context.editManager.editImages({
                            isContentImage: true,
                            attr: "src",
                            koobooId: koobooId,
                            el: el,
                            oldValue: imageItem.defaultImage.url,
                            newValue: url
                        });
                    }

                } else {
                    var jsonRule = {
                        //cssProperty: image.cssProperty, //todo confirm remove
                        koobooId: image.koobooId,
                        selector: image.selector,
                        styleSheetUrl: image.styleSheetUrl,
                        styleTagKoobooId: image.styleTagKoobooId
                    };
                    var oldValue = "url(\"" + imageItem.defaultImage.url + "\")";
                    var newValue = "url(\"" + url + "\")";

                    var shorthandProperty = Kooboo.ShorthandPropertyManager.setPropertyKeyDic(jsonRule, image.cssRule, image.cssProperty, el);

                    param.context.editManager.updateStyle({
                        domOperationType: Kooboo.SiteEditorTypes.DomOperationType.images,
                        el: el,
                        jsonRule: jsonRule,
                        cssRule: image.cssRule,
                        property: image.cssProperty,
                        important: image.important,
                        oldValue: oldValue,
                        newValue: newValue,
                        shorthandProperty: shorthandProperty
                    });
                }
            });
        });
    }
    function mouseoverFn(imageItem) {
        var $els = getElementsByImageData(imageItem.groupImages);
        if ($els && $els.length > 0)
            Kooboo.PluginHelper.scrollToElement($els[0],function(){
                $.each($els, function(idx, el) {
                    Kooboo.PluginHelper.lighterElement(param.lighters, this, idx, imageItem.url);
                });
            });
        
        
    }
    function mouseoutFn(imageItem) {
        Kooboo.PluginHelper.unLighterByGroup(param.lighters, imageItem.url);
    }
    function getElementsByImageData(images) {
        var fdoc = Kooboo.InlineEditor.getIFrameDoc(),
            elements = [];
        $.each(images, function(i, image) {
            var matchElements = image.els;
            if (matchElements)
                $.each(matchElements, function(i, el) {
                    elements.push(el);
                })

        });
        return elements;
    }
    return{
        dialogSetting:{
            title:Kooboo.text.inlineEditor.editImages,
            width: "500px",
            //tabs:getTabs()
        },
        getHtml:function(context){
            param.context=context;
            var data={
                tabs:getTabs()
            }
            k.setHtml("editImageHtml","EditImages.html");
            return _.template(editImageHtml)(data);
        },
        init:function(){
            var imageContainers = $(".image-container");
            $.each(imageContainers, function(i, container) {
                var tab = param.tabs[i];
                var imageNodes = $(container).find("tr"),
                    images = tab.images;
                $.each(imageNodes, function(j, imageNode) {
                    var imageItem = images[j];
                    $(imageNode).bind("mouseout", function() {
                        mouseoutFn(imageItem);
                    });
                    $(imageNode).bind("mouseover", function() {
                        mouseoverFn(imageItem)
                    });
                    $(imageNode).find(".table-action").find("a").bind("click", function() {
                        clickFn(imageNode, imageItem);
                    });
                });
            });
        },
    }
}