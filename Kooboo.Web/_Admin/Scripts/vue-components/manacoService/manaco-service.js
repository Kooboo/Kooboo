var MonacoEditorService =
  /*#__PURE__*/
  (function() {
    "use strict";
    var monaco;
    var self;

    function MonacoEditorService() {
      self = this;
      this.editor = undefined;
      this.model = undefined;
      this.monaco = undefined;
      this.require = undefined;
    }

    MonacoEditorService.prototype.loader = function(initParameter) {
      var cdnUrl = "https://unpkg.com/monaco-editor@0.18.1/min/vs/loader.js";
      $.getScript(cdnUrl, function(response, status) {
        if (status === "success") {
          self.require = window.require;
          self.require.config({
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
            self.monaco = window.monaco;

            monaco = self.monaco;

            if (self.monaco) {
              self.init(initParameter.callback, initParameter.files);
            } else {
              self.loader(initParameter);
            }
          });
        } else {
          self.loader(initParameter);
        }
      });
    };
    MonacoEditorService.prototype.init = function(callback, files) {
      if (this.require && this.monaco && monaco) {
        if (callback) {
          this.destroy();
          callback(monaco);
        }
      } else {
        var initParameter = { callback: callback, files: files };
        this.loader(initParameter);
      }
    };
    MonacoEditorService.prototype.create = function(
      el,
      value,
      language,
      options,
      path
    ) {
      if (!path) {
        path = "/file/" + Date.now();
      }
      this.model = this.createModel(value, path, language);
      var op;
      if (options) {
        op = options;
        delete op.value;
        delete op.language;
      }
      op.model = this.model;
      this.editor = monaco.editor.create(el, op);
      return { model: this.model, editor: this.editor };
    };
    MonacoEditorService.prototype.createModel = function createModel(
      value,
      path,
      language
    ) {
      return monaco.editor.createModel(value, language, monaco.Uri.file(path));
    };

    MonacoEditorService.prototype.destroy = function() {
      var models = monaco.editor.getModels();
      models.forEach(function(model) {
        model.dispose();
      });
      if (this.editor && this.editor.dispose) {
        this.editor.dispose();
      }
    };
    MonacoEditorService.prototype.changeValue = function(value, model) {
      if (model) {
        model.setValue(value);
      } else {
        this.model.setValue(value);
      }
    };
    MonacoEditorService.prototype.onModelContentChange = function(
      model,
      callback
    ) {
      if (!model) {
        model = this.model;
      }
      self.model.onDidChangeContent(function(event) {
        var content = model.getValue();
        callback(content);
      });
    };
    MonacoEditorService.prototype.format = function() {
      self.editor.getAction("editor.action.formatDocument").run();
    };
    MonacoEditorService.prototype.changeLanguage = function(language, model) {
      if (model) {
        monaco.editor.setModelLanguage(model, language);
      } else {
        monaco.editor.setModelLanguage(this.model, language);
      }
    };
    MonacoEditorService.prototype.changeTheme = function(value) {
      monaco.editor.setTheme(value);
    };
    return MonacoEditorService;
  })();
