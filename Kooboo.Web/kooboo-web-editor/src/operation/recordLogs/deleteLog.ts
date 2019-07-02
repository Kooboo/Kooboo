import { Log } from "./Log";
import { ACTION_TYPE, EDITOR_TYPE } from "@/common/constants";

export class deleteLog extends Log {
  getCommitObject(): object {
    let result: any = {};

    result.action = ACTION_TYPE.delete;
    result.editorType = EDITOR_TYPE.dom;
    result.koobooId = this.koobooId;
    result.nameOrId = this.comment.nameorid;
    result.objectType = this.comment.objecttype;

    return result;
  }
}
