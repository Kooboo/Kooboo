import { createLabel } from "./label";

export function createInput() {
  let el = document.createElement("input");
  el.style.borderRadius = "3px";
  el.style.border = "1px solid #ccc";
  el.style.lineHeight = "24px";
  el.style.padding = "5px 4.75px";
  el.style.fontSize = "16px";
  return el;
}

export function createLabelInput(
  text: string,
  labelWidth?: number,
  inputWidth?: number
) {
  let el = document.createElement("lable");
  let label = createLabel(text);
  let input = createInput();
  label.style.textAlign = "right";
  label.style.margin = "5px 0";
  if (labelWidth != undefined) label.style.width = labelWidth + "px";
  if (inputWidth != undefined) input.style.width = inputWidth + "px";
  label.style.display = "inline-block";
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
  let el = document.createElement("label");
  let radio = document.createElement("input");
  radio.type = "radio";
  radio.style.margin = "0 5px";
  radio.style.pointerEvents = "none";
  el.appendChild(radio);
  el.appendChild(document.createTextNode(label));
  el.style.width = "100%";
  el.style.lineHeight = "24px";
  el.style.padding = "5px 4.75px";
  el.style.fontSize = "16px";
  el.style.wordWrap = "break-word";
  return {
    radio: el,
    setChecked: (checked: boolean) => (radio.checked = checked),
    getContent: () => label
  };
}
