import { createDiv } from "@/dom/element";

export function createContainer() {
  let container = createDiv();
  container.style.position = "fixed";
  container.style.width = "60px";
  container.style.top = "60px";
  container.style.left = document.body.scrollWidth - 120 + "px";
  return container;
}
