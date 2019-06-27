export function createHeader(title: string, parent: HTMLElement) {
  var el = document.createElement("div");
  el.innerText = title;
  applyStyle(el.style);
  return el;
}

function applyStyle(style: CSSStyleDeclaration) {
  style.fontSize = "20px";
  style.fontWeight = "400";
  style.lineHeight = "1.3";
  style.fontStyle = "normal";
  style.padding = "20px 20px";
}
