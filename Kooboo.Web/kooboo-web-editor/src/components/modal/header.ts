export function createHeader(title: string) {
  var el = document.createElement("div");
  el.innerText = title;
  applyStyle(el.style);
  return el;
}

function applyStyle(style: CSSStyleDeclaration) {
  style.fontFamily = `BlinkMacSystemFont,"Segoe UI",Roboto,Oxygen-Sans,Ubuntu,Cantarell,"Helvetica Neue",sans-serif`;
  style.fontSize = "20px";
  style.fontWeight = "400";
  style.lineHeight = "1.3";
  style.fontStyle = "normal";
}
