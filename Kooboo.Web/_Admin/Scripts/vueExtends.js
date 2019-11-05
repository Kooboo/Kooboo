Vue.prototype.Kooboo = Kooboo;

// #region <kb-tooltip>
Vue.directive("kb-tooltip", {
  bind: function(el, binding) {
    let trigger = [];
    if (binding.modifiers.focus) trigger.push("focus");
    if (binding.modifiers.hover) trigger.push("hover");
    if (binding.modifiers.click) trigger.push("click");
    if (binding.modifiers.manual) trigger.push("manual");
    trigger = trigger.join(" ");

    $(el).tooltip({
      title: binding.value,
      placement: binding.arg,
      trigger: trigger || "hover",
      html: binding.modifiers.html,
      template: binding.modifiers.error
        ? '<div class="tooltip error" role="tooltip" style="z-index:199999"><div class="tooltip-arrow"></div><div class="tooltip-inner"></div></div>'
        : undefined,
      container: "body"
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
    const $el = $(el);
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
