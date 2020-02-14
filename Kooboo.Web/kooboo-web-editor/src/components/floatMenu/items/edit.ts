import { TEXT } from "@/common/lang";
import context from "@/common/context";
import { clearKoobooInfo, markDirty, setGuid, getUnpollutedEl, isDynamicContent } from "@/kooboo/utils";
import { isBody } from "@/dom/utils";
import { setInlineEditor } from "@/components/richEditor";
import { getEditComment, clearContent } from "../utils";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { InnerHtmlUnit } from "@/operation/recordUnits/InnerHtmlUnit";
import { operationRecord } from "@/operation/Record";
import BaseMenuItem from "./BaseMenuItem";
import { Menu } from "../menu";
import { kvInfo } from "@/common/kvInfo";
import { KOOBOO_ID } from "@/common/constants";

export default class EditItem extends BaseMenuItem {
  constructor(parentMenu: Menu) {
    super(parentMenu);

    const { el, setVisiable } = this.createItem(TEXT.EDIT);
    this.el = el;
    this.el.addEventListener("click", this.click.bind(this));
    this.setVisiable = setVisiable;
  }

  el: HTMLElement;

  setVisiable: (visiable: boolean) => void;

  update(comments: KoobooComment[]): void {
    this.setVisiable(true);
    let { element } = context.lastSelectedDomEventArgs;
    if (isBody(element)) return this.setVisiable(false);
    if (!getEditComment(comments)) return this.setVisiable(false);
    if (!getUnpollutedEl(element)) return this.setVisiable(false);
    if (isDynamicContent(element)) return this.setVisiable(false);
  }

  click() {
    let { element } = context.lastSelectedDomEventArgs;
    this.parentMenu.hidden();

    let startContent = element.innerHTML;
    const onSave = () => {
      if (clearContent(startContent) == clearContent(element.innerHTML)) return;
      let el = getUnpollutedEl(element)!;
      let comments = KoobooComment.getComments(element);
      markDirty(el);
      let guid = setGuid(element);
      let units = [new InnerHtmlUnit(startContent)];
      let comment = getEditComment(comments)!;
      let log = [...comment.infos, kvInfo.koobooId(el.getAttribute(KOOBOO_ID)), kvInfo.value(clearKoobooInfo(element.innerHTML))];
      let operation = new operationRecord(units, log, guid);
      context.operationManager.add(operation);
    };

    const onCancel = () => {
      element.innerHTML = startContent;
    };

    setInlineEditor({ selector: element, onSave, onCancel });
  }
}
