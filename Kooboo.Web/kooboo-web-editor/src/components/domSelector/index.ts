import { domHover } from "./hover";
import { domSelected } from "./selected";

export function registerDomSelector(document: Document) {
  domHover(document);
  domSelected(document);
}
