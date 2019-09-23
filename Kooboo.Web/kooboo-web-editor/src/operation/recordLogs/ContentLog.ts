import { EDITOR_TYPE } from "@/common/constants";
import { ActionType } from "../ActionType";
import { Log } from "./Log";

export class ContentLog extends Log {
  orgNameOrId!: string;
  fieldName!: string;
  value!: string;
  readonly editorType: string = EDITOR_TYPE.content;

  static createCopy(nameOrId: string, orgNameOrId: string) {
    let log = new ContentLog();
    log.action = ActionType.copy;
    log.nameOrId = nameOrId;
    log.orgNameOrId = orgNameOrId;
    return log;
  }

  static createDelete(nameOrId: string) {
    let log = new ContentLog();
    log.action = ActionType.delete;
    log.nameOrId = nameOrId;
    return log;
  }

  static createUpdate(nameOrId: string, fieldName: string, value: string) {
    let log = new ContentLog();
    log.action = ActionType.update;
    log.nameOrId = nameOrId;
    log.fieldName = fieldName;
    log.value = value;
    return log;
  }
}
