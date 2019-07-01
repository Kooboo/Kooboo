import { MenuItem, createItem } from "../basic";
import { TEXT } from "@/common/lang";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import { canJump } from "@/dom/utils";
import context from "@/common/context";
import { getPageId } from "@/common/koobooUtils";
import qs from "query-string";

export function createJumpLinkItem(): MenuItem {
  const { el, setVisiable } = createItem(TEXT.JUMP_LINK, MenuActions.jumpLink);

  const update = () => {
    let args = context.lastHoverDomEventArgs;
    let visiable = true;
    if (!canJump(args.element)) visiable = false;
    setVisiable(visiable);
  };

  el.addEventListener("click", e => {
    let url = context.lastHoverDomEventArgs.element.getAttribute("href")!;
    let pageId = getPageId();
    let parsed = qs.parse(parent.location.search);
    let query = {
      SiteId: parsed["SiteId"],
      accessToken: parsed["accessToken"],
      pageId: pageId,
      pageUrl: url
    };

    parent.location.href =
      parent.location.origin +
      parent.location.pathname +
      "?" +
      qs.stringify(query);
  });

  return {
    el,
    update
  };
}
