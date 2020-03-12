import { TEXT } from "@/common/lang";
import context from "@/common/context";
import { clearKoobooInfo, markDirty, setGuid, getUnpollutedEl, isDynamicContent } from "@/kooboo/utils";
import { isBody } from "@/dom/utils";
import { setInlineEditor } from "@/components/richEditor";
import { clearContent, getEditableComment, isEditable } from "../utils";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { InnerHtmlUnit } from "@/operation/recordUnits/InnerHtmlUnit";
import { operationRecord } from "@/operation/Record";
import BaseMenuItem from "./BaseMenuItem";
import { Menu } from "../menu";
import { kvInfo } from "@/common/kvInfo";
import { KOOBOO_ID } from "@/common/constants";
import { Log } from "@/operation/Log";

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
    if (!getEditableComment(comments)) return this.setVisiable(false);
    var el = getUnpollutedEl(element);
    if (!el || isDynamicContent(el)) return this.setVisiable(false);
    if (!isEditable(element)) return this.setVisiable(false);
    let aroundComments = KoobooComment.getAroundComments(element);
    if (aroundComments.find(f => f.source == "none" && !f.attribute)) return this.setVisiable(false);
  }

  click() {
    let { element } = context.lastSelectedDomEventArgs;
    this.parentMenu.hidden();

    let startContent = element.innerHTML;
    const onSave = () => {
      if (clearContent(startContent) == clearContent(element.innerHTML)) return;
      let el = getUnpollutedEl(element)!;
      let comments = KoobooComment.getComments(el);
      let comment = getEditableComment(comments)!;
      markDirty(el);
      let guid = setGuid(element);
      let units = [new InnerHtmlUnit(startContent)];
      let log = [...comment.infos, kvInfo.koobooId(el.getAttribute(KOOBOO_ID)), kvInfo.value(clearKoobooInfo(el.innerHTML))];
      let operation = new operationRecord(units, [new Log(log)], guid);
      context.operationManager.add(operation);
    };

    const onCancel = () => {
      element.innerHTML = startContent;
    };

    setInlineEditor({ selector: element, onSave, onCancel });
  }
}
