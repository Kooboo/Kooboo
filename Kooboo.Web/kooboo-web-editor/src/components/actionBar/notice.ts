import createDiv from "@/dom/div";

interface Notice extends HTMLDivElement {
  setCount(count: number): void;
}

export function createNotice(document: Document) {
  let el = createDiv();
  el.style.backgroundColor = "red";
  el.style.width = "20px";
  el.style.height = "20px";
  el.style.borderRadius = "10px";
  el.style.position = "absolute";
  el.style.top = "0";
  el.style.left = "0";
  el.style.color = "#fff";
  el.style.fontSize = "15px";
  el.style.textAlign = "center";
  el.style.lineHeight = "20px";
  el.style.opacity = "0.8";
  el.style.visibility = "hidden";
  let notice = el as Notice;
  notice.setCount = (count: number) => {
    notice.innerHTML = count + "";
    notice.style.visibility = count > 0 ? "visible" : "hidden";
  };
  return notice;
}
