Vue.prototype.Kooboo = Kooboo;

// #region <kb-tooltip>
Vue.directive("kb-tooltip", {
  bind: function(el, binding) {
    var trigger = [];
    if (binding.modifiers.focus) trigger.push("focus");
    if (binding.modifiers.hover) trigger.push("hover");
    if (binding.modifiers.click) trigger.push("click");
    if (binding.modifiers.manual) trigger.push("manual");
    trigger = trigger.join(" ");
    var $el = $(el);
    $el.tooltip({
      title: binding.value,
      placement: binding.arg,
      trigger: trigger || "hover",
      html: binding.modifiers.html,
      template: binding.modifiers.error
        ? '<div class="tooltip error" role="tooltip" style="z-index:199999;width: max-content;"><div class="tooltip-arrow"></div><div class="tooltip-inner"></div></div>'
        : '<div class="tooltip" role="tooltip" style="z-index:199999;width: max-content;"><div class="tooltip-arrow"></div><div class="tooltip-inner"></div></div>',
      container: $el.data("container") || "body"
    });
  },
  inserted: function(el, binding) {
    if (binding.modifiers.manual) {
      setTimeout(function() {
        $(el).tooltip("show");
      });
    }
  },
  update: function(el, binding) {
    if (binding.value == binding.oldValue) return;
    var $el = $(el);
    if (binding.value) {
      $el.attr("title", binding.value).tooltip("fixTitle");
      var data = $el.data("bs.tooltip");
      if (
        data.inState.hover ||
        data.inState.focus ||
        data.inState.click ||
        binding.modifiers.manual
      )
        $el.tooltip("show");
    } else {
      $el.tooltip("hide");
    }
  },
  unbind: function(el) {
    $(el).tooltip("destroy");
  }
});
// #endregion </kb-tooltip>

//#region <kb-upload>
Vue.directive("kb-upload", {
  bind: function(element, binding) {
    var config = binding.value;
    config.allowMultiple
      ? $(element).attr("multiple", true)
      : $(element).removeAttr("multiple");

    if (config.acceptTypes && config.acceptTypes.length) {
      $(element).attr("accept", config.acceptTypes.join(","));
    }

    $(element).change(function() {
      var files = this.files,
        len = files.length,
        acceptableFilesLength = 0;

      var availableFiles = [];

      if (len) {
        var data = new FormData();

        var errors = {
          size: [],
          type: [],
          suffix: []
        };

        _.forEach(files, function(file, idx) {
          var fileName = file.name;

          if (!config.acceptSuffix || !config.acceptSuffix.length) {
            if (!config.acceptTypes || !config.acceptTypes.length) {
              alert("Upload failed: please init the acceptType first.");
            } else {
              if (
                config.acceptTypes.indexOf(file.type) > -1 ||
                config.acceptTypes.indexOf("*/*") > -1
              ) {
                if (file.size) {
                  data.append("file_" + idx, file);
                  availableFiles.push(file);
                  acceptableFilesLength++;
                } else {
                  errors.size.push(file.name);
                }
              } else {
                errors.type.push(file.name);
              }
            }
          } else {
            if (fileName.indexOf(".") > -1) {
              var suffix = fileName
                .split(".")
                .reverse()[0]
                .toLowerCase();

              if (config.acceptSuffix.indexOf(suffix) > -1) {
                if (file.size) {
                  data.append("file_" + idx, file);
                  availableFiles.push(file);
                  acceptableFilesLength++;
                } else {
                  errors.size.push(file.name);
                }
              } else {
                errors.suffix.push(file.name);
              }
            }
          }
        });

        config.callback(data, availableFiles);
        resetValue(element);

        var errorString = getErrorString();
        errorString && alert(errorString);

        function getErrorString() {
          var string = "";
          if (errors.size.length) {
            string +=
              Kooboo.text.common.File +
              " " +
              errors.size.join(", ") +
              " " +
              Kooboo.text.alert.fileUpload.emptyFile +
              "\n";
          }
          if (errors.type.length) {
            string +=
              Kooboo.text.common.File +
              " " +
              errors.type.join(", ") +
              " " +
              Kooboo.text.alert.fileUpload.invalidSuffix +
              "\n";
          }
          if (errors.suffix.length) {
            string +=
              Kooboo.text.common.File +
              " " +
              errors.suffix.join(", ") +
              " " +
              Kooboo.text.alert.fileUpload.invalidType +
              "\n";
          }
          return string;
        }
      }
    });

    function resetValue(el) {
      $(el)
        .wrap("<form>")
        .parent("form")
        .trigger("reset");
      $(el).unwrap();
    }
  }
});
//#endregion </kb-upload>

