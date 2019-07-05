interface ActionButton extends HTMLDivElement {
  changeIcon(icon: any): void;
}

export function createButton(document: Document, icon: any, title: string) {
  let el = document.createElement("div");
  el.style.width = "60px";
  el.style.height = "60px";
  el.style.borderRadius = "50%";
  el.style.backgroundColor = "rgba(255,255,255,0.9)";
  el.style.boxShadow = "0 0 5px rgba(0,0,0,0.5)";
  el.style.position = "relative";
  el.style.marginBottom = "10px";
  el.title = title;
  let img = document.createElement("img");
  img.style.width = "50%";
  img.style.marginLeft = "25%";
  img.style.height = "50%";
  img.style.marginTop = "25%";
  img.src = icon;
  el.appendChild(img);
  let btn = el as ActionButton;
  btn.changeIcon = e => (img.src = e);
  return btn;
}
