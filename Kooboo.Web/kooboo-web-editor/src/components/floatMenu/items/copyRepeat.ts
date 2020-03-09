import { TEXT } from "@/common/lang";
import context from "@/common/context";
import { isBody, getAllNode } from "@/dom/utils";
import { getRepeatSourceComment } from "../utils";
import { getWrapDom, getGuidComment } from "@/kooboo/utils";
import { newGuid } from "@/kooboo/outsideInterfaces";
import { CopyRepeatUnit } from "@/operation/recordUnits/CopyRepeatUnit";
import { operationRecord } from "@/operation/Record";
import { KoobooComment } from "@/kooboo/KoobooComment";
import BaseMenuItem from "./BaseMenuItem";
import { Menu } from "../menu";
import { kvInfo } from "@/common/kvInfo";
import { Log } from "@/operation/Log";
import { objectId } from "@/common/objectId";

export default class CopyRepeatItem extends BaseMenuItem {
  constructor(parentMenu: Menu) {
    super(parentMenu);

    const { el, setVisiable } = this.createItem(TEXT.COPY_REPEAT);
    this.el = el;
    this.el.addEventListener("click", this.click.bind(this));
    this.setVisiable = setVisiable;
  }

  el: HTMLElement;

  setVisiable: (visiable: boolean) => void;

  update(): void {
    this.setVisiable(true);
    let { element } = context.lastSelectedDomEventArgs;
    if (isBody(element)) return this.setVisiable(false);
    let { nodes, endNode } = getWrapDom(element, "repeatitem");
    let comments = KoobooComment.getInnerComments(nodes);
    if (!endNode || !getRepeatSourceComment(comments)) return this.setVisiable(false);
  }

  click() {
    let args = context.lastSelectedDomEventArgs;
    this.parentMenu.hidden();

    let { nodes, endNode } = getWrapDom(args.element, "repeatitem");
    let comments = KoobooComment.getInnerComments(nodes);
    let repeatSourceComment = getRepeatSourceComment(comments)!;
    let guid = newGuid() + "_name";

    if (repeatSourceComment.source == "mongo") {
      guid = objectId();
    }

    let copiedNodes = nodes.reverse().map(m => m.cloneNode(true));

    for (const node of copiedNodes) {
      for (const i of getAllNode(node, true)) {
        if (KoobooComment.isComment(i)) {
          let koobooComment = new KoobooComment(i);
          koobooComment.setValue("uid", koobooComment.uid + "_copy");

          if (koobooComment.id == repeatSourceComment.id) {
            koobooComment.setValue("id", guid);
          }

          i.nodeValue = koobooComment.ToComment().nodeValue;
        }
      }
      endNode!.parentElement!.insertBefore(node, endNode!.nextSibling);
    }

    let units = [new CopyRepeatUnit(getGuidComment(guid))];
    let log = new Log([...repeatSourceComment.infos, new kvInfo("new", guid), kvInfo.copy]);
    let operation = new operationRecord(units, [log], guid);
    context.operationManager.add(operation);
  }
}
