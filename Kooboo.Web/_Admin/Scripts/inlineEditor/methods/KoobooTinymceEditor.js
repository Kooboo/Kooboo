function KoobooTinymceEditor(win,opt){
    var $,
        _,
       languageManager;
    if (!win.Kooboo) {
        var parentWindow = win.parent;
        $ = parentWindow.$;
        _ = parentWindow._;
        languageManager = parentWindow.Kooboo.LanguageManager;
    } else {
        $ = win.$;
        _ = win._;
        languageManager = Kooboo.LanguageManager;
        
    }
    var param={
        el:null,
        initFn:null,
        saveFn:null,
        pickImage:null,
        pickPage:null
    };
    var lang = languageManager.getTinyMceLanguage();
    $.extend(param,opt);

    function removeOnfocus(el) {
        //prevent edit is always blur.
        $(el).removeAttr("onfocus");
    }
    function pasteFn(e) {
        e.preventDefault();
        var clip = (e.originalEvent || e).clipboardData,
            text;
        if (clip) {
            text = clip.getData("text/plain");
        } else {
            text = win.clipboardData.getData("text");
        }
        var sel = win.getSelection(),
            range = sel.getRangeAt(0),
            node = win.document.createTextNode(text);
        range.deleteContents();
        range.insertNode(node);
        node.parentNode.normalize();
        range.setStart(node, node.length);
        range.collapse(true);
        sel.removeAllRanges();
        //sel.addRange(range);
    }
    function tinyMCEInit(uuid, option) {
        var el = param.el,
            $el = $(param.el);

        tinyMCE.init($.extend({
            selector: param.el.tagName + "." + uuid,
            inline: true,
            cleanup: true,
            relative_urls: false, //image don't use relative path
            browser_spellcheck: false,
            visual_anchor_class: "no-anchor", //no anchor
            schema: "html5",
            menubar: false,
            force_p_newlines: false,
            forced_root_block: "",
            toolbar_items_size: "small",
            noTrim: true,
            plugins: [
                "exit link image lists textcolor colorpicker contextmenu"
            ],
            toolbar: "save | undo redo | bold italic forecolor fontselect fontsizeselect | image link unlink",
            verify_html: false,
            setup: function(ed) {
                ed.on("BeforeSetContent", function(e) {
                    var targetElm = e.target.targetElm;
                    if (targetElm.tagName.toLowerCase() == "li") {
                        if (targetElm.children.length == 0) {
                            e.content = targetElm.textContent;
                        } else if (e.content === 0) {
                            e.content = targetElm.innerHTML;
                        }
                    }
                    e.format = "raw";
                });
            },
            init_instance_callback: function(ed) {
                ed.focus();

                ed.on("remove", function() {
                    el.normalize();
                    $el.removeClass(uuid).off("keypress keydown keyup", keyFn).off("paste", pasteFn);
                    resetAttrs(el);
                });
                // ed.on("keyup", function() {
                //     self.keyupFn(ed);
                // });
                param.ed = ed;
                param.initFn(el);
            },
            exit_onsavecallback: function(ed) {
                ed.save();
                ed.remove();
                if(param.saveFn)
                    param.saveFn(ed);
            },
            file_picker_callback: function(callback, value, meta) {
                if (meta.filetype === "image") {
                    param.pickImage({
                        onAdd: function(selected) {
                            var selects,
                                hvalue = [];
                                hvalue.push(selected.thumbnail);
                            callback && callback(hvalue, { alt: "My alt text" });
                        }
                    });
                } else {
                    param.pickPage({
                        onSave: function(val) {
                            callback(val, { alt: "My alt text" });
                        },
                        value: value
                    });

                }
            }
        }, option));
    }
    function resetAttrs(el) {
        var attrs = _.toArray(el.attributes);
        _.forEach(attrs, function(attr) {
            if (!_.has(param.origAttrs, attr.name)) {
                el.removeAttribute(attr.name);
            } else if (param.origAttrs[attr.name] != attr.value) {
                el.setAttribute(attr.name, param.origAttrs[attr.name]);
            }
        });

        _.forEach(param.origAttrs, function(orgiAttr, name) {
            if (!_.has(attrs, name)) {
                el.setAttribute(name, orgiAttr);
            }
        });
    }
    function keyFn(e) {
        debugger;
        if (e.keyCode == 13) {
            var that=this
            setTimeout(function(){
                if(opt.inframe){
                    win.parent.__gl.domSelector.resetPosition(that);
                    win.parent.__gl.shadow.mask({ el: that }, null, null);
                }
                
            },100);
        }
    }
    return {
        ed: param.ed,
        create: function(option) {
            if (param.el) {
                var el = param.el,
                    $el = $(el),
                    attrs = el.attributes,
                    origAttrs = {},
                    uuid = (new Date()).getTime() + "";
                _.forEach(attrs, function(attr) {
                    origAttrs[attr.name] = attr.value;
                });

                param.origAttrs = origAttrs;
                removeOnfocus(el);
                $el.addClass(uuid);
                //必须在tinyMCE实例化之前绑定，才能将回车换行拦截掉
                $el.on("keypress keydown keyup", keyFn).on("paste", pasteFn);
                $el.on("click",function(e){
                    e.preventDefault();
                    e.stopPropagation();
                })
                if (!option) {
                    option = {};
                }
                option.language = lang;

                tinyMCEInit(uuid, option);

            }
        }
    }
}