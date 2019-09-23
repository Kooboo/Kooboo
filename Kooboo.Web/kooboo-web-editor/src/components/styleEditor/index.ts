import { createModal } from "../modal";
import { TEXT } from "@/common/lang";
import { createDiv } from "@/dom/element";
import context from "@/common/context";
import { createImg } from "./img";
import { createColor } from "./color";
import { createFont } from "./font";
import { createSize } from "./size";
import { Log } from "@/operation/recordLogs/Log";
import { getCssRules } from "@/dom/style";

export function createStyleEditor(el: HTMLElement, nameOrId: string, objectType: string, koobooId: string) {
  const container = createDiv();
  let rules = getCssRules();

  const spliter = createSpliter(TEXT.BACKGROUND_IMAGE);
  spliter.style.margin = "0 0 15px 0";
  container.appendChild(spliter);
  const img = createImg(el, nameOrId, objectType, koobooId, rules);
  container.appendChild(img.el);

  container.appendChild(createSpliter(TEXT.COLOR));
  const color = createColor(el, nameOrId, objectType, koobooId, rules);
  container.appendChild(color.el);

  container.appendChild(createSpliter(TEXT.FONT));
  const font = createFont(el, nameOrId, objectType, koobooId, rules);
  container.appendChild(font.el);

  container.appendChild(createSpliter(TEXT.SIZE));
  const size = createSize(el, nameOrId, objectType, koobooId, rules);
  container.appendChild(size.el);

  const { modal, setOkHandler, setCancelHandler, close } = createModal(TEXT.EDIT_STYLE, container, "450px");
  context.container.appendChild(modal);

  return new Promise<Log[]>((rs, rj) => {
    setOkHandler(() => {
      let logs = [...img.getLogs(), ...color.getLogs(), ...font.getLogs(), ...size.getLogs()].filter(f => f) as Log[];
      rs(logs);
      close();
    });
    setCancelHandler(() => {
      rj();
      close();
    });
  });
}

function createSpliter(text: string) {
  let el = createDiv();
  el.innerText = text;
  el.style.padding = "3px 5px";
  el.style.margin = "5px 0";
  el.style.borderBottom = "1px solid #eee";
  return el;
}
