import { isCssRule, isMediaRule } from "@/global/style";

import type { Change } from "../state/change";
import { Completer } from "@/utils/lang";
import type { CssStyle } from "@/global/style";
import GlobalImageDialog from "./global-image-dialog.vue";
import { K_REF } from "../constants";
import type { Operation } from "../state/operation";
import { active } from "../features/edit-image";
import { addHistory } from "../state";
import { doc } from "@/views/inline-design/page";
import { excludeValues } from "@/global/style";
import { getElements } from "@/utils/dom";
import { getStyles } from "@/global/style";
import { isDesignerWidget } from "../binding";
import { newGuid } from "@/utils/guid";
import { ref } from "vue";

const show = ref(false);
let completer: Completer<void>;
const images = ref<ImageBinding[]>();

export interface ImageBinding {
  url: string;
  element?: Element;
  id?: string;
  rule?: CSSStyleRule;
  media?: string;
  file?: string;
  type: "style" | "src" | "inline";
}

const showImageDialog = async () => {
  completer = new Completer<void>();
  images.value = getImages();
  show.value = true;
  return await completer.promise;
};

const close = (success: boolean, operations: Operation<Change>[]) => {
  if (success) {
    if (operations.length) addHistory(operations);
    completer.resolve();
  } else {
    for (const operation of [...operations].reverse()) {
      operation.undo();
    }

    completer.reject();
  }

  show.value = false;
};

function getImages() {
  const result: ImageBinding[] = [];
  const styles = getStyles(doc.value);

  for (const style of styles) {
    const images = getImageFromStyle(style);
    result.push(...images);
  }

  const elements = getElements(doc.value);
  const images = elements.map((m) => getImageFromElement(m)!).filter((f) => f);
  result.push(...images);
  return result;
}

function getImageFromStyle(style: CssStyle, win?: Window) {
  const result: ImageBinding[] = [];

  for (const rule of Array.from(style.rules ?? [])) {
    if (!rule || isDesignerWidget(style.owner)) {
      continue;
    }
    if (isCssRule(rule)) {
      const styleRule = rule as CSSStyleRule;
      const url = styleRule.style.getPropertyValue("background-image");

      if (url && !excludeValues.includes(url) && url.includes("url")) {
        result.push({
          url,
          type: "style",
          element: style.owner!,
          rule: styleRule,
          file: style.url,
        });
      }
    } else if (isMediaRule(rule)) {
      const mediaRule = rule as CSSMediaRule;

      for (const i of Array.from(mediaRule.cssRules ?? [])) {
        if (!isCssRule(i)) continue;
        if (win && !win.matchMedia(mediaRule.media.mediaText).matches) continue;
        const styleRule = i as CSSStyleRule;
        const url = styleRule.style.getPropertyValue("background-image");

        if (url && !excludeValues.includes(url) && url.includes("url")) {
          result.push({
            url,
            type: "style",
            element: style.owner!,
            rule: styleRule,
            file: style.url,
            media: mediaRule.media?.mediaText,
          });
        }
      }
    }
  }

  return result;
}

function getImageFromElement(el: HTMLElement): ImageBinding | undefined {
  if (isDesignerWidget(el)) {
    return;
  }
  const id = newGuid();

  if (active(el)) {
    el.setAttribute(K_REF, id);
    const src = el.getAttribute("src");

    if (src) {
      return {
        type: "src",
        url: src,
        id,
      };
    }
  }

  const url = el.style.getPropertyValue("background-image");

  if (url && url != "none") {
    el.setAttribute(K_REF, id);
    return {
      type: "inline",
      url: url,
      id,
    };
  }
}

export { show, close, images, showImageDialog, GlobalImageDialog };
