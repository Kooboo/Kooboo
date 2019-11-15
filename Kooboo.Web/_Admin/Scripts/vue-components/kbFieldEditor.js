(function() {
  Kooboo.loadJS([
    "/_Admin/Scripts/vue-components/kbDialog.js",
    "/_Admin/Scripts/vue-components/kbTabs.js",
    "/_Admin/Scripts/vue-components/kbForm.js"
  ]);
  Kooboo.loadJS(["/_Admin/Scripts/Kooboo/ControlType.js"]);

  var self;
  Vue.component("kb-field-editor", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/vue-components/kbFieldEditor.html"
    ),
    props: {
      onSave: {},
      closeHandle: {},
      data: {},
      options: {}
    },
    data: function() {
      return {
        displayNames: [
          Kooboo.text.component.fieldEditor.basic,
          Kooboo.text.component.fieldEditor.advanced,
          Kooboo.text.component.fieldEditor.validation
        ],
        isNewItem: true,
        d_data: {
          name: "",
          displayName: "",
          controlType: "TextBox",
          isSummaryField: false,
          multipleLanguage: false,
          editable: true,
          order: 0,
          tooltip: "",
          validations: []
        },
        formRules: {
          subdomain: [
            {
              pattern: /^([A-Za-z][\w\-\.]*)*$/,
              message: Kooboo.text.validation.objectNameRegex
            },
            {
              min: 1,
              max: 63,
              message:
                Kooboo.text.validation.minLength +
                0 +
                ", " +
                Kooboo.text.validation.maxLength +
                63
            },
            {
              validate: function(value) {
                var exist = _.map(self.domainsData, function(dm) {
                  return dm.fullName;
                });
                if (
                  exist.indexOf(
                    self.d_data.subdomain + "." + self.d_data.root
                  ) > -1
                ) {
                  return false;
                }
                return true;
              },
              message: Kooboo.text.validation.taken
            },
            {
              remote: {
                url: Kooboo.Site.CheckDomainBindingAvailable(),
                data: function() {
                  return {
                    SubDomain: self.d_data.subdomain,
                    RootDomain: self.d_data.root
                  };
                }
              },
              message: Kooboo.text.validation.taken
            }
          ],
          port: [
            { required: Kooboo.text.validation.required },
            {
              pattern: /^\d*$/,
              message: Kooboo.text.validation.invaildPort
            },
            {
              min: 0,
              max: 65535,
              message: Kooboo.text.validation.portRange
            }
          ]
        },
        _controlTypes: []
      };
    },
    created: function() {
      self = this;

      self._controlTypes = self.getControlTypes(self.options.controlTypes);
      if (self.data) {
        self.isNewItem = false;
        self.d_data = self.data;
      }
    },
    watch: {
      d_data: {
        handler: function(val, oldVal) {
          console.log(val);
        },
        deep: true
      }
    },
    methods: {
      getControlTypes: function(types) {
        var _types = [];
        var CONTROL_TYPES = Kooboo.controlTypes;
        types.forEach(function(t) {
          var _t = CONTROL_TYPES.find(function(c) {
            return c.value.toLowerCase() == t;
          });

          _types.push(_t || { displayName: "NOT_FOUND" });
        });

        return _types;
      },
      getControlTypeByValue: function(value) {
        console.log(value);
        return self._controlTypes.find(function(item) {
          return item.value.toLowerCase() === value.toLowerCase();
        });
      }
    }
  });
})();

