import { EDITOR_TYPE } from "@/common/constants";
import { ActionType } from "../ActionType";
import { Log } from "./Log";

export class ColorEditLog extends Log {
  constructor() {
    super();

    this.action = ActionType.update;
  }
  readonly editorType: string = EDITOR_TYPE.style;

  objectType!: string;
  property!: string;
  value!: string;
  pseudo!: string;
  selector!: string | null;
  KoobooId!: string;
  styleTagKoobooId!: string;
  styleSheetUrl!: string;
}
