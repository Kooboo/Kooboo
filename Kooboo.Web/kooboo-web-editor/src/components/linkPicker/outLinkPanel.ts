import { createInput } from "@/dom/element";

export function createOutLinkPanel() {
  let el = createInput();
  el.style.width = "100%";
  el.placeholder = "http://";
  el.style.marginBottom = "10px";
  return {
    outLinkPanel: el,
    setContent: (content: string) => (el.value = content),
    getContent: () => el.value
  };
}
