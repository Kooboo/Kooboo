import { TEXT } from "@/common/lang";
import { canJump } from "@/dom/utils";
import context from "@/common/context";
import { getPageId } from "@/kooboo/utils";
import BaseMenuItem from "./BaseMenuItem";
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

  update(): void {
    this.setVisiable(true);
    let { element } = context.lastHoverDomEventArgs;
    if (!canJump(element)) return this.setVisiable(false);
  }

  click() {
    let url = context.lastHoverDomEventArgs.element.getAttribute("href")!.trim();
    if (!url.startsWith("/")) url = "/" + url;
    this.parentMenu.hidden();
    let pageId = getPageId();
    let siteId = getQueryString("SiteId");
    let accessToken = getQueryString("accessToken");
    let query = `?siteId=${siteId}&accessToken=${accessToken}&pageId=${pageId}&pageUrl=${url}`;
    parent.location.href = parent.location.origin + parent.location.pathname + query;
  }
}

function getQueryString(name: string) {
  var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
  var r = parent.location.search.substr(1).match(reg);
  if (r != null) return unescape(r[2]);
  return null;
}
