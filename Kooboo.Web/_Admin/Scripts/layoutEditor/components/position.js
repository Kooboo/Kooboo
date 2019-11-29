(function() {
  var PositionStore = Kooboo.layoutEditor.store.PositionStore;
  var self;

  Vue.component("kb-layout-placeholder", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/layoutEditor/components/position.html"
    ),
    data: function() {
      self = this;
      return {
        id: "",
        elem: null,
        model: {
          name: ""
        },
        rules: {
          name: [
            {
              required: true,
              message: Kooboo.text.validation.required
            },
            {
              validate: function(val) {
                return !_.some(PositionStore.getAll(), function(p) {
                  return p.name === val && p.name !== self._name;
                });
              },
              message: Kooboo.text.validation.taken
            },
            {
              pattern: /^([A-Za-z][\w\-\.]*)*[A-Za-z0-9]$/,
              message: Kooboo.text.validation.objectNameRegex
            }
          ]
        },
        type: "",
        isShow: false,
        applyOmit: false
      };
    },
    mounted: function() {
      Kooboo.EventBus.subscribe("position/edit", function(data) {
        self.id = data.id;
        self.elem = data.elem;
        self.model.name = data.name || "";
        self._name = data.name;
        self.type = data.type;
        self.isShow = true;
        self.applyOmit = data.id && self.elem.hasAttribute("k-omit");
      });
    },
    methods: {
      save: function() {
        if (!self.$refs.positionForm.validate()) {
          return;
        }
        var id = self.id,
          elem = self.elem,
          name = self.model.name,
          type = self.type,
          applyOmit = self.applyOmit;
        if (id) {
          Kooboo.EventBus.publish("position/update", {
            id: id,
            name: name,
            elem: elem,
            applyOmit: applyOmit
          });
        } else {
          Kooboo.EventBus.publish("position/add", {
            elem: elem,
            name: name,
            type: type,
            applyOmit: applyOmit
          });
        }
        Kooboo.EventBus.publish("kb/frame/dom/update");
        self.reset();
      },
      reset: function() {
        self.elem = null;
        self.id = "";
        self.model.name = "";
        self.type = "";
        self.applyOmit = false;
        self.isShow = false;
        self.$refs.positionForm.clearValid();
      }
    }
  });
})();
