import { createDiv, createButton, createImg } from "@/dom/element";
import { TEXT } from "@/common/lang";
import rect from "@/assets/img/transparent.png";
import { clearCssImageWarp } from "@/dom/utils";

export function createImagePreview(showDeleteBtn: boolean = false, onDelete?: () => void) {
  let el = createDiv();
  el.classList.add("kb_web_editor_image_preview");
  el.style.backgroundImage = `url(${rect})`;
  let preview = createImg();
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
    src = clearCssImageWarp(src);
    if (src == "none") (preview.src as any) = "";
    else preview.src = src;
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
