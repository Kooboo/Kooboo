import { EDITOR_TYPE } from "@/common/constants";
import { ActionType } from "../ActionType";
import { Log } from "./Log";

export class DomLog extends Log {
  value!: string;
  objectType!: string;
  koobooId!: string;
  attributeName!: string;
  readonly editorType: string = EDITOR_TYPE.dom;

  static createUpdate(nameOrId: string, value: string, koobooId: string, objectType: string, attributeName: string = "") {
    let log = new DomLog();
    log.action = ActionType.update;
    log.nameOrId = nameOrId;
    log.value = value;
    log.koobooId = koobooId;
    log.objectType = objectType;
    if (attributeName) log.attributeName = attributeName;
    return log;
  }

  static createDelete(nameOrId: string, koobooId: string, objectType: string) {
    let log = new DomLog();
    log.action = ActionType.delete;
    log.nameOrId = nameOrId;
    log.koobooId = koobooId;
    log.objectType = objectType;
    return log;
  }
}
