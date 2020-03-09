import { operationRecord } from "./Record";
import { OperationEventArgs } from "@/events/OperationEvent";
import context from "@/common/context";

export class operationManager {
  readonly previousRecords: operationRecord[] = [];
  readonly nextRecords: operationRecord[] = [];
  idCounter: number = 0;

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
    operation.id = ++this.idCounter;
    this.previousRecords.push(operation);
    console.log(this.previousRecords);
    this.nextRecords.splice(0, this.nextRecords.length);
    this.emit();
    return operation.id;
  }

  remove(id: number) {
    let item = this.previousRecords.find(f => f.id == id);
    if (!item) return;
    item.undo();
    let index = this.previousRecords.indexOf(item);
    this.previousRecords.splice(index, 1);
    this.emit();
  }

  private emit() {
    let args = new OperationEventArgs(this.previousRecords.length, this.nextRecords.length);
    context.operationEvent.emit(args);
  }
}
