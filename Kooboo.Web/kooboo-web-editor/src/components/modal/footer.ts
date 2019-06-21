import { TEXT } from "@/common/lang";

export function createFooter() {
  let el = document.createElement("div");
  el.style.borderTop = "1px solid #ccc";
  el.style.padding = "16px";
  el.style.position = "absolute";
  el.style.left = "0";
  el.style.right = "0";
  el.style.bottom = "0";
  el.style.textAlign = "right";
  let ok = createButton(TEXT.OK);
  ok.style.backgroundColor = "#207ab7";
  ok.style.color = "#fff";
  let cancel = createButton(TEXT.CANCEL);
  el.appendChild(ok);
  el.appendChild(cancel);

  const setOkHandler = (handler: () => void) => (ok.onclick = handler);
  const setCancelHandler = (handler: () => void) => (cancel.onclick = handler);

  return [el, setOkHandler, setCancelHandler] as [
    HTMLElement,
    typeof setOkHandler,
    typeof setCancelHandler
  ];
}

function createButton(text: string) {
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
