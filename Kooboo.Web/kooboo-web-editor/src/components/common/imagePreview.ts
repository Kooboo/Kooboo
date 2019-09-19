import { createDiv, createButton, createImg } from "@/dom/element";
import { TEXT } from "@/common/lang";
import rect from "@/assets/img/transparent.png";

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
    if (src == "none") preview.src = "";
    else if (src.indexOf("url(") > -1) preview.src = clearWarp(src);
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

export function clearWarp(src: string) {
  let startReg = /^\s*url\s*\(\s*[\',\"]?/i;
  let endReg = /[\',\"]?\)\s*$/i;
  src = src.trim().toLocaleLowerCase();
  if (startReg.test(src)) src = src.replace(startReg, "");
  if (endReg.test(src)) src = src.replace(endReg, "");
  return src;
}
