export function createBlank(document: Document) {
  let blank = document.createElement("div");
  blank.style.height = 10 + "px";

  return blank;
}
