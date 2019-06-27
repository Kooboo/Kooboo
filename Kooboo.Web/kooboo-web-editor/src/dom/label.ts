export function createLabel(text: string) {
  let el = document.createElement("label");
  el.style.lineHeight = "24px";
  el.style.padding = "5px 4.75px";
  el.innerText = text;
  return el;
}
