import { createModal } from "../modal";
import { TEXT } from "@/common/lang";
import { createDiv } from "@/dom/element";
import context from "@/common/context";
import { createImg } from "./img";
import { createColor } from "./color";
import { createFont } from "./font";
import { createSize } from "./size";
import { getCssRules } from "@/dom/style";
import { Log } from "@/operation/Log";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { getScopeComment } from "../floatMenu/utils";
import { KOOBOO_ID } from "@/common/constants";
import { kvInfo } from "@/common/kvInfo";

export function createStyleEditor(el: HTMLElement) {
  const container = createDiv();
  let rules = getCssRules();
  let comments = KoobooComment.getComments(el);
  let comment = getScopeComment(comments)!;

  const spliter = createSpliter(TEXT.BACKGROUND_IMAGE);
  spliter.style.margin = "0 0 15px 0";
  container.appendChild(spliter);
  const img = createImg(el, rules);
  container.appendChild(img.el);

  container.appendChild(createSpliter(TEXT.COLOR));
  const color = createColor(el, rules);
  container.appendChild(color.el);

  container.appendChild(createSpliter(TEXT.FONT));
  const font = createFont(el, rules);
  container.appendChild(font.el);

  container.appendChild(createSpliter(TEXT.SIZE));
  const size = createSize(el, rules);
  container.appendChild(size.el);

  const { modal, setOkHandler, setCancelHandler, close } = createModal(TEXT.EDIT_STYLE, container, "450px");
  context.container.appendChild(modal);

  return new Promise<Log[]>((rs, rj) => {
    setOkHandler(() => {
      let logs = [...img.getLogs(), ...color.getLogs(), ...font.getLogs(), ...size.getLogs()]
        .filter(f => f)
        .map(m => {
          return new Log([...m!, ...comment.infos, kvInfo.koobooId(el.getAttribute(KOOBOO_ID))]);
        });

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
