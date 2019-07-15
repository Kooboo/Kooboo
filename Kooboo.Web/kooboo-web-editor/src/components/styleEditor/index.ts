import { createModal } from "../modal";
import { TEXT } from "@/common/lang";
import { getEditorContainer } from "@/dom/utils";
import { createSpliter } from "./spliter";
import { createImagePreview } from "@/components/common/imagePreview";
import { createColorPicker } from "./colorPicker";
import { createLabelInput } from "@/dom/input";
import { pickImg } from "@/kooboo/outsideInterfaces";
import createDiv from "@/dom/div";

export function createStyleEditor(el: HTMLElement) {
  const container = createDiv();
  addImg(container, el);
  addColor(container, el);
  addSize(container, el);
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
  const spliter = createSpliter(TEXT.BACKGROUND_IMAGE);
  spliter.style.margin = "0 0 15px 0";
  container.appendChild(spliter);
  const { imagePreview, setImage } = createImagePreview(true, () => (el.style.backgroundImage = "none"));
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
  container.appendChild(createSpliter(TEXT.COLOR));
  let style = getComputedStyle(el);
  let bgPicker = createColorPicker(TEXT.BACKGROUND_COLOR, style.backgroundColor!, e => (el.style.backgroundColor = e));
  container.appendChild(bgPicker);

  let frontPicker = createColorPicker(TEXT.COLOR, style.color!, e => (el.style.color = e));
  container.appendChild(frontPicker);
}

function addFont(container: HTMLElement, el: HTMLElement) {
  container.appendChild(createSpliter(TEXT.FONT));
  let style = getComputedStyle(el);
  let size = createLabelInput(TEXT.FONT_SIZE, 80, 120);
  size.setContent(style.fontSize!);
  size.setInputHandler(content => {
    el.style.fontSize = (content.target! as HTMLInputElement).value;
  });
  container.appendChild(size.input);
  let weight = createLabelInput(TEXT.FONT_WEIGHT, 80, 120);
  weight.setContent(style.fontWeight!);
  weight.setInputHandler(content => {
    el.style.fontWeight = (content.target! as HTMLInputElement).value;
  });
  container.appendChild(weight.input);
  let family = createLabelInput(TEXT.FONT_FAMILY, 80, 320);
  family.setContent(style.fontFamily!);
  family.setInputHandler(content => {
    el.style.fontFamily = (content.target! as HTMLInputElement).value;
  });
  container.appendChild(family.input);
}

function addSize(container: HTMLElement, el: HTMLElement) {
  container.appendChild(createSpliter(TEXT.SIZE));
  let style = getComputedStyle(el);

  let width = createLabelInput(TEXT.WIDTH, 80, 120);
  width.setContent(style.width!);
  width.setInputHandler(content => {
    el.style.width = (content.target! as HTMLInputElement).value;
  });
  container.appendChild(width.input);

  let height = createLabelInput(TEXT.HEIGHT, 80, 120);
  height.setContent(style.height!);
  height.setInputHandler(content => {
    el.style.height = (content.target! as HTMLInputElement).value;
  });
  container.appendChild(height.input);
}
