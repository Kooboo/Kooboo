import { createModal } from "../modal";
import { TEXT } from "@/common/lang";
import { getEditorContainer } from "@/dom/utils";
import { createSpliter } from "./spliter";
import { createImagePreview } from "@/components/common/imagePreview";
import { createColorPicker } from "./colorPicker";
import { createLabelInput } from "@/dom/input";
import { pickImg } from "@/kooboo/outsideInterfaces";

export function createStyleEditor(el: HTMLElement) {
  const container = document.createElement("div");
  addImg(container, el);
  addColor(container, el);
  addFont(container, el);

  const { modal, setOkHandler, setCancelHandler, close } = createModal(TEXT.EDIT_STYLE, container, "450px");

  getEditorContainer().appendChild(modal);

  return new Promise<void>((rs, rj) => {
    setOkHandler(() => {
      rs();
      close();
    });
    setCancelHandler(() => {
      rj();
      close();
    });
  });
}

function addImg(container: HTMLElement, el: HTMLElement) {
  const spliter = createSpliter("背景图片");
  spliter.style.margin = "-10px 0 15px 0";
  container.appendChild(spliter);
  const { imagePreview, setImage } = createImagePreview(true, () => (el.style.backgroundImage = ""));
  imagePreview.style.marginLeft = "auto";
  imagePreview.style.marginRight = "auto";
  imagePreview.style.marginBottom = "15px";
  container.appendChild(imagePreview);
  let style = getComputedStyle(el);
  if (style.backgroundImage) setImage(style.backgroundImage);
  imagePreview.onclick = () => {
    pickImg(path => {
      el.style.backgroundImage = `url('${path}')`;
      setImage(path);
    });
  };
}

function addColor(container: HTMLElement, el: HTMLElement) {
  container.appendChild(createSpliter("颜色"));
  let style = getComputedStyle(el);
  let bgPicker = createColorPicker("背景颜色", style.backgroundColor!, e => (el.style.backgroundColor = e));
  container.appendChild(bgPicker);

  let frontPicker = createColorPicker("前景颜色", style.color!, e => (el.style.color = e));
  container.appendChild(frontPicker);
}

function addFont(container: HTMLElement, el: HTMLElement) {
  container.appendChild(createSpliter("字体"));
  let style = getComputedStyle(el);
  let size = createLabelInput("字体大小", 80, 120);
  size.setContent(style.fontSize!);
  size.setInputHandler(content => {
    el.style.fontSize = (content.target! as HTMLInputElement).value;
  });
  container.appendChild(size.input);
  let weight = createLabelInput("字体重量", 80, 120);
  weight.setContent(style.fontWeight!);
  weight.setInputHandler(content => {
    el.style.fontWeight = (content.target! as HTMLInputElement).value;
  });
  container.appendChild(weight.input);
  let family = createLabelInput("字体风格", 80, 320);
  family.setContent(style.fontFamily!);
  family.setInputHandler(content => {
    el.style.fontFamily = (content.target! as HTMLInputElement).value;
  });
  container.appendChild(family.input);
}
