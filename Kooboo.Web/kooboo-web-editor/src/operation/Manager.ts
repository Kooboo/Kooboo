import { operationRecord } from "./Record";
import { OperationEventArgs } from "@/events/OperationEvent";
import context from "@/common/context";

export class operationManager {
  readonly previousRecords: operationRecord[] = [];
  readonly nextRecords: operationRecord[] = [];

  previous() {
    let operation = this.previousRecords.pop();
    if (operation) {
      operation.undo();
      this.nextRecords.push(operation);
      this.emit();
    }
  }

  next() {
    let operation = this.nextRecords.pop();
    if (operation) {
      operation.redo();
      this.previousRecords.push(operation);
      this.emit();
    }
  }

  add(operation: operationRecord) {
    this.previousRecords.push(operation);
    this.nextRecords.splice(0, this.nextRecords.length);
    this.emit();
  }

  private emit() {
    let args = new OperationEventArgs(this.previousRecords.length, this.nextRecords.length);
    console.log(this.previousRecords, this.nextRecords);
    context.operationEvent.emit(args);
  }
}
