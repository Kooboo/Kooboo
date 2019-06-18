import { BaseEvent } from "./BaseEvent";

export class OperationEventArgs {
  operationCount: number = 0;
  backupOperationCount: number = 0;

  constructor(operationCount: number, backupOperationCount: number) {
    this.operationCount = operationCount;
    this.backupOperationCount = backupOperationCount;
  }

  get canSave() {
    return this.operationCount > 0;
  }
}

export class OperationEvent extends BaseEvent<OperationEventArgs> {}
