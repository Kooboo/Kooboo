import { Log } from "./Log";
import { ACTION_TYPE, EDITOR_TYPE, OBJECT_TYPE } from "@/common/constants";
import { clearKoobooInfo } from "@/kooboo/koobooUtils";

export class InnerHtmlLog extends Log {
  getCommitObject(): object {
    let result: any = {};

    let editorType = EDITOR_TYPE.dom;
    if (this.comment.objecttype == OBJECT_TYPE.label) {
      editorType = EDITOR_TYPE.label;
    } else if (this.comment.objecttype == OBJECT_TYPE.content) {
      editorType = EDITOR_TYPE.content;
    }

    result.action = ACTION_TYPE.update;
    result.editorType = editorType;
    result.koobooId = this.koobooId;
    result.value = clearKoobooInfo(this.element.innerHTML);
    result.nameOrId = this.comment.nameorid;
    result.objectType = this.comment.objecttype;

    return result;
  }
}
