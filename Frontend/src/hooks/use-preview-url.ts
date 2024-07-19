import { ElForm, ElFormItem, ElInput, ElMessageBox } from "element-plus";
import type { Ref, VNode } from "vue";
import { h, ref } from "vue";
import { openInNewTab, renderTemplate } from "@/utils/url";

import type { Rules } from "async-validator";
import { i18n } from "@/modules/i18n";
import { requiredRule } from "@/utils/validate";

function findParams(url: string) {
  const params = window.decodeURIComponent(url).matchAll(/\{([^\/\}]+)\}/g);
  const results = [];
  for (const item of params) {
    item[1] && results.push(item[1]);
  }
  return results;
}

function renderParamsForm(
  model: Ref<Record<string, string>>,
  t: (k: string) => string
) {
  const keys = Object.keys(model.value);
  const rules = ref<Rules>({});
  keys.forEach((name) => {
    rules.value[name] = [requiredRule(t("common.valueRequiredTips"))];
  });
  const formItems = keys.map((name) => {
    return h(
      ElFormItem,
      {
        prop: name,
        label: name,
      },
      {
        default: () =>
          h(ElInput, {
            modelValue: model.value[name],
            "onUpdate:modelValue"(value: string) {
              model.value[name] = value;
            },
          }),
      }
    );
  });
  return h(
    ElForm,
    {
      model: model,
      rules: rules.value,
      labelPosition: "top",
      class: "el-form--label-normal",
    },
    {
      default: () => formItems,
    }
  );
}

export const onPreviewInjectionFlag = "usePreviewUrl:onPreview";

export type PreviewConfig = {
  title?: string;
};

export type PreviewHandler = (url: string, config?: PreviewConfig) => void;

export function usePreviewUrl(): {
  onPreview: PreviewHandler;
} {
  const t = i18n.global.t;
  function onPreview(url: string, config?: PreviewConfig) {
    const params = findParams(url);
    if (!params.length) {
      openInNewTab(url);
      return;
    }

    const model = ref<Record<string, string>>({});
    params.forEach((name) => {
      model.value[name] = `{${name}}`;
    });

    ElMessageBox({
      title: config?.title ?? t("common.preview"),
      message: renderParamsForm(model, t),
      showCancelButton: true,
      confirmButtonText: t("common.goTo"),
      cancelButtonText: t("common.cancel"),
      roundButton: true,
      customClass: "el-message-box--preview font-family",
      beforeClose: (action, instance, done) => {
        if (action === "confirm") {
          const { component } = instance.message as any as VNode;
          component!.exposed!.validate((isValid: boolean) => {
            if (!isValid) {
              return;
            }
            openInNewTab(
              renderTemplate(window.decodeURIComponent(url), model.value)
            );
            done();
          });
          return;
        }
        done();
      },
    }).catch(() => {
      // preview cancel
    });
  }
  return {
    onPreview,
  };
}
