import { HOVER_BORDER_SKIP, STANDARD_Z_INDEX } from "../../constants";

export function createContainer(document: Document) {
  let container = document.createElement("div");
  container.style.position = "absolute";
  container.style.width = "150px";
  container.style.borderRadius = "3px";
  container.style.fontSize = "12px";
  container.style.opacity = "0.9";
  container.style.overflow = "hidden";
  container.style.boxShadow = "0 0 3px #fff";
  container.style.backgroundColor = "#fff";
  container.classList.add(HOVER_BORDER_SKIP);
  container.style.zIndex = STANDARD_Z_INDEX + 1 + "";
  return container;
}
