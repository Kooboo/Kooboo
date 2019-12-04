(function() {
  Kooboo.loadJS(["/_Admin/Scripts/lib/jquery.textarea_autosize.min.js"]);

  var self;
  Vue.component("kb-view-edit-node", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/viewEditor/components/editNode.html"
    ),
    data: function() {
      self = this;
      return {
        isShow: false,
        type: "normal",
        href: "",
        inNewWindow: false,
        html: ""
      };
    },
    mounted: function() {
      Kooboo.EventBus.subscribe("kb/view/edit/node", function(data) {
        self.isShow = true;
        self.type = data.type;
        self.html = data.html || "";
        self.href = data.href || "";
        self.inNewWindow = data.inNewWindow || "";
      });
      this.$nextTick(function() {
        $("#edit-node-modal").on("shown.bs.modal", function() {
          $(".autosize")
            .textareaAutoSize()
            .trigger("keyup");
        });
      });
    },
    methods: {
      reset: function() {
        self.isShow = false;
        self.html = "";
        self.href = "";
        self.inNewWindow = false;
        self.type = "normal";
      },
      save: function() {
        self.$emit("on-save", {
          type: self.type,
          html: self.html,
          href: self.href,
          inNewWindow: self.inNewWindow
        });
        Kooboo.EventBus.publish("kb/frame/dom/update");
        self.reset();
      }
    }
  });
})();
