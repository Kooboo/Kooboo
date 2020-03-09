Vue.prototype.Kooboo = Kooboo;

// #region <kb-tooltip>
Vue.directive("kb-tooltip", {
  inserted: function(el, binding) {
    var trigger = [];
    if (binding.modifiers.focus) trigger.push("focus");
    if (binding.modifiers.hover) trigger.push("hover");
    if (binding.modifiers.click) trigger.push("click");
    if (binding.modifiers.manual) trigger.push("manual");
    trigger = trigger.join(" ");
    var $el = $(el);
    var zIndex =
      binding.modifiers.error && $el.closest(".modal").length ? 199999 : 20000; // tip in modal

    $el.tooltip({
      title: binding.value,
      placement: binding.arg,
      trigger: trigger || "hover",
      html: binding.modifiers.html,
      template: binding.modifiers.error
        ? '<div class="tooltip error" role="tooltip" style="z-index:' +
          zIndex +
          ';width: max-content;word-wrap: break-word;"><div class="tooltip-arrow"></div><div class="tooltip-inner"></div></div>'
        : '<div class="tooltip" role="tooltip" style="z-index:199999;width: max-content;word-wrap: break-word;"><div class="tooltip-arrow"></div><div class="tooltip-inner"></div></div>',
      container: $el.data("container") || "body"
    });

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
        if (binding.modifiers.error) {
          // fixed error tip position bug in long view
          var bounding = el.getBoundingClientRect();
          if (bounding.top < 0 || bounding.bottom > $(window).height()) {
            el.scrollIntoView();
          }
        }
      setTimeout(function() {
        $el.tooltip("show");
      }, 100);
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

  var doScope = function(html) {
    var str = `.mce-content-body`;
    var scoper = function(css, prefix) {
      var re = new RegExp("([^\r\n,{}]+)(,(?=[^}]*{)|s*{)", "g");
      css = css.replace(re, function(g0, g1, g2) {
        if (
          g1.match(/^\s*(@media|@.*keyframes|to|from|@font-face|1?[0-9]?[0-9])/)
        ) {
          return g1 + g2;
        }

        if (g1.match(/:scope/)) {
          g1 = g1.replace(/([^\s]*):scope/, function(h0, h1) {
            if (h1 === "") {
              return "> *";
            } else {
              return "> " + h1;
            }
          });
        }

        g1 = g1.replace(/^(\s*)/, "$1" + prefix + " ");

        return g1 + g2;
      });

      return css;
    };

    // /* */ | <!-- --> || //
    let commentRegx = /(\/\*(\s|.)*?\*\/|<!--(\s|.)*?-->|\/\/.*)/g;
    let StyleTagRegx = /<style(([\s\S])*?)<\/style>/g;
    var commentList = html.match(commentRegx);
    var noCommentStr = html.replace(commentRegx, "/*KB_COMMENT_HOLDER*/");
    var scopedStr = noCommentStr.replace(StyleTagRegx, function($0) {
      var index1 = $0.indexOf(">") + 1;
      var index2 = $0.lastIndexOf("<");
      var pre = $0.slice(0, index1);
      var suf = $0.slice(index2);
      var styleContent = $0.slice(index1, index2);
      var scopedContent = scoper(styleContent, str);
      return pre + scopedContent + suf;
    });
    if (commentList && commentList.forEach && commentList.length > 0) {
      commentList.forEach(function(item) {
        scopedStr = scopedStr.replace(/\/\*KB_COMMENT_HOLDER\*\//, item);
      });
    }

    return scopedStr;
  };
  var unScope = function(html) {
    var str = `.mce-content-body`;
    html = html.replace(str, "");
    return html;
  };
  //source.html has useful
  window.Kooboo_scoper = { doScope, unScope };

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
          skin_url:
            location.origin +
            "\\_Admin\\Styles\\kooboo-web-editor\\tinymce\\ui\\oxide",
          branding: false,
          plugins:
            "autoresize link textcolor lists monaco image " +
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
              content = unScope(content);
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
          monaco: {
            width: 800, // Default value is 800
            height: 400 // Default value is 550
          }
        };

      if (languageManager.getLang() == "zh") {
        defaults.language = "zh_CN";
        defaults.language_url = `${location.origin}\\_Admin\\Scripts\\kooboo-web-editor\\${defaults.language}.js`;
      }

      if (!isMailEditor) {
        defaults.file_picker_callback = function(callBack) {
          Kooboo.Media.getList().then(function(res) {
            if (res.success) {
              res.model["show"] = true;
              res.model["onAdd"] = function(selected) {
                callBack(
                  selected.url + "?SiteId=" + Kooboo.getQueryString("SiteId")
                );
              };
            }
            binding.value.mediaDialogData = res.model;
          });
        };

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
      if (binding.value.value || element.value) {
        var _tempParent = $("<div>");
        let codeContent = binding.value.value || element.value;
        codeContent = doScope(codeContent);
        $(_tempParent).append(codeContent);
        var imgDoms = $(_tempParent).find("img");
        imgDoms.each(function(idx, el) {
          $(el).attr("src", $(el).attr("src") + SITE_ID_STRING);
        });
        var content = $(_tempParent).html();
        $(element).html(content);

        element.value = content;
        Kooboo.trigger(element, "input");
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
Vue.directive("kb-sortable", function(el, binding, vnode) {
  var $el = $(el);
  var sourceIndex;
  $(el).sortable({
    handle: ".sortable",
    connectWith: $el.data("sort-connect") || false,
    start: function(ev, ui) {
      sourceIndex = $el.children().index(ui.item[0]);
    },
    update: function(ev, ui) {
      var targetIndex = $el.children().index(ui.item[0]);
      if (targetIndex === -1) {
        return;
      }
      var newList = [];
      if (sourceIndex !== undefined) {
        binding.value.forEach(function(item, index) {
          if (index !== sourceIndex) {
            if (index === targetIndex) {
              var sourceItem = binding.value[sourceIndex];
              if (sourceIndex < targetIndex) {
                newList.push(item);
                newList.push(sourceItem);
              } else {
                newList.push(sourceItem);
                newList.push(item);
              }
            } else {
              newList.push(item);
            }
          }
        });
      } else {
        // from drop
        sourceItem = $el.data("__drop_item__");
        // hard code for "head"
        if (sourceItem.hasOwnProperty("head")) {
          sourceItem.head = !sourceItem.head;
        }
        binding.value.forEach(function(item, index) {
          if (targetIndex === index) {
            newList.push(sourceItem);
          }
          newList.push(item);
        });
        if (targetIndex === binding.value.length) {
          newList.push(sourceItem);
        }
      }
      binding.value.splice(0);
      setTimeout(function() {
        newList.forEach(function(item) {
          binding.value.push(item);
        });
        if (vnode.data.on) {
          var afterSortFn = vnode.data.on["after-sort"];
          if (afterSortFn) {
            afterSortFn({
              targetIndex: targetIndex
            });
          }
        }
      });
    },
    remove: function(ev, ui) {
      var removeItem = binding.value[sourceIndex];
      $($el.sortable("option").connectWith).data("__drop_item__", removeItem);
      var newList = [];
      for (var i = 0; i < binding.value.length; i++) {
        if (i != sourceIndex) newList.push(binding.value[i]);
      }
      binding.value.splice(0);
      setTimeout(function() {
        newList.forEach(function(item) {
          binding.value.push(item);
        });
        if (vnode.data.on) {
          var afterRemoveFn = vnode.data.on["after-remove"];
          if (afterRemoveFn) {
            afterRemoveFn(removeItem);
          }
        }
      });
    }
  });
});
// #endregion </kb-sortable>

// #region {{ | ellipsis}}
Vue.filter("ellipsis", function(value, len, str) {
  len = len ? len : 8;
  if (value && value.length > len) {
    return value.substr(0, len) + (str ? str : "…");
  }
  return value;
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

// #region <kb-hint>
Vue.directive("kb-hint", {
  update: function(el, binding) {
    if (!_.isEqual(binding.oldValue, binding.value)) {
      var element = $(el);
      var show = true;
      var tipsOptions = {
        title: "",
        placement: "right",
        trigger: "manual",
        template:
          '<div class="tooltip error" role="tooltip" style="z-index:199999;width: max-content;"><div class="tooltip-arrow"></div><div class="tooltip-inner"></div></div>'
      };
      var errorContainer = element.data("container");
      if (errorContainer) {
        tipsOptions.container = errorContainer;
      }
      try {
        tipsOptions = element.data("bs.tooltip").options;
      } catch (e) {}
      if (binding.arg) {
        tipsOptions.placement = binding.arg;
      }

      switch (typeof binding.value) {
        case "string":
          tipsOptions.title = binding.value;
          break;
        case "object":
          if (binding.value.options) {
            _.assign(tipsOptions, binding.value.options);
          }
          if (binding.value.hasOwnProperty("msg")) {
            if (binding.value.msg) {
              tipsOptions.title = binding.value.msg;
            } else {
              tipsOptions.title = "";
            }
          }
          break;
      }
      element.tooltip(tipsOptions);
      var hidetip = function() {
        element.tooltip("hide");
        if (el.parentNode.classList.contains("has-error")) {
          el.parentNode.classList.remove("has-error");
        }
      };
      var showtip = function() {
        element.tooltip("show");
        if (!el.parentNode.classList.contains("has-error")) {
          el.parentNode.classList.add("has-error");
        }
      };
      if (binding.value.show !== undefined) {
        show = binding.value.show;
      }
      if (!tipsOptions.title || tipsOptions.title === "") {
        show = false;
      } else {
        element.attr("title", tipsOptions.title).tooltip("fixTitle");
      }
      if (show) {
        var bounding = el.getBoundingClientRect();
        if (bounding.top < 0 || bounding.bottom > $(window).height()) {
          el.scrollIntoView();
        }
        showtip();
      } else {
        hidetip();
      }
    }
  },
  unbind: function(el) {
    $(el).tooltip("destroy");
  }
});
// #endregion </kb-hint>

//#region <kb-select2>
Vue.directive("kb-select2", {
  inserted: function(element, binding) {
    $(element)
      .select2({
        tags: true,
        language: {
          noResults: function() {
            return binding.value.noItemTip || "";
          }
        },
        allowClear: true,
        tokenSeparators: [",", " ", ";"],
        data: binding.value.options
      })
      .on("change", function(e) {
        $(element)
          .parent()
          .find(".select2-search__field")
          .val("");

        var selected = [];
        for (var i = 0; i < e.target.selectedOptions.length; i++) {
          selected.push({
            index: e.target.selectedOptions[i].index,
            text: e.target.selectedOptions[i].text
          });
        }
        binding.value.selected = selected;
      })
      .on("select2:closing", function() {
        var possibleValue = $(element)
          .parent()
          .find(".select2-search__field")
          .val();

        if (possibleValue) {
          if (possibleValue.indexOf(" ") == -1) {
            var origValues = $(element).val() || [];
            if (origValues.indexOf(possibleValue) == -1) {
              origValues.push(possibleValue);
              $(element)
                .val(origValues)
                .trigger("change");
            }
          }
        }
      });

    $(element)
      .val(binding.value.selected)
      .trigger("change");
  }
});
//#endregion </kb-select2>

//#region <kb-spectrum>
(function() {
  function _convertColor(tinycolor) {
    if (!tinycolor) {
      return "initial";
    }
    var value = tinycolor.toHexString();
    if (tinycolor._a == 0) {
      value = "transparent";
    } else if (tinycolor._a < 1) {
      value = tinycolor.toRgbString();
    }
    return value;
  }

  function replaceColor(value, newColor) {
    var color = Kooboo.Color.searchString(value);
    var origValueArr = value.split(/\s(?![^()]*\))/),
      origIdx = origValueArr.indexOf(color);
    if (origIdx > -1) {
      origValueArr[origIdx] = newColor;
    } else {
      origValueArr.splice(0, 0, newColor);
    }
    return origValueArr.join(" ");
  }

  function initSpectrum(element, color) {
    $(element).spectrum({
      color: color,
      preferredFormat: "hex",
      showInput: true,
      localStorageKey: "Color_Editor_ColorItem",
      showButtons: true,
      showAlpha: true,
      showPalette: true,
      allowEmpty: true,
      cancelText: Kooboo.text.common.cancel,
      chooseText: "OK",
      palette: [
        ["#000", "#444", "#666", "#999", "#ccc", "#eee", "#f3f3f3", "#fff"],
        ["#f00", "#f90", "#ff0", "#0f0", "#0ff", "#00f", "#90f", "#f0f"],
        [
          "#f4cccc",
          "#fce5cd",
          "#fff2cc",
          "#d9ead3",
          "#d0e0e3",
          "#cfe2f3",
          "#d9d2e9",
          "#ead1dc"
        ],
        [
          "#ea9999",
          "#f9cb9c",
          "#ffe599",
          "#b6d7a8",
          "#a2c4c9",
          "#9fc5e8",
          "#b4a7d6",
          "#d5a6bd"
        ],
        [
          "#e06666",
          "#f6b26b",
          "#ffd966",
          "#93c47d",
          "#76a5af",
          "#6fa8dc",
          "#8e7cc3",
          "#c27ba0"
        ],
        [
          "#c00",
          "#e69138",
          "#f1c232",
          "#6aa84f",
          "#45818e",
          "#3d85c6",
          "#674ea7",
          "#a64d79"
        ],
        [
          "#900",
          "#b45f06",
          "#bf9000",
          "#38761d",
          "#134f5c",
          "#0b5394",
          "#351c75",
          "#741b47"
        ],
        [
          "#600",
          "#783f04",
          "#7f6000",
          "#274e13",
          "#0c343d",
          "#073763",
          "#20124d",
          "#4c1130"
        ]
      ],
      okClick: function() {
        element.__oldColor = $(element).spectrum("get");
      },
      show: function() {
        element.__oldColor = $(element).spectrum("get");
      },
      hide: function() {
        element.value = replaceColor(
          element.__bindingValue,
          element.__oldColor
        );
        $(element).spectrum("set", element.__oldColor);
        Kooboo.trigger(element, "input");
      }
    });
  }

  Vue.directive("kb-spectrum", {
    inserted: function(element, binding) {
      var color = Kooboo.Color.searchString(binding.value);
      element.__bindingValue = binding.value;
      initSpectrum(element, color);
    },
    update: function(element, binding) {
      var color = Kooboo.Color.searchString(binding.value);
      element.__bindingValue = binding.value;
      $(element).spectrum("set", color);
    },
    unbind: function(element) {
      $(element).spectrum("destroy");
    }
  });
})();

//#endregion </kb-spectrum>
