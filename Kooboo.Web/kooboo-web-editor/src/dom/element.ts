function createElement<K extends keyof HTMLElementTagNameMap>(options: { tagName: K; className?: string; innerText?: string }) {
  let { tagName, className, innerText } = options;
  let el = document.createElement(tagName);
  if (className) el.classList.add(className);
  if (innerText) el.innerText = innerText;
  return el;
}

export const createButton = (text: string) => createElement({ tagName: "div", className: "kb_web_editor_btn", innerText: text });
export const createDiv = () => createElement({ tagName: "div", className: "kb_web_editor_div" });
export const createInput = () => createElement({ tagName: "input", className: "kb_web_editor_input" });
export const createIframe = () => createElement({ tagName: "iframe", className: "kb_web_editor_iframe" });
export const createImg = () => createElement({ tagName: "img" });
export const createP = () => createElement({ tagName: "p" });

export function createPrimaryButton(text: string) {
  let el = createButton(text);
  el.classList.add("kb_web_editor_btn_primary");
  return el;
}

export function createLabelInput(text: string, labelWidth?: number) {
  let el = createDiv();
  el.classList.add("kb_web_editor_label_input");
  let label = createDiv();
  label.innerText = text;
  let inputContainer = createDiv();
  let input = createInput();
  inputContainer.appendChild(input);
  if (labelWidth != undefined) label.style.width = labelWidth + "px";
  if (labelWidth != undefined) inputContainer.style.left = labelWidth + 5 + "px";
  el.appendChild(label);
  el.appendChild(inputContainer);
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

export function createCheckboxInput(label: string, checked: boolean, onChecked: (checked: boolean) => void) {
  let el = createDiv();
  el.classList.add("kb_web_editor_checkobx_input");
  let checkbox = createInput();
  checkbox.type = "checkbox";
  checkbox.oninput = () => onChecked(checkbox.checked);
  el.appendChild(checkbox);
  let text = createDiv();
  text.innerText = label;
  checkbox.checked = checked;
  el.appendChild(text);
  return {
    checkbox: el,
    setChecked: (checked: boolean) => (checkbox.checked = checked)
  };
}
