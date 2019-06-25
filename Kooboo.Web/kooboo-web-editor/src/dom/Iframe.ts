export function createIframe(url: string) {
  let el = document.createElement("iframe");
  el.style.width = "100%";
  el.style.height = "100%";
  el.style.border = "none";
  el.src = url;
  return el;
}
