import { HOVER_BORDER_SKIP, STANDARD_Z_INDEX } from "../../common/constants";

export function createContainer(document: Document) {
  let container = document.createElement("div");
  container.style.position = "fixed";
  container.style.width = "60px";
  container.style.top = "60px";
  container.style.left = document.body.scrollWidth - 120 + "px";
  container.classList.add(HOVER_BORDER_SKIP);
  container.style.zIndex = STANDARD_Z_INDEX - 1 + "";
  return container;
}
