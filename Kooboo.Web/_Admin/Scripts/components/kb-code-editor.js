(function() {
  Kooboo.loadJS([
    "/_Admin/Scripts/components/manacoService/manaco-service.js"
  ]);

  var state = Vue.observable({ loader: false });
  var monacoService = new MonacoEditorService();
  var monaco;
  monacoService.loader(function(Monaco) {
    state.loader = true;
    monaco = Monaco;
    var kscriptContent = Kooboo.getTemplate("/_Admin/Scripts/vue-components/manacoService/kscript.d.ts");
    monacoService.addExtraLib('javascript',kscriptContent,'kscript/kscript.d.ts');
    monacoService.init();
  });

  Vue.component("kb-code-editor", {
    template: "<div  style='width:100%;height:100%'></div>",
    props: {
      lang: {
        type: String,
        require: true
      },
      autoSize: { type: Boolean, default: false },
      code: { type: String, require: true },
      options: {}
    },
    data: function() {
      return {
        isInit: false,
        d_code: this.code,
        isCreate: false,
        monacoService: monacoService,
        model: undefined,
        editor: undefined
      };
    },
    watch: {
      loader: function(value) {
        var self = this;
        this.$nextTick(function() {
          self.render();
        });
      },
      d_code: function(value) {
        this.$emit("update:code", value);
      },
      code: function(value, old) {
        if (value !== old && value !== this.d_code) {
          if (this.model) {
            monacoService.changeValue(value, this.model);
          }
        }
      },
      lang: function(value) {
        if (this.model) {
          monacoService.changeLanguage(value, this.model);
        }
      }
    },
    computed: {
      loader: function() {
        return state.loader;
      }
    },
    mounted: function() {
      if (this.loader) this.render();
    },
    methods: {
      render: function() {
        var self = this;
        if (monacoService.isLoader) {
          var options = {};
          var path = self._uid;
          if (self.options) {
            options = self.options;
          }
          if (self.autoSize) {
            options.automaticLayout = true;
          }
          var temp = monacoService.create(
              self.$el,
              self.d_code || self.code,
              self.lang,
              options,
              path
          );
          self.editor = temp.editor;
          self.model = temp.model;
          self.monaco = monaco;
          self.isInit = true;
          self.isCreate = true;
          monacoService.onModelContentChange(self.model, function(content) {
            self.d_code = content;
          });
        }
      },
      changeTheme: function(value) {
        monacoService.changeTheme(value);
      },
      formatCode: function(callback) {
        monacoService.format(this.editor, callback);
      },
      replace: function (text,range) {
        monacoService.replace(this.editor,text,range);
      }
    }
  });
})();
