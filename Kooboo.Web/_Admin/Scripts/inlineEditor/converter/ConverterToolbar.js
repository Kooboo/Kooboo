function ConverterToolbar(opt){
    var param={
        context:opt.context,
        toolbarContainer : ".kb-inline-toolbar",
        toolbarSettingContainer : ".converter-setting",
        save : opt.save,
        close : opt.close,
        doc : opt.doc,
        convertResult:{}
    }
    
    function getData(){
        var toolbarItems = [{
            title: Kooboo.text.common.save,
            baseClass: "fa-save",
            click: param.save,
        }, {
            title: Kooboo.text.common.close,
            baseClass: "fa-close",
            click: param.close
        }];
        var data={
            closetext : Kooboo.text.common.close,
            convertNoSettings : Kooboo.text.inlineEditor.convertNoSettings,
            editDataText : Kooboo.text.inlineEditor.editData,
            editHtmlText : Kooboo.text.inlineEditor.editHtml,
            editContentListText : Kooboo.text.inlineEditor.editContentList,
            toolbarItems:toolbarItems,
            zIndex:Kooboo.InlineEditor.zIndex.middleZIndex
        }
        
        return data;
    }

    function initSettingEvent() {
        $(param.toolbarSettingContainer).find(".editContentList").bind("click", function() {
            editContentList();
        });
        $(param.toolbarSettingContainer).find(".editData").bind("click", function() {
            editData();
        });
        $(param.toolbarSettingContainer).find(".editHtml").bind("click", function() {
            editHtml();
        });
        $(param.toolbarSettingContainer).find(".cancel").bind("click", function() {
            //self.hideSetting();
            $(param.toolbarSettingContainer).hide();
        });
    }

    function editContentList() {
        var wind = window;
        if (param.convertResult.contentLink) {
            var contentList=Kooboo.converter.ContentListConverter(param.convertResult,param.context);
            Kooboo.PluginManager.click(contentList);

        } else {
            alert("Inner link, unable to bind varible(s).");
        }
    }
    function editData() {
        var data = param.convertResult.data;
        if (!param.convertResult.hasResult || !data) {
            alert("no convert result,can't edit");
            return;
        }
        var convertData = Kooboo.converter.DataConverter(function() {
            var data = convertData.getEditData();
            param.convertResult.data = data;
        });
        Kooboo.PluginManager.click(convertData);
        convertData.setData(data, param.convertResult.convertToType);
    }
    function editHtml() {
        if (!param.convertResult.hasResult) {
            alert("no convert result,can't edit");
            return;
        }
        var convertHtml = Kooboo.converter.HtmlConverter(function() {
            var newHtml = convertHtml.getConverterHtml();
            param.convertResult.oldHtml = param.convertResult.htmlBody;
            param.convertResult.newHtml = newHtml;
            param.convertResult.htmlBody = newHtml;
        });
        Kooboo.PluginManager.click(convertHtml);
        var html = param.convertResult.htmlBody;
        convertHtml.setConverterHtml(html);
    }
    function getSettingPosition(el) {
        var elementPosition = el.getBoundingClientRect(),
            settingHeight = $(param.toolbarSettingContainer).height(),
            settingWidth = $(param.toolbarSettingContainer).width(),
            screenOffset = Kooboo.PluginHelper.getOffset(),
            fix = 25,
            top, left;
        left = elementPosition.right;
       
        if (elementPosition.top + settingHeight - screenOffset.top + fix > $(window).height()) {
            top = $(window).height() - settingHeight - fix;
           
        } else {
            top = elementPosition.top - screenOffset.top;
        }
        if (left + settingWidth > $(window).width()) {
            var labelwidth=0;
            //todo:temp resolve,for reviewing
            if($(".kb-label").length>0){
                //var labelPosition=$(".kb-label")[0].getBoundingClientRect();
                var labelwidth=$(".kb-label").width();
                //left = $(window).width() - settingWidth-($(window).width()-labelPosition.x);
                left=$(window).width() - settingWidth-labelwidth-60;
            }
            
            var lableHeight = 25;
            top = top + lableHeight;
        }
        return {
            top: top,
            left: left
        }
    }
    function setSettingPosition() {
        var doc = Kooboo.InlineEditor.getIFrameDoc();
        var el = $("[kooboo-id='" + param.koobooId + "']", doc)[0];
        if (el) {
            var position =getSettingPosition(el);
            $(param.toolbarSettingContainer).offset(position);
        }
    }
    function isNeedShowContentListBtn(convertResult) {
        return convertResult.convertToType.toLowerCase() == "contentlist" &&
            convertResult.contentLink ? true : false;
    }
    return {
        getHtml:function(){
            k.setHtml("toolbarHtml","ConverterToolbar.html");
            var data=getData();
            param.data=data;
            return _.template(toolbarHtml)(data);
        },
        init:function(){
            var itemNodes = $(param.toolbarContainer).find("li");
            $.each(itemNodes, function(i, node) {
                var item = param.data.toolbarItems[i];
                $(node).bind("click", item.click);
            });
            initSettingEvent();
        },
        hide: function(koobooId) {
            if(koobooId==this.koobooId){
                $(param.toolbarContainer).hide();
                $(param.toolbarSettingContainer).hide();
            }
        },
        show: function() {
            $(param.toolbarContainer).show();
        },
        clear:function(){
            var contentList=Kooboo.converter.ContentListConverter(param.convertResult,param.context);
            contentList.clear();
        },
        showSetting: function(convertResult) {
            var koobooId = convertResult.koobooId;
            var el = $("[kooboo-id=" + koobooId + "]", param.doc).get(0);
            param.koobooId = koobooId;

            param.convertResult = convertResult;
            $(param.toolbarSettingContainer).show();
            if (convertResult.convertToType == "HtmlBlock" ||
                convertResult.convertToType == "View"
            ) {
                $(param.toolbarSettingContainer).find(".editData").hide();
            } else {
                $(param.toolbarSettingContainer).find(".editData").show();
            }

            if (isNeedShowContentListBtn(convertResult)) {
                $(param.toolbarSettingContainer).find(".editContentList").show();
            } else {
                $(param.toolbarSettingContainer).find(".editContentList").hide();
            }
            setSettingPosition();

            
        },
        isVisible:function(){
            return  $(".kb-inline-toolbar").is(":visible");
        },
        
        setPosition: function(el) {
            this.koobooId=$(el).attr("kooboo-id");
            var pos = el.getBoundingClientRect();
            var fix = 15,
                top = pos.top - $(".kb-inline-toolbar").height() - fix;
            if (top < 0) {
                top = pos.bottom;
            }
            var left = pos.right - $(".kb-inline-toolbar").width() - fix;

            var screen = $(".block-fullscreen");
            var offset ={
                top: screen.scrollTop(),
                left: screen.scrollLeft()
            };

            left = left - offset.left;
            $(".kb-inline-toolbar").show().offset({
                left: left > 0 ? left : 5,
                top: top - offset.top
            });

            setSettingPosition();
        },
    }
}