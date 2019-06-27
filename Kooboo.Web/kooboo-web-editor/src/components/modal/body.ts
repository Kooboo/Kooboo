export function createBody() {
  let el = document.createElement("div");
  el.style.margin = "0 20px";
  el.style.maxHeight = "80%";

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
