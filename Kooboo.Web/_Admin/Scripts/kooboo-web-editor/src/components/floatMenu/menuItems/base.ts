export function createBaseItem(document: Document) {
  let el = document.createElement("div");
  el.style.padding = "5px 10px";
  el.style.color = "#000";
  el.style.borderBottom = "1px solid #ddd";
  el.innerHTML = "编辑";
  return el;
}
