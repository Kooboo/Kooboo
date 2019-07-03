import { ActionType } from "../actionType";

export abstract class Log {
  action!: ActionType;
  nameOrId!: string;
}
