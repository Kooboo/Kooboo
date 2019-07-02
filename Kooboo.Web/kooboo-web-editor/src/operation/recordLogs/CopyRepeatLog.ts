import { Log } from "./Log";
import { ACTION_TYPE, EDITOR_TYPE } from "@/common/constants";
import { KoobooComment } from "@/kooboo/KoobooComment";

export class CopyRepeatLog extends Log {
  constructor(
    public comment: KoobooComment,
    public guid: string,
    public koobooId: string
  ) {
    super(comment, koobooId, document.createElement("div"));
  }
  getCommitObject(): object {
    let result: any = {};

    result.action = ACTION_TYPE.copy;
    result.editorType = EDITOR_TYPE.content;
    result.OrgNameOrId = this.comment.nameorid;
    result.objectType = this.comment.objecttype;
    result.nameOrId = this.guid;

    return result;
  }
}

// "action": "copy",
//       "editorType": "content",
//       "koobooId": "1-0-5-1-1-3-1-1-3",
//       "value": "",
//       "nameOrId": "f725840b-5450-4aba-a65a-a62ba27d37d1",
//       "objectType": "contentrepeater",
//       "OrgNameOrId": "70179bab-c26e-a3b0-2304-35964b303881"
