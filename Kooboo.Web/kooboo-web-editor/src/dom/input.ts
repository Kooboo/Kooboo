export function createInput() {
  let el = document.createElement("input");
  el.style.width = "100%";
  el.style.borderRadius = "3px";
  el.style.border = "1px solid #ccc";
  el.style.lineHeight = "24px";
  el.style.padding = "5px 4.75px";
  el.style.fontSize = "16px";
  return el;
}

export function createRadioInput(label: string) {
  let el = document.createElement("label");
  let radio = document.createElement("input");
  radio.type = "radio";
  radio.style.margin = "0 5px";
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
