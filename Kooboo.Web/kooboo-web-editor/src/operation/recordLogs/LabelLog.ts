import { EDITOR_TYPE } from "@/common/constants";
import { ActionType } from "../actionType";
import { Log } from "./Log";

export class LabelLog extends Log {
  value!: string;
  objectType!: string;
  readonly editorType: string = EDITOR_TYPE.label;

  static createDelete(nameOrId: string, objectType: string) {
    let log = new LabelLog();
    log.action = ActionType.delete;
    log.nameOrId = nameOrId;
    log.objectType = objectType;
    return log;
  }

  static createUpdate(nameOrId: string, objectType: string, value: string) {
    let log = new LabelLog();
    log.action = ActionType.update;
    log.nameOrId = nameOrId;
    log.value = value;
    log.objectType = objectType;
    return log;
  }
}
