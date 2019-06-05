import { STANDARD_Z_INDEX, HOVER_BORDER_SKIP } from "../constants";
import moveIcon from "../assets/icons/drag-move--fill.svg";
import preIcon from "../assets/icons/shangyibu.svg";
import nextIcon from "../assets/icons/xiayibu.svg";
import saveIcon from "../assets/icons/baocun.svg";
import context from "../context";
import OperationManager from "../OperationManager";

function createButton(document: Document, icon: any) {
  let btn = document.createElement("div");
  btn.style.width = "60px";
  btn.style.height = "60px";
  btn.style.borderRadius = "50%";
  btn.style.backgroundColor = "rgba(255,255,255,0.9)";
  btn.style.boxShadow = "0 0 5px rgba(0,0,0,0.5)";
  let img = document.createElement("img");
  img.style.width = "50%";
  img.style.marginLeft = "25%";
  img.style.height = "50%";
  img.style.marginTop = "25%";
  img.src = icon;
  btn.appendChild(img);
  return btn;
}

function createContainer(document: Document) {
  let container = document.createElement("div");
  document.body.appendChild(container);
  container.style.position = "fixed";
  container.style.width = "60px";
  container.style.top = "60px";
  container.style.left = document.body.scrollWidth - 120 + "px";
  container.classList.add(HOVER_BORDER_SKIP);
  container.style.zIndex = STANDARD_Z_INDEX - 1 + "";
  return container;
}

function createBlank(document: Document) {
  let blank = document.createElement("div");
  blank.style.height = 10 + "px";

  return blank;
}

function initMoveButton(element: HTMLElement, container: HTMLElement) {
  element.draggable = true;

  element.ondragstart = e => {
    context.editing = true;
    context.lastSelectedDomEventArgs = undefined;
  };

  element.ondrag = e => {
    if (e.x == 0 || e.y == 0) return;
    container.style.top = e.y - 25 + "px";
    container.style.left = e.x - 25 + "px";
  };

  element.ondragend = e => {
    context.editing = false;
  };
}

export function createActionBar(document: Document) {
  let container = createContainer(document);

  var moveBtn = createButton(document, moveIcon);
  initMoveButton(moveBtn, container);
  container.appendChild(moveBtn);

  container.appendChild(createBlank(document));

  var preBtn = createButton(document, preIcon);
  preBtn.onclick = () => OperationManager.previous();
  container.appendChild(preBtn);

  container.appendChild(createBlank(document));

  var nextBtn = createButton(document, nextIcon);
  nextBtn.onclick = () => OperationManager.next();
  container.appendChild(nextBtn);

  container.appendChild(createBlank(document));

  var saveBtn = createButton(document, saveIcon);
  container.appendChild(saveBtn);
}
