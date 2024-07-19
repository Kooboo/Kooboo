import type { Meta, PostPage } from "@/api/pages/types";
import { computed, ref, triggerRef } from "vue";
import { domParse, domStringify } from "@/utils/dom";
import { getMetas, getScripts, getStyles } from "@/global/document";
import { useRoute, useRouter } from "vue-router";

import type { SortEvent } from "@/global/types";
import { buildMetaElement } from "@/components/html-setting/html-meta";
import { renderBody } from "@/components/visual-editor/render";
import { useMultilingualStore } from "@/store/multilingual";
import { usePageStore } from "@/store/page";
import { useScriptStore } from "@/store/script";
import { useStyleStore } from "@/store/style";

const styleStore = useStyleStore();
const scriptStore = useScriptStore();
const pageStore = usePageStore();
const model = ref<PostPage>();
const doc = ref<Document>();

const preview = computed(() => (doc.value ? domStringify(doc.value) : ""));
const styles = computed(() => getStyles(doc.value));
const scripts = computed(() => getScripts(doc.value));
const metas = computed(() => getMetas(doc.value, model.value?.metas));

function setBody(
  body: string,
  classic?: boolean,
  rootProps?: Record<string, any>
) {
  doc.value!.body.innerHTML = renderBody(body, classic ?? false, rootProps);
}

function setPageStyle(cssText?: string) {
  let style = doc.value!.head.querySelector("#designer-page-style");
  if (cssText) {
    if (!style) {
      style = document.createElement("style");
      style.setAttribute("id", "designer-page-style");
      style.innerHTML = cssText;
      doc.value!.head.appendChild(style);
      return;
    }
    style.innerHTML = cssText;
    return;
  }

  if (style) {
    doc.value!.head.removeChild(style);
  }
}

async function init(id: string, args?: Record<string, string>) {
  const rsp = await pageStore.getPage(id, args);
  rsp.urlPath = rsp.urlPath ?? "";
  rsp.name = rsp.name ?? "";
  model.value = rsp;
  doc.value = domParse(model.value.body);
}

export function useSetting() {
  const router = useRouter();
  const route = useRoute();

  const sortStyle = (e: SortEvent) => {
    const item = styles.value[e.oldIndex];
    const current = styles.value[e.newIndex].el;
    if (current?.parentElement) {
      current.parentElement.insertBefore(
        item.el!,
        e.oldIndex > e.newIndex ? current : current.nextElementSibling
      );
    }
    triggerRef(doc);
  };

  const insertStyle = (e: string) => {
    if (!doc.value) return;
    const found = styleStore.all.find((f) => f.id === e);
    if (found) {
      const el = doc.value.createElement("link");
      el.rel = "stylesheet";
      el.href = found.href;
      doc.value.head.appendChild(el);
      triggerRef(doc);
    }
  };

  const sortScript = (e: SortEvent, position: string) => {
    const list = scripts.value.filter((f) => f.position === position);
    const item = list[e.oldIndex];
    const current = list[e.newIndex].el!;
    if (current.parentElement) {
      current.parentElement.insertBefore(
        item.el!,
        e.oldIndex > e.newIndex ? current : current.nextElementSibling
      );
    }
    triggerRef(doc);
  };

  const insertScript = (id: string, position: string) => {
    if (!doc.value) return;
    const found = scriptStore.all.find((f) => f.id === id);
    if (found) {
      const el = doc.value.createElement("script");
      el.src = found.href;
      if (position === "head") {
        doc.value.head.appendChild(el);
      } else {
        doc.value.body.appendChild(el);
      }

      triggerRef(doc);
    }
  };

  const deleteElement = (e: Element) => {
    if (!doc.value) return;
    e.remove();
    triggerRef(doc);
  };

  const insertElement = (e: Element, before?: Element) => {
    if (!doc.value) return;
    if (before && before.parentElement) {
      before.parentElement.insertBefore(e, before);
    } else {
      doc.value.head.appendChild(e);
    }

    triggerRef(doc);
  };

  const save = async () => {
    if (!model.value || !doc.value) return;
    model.value.body = domStringify(doc.value);
    model.value.metas = metas.value;

    const page = await pageStore.updatePage(model.value);
    router.replace({
      query: {
        ...route.query,
        id: page.id,
      },
    });
    return page;
  };

  const updateTitle = (title: string) => {
    if (!doc.value) return;
    doc.value.title = title;
  };

  const addOrUpdateMeta = (value: Meta) => {
    const multilingualStore = useMultilingualStore();
    const meta = buildMetaElement(value, multilingualStore.default);

    if (value.el && meta) {
      insertElement(meta, value.el);
      deleteElement(value.el);
    } else if (meta && model.value) {
      model.value.metas.push(value);
      insertElement(meta);
    }
  };

  const deleteMeta = (value: Meta) => {
    if (value.el) {
      deleteElement(value.el);
    }
  };

  return {
    init,
    setBody,
    setPageStyle,
    model,
    styles,
    scripts,
    metas,
    preview,
    sortStyle,
    insertStyle,
    sortScript,
    insertScript,
    deleteElement,
    insertElement,
    save,
    updateTitle,
    addOrUpdateMeta,
    deleteMeta,
  };
}