//#region <kb-modal>
Vue.directive("kb-modal", {
  bind: function(element, binding, vnode) {
    $(element).on("hidden.bs.modal", function() {
      vnode.context[binding.expression] = false; // sync binding.value = false;
      if ($("body").find(".modal.in").length) {
        if (!$("body").hasClass("modal-open")) {
          $("body").addClass("modal-open");
        }
      }
    });
    $(element).on("show.bs.modal", function() {
      setTimeout(function() {
        if (
          $(element).hasClass("media-dialog") ||
          $(element).hasClass("category-dialog")
        ) {
          $(".modal-backdrop:last").css("z-index", 200001);
        }
      }, 80);
    });
    $(element).on("shown.bs.modal", function() {
      Kooboo.EventBus.publish("ko/binding/modal/shown", {
        elem: element
      });
    });
  },
  update: function(element, binding) {
    $(element).modal(binding.value ? "show" : "hide");
  }
});
//#endregion </kb-modal>

//#region <kb-html>
Vue.directive("kb-html", {
  inserted: function(element, binding) {
    $(element).html(binding.value);
  },
  componentUpdated: function(element, binding) {
    if (binding.value !== binding.oldValue) {
      $(element).html(binding.value);
    }
  }
});
//#endregion </kb-html>

//#region <kb-focus>
Vue.directive("kb-focus", {
  inserted: function(element) {
    element.focus();
  }
});
//#endregion </kb-focus>

//#region <kb-collapsein>
Vue.directive("kb-collapsein", {
  bind: function(element, binding) {
    $(element).addClass("collapse");
    $(element).collapse(binding.value ? "show" : "hide");
  },
  update: function(element, binding) {
    $(element).collapse(binding.value ? "show" : "hide");
  }
});
//#endregion </kb-html>

// #region <kb-richeditor>
(function() {
  var languageManager = Kooboo.LanguageManager;

  var SITE_ID = Kooboo.getQueryString("SiteId"),
    SITE_ID_STRING = "?SiteId=" + SITE_ID;
  Vue.directive("kb-richeditor", {
    bind: function(element, binding, vnode) {
      // Get custom configuration object from the 'wysiwygConfig' binding, more settings here... http://www.tinymce.com/wiki.php/Configuration
      var options = binding.value.editorConfig
          ? binding.value.editorConfig
          : {},
        isMailEditor = binding.value.mailConfig
          ? binding.value.mailConfig
          : false,
        // Set up a minimal default configuration
        defaults = {
          language: languageManager.getTinyMceLanguage(), // params needed
          branding: false,
          plugins:
            "autoresize link textcolor lists codemirror image " +
            (isMailEditor ? "" : "codesample"),
          toolbar:
            "undo redo | " +
            "bold italic forecolor backcolor formatselect | " +
            "indent outdent | " +
            "alignleft aligncenter alignright alignjustify | " +
            "bullist numlist | " +
            "image link | " +
            "code" +
            (isMailEditor ? "" : " codesample"),
          menubar: false,
          statusbar: false,
          remove_script_host: false,
          convert_urls: false,
          extended_valid_elements: "style",
          valid_children: "+body[style]",
          autoresize_min_height: 300,
          autoresize_max_height: 600,
          setup: function(editor) {
            editor.on("change keyup nodechange", function(args) {
              var content = "";
              if (SITE_ID) {
                content = editor
                  .getContent()
                  .split(SITE_ID_STRING)
                  .join("");
              } else {
                content = editor.getContent();
              }
              binding.value.value = content;
              element.value = content;
              Kooboo.trigger(element, "input");
            });

            editor.on("NodeChange", function(e) {
              // if (e && e.element.nodeName.toLowerCase() == 'img') {
              //     tinyMCE.DOM.setAttribs(e.element, { 'width': null, 'height': null });
              // }
            });

            editor.on("init", function(e) {
              Kooboo.EventBus.publish("kb/tinymce/initiated", editor);
            });
          },
          codesample_dialog_height: 500,
          codesample_languages: [
            { text: "HTML/XML", value: "markup" },
            { text: "JavaScript", value: "javascript" },
            { text: "CSS", value: "css" },
            { text: "PHP", value: "php" },
            { text: "Ruby", value: "ruby" },
            { text: "Python", value: "python" },
            { text: "Java", value: "java" },
            { text: "C", value: "c" },
            { text: "C#", value: "csharp" },
            { text: "C++", value: "cpp" }
          ],
          content_style:
            "html { overflow-x: hidden; } .mce-content-body img { max-width: 100%; height: auto; }",
          verify_html: false,
          file_browser_callback_types: "image",
          codemirror: {
            indentOnInit: true, // Whether or not to indent code on init.
            path: "/_Admin/Scripts/lib/codemirror", // Path to CodeMirror distribution
            config: {
              // CodeMirror config object
              mode: "htmlmixed",
              lineNumbers: true,
              indentUnit: 4,
              tabSize: 4
            },
            width: 800, // Default value is 800
            height: 400, // Default value is 550
            // saveCursorPosition: true, // Insert caret marker
            jsFiles: [
              // Additional JS files to load
              "mode/htmlmixed/htmlmixed"
            ]
          }
        };

      if (!isMailEditor) {
        defaults.file_browser_callback = function(
          field_name,
          url,
          type,
          win,
          isPicked
        ) {
          if (!isPicked) {
            Kooboo.EventBus.publish("ko/style/list/pickimage/show", {
              settings: tinymce.editors[0].settings,
              field_name: field_name,
              type: type,
              win: win,
              from: "RICHEDITOR"
            });
          } else {
            win.document.getElementById(field_name).value = url;
          }
        };
      } else {
        defaults.file_browser_callback = function(field_name, url, type) {
          document.getElementById(field_name).value = url;
        };
        defaults.file_picker_types = "image";
        defaults.file_picker_callback = function(cb, value, meta) {
          var input = document.createElement("input");
          input.setAttribute("type", "file");
          input.setAttribute("accept", "image/*");
          input.onchange = function() {
            var files = this.files;
            if (files && files.length) {
              var data = new FormData();
              data.append("fileName", files[0].name);
              data.append("file", files[0]);

              Kooboo.EmailAttachment.ImagePost(data).then(function(res) {
                if (res.success) {
                  cb(res.model);
                }
              });
            }
            $(this).val("");
          };
          input.click();
        };
      }

      // Apply custom configuration over the defaults
      defaults = $.extend(defaults, options);
      // Ensure the valueAccessor's value has been applied to the underlying element, before instanciating the tinymce plugin

      if (binding.value.value) {
        var _tempParent = $("<div>");
        $(_tempParent).append(binding.value.value);
        var imgDoms = $(_tempParent).find("img");
        imgDoms.each(function(idx, el) {
          $(el).attr("src", $(el).attr("src") + SITE_ID_STRING);
        });

        $(element).text($(_tempParent).html());
      }
      // Tinymce will not be able to calculate the textarea height without this delay
      setTimeout(function() {
        if (!element.id) {
          element.id = tinymce.DOM.uniqueId();
        }
        tinyMCE.init(defaults);
        tinymce.execCommand("mceAddEditor", true, element.id);
      }, 50);
    },
    unbind: function(el) {
      el &&
        $(el).attr("id") &&
        tinyMCE.get($(el).attr("id")) &&
        tinyMCE.get($(el).attr("id")).remove();
    }
  });
})();
// #endregion </kb-richeditor>

