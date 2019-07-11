import { EDITOR_TYPE, OBJECT_TYPE } from "@/common/constants";
import { ActionType } from "../ActionType";
import { Log } from "./Log";

export class HtmlblockLog extends Log {
  value!: string;
  readonly objectType: string = OBJECT_TYPE.htmlblock;
  readonly editorType: string = EDITOR_TYPE.htmlblock;

  static createDelete(nameOrId: string) {
    let log = new HtmlblockLog();
    log.action = ActionType.delete;
    log.nameOrId = nameOrId;
    return log;
  }

  static createUpdate(nameOrId: string, value: string) {
    let log = new HtmlblockLog();
    log.action = ActionType.update;
    log.nameOrId = nameOrId;
    log.value = value;
    return log;
  }
}
