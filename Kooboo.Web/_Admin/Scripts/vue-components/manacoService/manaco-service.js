Kooboo.loadJS(["/_Admin/Scripts/lib/js-beautify/lib/beautify-css.js"]);
var MonacoEditorService =
    /*#__PURE__*/
    (function () {
      "use strict";
      var monaco;
      var self;

      function MonacoEditorService() {
        self = this;
        this.editor = undefined;
        this.monaco = undefined;
        this.isLoader = undefined;
      }

      MonacoEditorService.prototype.initCssformat = function (monaco) {
        if (window.css_beautify) {
          var cssFormatter = function (monaco, beautyOption) {
            if (monaco === void 0) {
              monaco = window.monaco;
            }

            if (beautyOption === void 0) {
              beautyOption = {};
            }

            if (!monaco) {
              console.error(
                  "css-format-monaco: 'monaco' should be either declared on window or passed as first parameter"
              );
              return;
            }

            var documentProvider = {
              provideDocumentFormattingEdits: function provideDocumentFormattingEdits(
                  model
              ) {
                var lineCount = model.getLineCount();
                return [
                  {
                    range: new monaco.Range(
                        1,
                        1,
                        lineCount,
                        model.getLineMaxColumn(lineCount) + 1
                    ),
                    text: css_beautify(model.getValue(), beautyOption)
                  }
                ];
              }
            };
            var rangeProvider = {
              provideDocumentRangeFormattingEdits: function provideDocumentRangeFormattingEdits(
                  model,
                  range
              ) {
                var fullLineRange = new monaco.Range(
                    range.startLineNumber,
                    1,
                    range.endLineNumber,
                    model.getLineMaxColumn(range.endLineNumber) + 1
                );
                var code = model.getValueInRange(fullLineRange);
                return [
                  {
                    range: fullLineRange,
                    text: css_beautify(code, beautyOption)
                  }
                ];
              }
            };
            var disposeArr = ["css", "less", "scss"].map(function (language) {
              return [
                monaco.languages.registerDocumentFormattingEditProvider(
                    language,
                    documentProvider
                ),
                monaco.languages.registerDocumentRangeFormattingEditProvider(
                    language,
                    rangeProvider
                )
              ];
            });
            return function () {
              disposeArr.forEach(function (arr) {
                return arr.forEach(function (disposable) {
                  return disposable.dispose();
                });
              });
            };
          };
          cssFormatter(monaco);
        }
      };
      MonacoEditorService.prototype.loader = function (callback) {
        var loaderUrl = "https://unpkg.com/monaco-editor@0.18.1/min/vs/loader.js";

        $.getScript(loaderUrl, function (response, status) {

          if (status === "success") {
            window.require.config({
              paths: {vs: "https://unpkg.com/monaco-editor@0.18.1/min/vs"}
            });
            window.MonacoEnvironment = {
              getWorkerUrl: function (workerId, label) {
                var encoded = encodeURIComponent(
                    "self.MonacoEnvironment = { baseUrl: 'https://unpkg.com/monaco-editor@0.18.1/min/' }; importScripts('https://unpkg.com/monaco-editor@0.18.1/min/vs/base/worker/workerMain.js');"
                );
                return "data:text/javascript;charset=utf-8," + encoded;
              }
            };
            window.require(["vs/editor/editor.main"], function () {
              debugger
              monaco = window.monaco;
              self.isLoader = true;
              callback()
            });
          }
        })
      };
      MonacoEditorService.prototype.init = function (callback, files) {
        if (window.monaco) {
          this.initCssformat(window.monaco || monaco);
          if (callback) {
            callback(monaco);
          }
        }
      };
      MonacoEditorService.prototype.create = function (
          el,
          value,
          language,
          options,
          path
      ) {
        if (!path) {
          path = "/file/" + Date.now();
        } else {
          path = path.toString()
        }
        var model = this.createModel(value, path, language);
        var op = {};
        if (options) {
          op = options;
          delete op.value;
          delete op.language;
        }
        op.model = model;
        return {model: model, editor: monaco.editor.create(el, op)};
      };
      MonacoEditorService.prototype.createModel = function createModel(
          value,
          path,
          language
      ) {
        return monaco.editor.createModel(value, language, monaco.Uri.file(path));
      };

      MonacoEditorService.prototype.destroy = function () {
        var models = monaco.editor.getModels();
        models.forEach(function (model) {
          model.dispose();
        });
        if (this.editor && this.editor.dispose) {
          this.editor.dispose();
        }
      };
      MonacoEditorService.prototype.changeValue = function (value, model) {
        if (model) {
          model.setValue(value);
        }
      };
      MonacoEditorService.prototype.onModelContentChange = function (
          model,
          callback
      ) {
        if (model) {
          model.onDidChangeContent(function (event) {
            var content = model.getValue();
              callback(content,event);
          });
        }
      };
      MonacoEditorService.prototype.format = function (editor) {
        editor.getAction("editor.action.formatDocument").run();
      };
      MonacoEditorService.prototype.changeLanguage = function (language, model) {
        if (model) {
          monaco.editor.setModelLanguage(model, language);
        }
      };
      MonacoEditorService.prototype.changeTheme = function (value) {
        monaco.editor.setTheme(value);
      };
      return MonacoEditorService;
    })();
