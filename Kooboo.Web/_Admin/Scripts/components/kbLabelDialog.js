(function() {
  var bindingType = "label",
    KB_NEW_LABEL = "__KB__NEW__LABEL";
  var self;
  Vue.component("kb-label-dialog", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/components/kbLabelDialog.html"
    ),
    data: function() {
      self = this;
      return {
        elem: null,
        id: "",
        title: "",
        isShow: false,
        existLabels: [],
        model: {
          text: ""
        },
        rules: {
          text: [
            {
              required: true,
              message: Kooboo.text.validation.required
            },
            {
              pattern: /^([A-Za-z][\w\-\.]*)*[A-Za-z0-9]$/,
              message: Kooboo.text.validation.objectNameRegex
            }
          ]
        },
        labelValue: "",
        placeholder: ""
      };
    },
    computed: {
      showInput: function() {
        return self.labelValue == KB_NEW_LABEL;
      },
      hasOnSave() {
        return self.$listeners && !!self.$listeners["on-save"];
      }
    },
    mounted: function() {
      Kooboo.EventBus.subscribe("binding/edit", function(data) {
        if (data.bindingType == bindingType || data.type == bindingType) {
          Kooboo.Label.getKeys().then(function(res) {
            if (res.success) {
              var list = [];
              _.forEach(res.model, function(label) {
                list.push({
                  name: label,
                  value: label
                });
              });

              var name =
                data && data.name
                  ? data.name
                  : Kooboo.text.site.label.createANewLabel;
              list.push({
                name: name,
                value: KB_NEW_LABEL
              });
              self.existLabels = list;

              var find = _.some(res.model, function(m) {
                return m == data.text;
              });

              self.labelValue = find ? data.text : KB_NEW_LABEL;
              self.model.text = data.text || "";

              var placeholder =
                data && data.placeholder
                  ? data.placeholder
                  : Kooboo.text.site.label.placeholder;
              self.placeholder = placeholder;

              self.elem = data.elem;
              self.id = data.id;
              self.isShow = true;
              self.title = data.title || Kooboo.text.common.Label;
            }
          });
        }
      });
    },
    methods: {
      reset: function() {
        self.elem = null;
        self.id = "";
        self.model.text = "";
        self.isShow = false;
        self.$refs.form.clearValid();
      },
      isValid: function() {
        if (self.showInput) {
          return self.$refs.form.validate();
        } else {
          return true;
        }
      },
      save: function() {
        if (self.isValid()) {
          var result = {
            elem: self.elem,
            text: self.showInput ? self.model.text : self.labelValue,
            type: bindingType
          };
          if (self.hasOnSave) {
            result.bindingType = bindingType;
            self.$emit("on-save", result);
          } else {
            result.id = self.id;
            Kooboo.EventBus.publish("binding/save", result);
          }
          self.reset();
        }
      }
    }
  });
})();
