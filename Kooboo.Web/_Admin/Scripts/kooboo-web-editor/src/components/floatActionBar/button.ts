export function createButton(document: Document, icon: any) {
  let btn = document.createElement("div");
  btn.style.width = "60px";
  btn.style.height = "60px";
  btn.style.borderRadius = "50%";
  btn.style.backgroundColor = "rgba(255,255,255,0.9)";
  btn.style.boxShadow = "0 0 5px rgba(0,0,0,0.5)";
  btn.style.position = "relative";
  let img = document.createElement("img");
  img.style.width = "50%";
  img.style.marginLeft = "25%";
  img.style.height = "50%";
  img.style.marginTop = "25%";
  img.src = icon;
  btn.appendChild(img);
  return btn;
}
