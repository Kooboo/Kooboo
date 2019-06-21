export function createBody() {
  let el = document.createElement("div");
  el.style.padding = "20px 0 80px 0";
  el.style.height = "100%";
  const setContent = (content: string) => (el.innerHTML = content);
  let result: [HTMLElement, (content: string) => void] = [el, setContent];
  return result;
}
