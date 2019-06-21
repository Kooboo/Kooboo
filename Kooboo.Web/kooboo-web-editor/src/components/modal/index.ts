import { createContainer } from "./container";
import { createHeader } from "./header";
import { createFooter } from "./footer";

export interface ModalOptions {
  ok?: () => {};
  cancel?: () => {};
  body: HTMLElement;
  title: string;
}

export function createModal(actions: ModalOptions) {
  const { title, body, cancel, ok } = actions;
  document.body.style.overflow = "hidden";
  let { el, win } = createContainer();
  const recovery = () => {
    document.body.style.overflow = "auto";
    el.parentElement!.removeChild(el);
  };
  win.appendChild(createHeader(title));
  win.appendChild(wrapBody(body));

  win.appendChild(
    createFooter(
      () => {
        recovery();
        if (ok) ok();
      },
      () => {
        recovery();
        if (cancel) cancel();
      }
    )
  );

  return el;
}

function wrapBody(body: HTMLElement) {
  let el = document.createElement("div");
  el.appendChild(body);
  el.style.padding = "20px 0 80px 0";
  el.style.height = "100%";
  return el;
}
