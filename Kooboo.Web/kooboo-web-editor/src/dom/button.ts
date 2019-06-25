export function createButton(text: string) {
  let el = document.createElement("button");
  el.innerText = text;
  el.style.fontWeight = "700";
  el.style.borderRadius = "3px";
  el.style.fontSize = "16px";
  el.style.lineHeight = "24px";
  el.style.marginLeft = "8px";
  el.style.padding = "4px 16px";
  el.style.outline = "none";
  el.style.backgroundColor = "#f0f0f0";
  el.style.border = "none";
  return el;
}

export function createPrimaryButton(text: string) {
  let el = createButton(text);
  el.style.backgroundColor = "#207ab7";
  el.style.color = "#fff";
  return el;
}
