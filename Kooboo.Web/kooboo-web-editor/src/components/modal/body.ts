import createDiv from "@/dom/div";

export function createBody(height?: string) {
  let el = createDiv();
  el.style.margin = "0 20px";
  el.style.maxHeight = "80%";
  el.style.overflowY = "auto";
  if (height != undefined) el.style.height = height;

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
