import { EDITOR_TYPE } from "@/common/constants";
import { Log } from "./Log";
import { ActionType } from "../ActionType";

export class ConverterLog extends Log {
    value!: string;
    readonly editorType: string = EDITOR_TYPE.converter;
    convertResult!: string;
  
    static create(convertResult: string) {
      let log = new ConverterLog();
      log.action = ActionType.add;
      log.nameOrId = "";
      log.convertResult = convertResult;
      return log;
    }
  }