import { createModal } from "../modal";
import { TEXT } from "@/common/lang";
import { getEditorContainer } from "@/dom/utils";
import { createSpliter } from "./spliter";
import { createImgPreview } from "@/dom/img";
import { createButton } from "@/dom/button";
import { createColorPicker } from "./colorPicker";
import { createLabelInput } from "@/dom/input";

export function createStyleEditor(el: HTMLElement) {
  const container = document.createElement("div");
  addImg(container, el);
  addColor(container, el);
  addFont(container, el);
  const { modal, setOkHandler, setCancelHandler, close } = createModal(
    TEXT.EDIT_STYLE,
    container,
    "450px"
  );

  getEditorContainer().appendChild(modal);
}

function addImg(container: HTMLElement, el: HTMLElement) {
  const spliter = createSpliter("背景图片");
  spliter.style.margin = "-10px 0 15px 0";
  container.appendChild(spliter);
  const { imagePreview, setImage } = createImgPreview();
  imagePreview.style.marginLeft = "auto";
  imagePreview.style.marginRight = "auto";
  container.appendChild(imagePreview);
  const pickImg = createButton("选择图片");
  const removeImg = createButton("移除");
  container.appendChild(pickImg);
  container.appendChild(removeImg);
}

function addColor(container: HTMLElement, el: HTMLElement) {
  container.appendChild(createSpliter("颜色"));
  let style = getComputedStyle(el);
  let bgPicker = createColorPicker(
    "背景颜色",
    style.backgroundColor!,
    e => (el.style.backgroundColor = e)
  );
  container.appendChild(bgPicker);

  let frontPicker = createColorPicker(
    "前景颜色",
    style.color!,
    e => (el.style.color = e)
  );
  container.appendChild(frontPicker);
}

function addFont(container: HTMLElement, el: HTMLElement) {
  container.appendChild(createSpliter("字体"));
  let size = createLabelInput("字体大小", 80, 120);
  container.appendChild(size.input);
  let weight = createLabelInput("字体重量", 80, 120);
  container.appendChild(weight.input);
  let family = createLabelInput("字体风格", 80, 320);
  container.appendChild(family.input);
}
