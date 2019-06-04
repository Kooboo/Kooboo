import { STANDARD_Z_INDEX, HOVER_BORDER_SKIP } from "../constants";
import moveIcon from "../assets/icons/drag-move--fill.svg";

function createButton(document: Document, icon: any) {
  let btn = document.createElement("div");
  btn.style.width = "50px";
  btn.style.height = "50px";
  btn.style.borderRadius = "25px";
  btn.style.backgroundColor = "rgba(255,255,255,0.7)";
  btn.style.boxShadow = "0 0 5px rgba(0,0,0,0.5)";
  let img = document.createElement("img");
  img.style.width = "70%";
  img.style.marginLeft = "15%";
  img.style.height = "70%";
  img.style.marginTop = "15%";
  img.src = icon;
  btn.appendChild(img);
  return btn;
}

export function createActionBar(document: Document) {
  let container = document.createElement("div");
  document.body.appendChild(container);
  container.style.position = "absolute";
  container.style.width = "50px";
  container.style.top = "50px";
  container.style.right = "50px";
  container.classList.add(HOVER_BORDER_SKIP);
  var btn = createButton(document, moveIcon);
  container.appendChild(btn);
  container.style.zIndex = STANDARD_Z_INDEX - 1 + "";
}
