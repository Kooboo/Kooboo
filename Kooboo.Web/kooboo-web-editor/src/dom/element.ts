import { TEXT } from "@/common/lang";
import rect from "@/assets/img/transparent.png";

export function createButton(text: string) {
  let el = document.createElement("button");
  el.classList.add("kb_web_editor_btn");
  el.innerText = text;
  return el;
}

export function createPrimaryButton(text: string) {
  let el = createButton(text);
  el.classList.add("kb_web_editor_btn_primary");
  return el;
}

export function createDiv() {
  let el = document.createElement("div");
  el.classList.add("kb_web_editor_div");
  return el;
}

export function createInput() {
  let el = document.createElement("input");
  el.classList.add("kb_web_editor_input");
  return el;
}

export function createLabelInput(text: string, labelWidth?: number, inputWidth?: number) {
  let el = createDiv();
  el.classList.add("kb_web_editor_label_input");
  let label = createDiv();
  label.innerText = text;
  let input = createInput();
  if (labelWidth != undefined) label.style.width = labelWidth + "px";
  if (inputWidth != undefined) input.style.width = inputWidth + "px";
  el.appendChild(label);
  el.appendChild(input);
  type inputHandler = (input: Event) => void;
  return {
    input: el,
    getContent: () => input.value,
    setContent: (c: string) => (input.value = c),
    setInputHandler: (h: inputHandler) => (input.oninput = h)
  };
}

export function createRadioInput(label: string) {
  let el = createDiv();
  el.classList.add("kb_web_editor_radio_input");
  let radio = createInput();
  radio.type = "radio";
  el.appendChild(radio);
  let text = createDiv();
  text.innerText = label;
  el.appendChild(text);
  return {
    radio: el,
    setChecked: (checked: boolean) => (radio.checked = checked),
    getContent: () => label
  };
}

export function createIframe(url: string) {
  let el = document.createElement("iframe");
  el.classList.add("kb_web_editor_iframe");
  el.src = url;
  return el;
}

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
    if (src.indexOf("url(") > -1) preview.style.backgroundImage = src;
    else preview.style.backgroundImage = `url('${src}')`;
  };

  return {
    imagePreview: el,
    setImage
  };
}
