(function() {
    var bindingType = ['script', 'style'];

    var ATTR_RES_TAG_ID = 'kb-res-tag-id';

    var template = Kooboo.getTemplate("/_Admin/Scripts/layoutEditor/components/style-script.html"),
        Script = Kooboo[Kooboo.layoutEditor ? 'layoutEditor' : 'pageEditor'].viewModel.Script,
        Style = Kooboo[Kooboo.layoutEditor ? 'layoutEditor' : 'pageEditor'].viewModel.Style,
        BindingStore = Kooboo[Kooboo.layoutEditor ? 'layoutEditor' : 'pageEditor'].store.BindingStore;

    ko.components.register("kb-layout-style-script", {
        viewModel: function() {
            var self = this;
            self.elem = null;
            self.title = ko.observable();
            self.id = ko.observable();
            self.text = ko.validateField({
                required: Kooboo.text.validation.required
            });
            self.defer = ko.observable(false);
            self.async = ko.observable(false);
            self.files = ko.observableArray();
            self.groups = ko.observableArray();
            self.type = null;
            self.showAppendHead = ko.observable(false);
            self.showError = ko.observable(false);
            self.isShow = ko.observable();
            self.appendHead = ko.observable(false);
            self.confirmDialog = null;
            self.isResourceExist = ko.observable(true);
            $("#inline-source-modal").on("shown.bs.modal", function() {
                var node = document.getElementById('inline-script');
                if (node) {
                    var cm = CodeMirror.fromTextArea(node, {
                        lineNumbers: true,
                        mode: "htmlmixed"
                    });
                    cm.on("change", function(c, obj) {
                        self.text(c.getValue());
                    });
                    $(node).data("cm", cm);
                }
            }).on("hidden.bs.modal", function() {
                self.reset();
            });
            Kooboo.EventBus.subscribe("binding/edit", function(data) {
                if (bindingType.indexOf(data.type) > -1) {
                    self.isShow(true);
                    self.elem = data.elem;
                    self.type = data.type;
                    self.title(data.type);
                    self.showAppendHead(data.type === "script");
                    self.id(data.id);
                    self.text(typeof data.text == "object" ? "" : data.text);
                    self.appendHead(data.isHead);
                    if (data.resources) {
                        var files = data.resources[data.type + "s"] || [],
                            groups = data.resources[data.type + "Group"] || [];
                        var exists = _.filter(BindingStore.getAll(), { type: data.type });
                        self.files(files);
                        self.groups(groups);
                        if (files.length + groups.length) {
                            self.text({
                                url: data.resources[data.resources[data.type + "s"].length ? (data.type + "s") : (data.type + "Group")][0].url,
                                name: data.resources[data.resources[data.type + "s"].length ? (data.type + "s") : (data.type + "Group")][0].url
                            });
                            self.isResourceExist(true);
                        } else {
                            self.isResourceExist(false);
                        }
                    }
                }
            });
            this.valid = function() {
                return self.text.valid();
            };
            this.save = function() {
                if (self.valid()) {
                    var doc = self.elem;
                    while (doc && doc.nodeType !== 9) {
                        doc = doc.parentNode;
                    }
                    if (self.id()) {
                        var old = BindingStore.byId(self.id());
                        var value = self.text();
                        if (old) {
                            old.text(value);
                            BindingStore.update(self.id(), old);
                        }
                        self.elem.innerHTML = value;
                        Kooboo.EventBus.publish("kb/frame/resource/update", {
                            type: old.type,
                            resTagId: $(old.elem).attr(ATTR_RES_TAG_ID),
                            content: value
                        })
                    } else {
                        switch (self.type) {
                            case "script":
                                var scriptTag = doc.createElement("script");
                                scriptTag.setAttribute("src", self.text().url);
                                if (self.appendHead()) {
                                    scriptTag.setAttribute(ATTR_RES_TAG_ID, Kooboo.getResourceTagId('script-head'));
                                    doc.head.appendChild(scriptTag);
                                } else {
                                    scriptTag.setAttribute(ATTR_RES_TAG_ID, Kooboo.getResourceTagId('script-body'));
                                    doc.body.appendChild(scriptTag);
                                }
                                var scriptEntry = new Script({
                                    id: Kooboo.Guid.NewGuid(),
                                    elem: scriptTag,
                                    head: !!self.appendHead(),
                                    url: self.text().url,
                                    displayName: self.text().name
                                });
                                BindingStore.add(scriptEntry);
                                Kooboo.EventBus.publish("kb/frame/resource/add", {
                                    type: "script",
                                    tag: $(scriptTag).clone()[0],
                                    isAppendToHead: self.appendHead()
                                })
                                break;
                            case "style":
                                if (doc) {
                                    var linkTag = doc.createElement("link");
                                    linkTag.setAttribute("rel", "stylesheet");
                                    linkTag.setAttribute("href", self.text().url);
                                    linkTag.setAttribute(ATTR_RES_TAG_ID, Kooboo.getResourceTagId('style-head'));
                                    var existStyles = $("link, style", doc);
                                    if (!existStyles.length) {
                                        doc.head.appendChild(linkTag);
                                    } else {
                                        $(linkTag).insertAfter(existStyles.last());
                                    }
                                    var styleEntry = new Style({
                                        id: Kooboo.Guid.NewGuid(),
                                        elem: linkTag,
                                        displayName: self.text().name,
                                        url: self.text().url
                                    });
                                    BindingStore.add(styleEntry);
                                    Kooboo.EventBus.publish("kb/frame/resource/add", {
                                        type: "style",
                                        tag: $(linkTag).clone()[0]
                                    });
                                } else {
                                    Kooboo.EventBus.publish("binding/select/style", {
                                        style: self.text().url
                                    });
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    self.reset();
                    Kooboo.EventBus.publish("kb/page/field/change", {
                        type: "resource"
                    })
                } else {
                    self.showError(true);
                }
            };
            this.reset = function() {
                self.elem = null;
                self.type = null;
                self.showError(false);
                self.id(null);
                self.text(null);
                self.isShow(false);
                self.title("");
                self.async(false);
                self.defer(false);
                self.appendHead(false);
                self.showAppendHead(false);
                self.files(null);
                self.groups(null);
                delete self.confirmDialog;
                Kooboo.EventBus.publish("binding/select/style/cancel", {});
                if (self.win) {
                    var evt = document.createEvent("Event");
                    evt.initEvent("load", false, false);
                    self.win.dispatchEvent(evt);
                }
            };
        },
        template: template
    });
})();