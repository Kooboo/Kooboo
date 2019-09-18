import { createDiv, createButton } from "@/dom/element";
import { TEXT } from "@/common/lang";
import rect from "@/assets/img/transparent.png";

export function createImagePreview(showDeleteBtn: boolean = false, onDelete?: () => void) {
  let el = createDiv();
  el.classList.add("kb_web_editor_image_preview");
  el.style.backgroundImage = `url(${rect})`;
  let preview = createDiv();
  el.appendChild(preview);
  let button = createButton(TEXT.DELETE);
  button.style.visibility = showDeleteBtn ? "visible" : "hidden";
  el.appendChild(button);

  button.onclick = e => {
    preview.style.backgroundImage = "";
    if (onDelete) onDelete();
    e.stopPropagation();
  };

  const setImage = (src: string) => {
    if (src == "none") preview.style.backgroundImage = "none";
    else if (src.indexOf("url(") > -1) preview.style.backgroundImage = src;
    else preview.style.backgroundImage = `url('${src}')`;
  };

  return {
    imagePreview: el,
    setImage
  };
}

export function createPickShade() {
  let el = createDiv();
  el.innerText = TEXT.CLICK_REPLACE;
  el.classList.add("kb_web_editor_pick_shade");
  return el;
}
