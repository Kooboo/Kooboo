import { createInput } from "@/dom/input";

export function createOutLinkPanel() {
  let el = createInput();
  el.placeholder = "http://";
  return {
    outLinkPanel: el,
    setContent: (content: string) => (el.value = content),
    getContent: () => el.value
  };
}
