(function() {
  Kooboo.loadJS([
    "/_Admin/Scripts/vue-components/manacoService/manaco-service.js"
  ]);
  var monacoService = new MonacoEditorService();
  var self;
  Vue.component("kb-code-editor", {
    template: "<div  ref='container' style='width:100%;height:100%'></div>",
    props: {
      lang: {
        type: String,
        require: true
      },
      autoSize: { default: false },
      code: { type: String, require: true },
      options: {}
    },
    data: function() {
      return {
        isInit: false,
        d_code: this.code,
        isCreate: false,
        monacoService: monacoService,
        monaco: undefined,
        model: undefined,
        editor: undefined
      };
    },
    watch: {
      d_code: function(value) {
        this.$emit("update:code", value);
      },
      code: function(value, old) {
        if (value !== old && value !== self.d_code) {
          if (this.model) {
            self.monacoService.changeValue(value, this.model);
          }
        }
      },
      lang: function(value) {
        if (this.model) {
          self.monacoService.changeLanguage(value, this.model);
        }
      }
    },
    created: function() {
      self = this;
    },
    mounted: function() {
      if (!self.isInit) {
        monacoService.init(function(monaco) {
          self.monaco = monaco;
          var options = {};
          if (self.options) {
            options = self.options;
          }
          if (self.autoSize) {
            options.automaticLayout = true;
          }
          var temp = self.monacoService.create(
            self.$refs.container,
            self.d_code || self.code,
            self.lang,
            options
          );
          self.editor = temp.editor;
          self.model = temp.model;
          self.isInit = true;
          self.isCreate = true;
          self.monacoService.onModelContentChange(self.model, function(
            content
          ) {
            self.d_code = content;
          });
        });
      }
    },
    methods: {
      setAutosize: function(value) {
        self.monacoService.setAutosize(value);
      },
      changeTheme: function(value) {
        self.monacoService.changeTheme(value);
      },
      formatCode: function() {
        self.monacoService.format();
      }
    }
  });
})();

