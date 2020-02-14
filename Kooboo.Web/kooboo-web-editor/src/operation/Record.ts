import { getAllNode } from "@/dom/utils";
import { KOOBOO_GUID } from "@/common/constants";
import { Unit } from "./recordUnits/Unit";
import { Log } from "./Log";

export class operationRecord {
  id?: number;
  constructor(public units: Unit[], public logs: Log[], public guid: string) {}

  undo() {
    let node = this.getOperationNode(this.guid);
    if (!node) return;

    for (const unit of this.units) {
      unit.undo(node);
    }
  }

  redo() {
    let node = this.getOperationNode(this.guid);
    if (!node) return;

    for (const unit of this.units) {
      unit.redo(node);
    }
  }

  private getOperationNode(guid: string) {
    for (const node of getAllNode(document.body, true)) {
      if (node instanceof HTMLElement) {
        let attr = node.getAttribute(KOOBOO_GUID);
        if (attr && attr == guid) return node;
      }

      if (node.nodeType == Node.COMMENT_NODE && node.nodeValue && node.nodeValue.indexOf(guid) > -1) {
        return node;
      }
    }
  }
}
