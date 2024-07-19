import { addonToXml, elementToAddon, layoutToAddon } from "@/utils/page";
import { domParse, domStringify } from "@/utils/dom";
import { ref, toRaw } from "vue";

import type { KeyValue } from "@/global/types";
import type { Layout } from "@/api/layout/types";
import type { Meta } from "./types";
import type { Placeholder } from "@/global/types";
import type { PostPage } from "@/api/pages/types";
import { cloneDeep } from "lodash-es";
import { getDesignTemplate } from "@/api/pages/index";
import { getLayout } from "@/api/layout";
import { getRootStyles } from "./page-styles";
import { isClassic } from "./utils";
import { renderBody } from "./render";
import { renderPage } from "./renderer";
import { usePageStore } from "@/store/page";

const layout = ref<Layout>();
const page = ref<PostPage>();
const template = ref<string>();
const rootMeta = ref<Meta>();
const doc = ref<Document>();
const allPages = ref<KeyValue[]>([]);
const { updatePage, getPage, load } = usePageStore();

function setPageStyle(cssText?: string) {
  let style = doc.value!.head.querySelector("style");
  if (cssText) {
    if (!style) {
      style = document.createElement("style");
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

async function initPageDesigner(id: string) {
  const pages = await load();
  allPages.value = pages.map((it) => {
    return {
      key: it.name,
      value: it.path,
    };
  });
  const postPage = await getPage(id, { type: "Designer" });
  rootMeta.value = JSON.parse(postPage.designConfig);

  if (postPage.layoutId && postPage.layoutName) {
    layout.value = await getLayout(postPage.layoutId, {
      design: "true",
      env: import.meta.env.MODE,
    });
    template.value = layout.value.body;
  } else {
    template.value = await getDesignTemplate();
    doc.value = domParse(postPage.body);
  }
  page.value = postPage;
}

function updateNormalPage(
  postPage: PostPage,
  sections: Record<string, string>,
  globalStyles?: string
) {
  if (!doc.value) {
    throw new Error("document is null");
  }
  doc.value.body.innerHTML = renderBody(sections[""], isClassic());

  setPageStyle(globalStyles);

  postPage.body = domStringify(doc.value);
}

function updateLayoutPage(
  postPage: PostPage,
  sections: Record<string, string>
) {
  if (!layout.value) {
    throw new Error("layout is null");
  }
  postPage.layoutId = layout.value.id;
  postPage.layoutName = layout.value.name;
  const layoutAddon = layoutToAddon(layout.value.name, layout.value.body);
  for (const section of layoutAddon.content as Placeholder[]) {
    const sectionHtml = sections[section.name];
    if (!sectionHtml) {
      continue;
    }

    const sectionEl = document.createElement("div");
    sectionEl.innerHTML = renderBody(sectionHtml, isClassic());
    for (let index = 0; index < sectionEl.children.length; index++) {
      const child = sectionEl.children[index];
      const addon = elementToAddon(child);
      section.addons.push(addon);
    }
  }

  postPage.body = addonToXml(layoutAddon);
}

async function onSave(postPage?: PostPage, classic?: boolean) {
  if (!page.value || !template.value || !rootMeta.value) {
    throw new Error("PageDesigner not initialized yet");
  }
  if (!postPage) {
    postPage = cloneDeep(toRaw(page.value));
    postPage.designConfig = JSON.stringify(rootMeta.value);
  } else {
    rootMeta.value = JSON.parse(postPage.designConfig);
  }
  postPage.type = "Designer";

  const sections = await renderPage(rootMeta.value!, classic ?? false);

  if (postPage.layoutId && postPage.layoutName) {
    updateLayoutPage(postPage, sections);
  } else {
    const globalStyles = getRootStyles(rootMeta.value!.props, classic);
    updateNormalPage(postPage, sections, globalStyles);
  }
  page.value = await updatePage(postPage);
}

export function usePageDesigner() {
  return {
    initPageDesigner,
    page,
    allPages,
    rootMeta,
    template,
    onSave,
  };
}
