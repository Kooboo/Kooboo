import { getAllElement, isLink } from "@/dom/utils";

export function delay(time: number) {
  return new Promise(rs => {
    setTimeout(rs, time);
  });
}

export function stopLinkElementClick() {
  for (const iterator of getAllElement(document.body)) {
    if (iterator instanceof HTMLElement && isLink(iterator)) {
      let a = iterator.cloneNode(true);
      (a as any)._a = iterator;
      iterator.parentElement!.replaceChild(a, iterator);
    }
  }
}
