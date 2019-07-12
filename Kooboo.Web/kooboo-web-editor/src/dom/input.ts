import { FONT_FAMILY } from "./utils";
import createDiv from "./div";

export function createInput() {
  let el = document.createElement("input");
  el.style.borderRadius = "3px";
  el.style.border = "1px solid #ccc";
  el.style.lineHeight = "24px";
  el.style.padding = "5px 4.75px";
  el.style.fontSize = "16px";
  el.style.boxSizing = "border-box";
  el.style.height = "36px";
  el.style.fontFamily = FONT_FAMILY;
  return el;
}

export function createLabelInput(text: string, labelWidth?: number, inputWidth?: number) {
  let el = createDiv();
  el.style.display = "inline-block";
  el.style.margin = "5px 0";
  let label = createDiv();
  label.innerText = text;
  label.style.display = "inline-block";
  let input = createInput();
  label.style.textAlign = "right";
  label.style.padding = "5px";
  if (labelWidth != undefined) label.style.width = labelWidth + "px";
  if (inputWidth != undefined) input.style.width = inputWidth + "px";
  input.style.display = "inline-block";
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
  let radio = createInput();
  radio.type = "radio";
  radio.style.margin = "0 5px";
  radio.style.pointerEvents = "none";
  radio.style.position = "static";
  radio.style.opacity = "1";
  el.appendChild(radio);
  let text = createDiv();
  text.innerText = label;
  text.style.lineHeight = "36px";
  text.style.fontSize = "16px";
  text.style.display = "inline-block";
  el.appendChild(text);
  el.style.width = "100%";
  el.style.lineHeight = "24px";
  el.style.padding = "5px 4.75px";
  el.style.fontSize = "16px";
  el.style.wordWrap = "break-word";
  el.style.display = "flex";
  return {
    radio: el,
    setChecked: (checked: boolean) => (radio.checked = checked),
    getContent: () => label
  };
}