// #region <kb-sortable>
Vue.directive("kb-sortable", function(el, binding) {
  $(el).sortable({
    handle: ".sortable",
    start: function() {
      var sortables = el.getElementsByClassName("sortable");
      for (var i = 0; i < sortables.length; i++) {
        sortables[i].__data_item = binding.value[i];
      }
    },
    update: function() {
      var newList = [];
      var sortables = document.getElementsByClassName("sortable");
      for (var j = 0; j < sortables.length; j++) {
        newList.push(sortables[j].__data_item);
      }
      binding.value.splice(0, binding.value.length);
      setTimeout(function() {
        newList.forEach(function(item) {
          binding.value.push(item);
        });
      });
    }
  });
});
// #endregion </kb-sortable>

// #region {{ | ellipsis}}
Vue.filter("ellipsis", function(value, len, str) {
  console.log(value);
  if (len && typeof len === "number") {
    if (str && typeof str === "string") {
      return value.substr(0, len - 2) + str;
    } else {
      return value.substr(0, len - 2) + "...";
    }
  } else {
    return value.substr(0, 8 - 2) + "...";
  }
});
// #endregion
// #region {{ | camelCase}}
Vue.filter("camelCase", function(value) {
  return _.camelCase(value);
});
// #endregion

// #region <kb-container>
//动态盒子和插槽，用来插入vnode，或者获取盒子内slot
Vue.component("kb-container", {
  props: {
    tag: {
      type: String
    },
    vNodes: {}
  },
  render: function(h) {
    var vm = this;
    var tag = "div";
    if (vm.tag && typeof vm.tag === "string") {
      tag = vm.tag;
    }
    if (vm.vNodes) {
      return h(tag, [vm.vNodes]);
    } else {
      if (vm.$scopedSlots.default) {
        return h(tag, {}, vm.$scopedSlots.default(""));
      } else {
        return h(tag, { class: "kb-container-bank" }, "");
      }
    }
  },
  mounted: function() {
    //clear kb-container-bank
    var els = document.getElementsByClassName("kb-container-bank");
    for (var i = 0; i < els.length; i++) {
      els[i].parentNode.removeChild(els[i]);
    }
  },
  methods: {
    getSlot: function(name) {
      return this.$slots[name];
    },
    getScopedSlot: function(name) {
      return this.$scopedSlots[name];
    },
    getSlots: function() {
      var vm = this;
      var temp = {};
      var keys = Object.keys(vm.$scopedSlots);
      keys.forEach(function(key) {
        temp[key] = {
          slot: vm.getSlot(key),
          scopedSlot: vm.getScopedSlot(key)
        };
      });
      return temp;
    }
  }
});
// #endregion </kb-container>
