import { createDiv } from "@/dom/element";

interface Notice extends HTMLDivElement {
  setCount(count: number): void;
}

export function createNotice() {
  let el = createDiv();
  el.classList.add("kb_web_editor_action_bar_notice");
  let notice = el as Notice;
  notice.setCount = (count: number) => {
    notice.innerHTML = count + "";
    notice.style.visibility = count > 0 ? "visible" : "hidden";
  };
  return notice;
}
