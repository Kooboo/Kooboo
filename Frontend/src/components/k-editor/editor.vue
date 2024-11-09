<template>
  <Editor
    ref="editorHtml"
    :key="editorKey"
    v-bind="$attrs"
    :init="editorInit"
  />
  <KMediaDialog
    v-if="visibleDialog"
    v-model="visibleDialog"
    @choose="handleSelectImage"
  />
</template>

<script lang="ts" setup>
import { dark } from "@/composables/dark";
import "tinymce/tinymce";
import "tinymce/plugins/autoresize";
import "tinymce/plugins/autolink";
import "tinymce/plugins/link";
import "tinymce/plugins/textcolor";
import "tinymce/plugins/paste";
import "tinymce/plugins/lists";
import "tinymce/plugins/image";
import "tinymce/plugins/codesample";
import "./plugin/monaco";
import "tinymce/themes/silver";
import "tinymce/icons/default";
import { style as darkStyle } from "./style/dark";
import { style as defaultStyle } from "./style/default";
import Editor from "@tinymce/tinymce-vue";
import KMediaDialog from "@/components/k-media-dialog";
import { computed, ref, useAttrs, watch } from "vue";
import type { MediaFileItem } from "@/api/content/media";
import lang_zh from "./langs/zh_CN.js?url";
import { useAppStore } from "@/store/app";
import { useSiteStore } from "@/store/site";
import { setOverlay, restoreOverlay } from "@/utils/dialog";
import { debounce, pick } from "lodash-es";
import { tinymceFonts, tinymceFontSizes } from "@/global/style";
import { useI18n } from "vue-i18n";
import { getQueryString } from "@/utils/url";

const { t } = useI18n();

const editorKey = ref(0);
const classic = ref(getQueryString("classic") === "true");
const editorHtml = ref();
const appStore = useAppStore();
const siteStore = useSiteStore();
const visibleDialog = ref(false);
type Callback = (imageUrl: string, meta?: Record<string, any>) => void;
const callback = ref<Callback>(() => void 0);
const attrs = useAttrs();

function getToolbarOptions() {
  if (!attrs["toolbar"]) {
    return (
      "undo redo | " +
      "fontselect formatselect fontsizeselect bold italic forecolor backcolor removeformat | " +
      "indent outdent | " +
      "alignleft aligncenter alignright alignjustify | " +
      "bullist numlist | " +
      (attrs["hidden-image"] ? "link | " : "image link | ") +
      (attrs["hidden-code"] ? "" : "codesample | code")
    );
  }

  return attrs["toolbar"];
}

function getFontOptions() {
  return attrs["font_formats"] || tinymceFonts.join(";");
}

function getFontSizeOptions() {
  return attrs["font_size_formats"] || tinymceFontSizes.join(" ");
}

watch(
  () => attrs["toolbar"],
  () => {
    editorKey.value++;
  }
);

const editorSetting = {
  forced_root_block: "div",
  branding: false,
  skin: false,
  content_css: false,
  plugins:
    "autoresize link textcolor lists image monaco codesample paste autolink",
  paste_data_images: attrs["hidden-image"] ? false : true,
  toolbar: getToolbarOptions(),
  menubar: false,
  statusbar: false,
  remove_script_host: false,
  document_base_url: siteStore?.site?.prUrl,
  convert_urls: false,
  extended_valid_elements: "style",
  valid_children: "+body[style]",
  min_height: attrs.min_height || 385,
  max_height: attrs.max_height || 600,
  codesample_dialog_height: 500,
  codesample_languages: [
    { text: "HTML/XML", value: "markup" },
    { text: "JavaScript", value: "javascript" },
    { text: "CSS", value: "css" },
    { text: "C#", value: "csharp" },
  ],
  codesample_global_prismjs: true,
  content_style: dark.value ? darkStyle : defaultStyle,
  formats: {
    removeformat: [
      {
        selector: "h1,h2,h3,h4,h5,h6",
        remove: "none",
        split: false,
        expand: false,
        block_expand: true,
        deep: true,
      },
      {
        selector:
          "b,strong,em,i,font,u,strike,s,sub,sup,dfn,code,samp,kbd,var,cite,mark,q,del,ins,small,table,tbody,tr,td,th,a",
        remove: "all",
        split: true,
        expand: false,
        deep: true,
      },
      {
        selector: "*",
        remove: "none",
      },
    ],
  },
  browser_spellcheck: true, //拼写检查
  // 字体
  font_formats: getFontOptions(),
  fontsize_formats: getFontSizeOptions(),
  file_picker_types: "image",
  file_picker_callback(cb: Callback) {
    if (attrs["manual-upload"]) {
      /* 手动上传图片 并转成base64 */
      const input = document.createElement("input");
      input.type = "file";
      input.accept = "image/*";
      input.addEventListener("change", (e: any) => {
        const file = e.target.files[0]; //这里是抓取到上传的对象。
        if (!file) return false;
        if (!/image\/\w+/.test(file.type)) {
          console.warn("请确保文件为图像类型");
          return false;
        }
        const reader = new FileReader();
        reader.readAsDataURL(file);
        reader.onload = function () {
          cb(this.result as string);
        };
      });
      input.click();
    } else {
      visibleDialog.value = true;
      callback.value = cb;
    }
  },
  monaco: {
    width: 800, // Default value is 800
    height: 400, // Default value is 550
  },
  // 是否链接选项只保留一个新窗口打开的选项
  // 移除富文本Insert/Edit link的Current window选项
  target_list:
    attrs["only-new-window"] || classic.value == true
      ? [{ title: t("common.newWindow"), value: "_blank" }]
      : undefined,
  setup: function (editor: any) {
    const setupFunc = attrs["editor-setup"];
    if (typeof setupFunc === "function") {
      setupFunc(editor);
    }

    editor.on(
      "init",
      debounce(function () {
        editor.addShortcut("meta+s", "", () => {
          const event = new KeyboardEvent("keydown", {
            metaKey: true,
            code: "KeyS",
            key: "s",
            keyCode: 83,
            bubbles: true,
          });
          editorHtml.value?.$el?.dispatchEvent(event);
        });
        setOverlay();
      }, 50)
    );

    editor.on("remove", debounce(restoreOverlay, 50));
  },
};

const editorInit = computed(() => {
  const config: Record<string, any> = editorSetting;
  if (appStore.currentLang === "zh") {
    config.language = "zh_CN";
    config.language_url = lang_zh;
  }

  const additionalButtons = attrs["additional-toolbar-buttons"];
  if (additionalButtons) {
    config.toolbar += " | " + additionalButtons;
  }

  return {
    ...config,
    ...pick(attrs, [
      "forced_root_block",
      "force_br_newlines",
      "force_p_newlines",
      "content_style",
    ]),
  };
});

function handleSelectImage(images: MediaFileItem[]) {
  const image = images[0];
  if (!image) return;
  callback.value(image.url, {
    alt: image.alt ?? image.name,
  });
}
</script>
