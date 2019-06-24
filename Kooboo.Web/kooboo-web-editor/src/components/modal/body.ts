export function createBody() {
  let el = document.createElement("div");
  el.style.padding = "20px 0 80px 0";
  el.style.height = "100%";
  type setContent = (content: string) => void;
  let result: [HTMLElement, setContent] = [el, c => (el.innerHTML = c)];
  return result;
}
