import { createModal } from "../modal";
import { TEXT } from "@/common/lang";
import { getElementCssRules } from "@/dom/style";

export function createColorEditor(el: HTMLElement) {
  let rules = getElementCssRules(el);
  console.log(getMatchedCSSRules(el));

  let modal = createModal(TEXT.EDIT_COLOR, "", "600px");
}
