import { TEXT } from "@/common/lang";
import { createPrimaryButton, createButton, createDiv } from "@/dom/element";

export function createFooter() {
  let el = createDiv();
  el.classList.add("kb_web_editor_modal_footer");
  let ok = createPrimaryButton(TEXT.OK);
  let cancel = createButton(TEXT.CANCEL);
  el.appendChild(ok);
  el.appendChild(cancel);
  return { footer: el, ok, cancel };
}
