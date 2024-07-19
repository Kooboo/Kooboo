import { getElementByTags, getElementParents, isTag } from "@/utils/dom";
import type { Resource } from "./types";
import { useStyleStore } from "@/store/style";
import { useScriptStore } from "@/store/script";
import { useMultilingualStore } from "@/store/multilingual";
import { newGuid } from "@/utils/guid";
import type { Meta } from "@/api/pages/types";

export function getStyles(doc: Document | undefined) {
  const list: Resource[] = [];
  if (!doc) return list;
  const styleStore = useStyleStore();
  const elements = getElementByTags(doc, ["link", "style"]);

  for (const i of elements) {
    if (isTag(i, "link")) {
      const rel = i.getAttribute("rel");
      const href = i.getAttribute("href");
      if (rel === "stylesheet" && href) {
        const found = styleStore.all.find((s) => s.href === href);
        if (found) {
          list.push({
            el: i,
            content: found.name,
            id: found.id,
          });
        }
      }
    }

    if (isTag(i, "style")) {
      list.push({
        el: i,
        content: i.innerHTML,
        id: newGuid(),
      });
    }
  }

  return list;
}

export function getScripts(doc: Document | undefined) {
  const list: Resource[] = [];
  if (!doc) return list;
  const scriptStore = useScriptStore();
  const elements = getElementByTags(doc, ["script"]);
  for (const i of elements) {
    const src = i.getAttribute("src");
    if (src) {
      const found = scriptStore.all.find((s) => s.href === src);
      if (found) {
        list.push({
          el: i,
          content: found.name,
          id: found.id,
          position: getElementPosition(i),
        });
      }
    } else {
      list.push({
        el: i,
        content: i.innerHTML,
        id: newGuid(),
        position: getElementPosition(i),
      });
    }
  }
  return list;
}

export function getMetas(
  doc: Document | undefined,
  oldMetas: Meta[] | undefined
) {
  const list: Meta[] = [];
  if (!doc || !oldMetas) return list;
  const multilingualStore = useMultilingualStore();

  for (const i of getElementByTags(doc, ["meta"])) {
    const charset = i.getAttribute("charset");
    const property = i.getAttribute("property");
    const httpequiv = i.getAttribute("http-equiv");
    const name = i.getAttribute("name");
    const content = i.getAttribute("content");

    if (charset) {
      let meta = oldMetas.find((f) => !!f.charset);
      if (meta) {
        meta.charset = charset;
        meta.el = i;
      } else {
        meta = { charset: charset, el: i };
      }
      list.push(meta);
    }

    if (httpequiv && content) {
      let meta = oldMetas.find((f) => f.httpequiv === httpequiv);
      if (meta) {
        if (!meta.content) meta.content = {};
        meta.el = i;
      } else {
        meta = { httpequiv: httpequiv, content: {}, el: i };
        meta.content![multilingualStore.default] = content;
      }
      list.push(meta);
    }

    if (name && content) {
      let meta = oldMetas.find((f) => f.name === name);
      if (meta) {
        if (!meta.content) meta.content = {};
        meta.el = i;
      } else {
        meta = { name: name, content: {}, el: i };
        meta.content![multilingualStore.default] = content;
      }
      list.push(meta);
    }

    if (property && content) {
      let meta = oldMetas.find((f) => f.property === property);
      if (meta) {
        if (!meta.content) meta.content = {};
        meta.el = i;
      } else {
        meta = { property: property, content: {}, el: i };
        meta.content![multilingualStore.default] = content;
      }
      list.push(meta);
    }
  }

  return list;
}

function getElementPosition(el: HTMLElement) {
  if (getElementParents(el).some((s) => isTag(s, "head"))) {
    return "head";
  } else return "body";
}
