import { Log } from "./Log";
import { ACTION_TYPE, EDITOR_TYPE } from "@/common/constants";
import { KoobooComment } from "@/kooboo/KoobooComment";

export class DeleteRepeatLog extends Log {
  constructor(public comment: KoobooComment, public guid: string) {
    super(comment, "");
  }

  getCommitObject(): object {
    let result: any = {};

    result.action = ACTION_TYPE.delete;
    result.editorType = EDITOR_TYPE.content;
    result.objectType = this.comment.objecttype;
    result.nameOrId = this.guid;

    return result;
  }
}
