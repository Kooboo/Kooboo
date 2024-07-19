import { emptyGuid } from "./guid";
import type { Addon, Placeholder } from "../global/types";
import { domParse, getElements } from "./dom";

import { type Model, modelToXml } from "./xml";
import type { RouteLocationRaw } from "vue-router";
import { useRouteSiteId } from "../hooks/use-site-id";

const matchPlaceholder = (i: Element) => {
  const name = i.getAttribute("k-placeholder");
  if (!name) return;

  const placeholder: Placeholder = {
    name: name,
    innerHtml: i.innerHTML,
    addons: [],
  };

  return placeholder;
};

export function elementToAddon(el: Element) {
  const result: Addon = {
    id: "",
    attributes: {},
    content: "",
    type: el.tagName.toLowerCase(),
  };

  for (const attributeName of el.getAttributeNames()) {
    const name = attributeName.toLowerCase();
    result.attributes[name] = el.getAttribute(name)!;
  }

  result.id = result.attributes["id"];

  if (result.type === "layout") {
    const placeholders: Placeholder[] = [];

    for (const i of Array.from(el.children)) {
      if (i.tagName.toLowerCase() !== "placeholder") continue;

      const placeholder: Placeholder = {
        name: i.getAttribute("id")!,
        addons: [],
        innerHtml: "",
      };

      for (const addon of Array.from(i.children)) {
        placeholder.addons.push(elementToAddon(addon));
      }

      placeholders.push(placeholder);
    }

    result.content = placeholders;
  } else {
    result.content = el.innerHTML;
  }

  return result;
}

export function layoutToAddon(name: string, html: string): Addon {
  const layoutDoc = domParse(html);
  const placeholders: Placeholder[] = [];

  for (const i of getElements(layoutDoc)) {
    const placeholder = matchPlaceholder(i);
    if (placeholder) placeholders.push(placeholder);
  }

  return {
    id: name,
    type: "layout",
    attributes: { id: name },
    content: placeholders,
  };
}

function addonToModel(addon: Addon) {
  const model: Model = {
    name: addon.type,
    attributes: addon.attributes,
    children: [],
  };

  if (addon.content && typeof addon.content !== "string") {
    for (const placeholder of addon.content) {
      const placeholderModel = {
        name: "placeholder",
        attributes: { id: placeholder.name },
        children: [] as Model[],
      };

      for (const addon of placeholder.addons) {
        placeholderModel.children!.push(addonToModel(addon));
      }

      (model.children as Model[]).push(placeholderModel);
    }
  } else {
    model.children = addon.content;
  }

  return model;
}

export function addonToXml(addon: Addon) {
  const xml = modelToXml(addonToModel(addon));
  return xml;
}

export function pageToAddon(html: string): Addon {
  const pageDoc = domParse(html);
  const el = pageDoc.body.children[0];
  return elementToAddon(el);
}

export function getDesignerPage(id: string, layout?: string): RouteLocationRaw {
  const layoutId = layout && layout !== emptyGuid ? layout : undefined;
  const routeName = layoutId ? "layout-page-design" : "page-design";
  return useRouteSiteId({
    name: routeName,
    query: {
      id,
      layoutId,
    },
  });
}
