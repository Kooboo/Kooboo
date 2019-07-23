import { MenuItem, createItem } from "../basic";
import { TEXT } from "@/common/lang";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import { canJump } from "@/dom/utils";
import context from "@/common/context";
import { getPageId } from "@/kooboo/utils";
import qs from "query-string";

export function createEnterLinkItem(): MenuItem {
  const { el, setVisiable } = createItem(TEXT.ENTER_LINK, MenuActions.enterLink);

  const update = () => {
    setVisiable(true);
    let args = context.lastHoverDomEventArgs;
    if (!canJump(args.element)) return setVisiable(false);
  };

  el.addEventListener("click", () => {
    let url = context.lastHoverDomEventArgs.element.getAttribute("href")!;
    let pageId = getPageId();
    let parsed = qs.parse(parent.location.search);
    let query = {
      SiteId: parsed["SiteId"],
      accessToken: parsed["accessToken"],
      pageId: pageId,
      pageUrl: url
    };

    parent.location.href = parent.location.origin + parent.location.pathname + "?" + qs.stringify(query);
  });

  return {
    el,
    update
  };
}
