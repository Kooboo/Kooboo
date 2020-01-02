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
      var baseUrl = "https://cdn.jsdelivr.net/npm";
      $.getScript(baseUrl + "/monaco-editor-core@0.19.0/min/vs/loader.js").done(
          function() {
            window.require.config({
              paths: {
                vs: baseUrl + "/monaco-editor-core@0.19.0/min/vs",
                "vs/basic-languages":
                    baseUrl + "/monaco-languages@1.9.0/release/min",
                "vs/language/html":
                    baseUrl + "/monaco-html-extra@2.6.0/release/min",
                "vs/language/css": baseUrl + "/monaco-css@2.6.0/release/min",
                "vs/language/typescript":
                    baseUrl + "/monaco-typescript@3.6.1/release/min"
              }
            });
            window.require(
                ["vs/editor/editor.main", "vs/editor/editor.main.nls"],
                function() {
                  require([
                    "vs/basic-languages/monaco.contribution",
                    "vs/language/html/monaco.contribution",
                    "vs/language/css/monaco.contribution",
                    "vs/language/typescript/monaco.contribution"
                  ], function() {
                    monaco = window.monaco;
                    callback(monaco);
                  });

                  self.isLoader = true;
                }
            );
          }
      );
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
    MonacoEditorService.prototype.addManualTriggerSuggest = function(editor) {
      editor.addAction({
        id: "ManualTriggerSuggest",
        label: "ManualTriggerSuggest",
        keybindings: [
          monaco.KeyMod.CtrlCmd | monaco.KeyCode.KEY_J,
          monaco.KeyMod.CtrlCmd | monaco.KeyCode.Space,
          monaco.KeyMod.CtrlCmd | monaco.KeyMod.Alt | monaco.KeyCode.Space
        ],
        precondition: null,
        keybindingContext: null,
        contextMenuGroupId: "ManualTriggerSuggest",
        contextMenuOrder: 1.5,
        run: function(ed) {
          ed.getAction("editor.action.triggerSuggest").run();
        }
      });
    };

    MonacoEditorService.prototype.addCompleteForHtmlTag = function(
      suggestions
    ) {
      monaco.languages.registerCompletionItemProvider("html", {
        triggerCharacters: ["<"],
        provideCompletionItems: function(model, position) {
          var textUntilPosition = model.getValueInRange({
            startLineNumber: position.lineNumber,
            startColumn: 1,
            endLineNumber: position.lineNumber,
            endColumn: position.column
          });

          if (!textUntilPosition.endsWith("<")) return;

          var extendTags = [
            "view",
            "htmlblock",
            "layout",
            "menu",
            "placeholder"
          ];

          return {
            suggestions: extendTags.map(function(item) {
              return {
                label: item,
                kind: monaco.languages.CompletionItemKind.Property,
                documentation: item,
                insertText: item
              };
            })
          };
        }
      });

      // monaco.languages.registerCompletionItemProvider("html", {
      //   triggerCharacters: [">"],
      //   provideCompletionItems: function(model, position) {
      //     var textUntilPosition = model.getValueInRange({
      //       startLineNumber: position.lineNumber,
      //       startColumn: 1,
      //       endLineNumber: position.lineNumber,
      //       endColumn: position.column
      //     });

      //     if (
      //       textUntilPosition.split("<").length !=
      //       textUntilPosition.split(">").length
      //     ) {
      //       return;
      //     }

      //     var tag = textUntilPosition.split("<").pop();

      //     if (tag) {
      //       return {
      //         suggestions: [
      //           {
      //             label: "</" + tag,
      //             kind: monaco.languages.CompletionItemKind.Property,
      //             documentation: "</" + tag,
      //             insertText: "${1}</" + tag,
      //             insertTextRules:
      //               monaco.languages.CompletionItemInsertTextRule
      //                 .InsertAsSnippet
      //           }
      //         ]
      //       };
      //     }
      //   }
      // });

      monaco.languages.registerCompletionItemProvider("html", {
        provideCompletionItems: function(model, position) {
          var textUntilPosition = model.getValueInRange({
            startLineNumber: 1,
            startColumn: 1,
            endLineNumber: position.lineNumber,
            endColumn: position.column
          });
          var matchs = textUntilPosition.match(
            /<[\w\d-]+\s+((?!<\/).)*[a-zA-Z\-]$/
          ); // <div .... k> or <div ....> k
          if (!matchs) return;
          var cleanTag = matchs[0]
            .replace(/=\s*"[^"]*"/g, "") // remove attributes ""
            .replace(/=\s*'[^"]*'/g, "") // remove attributes ''
            .replace(/<.+[^>]+>/g, ""); // remove tags
          if (/["']/.test(cleanTag)) return; // is inside other attribute
          if (!/<[\w\d-]+/.test(cleanTag)) return; // is not attribute

          // clone sugguestions
          var tempSuggestions = suggestions.map(function(item) {
            return {
              label: item.label,
              kind: item.kind || monaco.languages.CompletionItemKind.Value,
              documentation: item.documentation,
              insertText: item.insertText,
              insertTextRules:
                item.insertTextRules ||
                monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet
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
