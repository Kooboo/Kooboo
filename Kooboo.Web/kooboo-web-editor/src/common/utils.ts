import { getAllElement, isLink, isInEditorContainer } from "@/dom/utils";
import context from "./context";
import { SelectedDomEventArgs } from "@/events/SelectedDomEvent";

export function delay(time: number) {
  return new Promise(rs => {
    setTimeout(rs, time);
  });
}

export function setElementClick() {
  for (const i of getAllElement(document.body)) {
    if (i instanceof HTMLElement) {
      if (isLink(i)) {
        let a = i.cloneNode(true);
        (a as any)._a = i;
        i.parentElement!.replaceChild(a, i);
      } else {
        i.onclick = e => {
          if (e.isTrusted) {
            e.stopPropagation();
            e.preventDefault();
          }

          if (context.editing || isInEditorContainer(e)) return;
          let element = context.lastHoverDomEventArgs.closeElement;
          var args = new SelectedDomEventArgs(element);
          context.lastMouseEventArg = e;
          context.domChangeEvent.emit(args);
        };
      }
    }
  }
}
