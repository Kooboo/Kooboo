import { createInput } from "@/dom/element";

export function createOutLinkPanel() {
  let el = createInput();
  el.style.width = "100%";
  el.placeholder = "http://";
  return {
    outLinkPanel: el,
    setContent: (content: string) => (el.value = content),
    getContent: () => el.value
  };
}
