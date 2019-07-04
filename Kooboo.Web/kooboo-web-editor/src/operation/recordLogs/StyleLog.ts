import { EDITOR_TYPE } from "@/common/constants";
import { ActionType } from "../actionType";
import { Log } from "./Log";

export class StyleLog extends Log {
  value!: string;
  objectType!: string;
  ruleId!: string;
  styleId!: string;
  property!: string;
  selector!: string;
  important!: string;
  StyleSheetUrl!: string;
  StyleTagKoobooId!: string;
  KoobooId!: string;
  readonly editorType: string = EDITOR_TYPE.style;

  static createUpdate(
    nameOrId: string,
    objectType: string,
    value: string,
    property: string,
    koobooId: string
  ) {
    let log = new StyleLog();
    log.action = ActionType.update;
    log.nameOrId = nameOrId;
    log.value = value;
    log.objectType = objectType;
    log.property = property;
    log.KoobooId = koobooId;
    return log;
  }
}
