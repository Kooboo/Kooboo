function ImageHelper(context,isEditIamge){
    
    var defaultSrc = "/_admin/Styles/default.png";
    var param={
        emptyImageUrl : defaultSrc,
        imageClass : ".iPImage",
        altClass : ".iPImageAlt",
        titleClass : ".iPImageTitle",
        heightClass : ".iPImageHeight",
        widthClass : ".iPImageWidth",
        changeImageBtnClass : ".btnChangeImage",
        oldValue : getOldValue(context),
    }
    function getOldValue(context){
        var oldValue;
        if (isEditIamge) {
            var el = context.el;
            oldValue = {
                src: $(el).attr("src"),
                alt: $(el).attr("alt"),
                title: $(el).attr("title"),
                height: $(el).height(),
                width: $(el).width()
            };
        } else {
            oldValue = Kooboo.InlineEditor.cleanUnnecessaryHtml($(context.el).parent());
        }

        return oldValue;
    }
    function getData(context){
        var data,
            el=context.el;
        if (isEditIamge) {
            data={
                src: $(el).attr("src")||defaultSrc,
                alt: $(el).attr("alt"),
                title: $(el).attr("title"),
                height: $(el).height(),
                width: $(el).width()
            };
        } else {
            data={
                src: defaultSrc,
                alt: "",
                title: "",
                height: $(el).height(),
                width: $(el).width()
            };
        }
        $.extend(data,{
            baseClass:"kb-image-panel",
            labelSrc : Kooboo.text.common.Image,
            labelSize : Kooboo.text.common.size,
            buttonChange : Kooboo.text.inlineEditor.changeImage,
            labelAlt : Kooboo.text.inlineEditor.alt,
            labelTitle :Kooboo.text.common.title,
        });

        return data; 
    }
    function addLog(context){
        var newValue = getNewValue();
        if (isEditIamge) {
            var log = {
                el: context.el,
                context: context,
                oldValue: param.oldValue,
                newValue: newValue
            };
            context.editManager.editImage(log);
        } else {
            var log = {
                domOperationDetailType: Kooboo.SiteEditorTypes.DomOperationDetailType.htmlTextToImage,
                context: context,
                newValue: newValue
            };
            context.editManager.replaceWithImage(log);
        }
    }
    function getNewValue() {
        return {
            src: Kooboo.PluginHelper.getElementSelector(param.imageClass).attr("src") == defaultSrc ? "" : $(param.imageClass).attr("src"),
            alt: Kooboo.PluginHelper.getElementSelector(param.altClass).val(),
            title: Kooboo.PluginHelper.getElementSelector(param.titleClass).val(),
            height: Kooboo.PluginHelper.getElementSelector(param.heightClass).val(),
            width: Kooboo.PluginHelper.getElementSelector(param.widthClass).val(),
        }
    }
    function setSrc(context,src){
        Kooboo.PluginHelper.getElementSelector(param.imageClass).attr("src", src || defaultSrc);
        addLog(context);
    }
    return {
        getHtml:function(){
            k.setHtml("imageHtml","Image.html");
            var data=getData(context);
            var html=_.template(imageHtml)(data);
            return html;
        },
        init:function(){
            var selectors = [param.altClass, param.titleClass, param.heightClass, param.widthClass];
            for (var i = 0; i < selectors.length; i++) {
                var selector = selectors[i];
                Kooboo.PluginHelper.getElementSelector(selector).bind("change", function() {
                    $(this).val(this.value || "");
                    addLog(context);
                });
            }

            Kooboo.PluginHelper.getElementSelector(param.changeImageBtnClass).bind("click", function() {
                var mediagridParams = {
                    onAdd: function(selected) {
                        setSrc(context,selected.url);
                    },
                    files: [$(context.el).attr("src")]
                };
                context.siteEditor.showMediagrid(mediagridParams);
            });
        }
    }
}