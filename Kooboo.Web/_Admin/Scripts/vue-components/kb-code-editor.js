Vue.component("kb-code-editor", {
  template: "<div ref='container' style='height:100%;'></div>",
  props: {
    lang: {
      type: String,
      default: "javascript"
    },
    code: String,
    option: Function
  },
  mounted: function() {
    var loaderSrc = "https://unpkg.com/monaco-editor@0.18.1/min/vs/loader.js";
    var loaderScript = document.querySelector("[src='" + loaderSrc + "']");
    if (!loaderScript) {
      var loaderScript = document.createElement("script");
      loaderScript.onload = this.init;
      loaderScript.src = loaderSrc;
      loaderScript.type = "text/javascript";
      document.head.appendChild(loaderScript);
    } else this.init();
  },
  methods: {
    init: function() {
      var me = this;
      require.config({
        paths: { vs: "https://unpkg.com/monaco-editor@0.18.1/min/vs" }
      });
      window.MonacoEnvironment = {
        getWorkerUrl: function(workerId, label) {
          var encoded = encodeURIComponent(
            "self.MonacoEnvironment = { baseUrl: 'https://unpkg.com/monaco-editor@0.18.1/min/' }; importScripts('https://unpkg.com/monaco-editor@0.18.1/min/vs/base/worker/workerMain.js');"
          );
          return "data:text/javascript;charset=utf-8," + encoded;
        }
      };

      require(["vs/editor/editor.main"], function() {
        var el = me.$refs.container;
        if (this.option) {
          this.option(el, monaco);
        } else {
          monaco.editor.create(el, {
            value: me.code,
            language: me.lang
          });
        }
      });
    }
  }
});
