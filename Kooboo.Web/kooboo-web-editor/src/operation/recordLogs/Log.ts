import { ActionType } from "../ActionType";

export abstract class Log {
  action!: ActionType;
  nameOrId!: string;
}
