import { createDiv } from "@/dom/element";

export function createBody(height?: string) {
  let el = createDiv();
  el.classList.add("kb_web_editor_modal_body");
  if (height != undefined) el.style.maxHeight = height;

  type setContent = (content: string | HTMLElement) => void;
  let result: [HTMLElement, setContent] = [
    el,
    c => {
      if (c instanceof HTMLElement) {
        el.appendChild(c);
      } else {
        el.innerHTML = c;
      }
    }
  ];
  return result;
}
