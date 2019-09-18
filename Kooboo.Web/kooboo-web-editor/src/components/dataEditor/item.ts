import { createDiv, createButton } from "@/dom/element";

export type dataItem = { item: HTMLElement; el: HTMLElement };

export function createItem(el: HTMLElement, onDelete: (item: dataItem) => void, onCopy: (item: dataItem) => void) {
  let item = createDiv();
  item.classList.add("kb_web_editor_edit_data_item");
  let content = createDiv();
  let deleteBtn = createButton("-");
  deleteBtn.onclick = () => onDelete({ item, el });
  let copyBtn = createButton("+");
  copyBtn.onclick = () => onCopy({ item, el });
  content.innerHTML = el.innerHTML;
  item.appendChild(content);
  item.appendChild(deleteBtn);
  item.appendChild(copyBtn);
  return { item, content };
}
