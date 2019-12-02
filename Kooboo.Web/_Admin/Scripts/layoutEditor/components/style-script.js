(function() {
  var bindingType = ["script", "style"];
  var ATTR_RES_TAG_ID = "kb-res-tag-id";
  var Script =
      Kooboo[Kooboo.layoutEditor ? "layoutEditor" : "pageEditor"].viewModel
        .Script,
    Style =
      Kooboo[Kooboo.layoutEditor ? "layoutEditor" : "pageEditor"].viewModel
        .Style,
    BindingStore =
      Kooboo[Kooboo.layoutEditor ? "layoutEditor" : "pageEditor"].store
        .BindingStore;
  Vue.component("kb-layout-style-script", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/layoutEditor/components/style-script.html"
    ),
    data: function() {
      return {
        id: "",
        elem: null,
        type: null,
        text: null,
        textError: "",
        isShow: false,
        title: "",
        async: false,
        defer: false,
        appendHead: false,
        files: null,
        groups: null,
        win: null,
        isResourceExist: true,
        files: [],
        groups: []
      };
    },
    computed: {
      lang: function() {
        if (this.type == "script") return "javascript";
        if (this.type == "style") return "css";
        return "html";
      }
    },
    mounted: function() {
      var self = this;
      Kooboo.EventBus.subscribe("binding/edit", function(data) {
        if (bindingType.indexOf(data.type) > -1) {
          self.isShow = true;
          self.elem = data.elem;
          self.type = data.type;
          self.title = data.type;
          self.id = data.id;
          self.text = typeof data.text == "object" ? "" : data.text;
          self.appendHead = data.isHead;
          if (data.resources) {
            (self.files = data.resources[data.type + "s"] || []),
              (self.groups = data.resources[data.type + "Group"] || []);
            if (self.files.length + self.groups.length) {
              self.text = {
                url:
                  data.resources[
                    data.resources[data.type + "s"].length
                      ? data.type + "s"
                      : data.type + "Group"
                  ][0].url,
                name:
                  data.resources[
                    data.resources[data.type + "s"].length
                      ? data.type + "s"
                      : data.type + "Group"
                  ][0].text
              };
              self.isResourceExist = true;
            } else {
              self.isResourceExist = false;
            }
          }
        }
      });
    },
    methods: {
      reset: function() {
        var self = this;
        self.elem = null;
        self.type = null;
        self.showError = false;
        self.id = null;
        self.text = null;
        self.textError = "";
        self.isShow = false;
        self.title = "";
        self.async = false;
        self.defer = false;
        self.appendHead = false;
        self.files = null;
        self.groups = null;
        Kooboo.EventBus.publish("binding/select/style/cancel", {});
        if (self.win) {
          var evt = document.createEvent("Event");
          evt.initEvent("load", false, false);
          self.win.dispatchEvent(evt);
        }
      },
      save: function() {
        var self = this;
        var doc = self.elem;
        while (doc && doc.nodeType !== 9) {
          doc = doc.parentNode;
        }
        if (self.id) {
          if (!self.text) return alert(Kooboo.text.validation.required);
          var old = BindingStore.byId(self.id);
          if (old) {
            old.text = self.text;
            old.name = self.text;
            BindingStore.update(self.id, old);
          }
          self.elem.innerHTML = self.text;
          Kooboo.EventBus.publish("kb/frame/resource/update", {
            type: old.type,
            resTagId: $(old.elem).attr(ATTR_RES_TAG_ID),
            content: self.text
          });
        } else {
          switch (self.type) {
            case "script":
              var scriptTag = doc.createElement("script");
              scriptTag.setAttribute("src", self.text.url);
              if (self.appendHead) {
                scriptTag.setAttribute(
                  ATTR_RES_TAG_ID,
                  Kooboo.getResourceTagId("script-head")
                );
                doc.head.appendChild(scriptTag);
              } else {
                scriptTag.setAttribute(
                  ATTR_RES_TAG_ID,
                  Kooboo.getResourceTagId("script-body")
                );
                doc.body.appendChild(scriptTag);
              }
              var scriptEntry = new Script({
                id: Kooboo.Guid.NewGuid(),
                elem: scriptTag,
                head: !!self.appendHead,
                url: self.text.url,
                displayName: self.text.name
              });
              BindingStore.add(scriptEntry);
              Kooboo.EventBus.publish("kb/frame/resource/add", {
                type: "script",
                tag: $(scriptTag).clone()[0],
                isAppendToHead: self.appendHead
              });
              break;
            case "style":
              if (doc) {
                var linkTag = doc.createElement("link");
                linkTag.setAttribute("rel", "stylesheet");
                linkTag.setAttribute("href", self.text.url);
                linkTag.setAttribute(
                  ATTR_RES_TAG_ID,
                  Kooboo.getResourceTagId("style-head")
                );
                var existStyles = $("link, style", doc);
                if (!existStyles.length) {
                  doc.head.appendChild(linkTag);
                } else {
                  $(linkTag).insertAfter(existStyles.last());
                }
                var styleEntry = new Style({
                  id: Kooboo.Guid.NewGuid(),
                  elem: linkTag,
                  displayName: self.text.name,
                  url: self.text.url
                });
                BindingStore.add(styleEntry);
                Kooboo.EventBus.publish("kb/frame/resource/add", {
                  type: "style",
                  tag: $(linkTag).clone()[0]
                });
              } else {
                Kooboo.EventBus.publish("binding/select/style", {
                  style: self.text.url
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
        });
      },
      formatCode:function(){
        this.$refs.codeEditor.formatCode();
      }
    }
  });
})();
