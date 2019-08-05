import { TEXT } from "@/common/lang";
import { canJump } from "@/dom/utils";
import context from "@/common/context";
import { getPageId } from "@/kooboo/utils";
import qs from "query-string";
import BaseMenuItem from "./BaseMenuItem";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { Menu } from "../menu";

export default class EnterLinkItem extends BaseMenuItem {
  constructor(parentMenu: Menu) {
    super(parentMenu);

    const { el, setVisiable } = this.createItem(TEXT.ENTER_LINK);
    this.el = el;
    this.el.addEventListener("click", this.click.bind(this));
    this.setVisiable = setVisiable;
  }

  el: HTMLElement;

  setVisiable: (visiable: boolean) => void;

  update(comments: KoobooComment[]): void {
    this.setVisiable(true);
    let args = context.lastHoverDomEventArgs;
    if (!canJump(args.element)) return this.setVisiable(false);
  }

  click() {
    let url = context.lastHoverDomEventArgs.element.getAttribute("href")!;
    this.parentMenu.hidden();
    
    let pageId = getPageId();
    let parsed = qs.parse(parent.location.search);
    let query = {
      SiteId: parsed["SiteId"],
      accessToken: parsed["accessToken"],
      pageId: pageId,
      pageUrl: url
    };

    parent.location.href = parent.location.origin + parent.location.pathname + "?" + qs.stringify(query);
  }
}
