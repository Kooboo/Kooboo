Kooboo.loadJS(["/_Admin/Scripts/lib/js-beautify/lib/beautify-css.js"]);

var MonacoEditorService =
  /*#__PURE__*/
  (function() {
    "use strict";
    var monaco;
    var self;

    function MonacoEditorService() {
      self = this;
      this.editor = undefined;
      this.monaco = undefined;
      this.isLoader = undefined;
    }

    MonacoEditorService.prototype.initCssformat = function(monaco) {
      if (window.css_beautify) {
        var cssFormatter = function(monaco, beautyOption) {
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
          var disposeArr = ["css", "less", "scss"].map(function(language) {
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
          return function() {
            disposeArr.forEach(function(arr) {
              return arr.forEach(function(disposable) {
                return disposable.dispose();
              });
            });
          };
        };
        cssFormatter(monaco);
      }
    };
    MonacoEditorService.prototype.loader = function(callback) {
      var loaderUrl = "https://unpkg.com/monaco-editor@0.18.1/min/vs/loader.js";

      $.getScript(loaderUrl, function(response, status) {
        if (status === "success") {
          window.require.config({
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
          window.require(["vs/editor/editor.main"], function() {
            monaco = window.monaco;
            self.isLoader = true;
            callback(monaco);
          });
        }
      });
    };
    MonacoEditorService.prototype.init = function(callback, files) {
      if (window.monaco) {
        this.initCssformat(window.monaco || monaco);
        if (callback) {
          callback(monaco);
        }
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
      } else {
        path = path.toString();
      }
      var model = monaco.editor.getModel(monaco.Uri.file(path));
      if (model && model.dispose) {
        model.dispose();
      }
      model = this.createModel(value, path, language);
      var op = {};
      if (options) {
        op = options;
        delete op.value;
        delete op.language;
      }
      op.model = model;
      return { model: model, editor: monaco.editor.create(el, op) };
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
      }
    };
    MonacoEditorService.prototype.onModelContentChange = function(
      model,
      callback
    ) {
      if (model) {
        model.onDidChangeContent(function(event) {
          var content = model.getValue();
          callback(content, event);
        });
      }
    };
    MonacoEditorService.prototype.format = function(editor, callback) {
      editor
        .getAction("editor.action.formatDocument")
        .run()
        .then(callback);
    };
    MonacoEditorService.prototype.changeLanguage = function(language, model) {
      if (model) {
        monaco.editor.setModelLanguage(model, language);
      }
    };
    MonacoEditorService.prototype.changeTheme = function(value) {
      monaco.editor.setTheme(value);
    };
    MonacoEditorService.prototype.replace = function(editor, text, range) {
      if (!range) {
        var selection = editor.getSelection();
        range = new monaco.Range(
          selection.startLineNumber,
          selection.startColumn,
          selection.endLineNumber,
          selection.endColumn
        );
      }
      editor.executeEdits("", [{ range: range, text: text }]);
    };
    MonacoEditorService.prototype.addExtraLib = function(
      language,
      fileContent,
      path
    ) {
      if (!path) {
        path = Date.now();
        var languagesId = monaco.languages.getEncodedLanguageId(language);
        if (languagesId) {
          path =
            path +
            "." +
            monaco.languages
              .getLanguages()
              [languagesId - 1].extensions[0].toLowerCase();
        }
      }
      path = monaco.Uri.file(path);
      switch (language) {
        case "javascript":
          monaco.languages.typescript.javascriptDefaults.addExtraLib(
            fileContent,
            path
          );
          break;
        case "typescript":
          monaco.languages.typescript.typescriptDefaults.addExtraLib(
            fileContent,
            path
          );
          break;
        default:
          if (monaco.languages[language]) {
            monaco.languages[language].addExtraLib(fileContent, path);
          } else {
            console.error("monaco.languages is no " + language);
          }
      }
    };
    MonacoEditorService.prototype.addCompleteForHtmlTag = function(
      suggestions
    ) {
      monaco.languages.registerCompletionItemProvider("html", {
        provideCompletionItems: function(model, position) {
          var textUntilPosition = model.getValueInRange({
            startLineNumber: 1,
            startColumn: 1,
            endLineNumber: position.lineNumber,
            endColumn: position.column
          });
          var matchs = textUntilPosition.match(
            /<[a-zA-Z]+[^>]*\s+[a-zA-Z\-]$/g
          ); // <div .... k>
          if (!matchs) return;
          var isInAttribute = function(str) {
            var temp1 = str.match(/"/g);
            if (temp1 && temp1.length % 2) return true;
            var temp2 = str.match(/'/g);
            if (temp2 && temp2.length % 2) return true;
          };
          if (isInAttribute(matchs[0])) return;

          // clone sugguestions
          var tempSuggestions = suggestions.map(function(item) {
            return {
              label: item.label,
              kind: item.kind,
              documentation: item.documentation,
              insertText: item.insertText
            };
          });
          return {
            suggestions: tempSuggestions
          };
        }
      });
    };
    return MonacoEditorService;
  })();
