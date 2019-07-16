import { TEXT } from "@/common/lang";
import { createPrimaryButton, createButton, createDiv } from "@/dom/element";

export function createFooter() {
  let el = createDiv();
  el.style.borderTop = "1px solid #eee";
  el.style.marginTop = "16px";
  el.style.padding = "16px 26px";
  el.style.textAlign = "right";
  let ok = createPrimaryButton(TEXT.OK);
  let cancel = createButton(TEXT.CANCEL);
  el.appendChild(ok);
  el.appendChild(cancel);

  return { footer: el, ok, cancel };
}
